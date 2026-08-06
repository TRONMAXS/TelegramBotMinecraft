using System.Runtime.InteropServices;
using System.Text.Json;
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


        public async Task JavaDownloader(string? javaName)
        {
            try
            {
                string responseJavaJson = await _client.GetStringAsync(UrlJavaManifestJson);
                var ManifestJavaJson = JsonSerializer.Deserialize<JavaManifest>(responseJavaJson);
                if (ManifestJavaJson == null) return;

                string responseMinecraftJson = await _client.GetStringAsync(UrlManifestJson);
                var ManifestMinecraftJson = JsonSerializer.Deserialize<MinecraftManifest>(responseMinecraftJson);
                if (ManifestMinecraftJson == null) return;

  /*              string? urlSelectedVersion = ManifestMinecraftJson.versions.FirstOrDefault(v => v.id == VersionSelected)?.url;
                if (urlSelectedVersion == null) return;*/

/*                string responseМуVerisonJson = await _client.GetStringAsync(urlSelectedVersion);
                var ManifestVersionJson = JsonSerializer.Deserialize<VersionManifest>(responseМуVerisonJson);
                if (ManifestVersionJson == null) return;*/

                //int javaVersion = ManifestVersionJson.JavaVersion?.MajorVersion ?? 8;

                string ArchOS = GetPlatformString();

                string? urlSelectedJavaVersion = ManifestJavaJson[ArchOS][javaName].Select(entry => entry.Manifest.Url).FirstOrDefault() ?? string.Empty;

                string responseSelectedJavaJson = await _client.GetStringAsync(urlSelectedJavaVersion);
                var ManifestSelectedJavaJson = JsonSerializer.Deserialize<JavaFilesManifest>(responseSelectedJavaJson);
                if (ManifestSelectedJavaJson == null) return;

                string PathJavas = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Javas", javaName);
                try
                {
                    using (var semaphore = new SemaphoreSlim(4))
                    {
                        var downloadTasks = new List<Task>();

                        foreach (var file in ManifestSelectedJavaJson.Files)
                        {
                            string pathFile = Path.Combine(PathJavas, file.Key);

                            if (file.Value.Type == "directory")
                            {
                                Directory.CreateDirectory(pathFile);
                                continue;
                            }

                            if (file.Value.Type == "file")
                            {
                                if (file.Value.Downloads == null) continue;

                                string? parentDirectory = Path.GetDirectoryName(pathFile);
                                if (!string.IsNullOrEmpty(parentDirectory)) Directory.CreateDirectory(parentDirectory);

                                file.Value.Downloads.TryGetValue("raw", out DownloadInfo? rawDownload);

                                if (File.Exists(pathFile) && rawDownload != null)
                                {
                                    string localSha1 = _hashService.GetFileSha1(pathFile);
                                    if (localSha1 == rawDownload.Sha1) continue;
                                }

                                downloadTasks.Add(Task.Run(async () =>
                                {
                                    await semaphore.WaitAsync();
                                    try
                                    {
                                        if (file.Value.Downloads.TryGetValue("lzma", out DownloadInfo? lzmaDownload) && lzmaDownload != null)
                                        {
                                            string lzmaPath = pathFile + ".lzma";

                                            await _downloader.DownloadFileAsync(lzmaDownload.Url, lzmaPath, lzmaDownload.Sha1);
                                            await _decompressor.UnzipFile(lzmaPath, pathFile);
                                        }
                                        else if (rawDownload != null)
                                        {

                                            await _downloader.DownloadFileAsync(rawDownload.Url, pathFile, rawDownload.Sha1);
                                        }
                                    }
                                    finally
                                    {
                                        semaphore.Release();
                                    }
                                }));
                            }
                        }
                        await Task.WhenAll(downloadTasks);
                        //Console.WriteLine($"Java {javaName} скачана");
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"\n[Ошибка обработки файла: {ex.Message}");
                }
            }
            catch (HttpRequestException e)
            {
                //Console.WriteLine("\nException Caught!");
                //Console.WriteLine("Message :{0} ", e.Message);
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
    }
}
