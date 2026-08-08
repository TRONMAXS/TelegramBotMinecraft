using System.Text.Json.Serialization;

namespace TelegramBotMinecraft.Core.Models
{
    public class JavaFilesManifest
    {
        [JsonPropertyName("files")]
        public Dictionary<string, FileDownload>? Files { get; set; }
    }

    public class FileDownload
    {
        [JsonPropertyName("downloads")]
        public Dictionary<string, DownloadInfo>? Downloads { get; set; }

        [JsonPropertyName("executable")]
        public bool? Executable { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
    public class DownloadInfo
    {
        [JsonPropertyName("sha1")]
        public string? Sha1 { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
