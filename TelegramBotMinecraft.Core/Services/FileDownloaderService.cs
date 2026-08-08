using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace TelegramBotMinecraft.Core.Services
{
    public class FileDownloaderService
    {
        private readonly HttpClient _client;

        public FileDownloaderService(HttpClient client)
        {
            _client = client;
        }

        public async IAsyncEnumerable<long> DownloadFileAsync(string downloadUrl, string pathToFile, string expectedSha1, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            const int maxRetries = 3;
            int currentRetry = 0;

            while (currentRetry < maxRetries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (HttpResponseMessage response = await _client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    long? totalBytes = response.Content.Headers.ContentLength;

                    using (var sha1 = SHA1.Create())
                    using (var fileStream = new FileStream(pathToFile, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    using (var cryptoStream = new CryptoStream(fileStream, sha1, CryptoStreamMode.Write))
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync(cancellationToken))
                    {
                        var buffer = new byte[8192];

                        while (true)
                        {
                            cancellationToken.ThrowIfCancellationRequested();

                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                            if (read == 0) break;

                            await cryptoStream.WriteAsync(buffer, 0, read, cancellationToken);

                            yield return read;
                        }

                        cryptoStream.FlushFinalBlock();

                        byte[] hashBytes = sha1.Hash;
                        string downloadedSha1 = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                        if (downloadedSha1 == expectedSha1.ToLowerInvariant())
                        {
                            yield break;
                        }
                        else
                        {
                            currentRetry++;

                            cryptoStream.Close();
                            fileStream.Close();

                            if (File.Exists(pathToFile)) File.Delete(pathToFile);
                        }
                    }
                }
            }
            throw new Exception($"Не удалось скачать файл {Path.GetFileName(pathToFile)}: ошибка проверки SHA-1 после {maxRetries} попыток.");
        }

    }
}
