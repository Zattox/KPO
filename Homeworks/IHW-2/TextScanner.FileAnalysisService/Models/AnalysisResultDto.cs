namespace TextScanner.FileAnalysisService.Models
{
    public class AnalysisResultDto
    {
        public int FileId { get; set; }
        public string ExtractedText { get; set; } = string.Empty;
        public int WordCount { get; set; }
        public int CharacterCount { get; set; }
        public DateTime AnalysisDate { get; set; }
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class AnalysisRequestDto
    {
        public int FileId { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}