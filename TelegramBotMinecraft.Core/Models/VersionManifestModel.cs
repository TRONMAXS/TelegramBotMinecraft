using System.Text.Json.Serialization;

namespace TelegramBotMinecraft.Core.Models
{
    public class VersionManifest
    {
        [JsonPropertyName("javaVersion")]
        public JavaVersion? JavaVersion { get; set; }
    }

    public class JavaVersion
    {
        [JsonPropertyName("component")]
        public string? Component { get; set; }

        [JsonPropertyName("majorVersion")]
        public int? MajorVersion { get; set; }
    }
}
