using System.Text.Json.Serialization;

namespace TextScanner.ApiGateway.Models;

public class AnalysisStat
{
    [JsonPropertyName("fileId")] public required string FileId { get; set; }
    [JsonPropertyName("paragraphCount")] public int ParagraphCount { get; set; }
    [JsonPropertyName("wordCount")] public int WordCount { get; set; }
    [JsonPropertyName("characterCount")] public int CharacterCount { get; set; }
    [JsonPropertyName("hash")] public required string Hash { get; set; }
}