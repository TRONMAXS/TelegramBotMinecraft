using System.Text.Json.Serialization;

namespace TelegramBotMinecraft.Core.Models
{
    public class MinecraftManifest
    {
        [JsonPropertyName("latest")]
        public Latest? Latest { get; set; }

        [JsonPropertyName("versions")]
        public Version[]? Versions { get; set; }
    }

    public class Latest
    {
        [JsonPropertyName("release")]
        public string? Release { get; set; }

        [JsonPropertyName("snapshot")]
        public string? Snapshot { get; set; }
    }
    public class Version
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }
}
