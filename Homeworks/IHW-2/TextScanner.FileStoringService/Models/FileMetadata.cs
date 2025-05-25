using System.Text.Json.Serialization;

namespace TextScanner.FileStoringService.Models;

public class FileMetadata
{
    [JsonPropertyName("id")] public required string Id { get; set; }
    [JsonPropertyName("fileName")] public required string FileName { get; set; }
    [JsonPropertyName("storagePath")] public required string StoragePath { get; set; }
    [JsonPropertyName("hash")] public string? Hash { get; set; }
}