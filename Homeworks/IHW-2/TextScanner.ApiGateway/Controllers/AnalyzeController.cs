using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace TextScanner.ApiGateway.Controllers
{
    [ApiController]
    [Route("analyze")]
    public class AnalyzeController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AnalyzeController> _logger;

        public AnalyzeController(IHttpClientFactory httpClientFactory, ILogger<AnalyzeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AnalyzeFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Файл не загружен.");

            var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(file.OpenReadStream());
            streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain");
            content.Add(streamContent, "file", file.FileName);

            var response = await fileStoringClient.PostAsync("/files", content);
            if (!response.IsSuccessStatusCode) return StatusCode((int)response.StatusCode);

            var fileIdResponse = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Response from FileStoringService: {Response}", fileIdResponse);

            var fileIdJson = JObject.Parse(fileIdResponse);
            var fileIdToken = fileIdJson["FileId"] ?? fileIdJson["fileId"];
            if (fileIdToken == null)
            {
                _logger.LogError("FileId not found in FileStoringService response: {Response}", fileIdResponse);
                return BadRequest("FileId не найден в ответе FileStoringService.");
            }
            var fileId = fileIdToken.ToString();

            var analysisClient = _httpClientFactory.CreateClient("FileAnalysisService");
            var analyzeRequest = new { FileId = fileId };
            var analyzeContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(analyzeRequest), System.Text.Encoding.UTF8, "application/json");
            var analyzeResponse = await analysisClient.PostAsync("/analyze", analyzeContent);
            if (!analyzeResponse.IsSuccessStatusCode) return StatusCode((int)analyzeResponse.StatusCode);

            var analysisResult = await analyzeResponse.Content.ReadAsStringAsync();
            return Content(analysisResult, "application/json");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFilesAndStats()
        {
            var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");
            var filesResponse = await fileStoringClient.GetAsync("/files");
            if (!filesResponse.IsSuccessStatusCode) return StatusCode((int)filesResponse.StatusCode);
            var filesJson = await filesResponse.Content.ReadAsStringAsync();
            var files = JArray.Parse(filesJson);

            var analysisClient = _httpClientFactory.CreateClient("FileAnalysisService");
            var statsResponse = await analysisClient.GetAsync("/analyze");
            if (!statsResponse.IsSuccessStatusCode) return StatusCode((int)statsResponse.StatusCode);
            var statsJson = await statsResponse.Content.ReadAsStringAsync();
            var stats = JArray.Parse(statsJson);

            var result = files.Select(file =>
            {
                var fileId = file["Id"]?.ToString();
                var matchingStat = stats.FirstOrDefault(s => s["FileId"]?.ToString() == fileId);
                return new
                {
                    FileId = fileId,
                    FileName = file["FileName"]?.ToString(),
                    StoragePath = file["StoragePath"]?.ToString(),
                    ParagraphCount = matchingStat?["ParagraphCount"]?.ToObject<int>(),
                    WordCount = matchingStat?["WordCount"]?.ToObject<int>(),
                    CharacterCount = matchingStat?["CharacterCount"]?.ToObject<int>(),
                    IsPlagiarized = matchingStat?["IsPlagiarized"]?.ToObject<bool>(),
                    Hash = matchingStat?["Hash"]?.ToString()
                };
            }).ToList();

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFileAndStats(string id)
        {
            var fileStoringClient = _httpClientFactory.CreateClient("FileStoringService");
            var fileDeleteResponse = await fileStoringClient.DeleteAsync($"/files/{id}");
            if (!fileDeleteResponse.IsSuccessStatusCode && fileDeleteResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                return StatusCode((int)fileDeleteResponse.StatusCode);
            }

            var analysisClient = _httpClientFactory.CreateClient("FileAnalysisService");
            var statsDeleteResponse = await analysisClient.DeleteAsync($"/analyze/{id}");
            if (!statsDeleteResponse.IsSuccessStatusCode && statsDeleteResponse.StatusCode != System.Net.HttpStatusCode.NotFound)
            {
                return StatusCode((int)statsDeleteResponse.StatusCode);
            }

            return NoContent();
        }
    }
}