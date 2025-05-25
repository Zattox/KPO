using Microsoft.AspNetCore.Mvc;
using TextScanner.FileAnalysisService.Data;
using TextScanner.FileAnalysisService.Models;

namespace TextScanner.FileAnalysisService.Controllers
{
    [ApiController]
    [Route("analyze")]
    public class AnalysisController : ControllerBase
    {
        private readonly AnalysisDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public AnalysisController(AnalysisDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Analyze([FromBody] AnalysisRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.FileId))
            {
                return BadRequest("FileId is required.");
            }

            var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");
            var response = await fileStoringClient.GetAsync($"/files/{request.FileId}");
            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode);
            }

            using var fileStream = await response.Content.ReadAsStreamAsync();
            var analysisResult = AnalyzeFile(fileStream, request.FileId);

            _context.AnalysisResults.Add(analysisResult);
            await _context.SaveChangesAsync();

            return Ok(analysisResult);
        }

        [HttpGet]
        public IActionResult GetAllStats()
        {
            var stats = _context.AnalysisResults
                .Select(s => new
                {
                    s.Id,
                    s.FileId,
                    s.ParagraphCount,
                    s.WordCount,
                    s.CharacterCount,
                    s.IsPlagiarized,
                    s.Hash
                })
                .ToList();

            return Ok(stats);
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteStats(string fileId)
        {
            var stats = _context.AnalysisResults
                .Where(s => s.FileId == fileId)
                .ToList();

            if (!stats.Any())
            {
                return NotFound();
            }

            _context.AnalysisResults.RemoveRange(stats);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private AnalysisResult AnalyzeFile(Stream stream, string fileId)
        {
            var result = new AnalysisResult
            {
                FileId = fileId,
                ParagraphCount = CountParagraphs(stream),
                WordCount = CountWords(stream),
                CharacterCount = CountCharacters(stream),
                IsPlagiarized = CheckPlagiarism(stream),
                Hash = ComputeHash(stream)
            };
            return result;
        }

        private string ComputeHash(Stream stream)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            stream.Position = 0;
            var hash = sha256.ComputeHash(stream);
            return Convert.ToBase64String(hash);
        }

        private int CountParagraphs(Stream stream)
        {
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            return text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private int CountWords(Stream stream)
        {
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();
            return text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private int CountCharacters(Stream stream)
        {
            stream.Position = 0;
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd().Length;
        }

        private bool CheckPlagiarism(Stream stream)
        {
            return false;
        }
    }
}