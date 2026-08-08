using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Channels;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Core.Services
{
    public class JavaManagerService
    {
        private readonly HttpClient _client;
        private readonly HashService _hashService;
        private readonly FileDownloaderService _downloader;
        private readonly LzmaDecompressorService _decompressor;


        private string UrlManifestJson = "https://piston-meta.mojang.com/mc/game/version_manifest_v2.json";
        private string UrlJavaManifestJson = "https://launchermeta.mojang.com/v1/products/java-runtime/2ec0cc96c44e5a76b9c8b7c39df7210883d12871/all.json";

        public JavaManagerService(HttpClient client, HashService hashService, FileDownloaderService downloader, LzmaDecompressorService decompressor)
        {
            _client = client;
            _hashService = hashService;
            _downloader = downloader;
            _decompressor = decompressor;
        }


        public async IAsyncEnumerable<(int Progress, int completedFiles, int AllFiles)> JavaDownloader(string? javaName, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var channel = Channel.CreateBounded<(int Progress, int Completed, int All)>(new BoundedChannelOptions(100)
            {
                FullMode = BoundedChannelFullMode.DropOldest,
                SingleWriter = false,
                SingleReader = true
            });


            long totalBytesDownloaded = 0;
            int completedFilesCount = 0;
            List<JavaFileTask> javaFileTasks = new();
            string PathJavas = "";

            string responseJavaJson = await _client.GetStringAsync(UrlJavaManifestJson);
            var ManifestJavaJson = JsonSerializer.Deserialize<JavaManifest>(responseJavaJson);
            if (ManifestJavaJson == null) { channel.Writer.Complete(); yield return (0, 0, 0); yield break; }

            string responseMinecraftJson = await _client.GetStringAsync(UrlManifestJson);
            var ManifestMinecraftJson = JsonSerializer.Deserialize<MinecraftManifest>(responseMinecraftJson);
            if (ManifestMinecraftJson == null) { channel.Writer.Complete(); yield return (0, 0, 0); yield break; }

            try
            {
                string ArchOS = GetPlatformString();
                string? urlSelectedJavaVersion = ManifestJavaJson[ArchOS][javaName].Select(entry => entry.Manifest.Url).FirstOrDefault() ?? string.Empty;

                var (filesToDownload, totalBytesToDownload) = await GetAllDownloadableFiles(urlSelectedJavaVersion);
                javaFileTasks = filesToDownload;

                int totalFilesCount = javaFileTasks.Count(x => x.Type == "file");

                PathJavas = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Javas", javaName);
                if (totalBytesToDownload <= 0) totalBytesToDownload = 1;

                _ = Task.Run(async () =>
                {
                    try
                    {
                        using (var semaphore = new SemaphoreSlim(4))
                        {
                            var downloadTasks = new List<Task>();

                            foreach (var file in filesToDownload)
                            {
                                cancellationToken.ThrowIfCancellationRequested();

                                string pathFile = Path.Combine(PathJavas, file.Path);

                                if (file.Type == "directory")
                                {
                                    Directory.CreateDirectory(pathFile);
                                    continue;
                                }

                                if (file.Type == "file")
                                {
                                    string? parentDirectory = Path.GetDirectoryName(pathFile);
                                    if (!string.IsNullOrEmpty(parentDirectory)) Directory.CreateDirectory(parentDirectory);

                                    if (File.Exists(pathFile) && file.RawSha1 != null)
                                    {
                                        string localSha1 = _hashService.GetFileSha1(pathFile);
                                        if (localSha1 == file.RawSha1)
                                        {
                                            long current = Interlocked.Add(ref totalBytesDownloaded, file.RawSize);
                                            int currentCompleted = Interlocked.Increment(ref completedFilesCount);
                                            int percent = (int)((double)current * 100 / totalBytesToDownload);

                                            await channel.Writer.WriteAsync((Math.Clamp(percent, 0, 100), currentCompleted, totalFilesCount), cancellationToken);
                                            continue;
                                        }
                                    }

                                    downloadTasks.Add(Task.Run(async () =>
                                    {
                                        await semaphore.WaitAsync(cancellationToken);
                                        try
                                        {
                                            if (file.TypeFile == "lzma")
                                            {
                                                string lzmaPath = pathFile + ".lzma";

                                                await foreach (var bytesRead in _downloader.DownloadFileAsync(file.Url, lzmaPath, file.NetworkSha1, cancellationToken))
                                                {
                                                    long current = Interlocked.Add(ref totalBytesDownloaded, bytesRead);
                                                    int percent = (int)((double)current * 100 / totalBytesToDownload);
                                                    await channel.Writer.WriteAsync((Math.Clamp(percent, 0, 100), completedFilesCount, totalFilesCount), cancellationToken);
                                                }
                                                await _decompressor.UnzipFile(lzmaPath, pathFile, cancellationToken);

                                                if (File.Exists(lzmaPath)) File.Delete(lzmaPath);
                                            }
                                            else if (file.TypeFile == "raw")
                                            {
                                                await foreach (var bytesRead in _downloader.DownloadFileAsync(file.Url, pathFile, file.NetworkSha1, cancellationToken))
                                                {
                                                    long current = Interlocked.Add(ref totalBytesDownloaded, bytesRead);
                                                    int percent = (int)((double)current * 100 / totalBytesToDownload);
                                                    await channel.Writer.WriteAsync((Math.Clamp(percent, 0, 100), completedFilesCount, totalFilesCount), cancellationToken);
                                                }
                                            }
                                        }
                                        finally
                                        {
                                            int currentCompleted = Interlocked.Increment(ref completedFilesCount);
                                            semaphore.Release();

                                            int finalPercent = (int)((double)Interlocked.Read(ref totalBytesDownloaded) * 100 / totalBytesToDownload);
                                            await channel.Writer.WriteAsync((Math.Clamp(finalPercent, 0, 100), currentCompleted, totalFilesCount));
                                        }
                                    }, cancellationToken));
                                }
                            }
                            await Task.WhenAll(downloadTasks);
                        }

                        bool validate = await ValidateInstallation(filesToDownload, PathJavas);
                        if (validate) await channel.Writer.WriteAsync((100, totalFilesCount, totalFilesCount));
                        else await channel.Writer.WriteAsync((-1, completedFilesCount, totalFilesCount));
                    }
                    catch (OperationCanceledException)
                    {
                        if (Directory.Exists(PathJavas)) Directory.Delete(PathJavas, true);
                    }
                    catch (Exception)
                    {
                        await channel.Writer.WriteAsync((-1, completedFilesCount, totalFilesCount));
                    }
                    finally
                    {
                        channel.Writer.Complete();
                    }
                });
            }
            catch (HttpRequestException)
            {
                channel.Writer.Complete();
            }

            await foreach (var progressReport in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return progressReport;
            }
        }

        public async Task<List<JavaManager>> GetAllAvailableNamesJava()
        {
            List<JavaManager> availableJavaVersions = new();

            string ArchOS = GetPlatformString();
            if (ArchOS == "unknown") return availableJavaVersions;

            string responseJavaJson = await _client.GetStringAsync(UrlJavaManifestJson);
            var ManifestJavaJson = JsonSerializer.Deserialize<JavaManifest>(responseJavaJson);
            if (ManifestJavaJson == null) return availableJavaVersions;

            foreach (var entry in ManifestJavaJson[ArchOS])
            {
                if (entry.Key == "minecraft-java-exe") continue;

               foreach (var runtimeEntry in entry.Value)
                {
                    availableJavaVersions.Add(new JavaManager(new string(runtimeEntry.Version.Name.TakeWhile(char.IsDigit).ToArray()), entry.Key));
                }
            }
            var sorted = availableJavaVersions
                .DistinctBy(x => x.Version)
                .OrderByDescending(x => int.TryParse(x.Version, out int num) ? num : 0).ToList();

            return sorted;
        }

        public async Task<List<JavaInfoDownload>> GetAllInfoSelectedJava(string JavaArchitecture)
        {
            List<JavaInfoDownload>? InfoJava = new();

            string ArchOS = GetPlatformString();
            if (ArchOS == "unknown") return InfoJava;

            string responseJavaJson = await _client.GetStringAsync(UrlJavaManifestJson);
            var ManifestJavaJson = JsonSerializer.Deserialize<JavaManifest>(responseJavaJson);
            if (ManifestJavaJson == null) return InfoJava;

            foreach (var entry in ManifestJavaJson[ArchOS][JavaArchitecture])
            {

                InfoJava?.Add(new JavaInfoDownload(entry.Version.Name, JavaArchitecture, entry.Version.Released.Date.ToShortDateString(), "jre"));

            }
            /*            var sorted = InfoJava
                            .DistinctBy(x => x)
                            .OrderByDescending(x => int.TryParse(x.Version, out int num) ? num : 0).ToList();*/

            return InfoJava;
        }

        public async Task<List<JavaManager>> GetAllDownloadedJava()
        {
            List<JavaManager> javaManagers = new();

            string PathToFolderJavas = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Javas");

            string ArchOS = GetPlatformString();
            if (ArchOS == "unknown") return javaManagers;

            string responseJavaJson = await _client.GetStringAsync(UrlJavaManifestJson);
            var ManifestJavaJson = JsonSerializer.Deserialize<JavaManifest>(responseJavaJson);
            if (ManifestJavaJson == null) return javaManagers;

            foreach (var entry in ManifestJavaJson[ArchOS])
            {
                if (entry.Key == "minecraft-java-exe") continue;

                string PathJavas = Path.Combine(PathToFolderJavas, entry.Key, "bin", "javaw.exe");

                if(!Path.Exists(PathJavas)) continue;

                foreach (var runtimeEntry in entry.Value)
                {
                    javaManagers.Add(new JavaManager(entry.Key, runtimeEntry.Version.Name, ArchOS, PathJavas));
                }
            }

            return javaManagers;
        }

        public async Task DeletingJavaFolder(string? javaName)
        {
            string pathJavas = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Javas", javaName);

            if (Directory.Exists(pathJavas))
            {
                try
                {
                    Directory.Delete(pathJavas, true);
                }
                catch {}
            }
        }

        private async Task<(List<JavaFileTask> Tasks, long TotalSize)> GetAllDownloadableFiles(string? urlJava)
        {
            var tasks = new List<JavaFileTask>();
            long totalSize = 0;

            if (string.IsNullOrEmpty(urlJava)) return (tasks, 0);

            string responseSelectedJavaJson = await _client.GetStringAsync(urlJava);
            var ManifestSelectedJavaJson = JsonSerializer.Deserialize<JavaFilesManifest>(responseSelectedJavaJson);

            if (ManifestSelectedJavaJson?.Files == null) return (tasks, 0);

            foreach (var file in ManifestSelectedJavaJson.Files)
            {
                if (file.Value.Type == "directory")
                {
                    tasks.Add(new JavaFileTask
                    {
                        Type = file.Value.Type,
                        Path = file.Key,
                        Url = "",
                        LzmaSize = 0,
                        RawSize = 0,
                        NetworkSha1 = "",
                        RawSha1 = "",
                        TypeFile = ""
                    });
                    continue;
                }

                if (file.Value.Type == "file")
                {
                    if (file.Value.Downloads == null) continue;

                    file.Value.Downloads.TryGetValue("raw", out DownloadInfo? rawDownload);
                    file.Value.Downloads.TryGetValue("lzma", out DownloadInfo? lzmaDownload);

                    if (lzmaDownload != null)
                    {
                        tasks.Add(new JavaFileTask
                        {
                            Type = file.Value.Type,
                            Path = file.Key,
                            Url = lzmaDownload.Url,
                            LzmaSize = lzmaDownload.Size,
                            RawSize = rawDownload?.Size ?? 0,
                            NetworkSha1 = lzmaDownload.Sha1,
                            RawSha1 = rawDownload?.Sha1 ?? "",
                            TypeFile = "lzma"
                        });
                        totalSize += lzmaDownload.Size;

                    }
                    else if (rawDownload != null)
                    {
                        tasks.Add(new JavaFileTask
                        {
                            Type = file.Value.Type,
                            Path = file.Key,
                            Url = rawDownload.Url,
                            LzmaSize = 0,
                            RawSize = rawDownload.Size,
                            NetworkSha1 = rawDownload.Sha1,
                            RawSha1 = rawDownload.Sha1,
                            TypeFile = "raw"
                        });
                        totalSize += rawDownload.Size;
                    }
                }
            }

            return (tasks, totalSize);
        }

        private string GetPlatformString()
        {
            Architecture arch = RuntimeInformation.OSArchitecture;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return arch switch
                {
                    Architecture.X64 => "windows-x64",
                    Architecture.X86 => "windows-x86",
                    Architecture.Arm64 => "windows-arm64",
                    _ => "windows"
                };
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return arch switch
                {
                    Architecture.Arm64 => "mac-os-arm64",
                    _ => "mac-os"
                };
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return arch switch
                {
                    Architecture.X86 => "linux-i386",
                    _ => "linux"
                };
            }

            return "unknown";
        }

        public async Task<bool> ValidateInstallation(List<JavaFileTask> expectedFiles, string javaFolderPath)
        {
            if (!Directory.Exists(javaFolderPath)) return false;

            bool hasCorruptedLzma = Directory.GetFiles(javaFolderPath, "*.lzma", SearchOption.AllDirectories).Any();
            if (hasCorruptedLzma) return false;

            foreach (var file in expectedFiles)
            {
                if (file.Type != "file") continue;

                string localFilePath = Path.Combine(javaFolderPath, file.Path);

                if (!File.Exists(localFilePath)) return false;

                var fileInfo = new FileInfo(localFilePath);
                if (fileInfo.Length != file.RawSize) return false;

                string localSha1 = _hashService.GetFileSha1(localFilePath);
                if (localSha1 != file.RawSha1) return false;
            }

            return true;
        }

        public async Task<bool> ValidateDownloadedJava(string? javaName)
        {
            string responseJavaJson = await _client.GetStringAsync(UrlJavaManifestJson);
            var ManifestJavaJson = JsonSerializer.Deserialize<JavaManifest>(responseJavaJson);
            if (ManifestJavaJson == null) { return false; }

            string responseMinecraftJson = await _client.GetStringAsync(UrlManifestJson);
            var ManifestMinecraftJson = JsonSerializer.Deserialize<MinecraftManifest>(responseMinecraftJson);
            if (ManifestMinecraftJson == null) { return false; }

            string ArchOS = GetPlatformString();
            string? urlSelectedJavaVersion = ManifestJavaJson[ArchOS][javaName].Select(entry => entry.Manifest.Url).FirstOrDefault() ?? string.Empty;

            var (filesToDownload, totalBytesToDownload) = await GetAllDownloadableFiles(urlSelectedJavaVersion);

            string PathJavas = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Javas", javaName);

            bool validate = await ValidateInstallation(filesToDownload, PathJavas);
            if (validate) return true;
            else return false;
        }
    }
}
