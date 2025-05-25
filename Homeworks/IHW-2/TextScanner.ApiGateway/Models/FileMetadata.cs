using System.Text.Json.Serialization;

namespace TextScanner.ApiGateway.Models;

public class FileMetadata
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("fileName")] public string FileName { get; set; }
    [JsonPropertyName("storagePath")] public string StoragePath { get; set; }
}