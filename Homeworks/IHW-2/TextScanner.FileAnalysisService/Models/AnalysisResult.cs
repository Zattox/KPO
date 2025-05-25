using System.ComponentModel.DataAnnotations;

namespace TextScanner.FileAnalysisService.Models
{
    public class AnalysisResult
    {
        [Key] public int Id { get; set; }
        public string FileId { get; set; }
        public int ParagraphCount { get; set; }
        public int WordCount { get; set; }
        public int CharacterCount { get; set; }
        public bool IsPlagiarized { get; set; }
        public string Hash { get; set; }
    }
}