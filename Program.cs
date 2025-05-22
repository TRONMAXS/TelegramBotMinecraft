using System.Text.Json;

namespace TelegramBotMinecraft
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            LoadJsons(); // Загружаем JSON файлы при запуске программы


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Бот стартует из Form1
        }

        static void LoadJsons()
        {
            string pathServers = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Servers.json");
            string pathSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");

            if (!File.Exists(pathServers) || new FileInfo(pathServers).Length == 0)
            {
                var servers = new List<ServerConfig>
                {
                    new ServerConfig
                    {
                        Name = "Name Server",
                        Path = @"Path to the server folder (example:  G:\MinecraftServers\Survival)",
                        Ip = "server ip (example:  127.0.0.1)",
                        RconPort = "25565",
                        RconPassword = "Rcon Password",
                        Enabled = true
                    }
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = System.Text.Json.JsonSerializer.Serialize(servers, options);
                File.WriteAllText(pathServers, json);
            }
            if (!File.Exists(pathSettings) || new FileInfo(pathSettings).Length == 0)
            {
                var settings = new List<SettingsConfig>
                {
                    new SettingsConfig
                    {
                        Notifications = true,
                        BotToken = "Your bot token (example:  123456789:ABCdefGHIjklMNOpqrSTUvwxYZ)"
                    }
                };

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = System.Text.Json.JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, json);
            }

        }
    }
}
