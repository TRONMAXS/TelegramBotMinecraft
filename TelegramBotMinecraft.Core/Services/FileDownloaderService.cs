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

        public async Task DownloadFileAsync(string downloadUrl, string pathToFile, string expectedSha1)
        {
            const int maxRetries = 3;
            int currentRetry = 0;
            while (currentRetry < maxRetries)
            {
                using (HttpResponseMessage response = await _client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    long? totalBytes = response.Content.Headers.ContentLength;
                    const int progressBarWidth = 30;

                    using (var sha1 = SHA1.Create())
                    using (var fileStream = new FileStream(pathToFile, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    using (var cryptoStream = new CryptoStream(fileStream, sha1, CryptoStreamMode.Write))
                    using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        var totalRead = 0L;
                        var buffer = new byte[8192];
                        var isMoreToRead = true;

                        do
                        {
                            var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                            if (read == 0)
                            {
                                isMoreToRead = false;
                            }
                            else
                            {
                                await cryptoStream.WriteAsync(buffer, 0, read);
                                totalRead += read;

                                if (totalBytes.HasValue)
                                {
                                    double percentage = (double)totalRead / totalBytes.Value;
                                    int filledChunks = (int)(percentage * progressBarWidth);
                                    string progressChunks = new string('█', filledChunks);
                                    string emptyChunks = new string('░', progressBarWidth - filledChunks);
                                    //Console.Write($"\r[{progressChunks}{emptyChunks}] {percentage * 100:F1}%");
                                }
                                else
                                {
                                    double megabytes = totalRead / (1024.0 * 1024.0);
                                    //Console.Write($"\rСкачивание... Загружено: {megabytes:F2} MB");
                                }
                            }
                        }
                        while (isMoreToRead);

                        cryptoStream.FlushFinalBlock();

                        byte[] hashBytes = sha1.Hash;
                        string downloadedSha1 = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

                        if (downloadedSha1 == expectedSha1.ToLowerInvariant())
                        {
                            //Console.Write($"\r{new string(' ', progressBarWidth + 15)}\r");
                            return;
                        }
                        else
                        {
                            currentRetry++;
                            //Console.WriteLine($"\r[Ошибка хэша] Файл поврежден (Ожидалось: {expectedSha1}, получено: {downloadedSha1}). Попытка {currentRetry} из {maxRetries}...");

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
