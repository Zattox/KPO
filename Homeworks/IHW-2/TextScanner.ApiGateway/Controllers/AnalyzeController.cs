using Microsoft.AspNetCore.Mvc;
using TextScanner.ApiGateway.Models;
using TextScanner.FileAnalysisService.Models;
using FileMetadata = TextScanner.FileStoringService.Models.FileMetadata;

namespace TextScanner.ApiGateway.Controllers;

[Route("files")]
[ApiController]
public class AnalyzeController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AnalyzeController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> AnalyzeFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("File is required.");
        }

        var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");
        var fileAnalysisClient = _httpClientFactory.CreateClient("FileAnalysisService");

        using var multipartContent = new MultipartFormDataContent();
        using var fileStream = file.OpenReadStream();
        using var streamContent = new StreamContent(fileStream);
        multipartContent.Add(streamContent, "file", file.FileName);

        var response = await fileStoringClient.PostAsync("/store/upload", multipartContent);
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        var fileResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        var fileId = fileResponse?["fileId"];
        if (string.IsNullOrEmpty(fileId))
        {
            return BadRequest("Failed to retrieve fileId from storage service.");
        }

        var analysisRequest = new AnalysisRequest { FileId = fileId };
        var analysisResponse = await fileAnalysisClient.PostAsJsonAsync("/analyze/upload", analysisRequest);
        if (!analysisResponse.IsSuccessStatusCode)
        {
            return StatusCode((int)analysisResponse.StatusCode);
        }

        return Ok(await analysisResponse.Content.ReadFromJsonAsync<AnalysisResult>());
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAllFilesAndStats()
    {
        var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");
        var fileAnalysisClient = _httpClientFactory.CreateClient("FileAnalysisService");

        var filesResponse = await fileStoringClient.GetAsync("/store/list");
        if (!filesResponse.IsSuccessStatusCode)
        {
            return StatusCode((int)filesResponse.StatusCode, "Failed to fetch files from File Storing Service.");
        }

        var analysisResponse = await fileAnalysisClient.GetAsync("/analyze/list");
        if (!analysisResponse.IsSuccessStatusCode)
        {
            return StatusCode((int)analysisResponse.StatusCode, "Failed to fetch analysis from File Analysis Service.");
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

    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> DeleteFileAndStats(string id)
    {
        var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");
        var fileAnalysisClient = _httpClientFactory.CreateClient("FileAnalysisService");

        var deleteFileResponse = await fileStoringClient.DeleteAsync($"/store/remove/{id}");
        var deleteStatsResponse = await fileAnalysisClient.DeleteAsync($"/analyze/remove/{id}");

        if (!deleteFileResponse.IsSuccessStatusCode || !deleteStatsResponse.IsSuccessStatusCode)
        {
            return StatusCode((int)(deleteFileResponse.IsSuccessStatusCode
                ? deleteStatsResponse.StatusCode
                : deleteFileResponse.StatusCode));
        }

        return NoContent();
    }
}