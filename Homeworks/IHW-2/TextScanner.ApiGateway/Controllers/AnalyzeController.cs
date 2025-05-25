using Microsoft.AspNetCore.Mvc;
using TextScanner.ApiGateway.Models;
using TextScanner.FileAnalysisService.Models;
using FileMetadata = TextScanner.FileStoringService.Models.FileMetadata;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using ILogger = Serilog.ILogger;
using System.Text.Json;

namespace TextScanner.ApiGateway.Controllers;

[Route("files")]
[ApiController]
public class AnalyzeController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public AnalyzeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _logger = Log.ForContext<AnalyzeController>();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Ignore case for JSON property names
        };
    }

    // Retry policy for transient errors
    private static readonly IAsyncPolicy<HttpResponseMessage> RetryPolicy =
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    Log.Warning($"Retry attempt {retryAttempt} after error: {outcome.Exception?.Message}");
                });

    // Circuit breaker policy for service unavailability
    private static readonly IAsyncPolicy<HttpResponseMessage> CircuitBreakerPolicy =
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(60),
                onBreak: (outcome, breakDuration) =>
                {
                    Log.Error(
                        $"Circuit breaker triggered: {outcome.Exception?.Message}, pause for {breakDuration.TotalSeconds} seconds");
                },
                onReset: () => Log.Information("Circuit breaker reset"));

    [HttpPost("upload")]
    public async Task<IActionResult> AnalyzeFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.Warning("Attempt to upload an empty file");
            return BadRequest("File is required.");
        }

        if (file.Length > MaxFileSize)
        {
            _logger.Warning("File size exceeds the maximum limit");
            return BadRequest("File size exceeds 10 MB.");
        }

        var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");
        var fileAnalysisClient = _httpClientFactory.CreateClient("FileAnalysisService");

        try
        {
            using var multipartContent = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            using var streamContent = new StreamContent(fileStream);
            multipartContent.Add(streamContent, "file", file.FileName);

            var response = await RetryPolicy
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(() => fileStoringClient.PostAsync("/store/upload", multipartContent));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest &&
                    errorContent.Contains("Plagiarism detected"))
                {
                    _logger.Warning($"Plagiarism detected: {errorContent}");
                    return BadRequest(errorContent);
                }

                _logger.Error($"Error saving file: {response.StatusCode}, Details: {errorContent}");
                return StatusCode((int)response.StatusCode, errorContent);
            }

            var fileResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>(_jsonOptions);
            var fileId = fileResponse?["fileId"];
            if (string.IsNullOrEmpty(fileId))
            {
                _logger.Error("Failed to retrieve fileId from FileStoringService");
                return BadRequest("Failed to get fileId from storage service.");
            }

            var analysisResponse = await RetryPolicy
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(() => fileAnalysisClient.GetAsync($"/analyze/{fileId}"));

            if (!analysisResponse.IsSuccessStatusCode)
            {
                var analysisError = await analysisResponse.Content.ReadAsStringAsync();
                if (analysisResponse.StatusCode == System.Net.HttpStatusCode.BadRequest &&
                    analysisError.Contains("Plagiarism detected"))
                {
                    _logger.Warning($"Plagiarism detected during analysis: {analysisError}");
                    return BadRequest(analysisError);
                }

                _logger.Error($"Error analyzing file: {analysisResponse.StatusCode}, Details: {analysisError}");
                return StatusCode((int)analysisResponse.StatusCode, analysisError);
            }

            var analysisResult = await analysisResponse.Content.ReadFromJsonAsync<AnalysisResult>(_jsonOptions);
            return Ok(analysisResult);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error processing file");
            return StatusCode(503, "Service temporarily unavailable. Try again later.");
        }
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAllFilesAndStats()
    {
        var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");
        var fileAnalysisClient = _httpClientFactory.CreateClient("FileAnalysisService");

        try
        {
            var filesResponse = await RetryPolicy
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(() => fileStoringClient.GetAsync("/store/list"));

            if (!filesResponse.IsSuccessStatusCode)
            {
                _logger.Error($"Error retrieving file list: {filesResponse.StatusCode}");
                return StatusCode((int)filesResponse.StatusCode, "Failed to retrieve file list.");
            }

            var filesJson = await filesResponse.Content.ReadAsStringAsync();
            _logger.Information($"FileStoringService response: {filesJson}");

            var files = JsonSerializer.Deserialize<List<FileMetadata>>(filesJson, _jsonOptions);
            if (files == null)
            {
                _logger.Error("Failed to deserialize files from FileStoringService");
                return StatusCode(500, "Failed to deserialize file list.");
            }

            var analysisResponse = await RetryPolicy
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(() => fileAnalysisClient.GetAsync("/analyze/list"));

            if (!analysisResponse.IsSuccessStatusCode)
            {
                _logger.Error($"Error retrieving stats: {analysisResponse.StatusCode}");
                return StatusCode((int)analysisResponse.StatusCode, "Failed to retrieve stats.");
            }

            var stats = await analysisResponse.Content.ReadFromJsonAsync<List<AnalysisStat>>(_jsonOptions);
            if (stats == null)
            {
                _logger.Error("Failed to deserialize stats from FileAnalysisService");
                return StatusCode(500, "Failed to deserialize stats.");
            }

            if (!files.Any())
            {
                return Ok(new List<object>());
            }

            var result = files.Select(f =>
            {
                var matchingStat = stats.FirstOrDefault(s => s.FileId == f.Id);
                return new
                {
                    FileId = f.Id,
                    FileName = f.FileName,
                    StoragePath = f.StoragePath,
                    ParagraphCount = matchingStat?.ParagraphCount,
                    WordCount = matchingStat?.WordCount,
                    CharacterCount = matchingStat?.CharacterCount,
                    Hash = matchingStat?.Hash
                };
            }).ToList();

            return Ok(result);
        }
        catch (JsonException ex)
        {
            _logger.Error(ex, "Error deserializing response from services");
            return StatusCode(500, "Error processing service response.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving file list and stats");
            return StatusCode(503, "Service temporarily unavailable. Try again later.");
        }
    }

    [HttpGet("content/{id}")]
    public async Task<IActionResult> GetFileContent(string id)
    {
        var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");

        try
        {
            var response = await RetryPolicy
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(() => fileStoringClient.GetAsync($"/store/content/{id}"));

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error($"Error retrieving file content: {response.StatusCode}");
                return StatusCode((int)response.StatusCode, "Failed to retrieve file content.");
            }

            var content = await response.Content.ReadAsStringAsync();
            return Ok(new { FileId = id, Content = content });
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error retrieving content for file {id}");
            return StatusCode(503, "Service temporarily unavailable. Try again later.");
        }
    }

    [HttpGet("stats/{id}")]
    public async Task<IActionResult> GetFileStatsAsync(string id)
    {
        var fileAnalysisClient = _httpClientFactory.CreateClient("FileAnalysisService");

        try
        {
            var response = await RetryPolicy
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(() => fileAnalysisClient.GetAsync($"/analyze/{id}"));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.Error($"Error retrieving stats for file {id}: {response.StatusCode}, Details: {errorContent}");
                return StatusCode((int)response.StatusCode, errorContent);
            }

            var stats = await response.Content.ReadFromJsonAsync<AnalysisResult>(_jsonOptions);
            if (stats == null)
            {
                _logger.Error($"Failed to deserialize stats for file {id}");
                return StatusCode(500, "Failed to deserialize stats.");
            }

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error retrieving stats for file {id}");
            return StatusCode(503, "Service temporarily unavailable. Try again later.");
        }
    }

    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> DeleteFileAndStats(string id)
    {
        var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");
        var fileAnalysisClient = _httpClientFactory.CreateClient("FileAnalysisService");

        try
        {
            var deleteFileResponse = await RetryPolicy
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(() => fileStoringClient.DeleteAsync($"/store/remove/{id}"));

            if (!deleteFileResponse.IsSuccessStatusCode)
            {
                var errorContent = await deleteFileResponse.Content.ReadAsStringAsync();
                _logger.Error(
                    $"Error deleting file {id} from FileStoringService: {deleteFileResponse.StatusCode}, Details: {errorContent}");
                return StatusCode((int)deleteFileResponse.StatusCode, errorContent);
            }

            _logger.Information($"Successfully deleted file {id} from FileStoringService");

            var deleteStatsResponse = await RetryPolicy
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(() => fileAnalysisClient.DeleteAsync($"/analyze/remove/{id}"));

            if (!deleteStatsResponse.IsSuccessStatusCode &&
                deleteStatsResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                var errorContent = await deleteStatsResponse.Content.ReadAsStringAsync();
                _logger.Error(
                    $"Error deleting stats for file {id} from FileAnalysisService: {deleteStatsResponse.StatusCode}, Details: {errorContent}");
                return StatusCode((int)deleteStatsResponse.StatusCode, errorContent);
            }

            _logger.Information(
                $"Successfully deleted stats for file {id} from FileAnalysisService (or stats not found)");
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error deleting file {id}");
            return StatusCode(503, "Service temporarily unavailable. Try again later.");
        }
    }
}