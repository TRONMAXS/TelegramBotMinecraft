namespace TelegramBotMinecraft.Core.Models
{
    public class JavaInfoDownload
    {
       public int? Id { get; set; }
        public string? Version { get; set; }
        public string? Name { get; set; }
        public string? ReleasData { get; set; }
        public string? Type { get; set; }

        public JavaInfoDownload() { }

        public JavaInfoDownload(string? version, string? name, string? releasData, string? type)
        {
            Version = version;
            Name = name;
            ReleasData = releasData;
            Type = type;
        }
    }
}
