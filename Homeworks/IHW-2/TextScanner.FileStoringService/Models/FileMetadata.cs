namespace TextScanner.FileStoringService.Models;

public class FileMetadata
{
    public string Id { get; set; }
    public string FileName { get; set; }
    public string StoragePath { get; set; }
    public string Hash { get; set; }
}