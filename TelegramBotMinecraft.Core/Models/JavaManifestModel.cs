using System.Text.Json.Serialization;

namespace TelegramBotMinecraft.Core.Models
{
    public class JavaManifest : Dictionary<string, Dictionary<string, List<JavaRuntimeEntry>>> { }

    public class JavaRuntimeEntry
    {
        [JsonPropertyName("availability")]
        public Availability Availability { get; set; } = new();

        [JsonPropertyName("manifest")]
        public JavaRuntimeManifest Manifest { get; set; } = new();

        [JsonPropertyName("version")]
        public JavaRuntimeVersion Version { get; set; } = new();
    }

    public class Availability
    {
        [JsonPropertyName("group")]
        public int Group { get; set; }

        [JsonPropertyName("progress")]
        public int Progress { get; set; }
    }
    public class JavaRuntimeManifest
    {
        [JsonPropertyName("sha1")]
        public string Sha1 { get; set; } = string.Empty;

        [JsonPropertyName("size")]
        public long Size { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
    public class JavaRuntimeVersion
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("released")]
        public DateTime Released { get; set; }
    }
}
