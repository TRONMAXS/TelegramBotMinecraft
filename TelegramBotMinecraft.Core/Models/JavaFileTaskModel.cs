namespace TelegramBotMinecraft.Core.Models
{
    public class JavaFileTask
    {
        public string? Type { get; init; }
        public string? Path { get; init; }
        public string? Url { get; init; }
        public long LzmaSize { get; init; }
        public long RawSize { get; set; }
        public string? NetworkSha1 { get; set; }
        public string? RawSha1 { get; set; }
        public string? TypeFile { get; init; }
    }
}
