namespace TextScanner.FileAnalysisService.Models
{
    public class AnalysisResult
    {
        public required string FileId { get; set; }
        public int ParagraphCount { get; set; }
        public int WordCount { get; set; }
        public int CharacterCount { get; set; }
        public required string Hash { get; set; }
    }
}