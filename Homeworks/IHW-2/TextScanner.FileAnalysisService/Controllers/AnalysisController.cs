using Microsoft.AspNetCore.Mvc;
using TextScanner.FileAnalysisService.Data;
using TextScanner.FileAnalysisService.Models;
using Microsoft.EntityFrameworkCore;
using TextScanner.FileAnalysisService.Utilities;

namespace TextScanner.FileAnalysisService.Controllers;

[Route("analyze")]
[ApiController]
public class AnalysisController : ControllerBase
{
    private readonly AnalysisDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public AnalysisController(AnalysisDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Analyze([FromBody] AnalysisRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.FileId))
        {
            return BadRequest("FileId is required.");
        }

        var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");
        var response = await fileStoringClient.GetAsync($"/store/{request.FileId}");
        if (!response.IsSuccessStatusCode)
        {
            return StatusCode((int)response.StatusCode);
        }

        using var fileStream = await response.Content.ReadAsStreamAsync();
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        var analysisResult = AnalyzeFile(memoryStream, request.FileId);
        
        var existingHash = await _context.AnalysisResults
            .Where(a => a.Hash == analysisResult.Hash && a.FileId != request.FileId)
            .AnyAsync();
        analysisResult.IsPlagiarized = existingHash;

        _context.AnalysisResults.Add(analysisResult);
        await _context.SaveChangesAsync();

        return Ok(analysisResult);
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
            return NotFound();

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