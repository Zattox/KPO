using Microsoft.AspNetCore.Mvc;
using TextScanner.FileStoringService.Data;
using TextScanner.FileStoringService.Models;
using TextScanner.FileStoringService.Utilities;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;
using System.Text.Json;

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

        if (!IsValidTextContent(content, out var invalidReason))
        {
            _logger.Warning($"Invalid file content detected: {invalidReason}");
            return BadRequest($"File contains invalid content: {invalidReason}");
        }

        var fileHash = TextAnalyzer.ComputeHash(content);
        var existingFile = await _context.FileMetadatas
            .FirstOrDefaultAsync(f => f.Hash == fileHash);

        if (existingFile != null)
        {
            _logger.Information($"Plagiarism detected: File already exists with id: {existingFile.Id}");
            return BadRequest($"Plagiarism detected: File already exists with fileId: {existingFile.Id}");
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

        _logger.Information($"File uploaded successfully with id: {fileId}");
        return Ok(new { fileId });
    }

    [HttpGet("list")]
    public IActionResult GetAllFiles()
    {
        var files = _context.FileMetadatas
            .Select(f => new FileMetadata
            {
                Id = f.Id,
                FileName = f.FileName,
                StoragePath = f.StoragePath,
                Hash = f.Hash
            })
            .ToList();

        _logger.Information($"Returning {files.Count} files: {JsonSerializer.Serialize(files)}");
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

    [HttpGet("content/{id}")]
    public async Task<IActionResult> GetFileContent(string id)
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

        var content = await System.IO.File.ReadAllTextAsync(filePath);
        return Ok(content);
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
        {
            System.IO.File.Delete(filePath);
            _logger.Information($"Deleted file from disk: {filePath}");
        }

        _context.FileMetadatas.Remove(fileMetadata);
        await _context.SaveChangesAsync();

        _logger.Information($"Deleted file metadata with id: {id}");
        return NoContent();
    }

    private bool IsValidTextContent(string content, out string invalidReason)
    {
        invalidReason = string.Empty;
        if (string.IsNullOrEmpty(content))
        {
            invalidReason = "Content is empty";
            return false;
        }

        foreach (char c in content)
        {
            if (char.IsControl(c) && c != '\n' && c != '\r' && c != '\t')
            {
                invalidReason = $"Contains invalid control character: {c}";
                return false;
            }
        }

        return true;
    }
}