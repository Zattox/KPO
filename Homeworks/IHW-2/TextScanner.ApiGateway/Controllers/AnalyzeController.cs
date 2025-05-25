using Microsoft.AspNetCore.Mvc;
using TextScanner.ApiGateway.Models;
using TextScanner.FileAnalysisService.Models;
using FileMetadata = TextScanner.FileStoringService.Models.FileMetadata;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using ILogger = Serilog.ILogger;

namespace TextScanner.ApiGateway.Controllers;

[Route("files")]
[ApiController]
public class AnalyzeController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public AnalyzeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _logger = Log.ForContext<AnalyzeController>();
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
            .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30),
                onBreak: (outcome, breakDuration) =>
                {
                    Log.Error(
                        $"Circuit breaker triggered: {outcome.Exception?.Message}, pause for {breakDuration.TotalSeconds} seconds");
                },
                onReset: () => Log.Information("Circuit breaker reset"));

    [HttpPost("upload")]
    public async Task<IActionResult> AnalyzeFile(IFormFile file)
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
                _logger.Error($"Error saving file: {response.StatusCode}");
                return StatusCode((int)response.StatusCode, "Failed to save file.");
            }

            var fileResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
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
                _logger.Error($"Error analyzing file: {analysisResponse.StatusCode}");
                return StatusCode((int)analysisResponse.StatusCode, "Failed to analyze file.");
            }

            return Ok(await analysisResponse.Content.ReadFromJsonAsync<AnalysisResult>());
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

            var analysisResponse = await RetryPolicy
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(() => fileAnalysisClient.GetAsync("/analyze/list"));

            if (!analysisResponse.IsSuccessStatusCode)
            {
                _logger.Error($"Error retrieving stats: {analysisResponse.StatusCode}");
                return StatusCode((int)analysisResponse.StatusCode, "Failed to retrieve stats.");
            }

            var files = await filesResponse.Content.ReadFromJsonAsync<List<FileMetadata>>();
            var stats = await analysisResponse.Content.ReadFromJsonAsync<List<AnalysisStat>>();

            if (files == null || !files.Any())
            {
                return Ok(new List<object>());
            }

            var result = files.Select(f =>
            {
                var matchingStat = stats?.FirstOrDefault(s => s.FileId == f.Id);
                return new
                {
                    Id = f.Id,
                    FileName = f.FileName,
                    StoragePath = f.StoragePath,
                    ParagraphCount = matchingStat?.ParagraphCount,
                    WordCount = matchingStat?.WordCount,
                    CharacterCount = matchingStat?.CharacterCount,
                    IsPlagiarized = matchingStat?.IsPlagiarized,
                    Hash = matchingStat?.Hash
                };
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error retrieving file list and stats");
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

            var deleteStatsResponse = await RetryPolicy
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(() => fileAnalysisClient.DeleteAsync($"/analyze/remove/{id}"));

            if (!deleteFileResponse.IsSuccessStatusCode || !deleteStatsResponse.IsSuccessStatusCode)
            {
                var statusCode = deleteFileResponse.IsSuccessStatusCode
                    ? deleteStatsResponse.StatusCode
                    : deleteFileResponse.StatusCode;
                _logger.Error($"Error deleting file or stats: {statusCode}");
                return StatusCode((int)statusCode, "Failed to delete file or stats.");
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error deleting file");
            return StatusCode(503, "Service temporarily unavailable. Try again later.");
        }
    }
}