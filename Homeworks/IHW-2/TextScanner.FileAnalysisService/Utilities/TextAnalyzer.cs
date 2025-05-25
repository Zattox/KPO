using System.Security.Cryptography;
using System.Text;

namespace TextScanner.FileAnalysisService.Utilities;

public static class TextAnalyzer
{
    public static int CountParagraphs(string content)
    {
        if (string.IsNullOrEmpty(content))
            return 0;
        var paragraphs = content.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        return paragraphs.Length;
    }

    public static int CountWords(string content)
    {
        if (string.IsNullOrEmpty(content))
            return 0;
        var words = content.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }

    public static int CountCharacters(string content)
    {
        return content?.Length ?? 0;
    }

    public static string ComputeHash(string content)
    {
        if (string.IsNullOrEmpty(content))
            return string.Empty;
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}