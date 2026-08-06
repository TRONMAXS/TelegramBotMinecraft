using System.Security.Cryptography;

namespace TelegramBotMinecraft.Core.Services
{
    public class HashService
    {
        public string GetFileSha1(string filePath)
        {
            if (!File.Exists(filePath)) return string.Empty;

            using (var sha1 = SHA1.Create())
            using (var stream = File.OpenRead(filePath))
            {
                byte[] hashBytes = sha1.ComputeHash(stream);

                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}