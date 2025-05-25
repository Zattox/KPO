using Microsoft.AspNetCore.Mvc;
using TextScanner.FileStoringService.Data;
using TextScanner.FileStoringService.Models;

namespace TextScanner.FileStoringService.Controllers
{
    [ApiController]
    [Route("files")]
    public class FilesController : ControllerBase
    {
        private readonly FileStorageDbContext _context;
        private readonly string _storageDirectory;

        public FilesController(FileStorageDbContext context, IConfiguration configuration)
        {
            _context = context;
            _storageDirectory = configuration["StorageDirectory"] ?? throw new ArgumentNullException("StorageDirectory is not configured.");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Файл не загружен.");

            if (!Directory.Exists(_storageDirectory)) Directory.CreateDirectory(_storageDirectory);

            var fileId = Guid.NewGuid().ToString();
            var filePath = Path.Combine(_storageDirectory, fileId);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileMetadata = new FileMetadata
            {
                Id = fileId,
                FileName = file.FileName,
                StoragePath = filePath
            };
            _context.FileMetadatas.Add(fileMetadata);
            await _context.SaveChangesAsync();

            return Ok(new { FileId = fileId });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFile(string id)
        {
            var fileMetadata = await _context.FileMetadatas.FindAsync(id);
            if (fileMetadata == null) return NotFound();

            var fileStream = new FileStream(fileMetadata.StoragePath, FileMode.Open, FileAccess.Read);
            return File(fileStream, "application/octet-stream", fileMetadata.FileName);
        }

        [HttpGet]
        public IActionResult GetAllFiles()
        {
            var files = _context.FileMetadatas
                .Select(f => new
                {
                    f.Id,
                    f.FileName,
                    f.StoragePath
                })
                .ToList();

            return Ok(files);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFile(string id)
        {
            var fileMetadata = await _context.FileMetadatas.FindAsync(id);
            if (fileMetadata == null) return NotFound();
            
            if (System.IO.File.Exists(fileMetadata.StoragePath))
            {
                System.IO.File.Delete(fileMetadata.StoragePath);
            }
            
            _context.FileMetadatas.Remove(fileMetadata);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}