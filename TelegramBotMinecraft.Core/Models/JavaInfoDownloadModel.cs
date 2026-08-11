namespace TelegramBotMinecraft.Core.Models
{
    public class JavaManager
    {
        public string? Name { get; set; }
        public string? Version { get; set; }
        public string? Architecture { get; set; }
        public string? Path { get; set; }

        public JavaManager() { }

        public JavaManager(string? name, string? version, string? architecture, string? path)
        {
            Name = name;
            Version = version;
            Architecture = architecture;
            Path = path;
        }

        public JavaManager(string? version, string? architecture, string? path)
        {
            Version = version;
            Architecture = architecture;
            Path = path;
        }

        public JavaManager(string? version, string? architecture)
        {
            Version = version;
            Architecture = architecture;
        }
        public JavaManager(string? version)
        {
            Version = version;
        }
    }
}
