using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace TelegramBotMinecraft
{
    static class Program
    {
        private static string pathServers;
        private static string pathSettings;
        private static string pathUserSettings;
        private static string pathServersBackup;
        private static string pathSettingsBackup;
        private static string pathUserSettingsBackup;


        static JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        [STAThread]
        static void Main()
        {
            string appName = "TelegramBotMinecraft";
            if (IsAppAlreadyRunning(appName))
            {
                MessageBox.Show("Бот уже запущен!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            pathServers = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Servers.json");
            pathSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
            pathUserSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserSettings.json");
            pathServersBackup = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServersBackup.json");
            pathSettingsBackup = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SettingsBackup.json");
            pathUserSettingsBackup = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserSettingsBackup.json");


            LoadJsons();
            CheckAndFixSettingsJson();
            CheckAndFixServersJson();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public static bool IsAppAlreadyRunning(string appName)
        {
            Process[] processes = Process.GetProcessesByName(appName);
            return processes.Length > 1;
        }

        static void LoadJsons()
        {

            if (!File.Exists(pathServersBackup) || new FileInfo(pathServersBackup).Length == 0)
            {
                var servers = new List<ServerConfig>
                {
                    new ServerConfig
                    {
                        Name = "Name Server (example:  Vanilla(Survival) - 1.20.1)",
                        StartupPath = @"Path to the startup file (example:  G:\MinecraftServers\Survival\srart.bat)",
                        FolderPath = @"Path to the server folder (example:  G:\MinecraftServers\Survival\)",
                        Ip = "server ip (example:  127.0.0.1)",
                        RconPort = "rcon port (example:  25565)",
                        RconPassword = "rcon password(example:  12345)",
                        ConnectIp = "connect ip (example: 127.0.0.1)",
                        Port = "server port (example:  25565)"
                    }
                };

                string json = JsonSerializer.Serialize(servers, options);
                File.WriteAllText(pathServersBackup, json);
            }
            if (!File.Exists(pathSettingsBackup) || new FileInfo(pathSettingsBackup).Length == 0)
            {
                var settings = new List<SettingsConfig>
                {
                    new SettingsConfig
                    {
                        Notifications = true,
                        BotToken = "Your bot token (example:  123456789:ABCdefGHIjklMNOpqrSTUvwxYZ)",
                        AdminPassword = "Admin Password (example:  12345)",
                        ChatIds = new List<ChatId>
                        {
                            new ChatId { Identifier = "example: 646516246", Name = "example: Admin" },
                        }
                    }
                };

                string json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettingsBackup, json);
            }
            if (!File.Exists(pathUserSettingsBackup) || new FileInfo(pathUserSettingsBackup).Length == 0)
            {
                var settings = new List<UserSettings>
                {
                    new UserSettings
                    {
                        AllowedCommands = new List<AllowedCommands>
                        {
                            new AllowedCommands { Command = "example: /start_server"}
                        },
                        AllowedServers = new List<AllowedServers>
                        {
                            new AllowedServers { Enabled = true, Server = "example: Vanilla(Survival) - 1.20.1"}
                        },
                        Identifier = "example: 646516246",
                        Name = "example: Admin",
                    }
                };

                string json = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathUserSettingsBackup, json);
            }

            File.Copy(pathUserSettingsBackup, pathUserSettings, true);
            File.Copy(pathServersBackup, pathServers, true);
            File.Copy(pathSettingsBackup, pathSettings, true);
        }
        static void CheckAndFixSettingsJson()
        {
            if (!File.Exists(pathSettings) || new FileInfo(pathSettings).Length == 0)
                return;

            try
            {
                string json = File.ReadAllText(pathSettings);
                var settings = JsonSerializer.Deserialize<List<SettingsConfig>>(json);

                bool changed = false;
                if (settings == null || settings.Count == 0)
                    return;

                var config = settings[0];

                if (config.Notifications == null)
                {
                    config.Notifications = true;
                    changed = true;
                }
                if (string.IsNullOrWhiteSpace(config.BotToken))
                {
                    config.BotToken = "Your bot token (example:  123456789:ABCdefGHIjklMNOpqrSTUvwxYZ)";
                    changed = true;
                }

                if (string.IsNullOrWhiteSpace(config.AdminPassword))
                {
                    config.AdminPassword = "Admin Password (example:  12345)";
                    changed = true;
                }

                if (config.ChatIds == null || config.ChatIds.Count == 0)
                {
                    config.ChatIds = new List<ChatId>
                    {
                        new ChatId { Identifier = "example: 646516246", Name = "example: Admin" },
                    };
                    changed = true;
                }
                else
                {
                    foreach (var chatId in config.ChatIds)
                    {
                        if (string.IsNullOrWhiteSpace(chatId.Identifier) || string.IsNullOrWhiteSpace(chatId.Name))
                        {
                            chatId.Identifier = "example: 646516246";
                            chatId.Name = "example: Admin";
                            changed = true;
                        }
                    }
                }


                if (changed)
                {
                    string newJson = JsonSerializer.Serialize(settings, options);
                    File.WriteAllText(pathSettings, newJson);
                }
            }
            catch { }
           
        }
        static void CheckAndFixServersJson()
        {
            if (!File.Exists(pathServers) || new FileInfo(pathServers).Length == 0)
                return;

            try
            {

                string json = File.ReadAllText(pathServers);
                var servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);

                bool changed = false;
                if (servers == null || servers.Count == 0)
                    return;

                foreach (var server in servers)
                {
                    if (string.IsNullOrWhiteSpace(server.Name))
                    {
                        server.Name = "Name Server (example:  Vanilla(Survival) - 1.20.1)";
                        changed = true;
                    }
                    if (string.IsNullOrWhiteSpace(server.StartupPath))
                    {
                        server.StartupPath = @"Path to the startup file (example:  G:\\MinecraftServers\Survival\\srart.bat)";
                        changed = true;
                    }
                    if (string.IsNullOrWhiteSpace(server.FolderPath))
                    {
                        server.FolderPath = @"Path to the server folder (example:  G:\\MinecraftServers\Survival\\)";
                        changed = true;
                    }
                    if (string.IsNullOrWhiteSpace(server.Ip))
                    {
                        server.Ip = "server ip (example:  127.0.0.1)";
                        changed = true;
                    }
                    if (string.IsNullOrWhiteSpace(server.RconPort))
                    {
                        server.RconPort = "rcon port (example:  25565)";
                        changed = true;
                    }
                    if (string.IsNullOrWhiteSpace(server.RconPassword))
                    {
                        server.RconPassword = "rcon password(example:  12345)";
                        changed = true; 
                    }
                    if (string.IsNullOrWhiteSpace(server.ConnectIp))
                    {
                        server.ConnectIp = "connect ip (example: 127.0.0.1)";
                        changed = true;
                    }
                    if (string.IsNullOrWhiteSpace(server.Port))
                    {
                        server.Port = "server port (example:  25565)";
                        changed = true;
                    }
                }

                if (changed)
                {
                    string newJson = JsonSerializer.Serialize(servers, options);
                    File.WriteAllText(pathServers, newJson);
                }
            }
            catch { }
        }
    }
}
