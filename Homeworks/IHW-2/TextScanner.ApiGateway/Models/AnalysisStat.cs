using System.Text.Json.Serialization;

namespace TextScanner.ApiGateway.Models;

public class AnalysisStat
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("fileId")] public string FileId { get; set; }
    [JsonPropertyName("paragraphCount")] public int ParagraphCount { get; set; }
    [JsonPropertyName("wordCount")] public int WordCount { get; set; }
    [JsonPropertyName("characterCount")] public int CharacterCount { get; set; }
    [JsonPropertyName("isPlagiarized")] public bool IsPlagiarized { get; set; }
    [JsonPropertyName("hash")] public string Hash { get; set; }
}