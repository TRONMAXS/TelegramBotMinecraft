namespace TelegramBotMinecraft.Core.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public string? BotToken { get; set; }
        public int Auto_Bot { get; set; } = 1;
        public int TrayOnStart { get; set; } = 0;
        public int RunAtStartup { get; set; } = 0;
        public int AutoReconnect { get; set; } = 1;
        public int? Notifications { get; set; } = 1;
        public string? Proxy_Host { get; set; }
        public string? Proxy_Port { get; set; }
        public string? Proxy_Username { get; set; }
        public string? Proxy_Password { get; set; }
    }
}
