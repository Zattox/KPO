namespace TextScanner.FileStoringService.Models;

public class FileMetadata
{
    public required string Id { get; set; }
    public required string FileName { get; set; }
    public required string StoragePath { get; set; }
    public string? Hash { get; set; }
}