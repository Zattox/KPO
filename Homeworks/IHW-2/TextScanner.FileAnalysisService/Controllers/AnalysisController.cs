using Microsoft.AspNetCore.Mvc;
using TextScanner.FileAnalysisService.Data;
using TextScanner.FileAnalysisService.Models;
using Microsoft.EntityFrameworkCore;
using TextScanner.FileAnalysisService.Utilities;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using ILogger = Serilog.ILogger;

namespace TextScanner.FileAnalysisService.Controllers;

[Route("analyze")]
[ApiController]
public class AnalysisController : ControllerBase
{
    private readonly AnalysisDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;

    public AnalysisController(AnalysisDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _logger = Log.ForContext<AnalysisController>();
    }

    private static readonly IAsyncPolicy<HttpResponseMessage> RetryPolicy =
        HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryAttempt, context) =>
                {
                    Log.Warning($"Retry attempt {retryAttempt} after error: {outcome.Exception?.Message}");
                });

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

    [HttpGet("{fileId}")]
    public async Task<IActionResult> Analyze(string fileId)
    {
        if (string.IsNullOrEmpty(fileId))
        {
            _logger.Warning("Attempt to analyze file without fileId");
            return BadRequest("FileId is required.");
        }

        var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");

        try
        {
            var response = await RetryPolicy
                .WrapAsync(CircuitBreakerPolicy)
                .ExecuteAsync(() => fileStoringClient.GetAsync($"/store/{fileId}"));

            if (!response.IsSuccessStatusCode)
            {
                _logger.Error($"Error retrieving file: {response.StatusCode}");
                return StatusCode((int)response.StatusCode, "Failed to retrieve file.");
            }

            using var fileStream = await response.Content.ReadAsStreamAsync();
            using var memoryStream = new MemoryStream();
            await fileStream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var analysisResult = AnalyzeFile(memoryStream, fileId);

            var existingHash = await _context.AnalysisResults
                .Where(a => a.Hash == analysisResult.Hash && a.FileId != fileId)
                .AnyAsync();
            analysisResult.IsPlagiarized = existingHash;

            _context.AnalysisResults.Add(analysisResult);
            await _context.SaveChangesAsync();

            return Ok(analysisResult);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error analyzing file");
            return StatusCode(503, "Service temporarily unavailable. Try again later.");
        }
    }

    [HttpGet("list")]
    public IActionResult GetAllStats()
    {
        var stats = _context.AnalysisResults
            .Select(s => new
            {
                Id = s.Id,
                FileId = s.FileId,
                ParagraphCount = s.ParagraphCount,
                WordCount = s.WordCount,
                CharacterCount = s.CharacterCount,
                IsPlagiarized = s.IsPlagiarized,
                Hash = s.Hash
            })
            .ToList();

        return Ok(stats);
    }

    [HttpDelete("remove/{fileId}")]
    public async Task<IActionResult> DeleteStats(string fileId)
    {
        var analysisResult = await _context.AnalysisResults
            .FirstOrDefaultAsync(s => s.FileId == fileId);
        if (analysisResult == null)
        {
            _logger.Warning($"Stats for file {fileId} not found");
            return NotFound();
        }

        _context.AnalysisResults.Remove(analysisResult);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private AnalysisResult AnalyzeFile(Stream stream, string fileId)
    {
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        var analysisResult = new AnalysisResult
        {
            FileId = fileId,
            ParagraphCount = TextAnalyzer.CountParagraphs(content),
            WordCount = TextAnalyzer.CountWords(content),
            CharacterCount = TextAnalyzer.CountCharacters(content),
            Hash = TextAnalyzer.ComputeHash(content),
            IsPlagiarized = false
        };

        return analysisResult;
    }
}