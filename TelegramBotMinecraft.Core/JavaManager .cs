using System.IO.Compression;

namespace TelegramBotMinecraft
{
    internal sealed class JavaManager
    {
        private HttpClient client = new HttpClient();

        private string UrlJava8 = "https://download.java.net/openjdk/jdk8u44/ri/openjdk-8u44-windows-i586.zip";
        private string UrlJava16 = "https://download.java.net/openjdk/jdk16/ri/openjdk-16+36_windows-x64_bin.zip";
        private string UrlJava17 = "https://download.java.net/openjdk/jdk17.0.0.1/ri/openjdk-17.0.0.1+2_windows-x64_bin.zip";
        private string UrlJava21 = "https://download.java.net/openjdk/jdk21/ri/openjdk-21+35_windows-x64_bin.zip";
        private string UrlJava25 = "https://download.java.net/openjdk/jdk25/ri/openjdk-25+36_windows-x64_bin.zip";

        private string PathJavas;

        private string PathJava8;
        private string PathJava16;
        private string PathJava17;
        private string PathJava21;
        private string PathJava25;

        private string PathZipJava8;
        private string PathZipJava16;
        private string PathZipJava17;
        private string PathZipJava21;
        private string PathZipJava25;

        private JavaManager()
        {
            PathJavas = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Java");

            PathJava8 = Path.Combine(PathJavas, "jdk-8");
            PathJava16 = Path.Combine(PathJavas, "jdk-16");
            PathJava17 = Path.Combine(PathJavas, "jdk-17");
            PathJava21 = Path.Combine(PathJavas, "jdk-21");
            PathJava25 = Path.Combine(PathJavas, "jdk-25");

            PathZipJava8 = Path.Combine(PathJavas, "jdk-8.zip");
            PathZipJava16 = Path.Combine(PathJavas, "jdk-16.zip");
            PathZipJava17 = Path.Combine(PathJavas, "jdk-17.zip");
            PathZipJava21 = Path.Combine(PathJavas, "jdk-21.zip");
            PathZipJava25 = Path.Combine(PathJavas, "jdk-25.zip");
        }

        private async Task JavaVersion()
        {
            if (!Directory.Exists(PathJavas))
            {
                Directory.CreateDirectory(PathJavas);
            }
            if (!Directory.Exists(PathJava8))
            {
                Task.Run(async () => await DownloadFile(UrlJava8, PathZipJava8))
                    .ContinueWith((result) => UnzipFile(PathZipJava8))
                    .ContinueWith((result) => Directory.Move(Path.Combine(PathJavas, "java-se-8u44-ri"), PathJava8));
            }
            else { Console.WriteLine("Java 8 установлена"); }
            if (!Directory.Exists(PathJava16))
            {
                Task.Run(async () => await DownloadFile(UrlJava16, PathZipJava16))
                    .ContinueWith((result) => UnzipFile(PathZipJava16));
            }
            else { Console.WriteLine("Java 16 установлена"); }
            if (!Directory.Exists(PathJava17))
            {
                Task.Run(async () => await DownloadFile(UrlJava17, PathZipJava17))
                    .ContinueWith((result) => UnzipFile(PathZipJava17))
                    .ContinueWith((result) => Directory.Move(Path.Combine(PathJavas, "jdk-17.0.0.1"), PathJavas));
            }
            else { Console.WriteLine("Java 17 установлена"); }
            if (!Directory.Exists(PathJava21))
            {
                Task.Run(async () => await DownloadFile(UrlJava21, PathZipJava21))
                    .ContinueWith((result) => UnzipFile(PathZipJava21));
            }
            else { Console.WriteLine("Java 21 установлена"); }
            if (!Directory.Exists(PathJava25))
            {
                Task.Run(async () => await DownloadFile(UrlJava25, PathZipJava25))
                    .ContinueWith((result) => UnzipFile(PathZipJava25));
            }
            else { Console.WriteLine("Java 25 установлена"); }
        }

        private async Task DownloadFile(string downloadUrl, string pathToFile)
        {
            using (HttpResponseMessage response = await client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead))
            {
                response.EnsureSuccessStatusCode();

                using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                    fileStream = new FileStream(pathToFile, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                {
                    var totalRead = 0L;
                    var totalReads = 0L;
                    var buffer = new byte[8192];
                    var isMoreToRead = true;

                    long? totalBytes = response.Content.Headers.ContentLength;
                    const int progressBarWidth = 30;
                    char[] spinnerAnimation = new char[] { '|', '/', '-', '\\' };
                    int animationIndex = 0;

                    do
                    {
                        var read = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        if (read == 0)
                        {
                            isMoreToRead = false;
                        }
                        else
                        {
                            await fileStream.WriteAsync(buffer, 0, read);

                            totalRead += read;

                            if (totalBytes.HasValue)
                            {
                                // Вычисляем проценты
                                double percentage = (double)totalRead / totalBytes.Value;
                                int filledChunks = (int)(percentage * progressBarWidth);

                                // Строим полосу: блоки '█' и пустота '░'
                                string progressChunks = new string('█', filledChunks);
                                string emptyChunks = new string('░', progressBarWidth - filledChunks);

                                // Выводим строку в формате: \r[████░░░░░░] 45.5%
                                Console.Write($"\r[{progressChunks}{emptyChunks}] {percentage * 100:F1}%");
                            }
                            else
                            {
                                // Резервный вариант, если размер файла неизвестен
                                double megabytes = totalRead / (1024.0 * 1024.0);
                                Console.Write($"\rСкачивание... Загружено: {megabytes:F2} MB");
                            }
                        }
                    }
                    while (isMoreToRead);
                    {
                        Console.Write($"\r{new string(' ', progressBarWidth + 15)}");
                        Console.WriteLine("\rГотово! Загрузка завершена успешно.");
                    }
                }
            }
        }

        private void UnzipFile(string pathToFile)
        {
            if (!File.Exists(pathToFile))
            {
                Console.WriteLine("Указанный файл не найден.");
                return;
            }
            Console.WriteLine("Начинаем распаковку...");

            ZipFile.ExtractToDirectory(pathToFile, Path.GetDirectoryName(pathToFile));
            File.Delete(pathToFile);

            Console.WriteLine("Готово!");
        }
    }
}
