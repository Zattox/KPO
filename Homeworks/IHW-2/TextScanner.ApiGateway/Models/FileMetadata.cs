using System.Text.Json.Serialization;

namespace TextScanner.ApiGateway.Models;

public class FileMetadata
{
    [JsonPropertyName("id")] public required string Id { get; set; }
    [JsonPropertyName("fileName")] public required string FileName { get; set; }
    [JsonPropertyName("storagePath")] public required string StoragePath { get; set; }
}