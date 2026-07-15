namespace TelegramBotMinecraft.Core.Models
{
    public class Setting
    {
        public int Id { get; set; }
        public string? BotToken { get; set; }
        public int AutoBot { get; set; } = 1;
        public int TrayOnStart { get; set; } = 0;
        public int RunAtStartup { get; set; } = 0;
        public int AutoReconnect { get; set; } = 1;
        public int? Notifications { get; set; } = 1;
        public string? ProxyHost { get; set; }
        public string? ProxyPort { get; set; }
        public string? ProxyUsername { get; set; }
        public string? ProxyPassword { get; set; }

        public Setting(int id, string? botToken, int autoBot, int trayOnStart, int runAtStartup,
                  int autoReconnect, int? notifications, string? proxyHost, string? proxyPort,
                  string? proxyUsername, string? proxyPassword)
        {
            Id = id;
            BotToken = botToken;
            AutoBot = autoBot;
            TrayOnStart = trayOnStart;
            RunAtStartup = runAtStartup;
            AutoReconnect = autoReconnect;
            Notifications = notifications;
            ProxyHost = proxyHost;
            ProxyPort = proxyPort;
            ProxyUsername = proxyUsername;
            ProxyPassword = proxyPassword;
        }
    }
}
