using Microsoft.AspNetCore.Mvc;
using TextScanner.FileStoringService.Data;
using TextScanner.FileStoringService.Models;
using TextScanner.FileAnalysisService.Utilities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;

namespace TextScanner.FileStoringService.Controllers;

[Route("store")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly FileStorageDbContext _context;
    private readonly ILogger _logger;
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public FilesController(FileStorageDbContext context)
    {
        _context = context;
        _logger = Log.ForContext<FilesController>();
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
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

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        using var reader = new StreamReader(memoryStream);
        var content = await reader.ReadToEndAsync();

        // Basic content validation (e.g., check if it's a text file)
        if (!IsValidTextContent(content))
        {
            _logger.Warning("Invalid file content detected");
            return BadRequest("File contains invalid content.");
        }

        var fileHash = TextAnalyzer.ComputeHash(content);
        var existingFile = await _context.FileMetadatas
            .FirstOrDefaultAsync(f => f.Hash == fileHash);
        if (existingFile != null)
        {
            _logger.Information($"File already exists with id: {existingFile.Id}");
            return Ok(new { fileId = existingFile.Id });
        }

        var fileId = Guid.NewGuid().ToString();
        var storagePath = Path.Combine("uploads", $"{fileId}_{file.FileName}");
        Directory.CreateDirectory(Path.GetDirectoryName(storagePath) ?? "uploads");
        await using (var stream = new FileStream(storagePath, FileMode.Create))
        {
            memoryStream.Position = 0;
            await memoryStream.CopyToAsync(stream);
        }

        var fileMetadata = new FileMetadata
        {
            Id = fileId,
            FileName = file.FileName,
            StoragePath = storagePath,
            Hash = fileHash
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
    public async Task<IActionResult> GetFile(string id, [FromQuery] bool analyze = false)
    {
        var fileMetadata = await _context.FileMetadatas.FindAsync(id);
        if (fileMetadata == null)
        {
            _logger.Warning($"File with id {id} not found");
            return NotFound();
        }

        var filePath = fileMetadata.StoragePath;
        if (!System.IO.File.Exists(filePath))
        {
            _logger.Error($"File at path {filePath} does not exist");
            return NotFound();
        }

        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(fileStream, "application/octet-stream", fileMetadata.FileName);
    }

    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> DeleteFile(string id)
    {
        var fileMetadata = await _context.FileMetadatas.FindAsync(id);
        if (fileMetadata == null)
        {
            _logger.Warning($"File with id {id} not found");
            return NotFound();
        }

        var filePath = fileMetadata.StoragePath;
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        _context.FileMetadatas.Remove(fileMetadata);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // Simple validation to ensure the file contains text
    private bool IsValidTextContent(string content)
    {
        return !string.IsNullOrEmpty(content) &&
               content.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c));
    }
}