using System.Security.Cryptography;
using System.Text;

namespace TextScanner.FileStoringService.Utilities
{
    public static class TextAnalyzer
    {
        public static string ComputeHash(string content)
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(hashBytes);
        }
    }
}