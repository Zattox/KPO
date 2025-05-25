using Microsoft.AspNetCore.Mvc;
using TextScanner.FileStoringService.Data;
using TextScanner.FileStoringService.Models;

namespace TextScanner.FileStoringService.Controllers;

[Route("store")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly FileStorageDbContext _context;

    public FilesController(FileStorageDbContext context)
    {
        _context = context;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("File is required.");

        var fileId = Guid.NewGuid().ToString();
        var storagePath = Path.Combine("uploads", $"{fileId}_{file.FileName}");
        Directory.CreateDirectory(Path.GetDirectoryName(storagePath) ?? "uploads");
        await using (var stream = new FileStream(storagePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileMetadata = new FileMetadata
        {
            Id = fileId,
            FileName = file.FileName,
            StoragePath = storagePath
        };

        _context.FileMetadatas.Add(fileMetadata);
        await _context.SaveChangesAsync();

        return Ok(new { fileId });
    }

    [HttpGet("list")]
    public IActionResult GetAllFiles()
    {
        var files = _context.FileMetadatas
            .Select(f => new { Id = f.Id, FileName = f.FileName, StoragePath = f.StoragePath })
            .ToList();
        return Ok(files);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFile(string id)
    {
        var fileMetadata = await _context.FileMetadatas.FindAsync(id);
        if (fileMetadata == null)
            return NotFound();

        var filePath = fileMetadata.StoragePath;
        if (!System.IO.File.Exists(filePath))
            return NotFound();

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(fileStream, "application/octet-stream", fileMetadata.FileName);
    }

    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> DeleteFile(string id)
    {
        var fileMetadata = await _context.FileMetadatas.FindAsync(id);
        if (fileMetadata == null)
            return NotFound();

        var filePath = fileMetadata.StoragePath;
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        _context.FileMetadatas.Remove(fileMetadata);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}