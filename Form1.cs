﻿using CoreRCON;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


namespace TelegramBotMinecraft
{
    public partial class Form1 : Form
    {
        private TelegramBotClient botClient;
        private CancellationTokenSource cts = new CancellationTokenSource();

        private bool errorSettingsJson = false;
        private bool errorServersJson = false;
        private bool errorUserSettingsJson = false;
        private bool errorReloadReconnect = false;

        private readonly string pathUserSettings;
        private string jsonUserSettings;

        private readonly string pathServersBackup;
        private readonly string pathSettingsBackup;
        private readonly string pathUserSettingsBackup;

        private bool SendErrorMessageJsonSettings = false;
        private bool CheckTokenJsonSettings = false;
        private bool CheckChatIdsJsonSettings = false;
        private bool SendErrorMessageJsonServers = false;
        private bool CheckErrorJsonServers = false;

        private bool fatalJsonFilesError = false;

        private readonly string pathServers;
        private string json;
        private readonly string pathSettings;
        private string jsonSettings;
        private string jsonServers;
        private List<SettingsConfig> settings = new List<SettingsConfig>();
        private List<ServerConfig> servers = new List<ServerConfig>();
        private string BotToken;
        private int CheckEnableServer = -1;
        private bool flagStartCheck = true;
        private bool StartCheck = false;
        private static readonly HashSet<long> AllowedUsers = new HashSet<long> { };
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        private List<bool> serversRunning;
        private int serverNumber = 0;
        private bool StartBot = true;
        private bool botStartedForm1_Load = false;
        private bool MessageShow = true;
        private static readonly int ThreadId = 2258;
        RCON rcon;
        RCON rconCheckingServers;


        private Form2 form2;
        private Form3 form3;
        private Form5 form5;


        private ContextMenuStrip notifyIconMenu;
        private ToolStripMenuItem exitMenuItem;

        private readonly Dictionary<string, string> AllowedCommands = new Dictionary<string, string>()
        {
            { "/list_servers", "показывает список доступных серверов Minecraft" },
            { "/select_server",  " <password> <number> переключиться указанный сервер Minecraft" },
            { "/start_server", "запускает выбранный сервер" },
            { "/check_servers_status", "проверяет статус серверов" },
            { "/players_count", "показывает количество игроков на выбранном сервере" },
            { "/stop_server", "остановливает выбранный сервер" },
            { "/server_command", "<password> <command> выполняет написанную команду на выбранном сервере" },
            { "/delete_world", "удаляет мир на выбранном сервере" }
        };

        private Dictionary<int, bool> serverStarting = new Dictionary<int, bool>();

        public Form1()
        {
            InitializeComponent();
            pathServers = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Servers.json");
            json = File.ReadAllText("Servers.json");
            servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);

            pathSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
            jsonSettings = File.ReadAllText("Settings.json");
            settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);

            pathServersBackup = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServersBackup.json");
            pathSettingsBackup = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SettingsBackup.json");
            pathUserSettingsBackup = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserSettingsBackup.json");

            pathUserSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserSettings.json");
            jsonUserSettings = File.ReadAllText(pathUserSettings);

            serversRunning = new List<bool>(new bool[servers.Count]);

            this.Load += Form1_Load;
            this.Resize += Form1_Resize;

            button1.Click += ButtonSettingsForm;
            button2.Click += ButtonServersForm;
            button3.Click += ButtonReloadReconnect;
            button4.Click += ButtonRestoreFileSettings;
            button5.Click += ButtonRestoreFileServers;
            button6.Click += ButtonRestoreFileUserSettings;
            label1.Click += LabelInfo;

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide();

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Открыть", null, (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.BringToFront();
            });
            contextMenu.Items.Add("Выход", null, (s, e) => ExitMenuItem_Click(s, e));
            notifyIcon1.ContextMenuStrip = contextMenu;

            _ = ShowBalloonTip("Приложение запущено", "Ожидает команд через Telegram.");
        }

        private async void ExitMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                await SaveJsonFiles();
                Application.Exit();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.FormClosing += Form1_FormClosing;
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            e.Cancel = true;


            bool Closing = await SaveJsonFiles();

            this.FormClosing -= Form1_FormClosing;
            this.Close();
        }

        private async Task<bool> SaveJsonFiles()
        {
            int completed = 0;
            int notCompleted = 0;

            try
            {
                try
                {
                    string jsonTestSave = File.ReadAllText(pathServers);
                    var objTestSave = JsonSerializer.Deserialize<List<ServerConfig>>(jsonTestSave);
                    File.Copy(pathServers, pathServersBackup, true);

                    if (File.Exists(pathServersBackup) && new FileInfo(pathServersBackup).Length != 0)
                    {
                        completed++;
                    }
                    else
                    {
                        notCompleted++;
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла Servers.json: {ex.Message}", "Ошибка при сохранении файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    notCompleted++;
                }

                try
                {
                    string jsonTestSave = File.ReadAllText(pathSettings);
                    var objTestSave = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonTestSave);
                    File.Copy(pathSettings, pathSettingsBackup, true);

                    if (File.Exists(pathSettingsBackup) && new FileInfo(pathSettingsBackup).Length != 0)
                    {
                        completed++;
                    }
                    else
                    {
                        notCompleted++;
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла Settings.json: {ex.Message}", "Ошибка при сохранении файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    notCompleted++;
                }

                try
                {
                    string jsonTestSave = File.ReadAllText(pathUserSettings);
                    var objTestSave = JsonSerializer.Deserialize<List<UserSettings>>(jsonTestSave);
                    File.Copy(pathUserSettings, pathUserSettingsBackup, true);

                    if (File.Exists(pathUserSettingsBackup) && new FileInfo(pathUserSettingsBackup).Length != 0)
                    {
                        completed++;
                    }
                    else
                    {
                        notCompleted++;
                    }
                }
                catch (JsonException ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файла UserSettings.json: {ex.Message}", "Ошибка при сохранении файла", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    notCompleted++;
                }
            }
            catch { }
            if (completed > 0 && notCompleted == 0)
            {
                File.Delete(pathServers);
                File.Delete(pathSettings);
                File.Delete(pathUserSettings);
                return true;
            }
            else if (notCompleted > 0)
            {
                AppendText($"Не удалось сохранить некоторые файлы. Проверьте наличие файлов и их содержимое.");
                await ShowBalloonTip("Ошибка сохранения файлов", "Не удалось сохранить некоторые файлы. Проверьте наличие файлов и их содержимое.");
                return false;
            }
            return false;
        }

        private async Task ShowBalloonTip(string title, string text)
        {
            jsonSettings = File.ReadAllText("Settings.json");
            settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);
            if (settings[0].Notifications == true)
            {
                notifyIcon1.BalloonTipText = text;
                notifyIcon1.BalloonTipTitle = title;
                notifyIcon1.ShowBalloonTip(2000);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                this.Hide();
            }
        }

        private void LabelInfo(object sender, EventArgs e)
        {
            if (form5 == null || form5.IsDisposed)
            {
                form5 = new Form5();
            }
            form5.Show();
            form5.BringToFront();
            form5.WindowState = FormWindowState.Normal;
            form5.Update();
        }

        private void ButtonSettingsForm(object sender, EventArgs e)
        {
            if (form3 == null || form3.IsDisposed)
            {
                form3 = new Form3();
            }
            form3.Show();
            form3.BringToFront();
            form3.WindowState = FormWindowState.Normal;
            form3.Update();
        }

        private void ButtonServersForm(object sender, EventArgs e)
        {
            if (form2 == null || form2.IsDisposed)
            {
                form2 = new Form2();
            }
            form2.Show();
            form2.BringToFront();
            form2.WindowState = FormWindowState.Normal;
            form2.Update();
        }

        private async void ButtonReloadReconnect(object sender, EventArgs e)
        {
            fatalJsonFilesError = false;
            Form1_Load(this, EventArgs.Empty);
        }

        private async void ButtonRestoreFileSettings(object sender, EventArgs e)
        {
            await LoadBackupJson(DialogResult.Yes, "Settings.json");
        }

        private async void ButtonRestoreFileServers(object sender, EventArgs e)
        {
            await LoadBackupJson(DialogResult.Yes, "Servers.json");
        }

        private async void ButtonRestoreFileUserSettings(object sender, EventArgs e)
        {
            await LoadBackupJson(DialogResult.Yes, "UserSettings.json");
        }

        private void UpdateErrorButtons()
        {
            this.Invoke(new Action(() =>
            {
                button3.Enabled = errorReloadReconnect;
                button3.Visible = errorReloadReconnect;

                button5.Enabled = errorServersJson;
                button5.Visible = errorServersJson;

                button6.Enabled = errorUserSettingsJson;
                button6.Visible = errorUserSettingsJson;

                button4.Enabled = errorSettingsJson;
                button4.Visible = errorSettingsJson;

            }));

        }

        private async void Form1_Load(object sender, EventArgs e)
        {

            try
            {

                for (int i = 0; i < settings[0].ChatIds.Count; i++)
                {
                    if (long.TryParse(settings[0].ChatIds[i].Identifier, out long chatId))
                    {
                        AllowedUsers.Add(chatId);
                    }
                }

                errorServersJson = false;
                errorSettingsJson = false;
                errorUserSettingsJson = false;
                errorReloadReconnect = false;
                UpdateErrorButtons();

                bool CheckSettings = false;
                bool CheckServers = false;
                while (!fatalJsonFilesError)
                {
                    CheckSettings = await CheckJsonSettings();
                    CheckServers = await CheckJsonServers();
                    if (CheckSettings == true && CheckServers == true)
                    {
                        CheckTokenJsonSettings = false;
                        CheckChatIdsJsonSettings = false;
                        CheckErrorJsonServers = false;

                        if (!botStartedForm1_Load)
                        {
                            fatalJsonFilesError = false;
                            StartCheck = true;
                            StartBot = true;
                            botStartedForm1_Load = true;
                            _ = RunBotLoopAsync();
                        }
                        break;
                    }
                    else
                    {
                        fatalJsonFilesError = true;
                        StartCheck = false;
                        StartBot = false;
                        await Task.Delay(1000);
                    }
                }

                if (CheckEnableServer != -1)
                {
                    var newRcon = await ConnectToRconAsync(servers[CheckEnableServer]);
                    if (newRcon != null)
                    {
                        rcon = newRcon;
                    }
                }

                _ = Task.Run(() => CheckJson());
                _ = Task.Run(() => CheckRconAsync());
            }
            catch { }
        }

        private async Task CheckJson()
        {
            while (StartCheck)
            {
                _ = Task.Run(() => CheckJsonSettings());
                _ = Task.Run(() => CheckJsonServers());

                await Task.Delay(5000);
            }
        }

        private async Task RunBotLoopAsync()
        {
            while (!fatalJsonFilesError)
            {
                try
                {
                    await StartBotAsync();
                    break;
                }
                catch (Exception ex)
                {
                    AppendText($"Бот завершил работу: {ex.Message}.");
                    fatalJsonFilesError = true;
                    botStartedForm1_Load = false;
                    await Task.Delay(5000);
                }
            }
        }

        private async Task StartBotAsync()
        {
            cts = new CancellationTokenSource();

            if (StartBot == true)
            {
                BotToken = settings[0].BotToken;
                botClient = new TelegramBotClient(BotToken);
                var me = await botClient.GetMe();
                AppendText($"Бот {me.Username} запущен и готов к работе.");

                botClient.StartReceiving(
                    new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync),
                    cancellationToken: cts.Token
                );
                StartBot = false;

            }
            await Task.Delay(Timeout.Infinite, cts.Token);
        }

        private async Task LoadBackupJson(DialogResult result, string jsonName)
        {
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (jsonName == "Servers.json")
                    {
                        File.Copy(pathServersBackup, pathServers, true);
                        json = File.ReadAllText(pathServers);
                        servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);
                        AppendText($"Файл Servers.json был восстановлен из резервной копии.");
                    }
                    else if (jsonName == "Settings.json")
                    {
                        File.Copy(pathSettingsBackup, pathSettings, true);
                        jsonSettings = File.ReadAllText(pathSettings);
                        settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);
                        AppendText($"Файл Settings.json был восстановлен из резервной копии.");
                    }
                    else if (jsonName == "UserSettings.json")
                    {
                        File.Copy(pathUserSettingsBackup, pathUserSettings, true);
                        jsonUserSettings = File.ReadAllText(pathUserSettings);
                        AppendText($"Файл UserSettings.json был восстановлен из резервной копии.");
                    }
                }
                catch (Exception ex)
                {
                    AppendText($"Ошибка при восстановлении файла {jsonName}: {ex.Message}");
                }
            }
        }

        private async Task<bool> CheckJsonServers()
        {
            int countFalse = 0;

            try
            {
                jsonServers = File.ReadAllText(pathServers);
                servers = JsonSerializer.Deserialize<List<ServerConfig>>(jsonServers);
                SendErrorMessageJsonServers = false;
            }
            catch (Exception ex)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;

                if (!SendErrorMessageJsonServers)
                {
                    SendErrorMessageJsonServers = true;
                    AppendText("Проверьте файл Servers.json на наличие ошибок.");
                    var result = MessageBox.Show(
                        $"Ошибка в чтении файла Servers.json: \n{ex.Message}\n\nПерезаписать файл, используя старую рабочую версию?",
                        "Ошибка в файле Servers.json",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);
                    await LoadBackupJson(result, "Servers.json");
                }

                errorServersJson = true;
                errorReloadReconnect = true;
                UpdateErrorButtons();

                fatalJsonFilesError = true;
                return false;
            }

            if (servers == null || servers.Count == 0)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;

                errorServersJson = true;
                errorReloadReconnect = true;
                UpdateErrorButtons();

                AppendText("Файл Servers.json пуст или повреждён.");
                fatalJsonFilesError = true;
                return false;
            }

            bool hasCriticalError =
                servers[0].StartupPath == @"Path to the startup file (example:  G:\MinecraftServers\Survival\srart.bat)" ||
                servers[0].FolderPath == @"Path to the server folder (example:  G:\MinecraftServers\Survival\)" ||
                countFalse == servers.Count ||
                servers.Any(c => string.IsNullOrEmpty(c.Name) || string.IsNullOrEmpty(c.StartupPath)) ||
                servers.Any(c => string.IsNullOrEmpty(c.Ip) || string.IsNullOrEmpty(c.RconPort)) ||
                servers.Any(c => string.IsNullOrEmpty(c.RconPassword) || string.IsNullOrEmpty(c.ConnectIp)) ||
                servers.Any(c => string.IsNullOrEmpty(c.Port));

            if (hasCriticalError)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;

                if (MessageShow)
                {
                    MessageShow = false;
                    if (servers[0].StartupPath == @"Path to the startup file (example:  G:\MinecraftServers\Survival\srart.bat)" ||
                        servers[0].FolderPath == @"Path to the server folder (example:  G:\MinecraftServers\Survival\)")
                    {
                        MessageBox.Show("В файле Servers.json присутсвует неправильный путь. Пожалуйста, измените на правильный.", "Ошибка в файле Servers.json", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (servers.Any(c => string.IsNullOrEmpty(c.Name) || string.IsNullOrEmpty(c.StartupPath)) ||
                        servers.Any(c => string.IsNullOrEmpty(c.FolderPath)) ||
                        servers.Any(c => string.IsNullOrEmpty(c.Ip) || string.IsNullOrEmpty(c.RconPort)) ||
                        servers.Any(c => string.IsNullOrEmpty(c.RconPassword) || string.IsNullOrEmpty(c.ConnectIp)) ||
                        servers.Any(c => string.IsNullOrEmpty(c.Port)))
                    {
                        MessageBox.Show("В файле Servers.json присутствуют пустые поля. Пожалуйста, измените на правильные.", "Ошибка в файле Servers.json", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                errorServersJson = true;
                errorReloadReconnect = true;
                UpdateErrorButtons();

                fatalJsonFilesError = true;
                return false;
            }

            if (MessageShow == false)
            {
                MessageShow = true;

                errorServersJson = true;
                errorReloadReconnect = true;
                UpdateErrorButtons();

            }

            flagStartCheck = false;
            return true;
        }

        private async Task<bool> CheckJsonSettings()
        {
            try
            {
                jsonSettings = File.ReadAllText(pathSettings);
                settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);
                SendErrorMessageJsonSettings = false;
            }
            catch (Exception ex)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;

                if (!SendErrorMessageJsonSettings)
                {
                    SendErrorMessageJsonSettings = true;
                    AppendText("Проверьте файл Settings.json на наличие ошибок.");
                    var result = MessageBox.Show(
                        $"Ошибка в чтении файла Settings.json: \n{ex.Message}\n\nПерезаписать файл, используя старую рабочую версию?",
                        "Ошибка в файле Settings.json",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);
                    LoadBackupJson(result, "Settings.json");
                }

                errorSettingsJson = true;
                errorReloadReconnect = true;
                UpdateErrorButtons();


                fatalJsonFilesError = true;
                return false;
            }


            var config = settings.FirstOrDefault();
            if (config == null)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;

                errorSettingsJson = true;
                errorReloadReconnect = true;
                UpdateErrorButtons();

                AppendText("Файл Settings.json пуст или повреждён.");
                fatalJsonFilesError = true;
                return false;
            }

            bool needRewrite = false;

            if (string.IsNullOrWhiteSpace(config.BotToken) ||
                config.BotToken == "Your bot token (example:  123456789:ABCdefGHIjklMNOpqrSTUvwxYZ)")
            {

                if (CheckTokenJsonSettings) AppendText("Перейдите в настройки и введите токен бота");
                fatalJsonFilesError = true;
                CheckTokenJsonSettings = false;
            }
            else { CheckTokenJsonSettings = true; }

            if (config.ChatIds == null || config.ChatIds.Count == 0 ||
                config.ChatIds.Any(c => string.IsNullOrWhiteSpace(c.Identifier) || string.IsNullOrWhiteSpace(c.Name)) ||
                config.ChatIds.Any(c => c.Identifier == "example: 646516246" || c.Name == "example: Admin"))
            {
                if (config.ChatIds == null || config.ChatIds.Count == 0)
                {
                    config.ChatIds = new List<ChatId>
                    {
                        new ChatId { Identifier = "example: 646516246", Name = "example: Admin" }
                    };
                    needRewrite = true;
                }
                else
                {
                    config.ChatIds.RemoveAll(c =>
                        string.IsNullOrWhiteSpace(c.Identifier) ||
                        string.IsNullOrWhiteSpace(c.Name) ||
                        c.Identifier == "example: 646516246" ||
                        c.Name == "example: Admin");
                    if (config.ChatIds.Count == 0)
                    {
                        config.ChatIds.Add(new ChatId { Identifier = "example: 646516246", Name = "example: Admin" });
                    }
                    needRewrite = true;
                }
                if (CheckChatIdsJsonSettings) AppendText("Перейдите в настройки и введите ID нужных групп или чатов.");
                fatalJsonFilesError = true;
                CheckChatIdsJsonSettings = false;
            }
            else { CheckChatIdsJsonSettings = true; }

            if (needRewrite)
            {
                string jsonStr = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, jsonStr);
            }

            if (config.ChatIds == null || config.ChatIds.Count == 0)
            {
                cts?.Cancel();
                cts?.Dispose();
                cts = null;

                errorSettingsJson = true;
                errorReloadReconnect = true;
                UpdateErrorButtons();

                Form1_Load(this, EventArgs.Empty);
                return true;
            }

            return true;
        }

        public static bool IsValidJson(string json)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    return true;
                }
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private async Task<RCON?> ConnectToRconAsync(ServerConfig config)
        {
            if (rcon != null && rcon.Connected == true)
            {
                AppendText($"Связь с сервером {servers[CheckEnableServer].Name} ({rcon.IPAddress}:{rcon.Port}) разорвана.");
                rcon.Dispose();
                rcon = null;
            }
            try
            {

                var rcon = new RCON(
                    IPAddress.Parse(config.Ip),
                    Convert.ToUInt16(config.RconPort),
                    config.RconPassword
                );
                await rcon.ConnectAsync();
                await rcon.AuthenticateAsync();

                AppendText($"Успешное подключение к серверу {servers[CheckEnableServer].Name} ({config.Ip}:{config.RconPort})");
                return rcon;
            }
            catch
            {
                return null;
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            json = File.ReadAllText("Servers.json");
            servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);

            jsonUserSettings = File.ReadAllText(pathUserSettings);
            var UserSettings = JsonSerializer.Deserialize<List<UserSettings>>(jsonUserSettings);

            jsonSettings = File.ReadAllText(pathSettings);
            var Settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);

            if (update.Type != UpdateType.Message || update.Message?.Type != MessageType.Text)
                return;

            var msg = update.Message;
            string text = msg.Text.Trim();
            string chatReply = "";
            int IndexUser = -1;
            bool isCommandAllowed = false;
            int IndexServer = -1;

            AppendText($"Получено сообщение от {msg.Chat.Id}: {msg.Text}{Environment.NewLine}");

            try
            {

                var identifiersInSettings = Settings
                    .SelectMany(setting => setting.ChatIds)
                    .Select(chatId => chatId.Identifier)
                    .ToHashSet();

                if (identifiersInSettings.Contains(Convert.ToString(msg.Chat.Id).Trim()))
                {
                    foreach (var user in UserSettings.Select((value, index) => new { value, index }))
                    {
                        if (user.value.Identifier == Convert.ToString(msg.Chat.Id).Trim())
                        {
                            IndexUser = user.index;
                            break;
                        }
                    }

                    for (int i = 0; i < UserSettings.Count; i++)
                    {
                        if (IndexUser >= 0)
                        {
                            var chatIdString = Convert.ToString(msg.Chat.Id);
                            isCommandAllowed = UserSettings[IndexUser].AllowedCommands.Any(command => command.Command == text);
                            if (isCommandAllowed) break;
                        }
                    }

                    int NoEnableServer = 0;
                    for (int i = 0; i < UserSettings[IndexUser].AllowedServers.Count; i++)
                    {
                        if (UserSettings[IndexUser].AllowedServers[i].Enabled)
                        {
                            if (servers.Any(c => c.Name == UserSettings[IndexUser].AllowedServers[i].Server))
                            {
                                IndexServer = servers.FindIndex(c => c.Name == UserSettings[IndexUser].AllowedServers[i].Server);
                                CheckEnableServer = IndexServer;
                                break;
                            }
                        }
                        else
                        {
                            NoEnableServer++;
                        }
                    }

                    if (NoEnableServer >= UserSettings[IndexUser].AllowedServers.Count)
                    {
                        if (!text.StartsWith($"/select_server {Settings[0].AdminPassword}"))
                        {
                            string errorText = "Нету выбранных серверов для использования";

                            if (msg.MessageThreadId.HasValue)
                            {
                                await bot.SendMessage(msg.Chat.Id, errorText, messageThreadId: msg.MessageThreadId.Value, cancellationToken: token);
                                AppendText(errorText);
                            }
                            else
                            {
                                await bot.SendMessage(msg.Chat.Id, errorText, cancellationToken: token);
                                AppendText(errorText);
                            }
                        }
                    }
                    else
                    {
                        if (rcon == null)
                        {
                            var newRcon = await ConnectToRconAsync(servers[CheckEnableServer]);
                            if (newRcon != null)
                            {
                                rcon = newRcon;
                            }
                        }
                    }


                    switch (text)
                    {
                        case "/start":
                            if (isCommandAllowed)
                            {
                                chatReply = "Привет! напиши /help для списка всех команд";
                            }
                            else
                            {
                                chatReply = chatReply = "Доступ к команде ограничен";
                            }
                            break;

                        case "/help":

                            if (isCommandAllowed)
                            {
                                string ChatCommand = "";
                                foreach (var command in UserSettings[IndexUser].AllowedCommands)
                                {
                                    if (AllowedCommands.ContainsKey(command.Command))
                                    {
                                        ChatCommand += $"{command.Command} — {AllowedCommands[command.Command]}\n";
                                    }
                                }

                                if (ChatCommand != "")
                                {
                                    chatReply = ChatCommand;
                                }
                                else
                                {
                                    chatReply = "У вас нету разрешенных команд";
                                }
                            }
                            else
                            {
                                chatReply = "Доступ к команде ограничен";
                            }
                            break;

                        case "/check_servers_status":

                            if (isCommandAllowed)
                            {
                                _ = Task.Run(() => RconCheckingServersAsync(msg.Chat.Id, msg.MessageThreadId, IndexUser));
                            }
                            else
                            {
                                chatReply = "Доступ к команде ограничен";
                            }
                            break;

                        case "/start_server":
                            if (isCommandAllowed)
                            {
                                if (serverStarting.ContainsKey(IndexServer) && serverStarting[IndexServer])
                                {
                                    chatReply = "Сервер уже запускается. Пожалуйста, подождите.";
                                    break;
                                }
                                serverStarting[IndexServer] = true;
                                int IndexServerStart = -1;
                                bool ServerStart = false;
                                string response = "";
                                try
                                {
                                    foreach (var User in UserSettings)
                                    {
                                        for (int i = 0; i < User.AllowedServers.Count; i++)
                                        {
                                            if (User.AllowedServers[i].Enabled == true)
                                            {
                                                IndexServerStart = User.AllowedServers.FindIndex(c => c.Enabled == User.AllowedServers[i].Enabled);

                                                if (rcon != null)
                                                {
                                                    response = await rcon.SendCommandAsync("list");
                                                }

                                                var match = Regex.Match(response, @"There are (\d+) of a max of (\d+) players online");

                                                if (match.Success)
                                                {
                                                    ServerStart = true;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                                catch { }
                                finally
                                {
                                    if (ServerStart == false)
                                    {
                                        Process.Start(new ProcessStartInfo
                                        {
                                            FileName = $@"{servers[IndexServer].StartupPath}",
                                            UseShellExecute = false,
                                            WorkingDirectory = @$"{servers[IndexServer].FolderPath}"
                                        });
                                        _ = ShowBalloonTip("Сервер", "Minecraft - сервер запускается!");
                                        chatReply = "Сервер запускается...";
                                        _ = Task.Run(async () =>
                                        {
                                            await CheckServerReadyAsync(msg.Chat.Id, msg.MessageThreadId, servers[IndexServer]);
                                            serverStarting[IndexServer] = false;
                                        });
                                    }
                                    else
                                    {
                                        chatReply = "Сервер запущен!";
                                        serverStarting[IndexServer] = false;
                                    }
                                }
                            }
                            else
                            {
                                chatReply = "Доступ к команде ограничен";
                            }

                            break;

                        case "/delete_world":
                            if (isCommandAllowed)
                            {
                                try
                                {
                                    string folderPath = @$"{servers[IndexServer].FolderPath}world";

                                    if (Directory.Exists(folderPath))
                                    {
                                        Directory.Delete(folderPath, true);
                                        chatReply = "Мир успешно удалён!";
                                        _ = ShowBalloonTip("Сервер", "Мир успешно удалён!");
                                    }
                                    else
                                    {
                                        chatReply = "Мир для удаления не найден. Возможно, он уже был удалён. ";
                                    }
                                }
                                catch { }
                            }
                            else
                            {
                                chatReply = "Доступ к команде ограничен";
                            }

                            break;

                        case "/players_count":
                            if (isCommandAllowed)
                            {
                                chatReply = await RconList(IndexUser, servers[IndexServer]);
                            }
                            else
                            {
                                chatReply = "Доступ к команде ограничен";
                            }
                            break;

                        case "/list_servers":
                            if (isCommandAllowed)
                            {
                                chatReply = await ServersList(IndexUser);
                            }
                            else
                            {
                                chatReply = "Доступ к команде ограничен";
                            }
                            break;

                        case "/select_server":
                            chatReply = "Укажите номер сервера:\n/select_server <админ пароль> <номер> .\nМожно переключиться только на один сервер.";
                            break;
                        case string s when s.StartsWith($"/select_server {Settings[0].AdminPassword}"):
                            {
                                var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length == 3 && int.TryParse(parts[2], out int serverIndex))
                                {
                                    chatReply = await ServerEnable(Convert.ToInt32(parts[2]), IndexUser);
                                }
                                else
                                {
                                    chatReply = "Укажите номер сервера:\n/select_server <админ пароль> <номер> .\nМожно переключиться только на один сервер..";
                                }
                                break;
                            }

                        case "/stop_server":
                            if (isCommandAllowed)
                            {
                                chatReply = await RconServerStop(IndexUser, servers[IndexServer]);
                                _ = ShowBalloonTip("Сервер", "Minecraft-сервер остановлен!");
                            }
                            else
                            {
                                chatReply = "Доступ к команде ограничен";
                            }
                            break;

                        case "/server_command":
                            chatReply = "Укажите пароль и команду:\n/server_command <админ пароль> <команда> .";
                            break;
                        case string k when k.StartsWith($"/server_command {Settings[0].AdminPassword}"):
                            {
                                var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 3)
                                {
                                    string runCommand = string.Join(' ', parts.Skip(2));
                                    chatReply = await ServerRunCommand(runCommand);
                                }
                                else
                                {
                                    chatReply = "Укажите пароль и команду:\n/server_command <админ пароль> <команда> .";
                                }
                                break;
                            }

                        default:
                            chatReply = "Неизвестная команда. Напиши /help.";
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(chatReply))
                    {
                        if (msg.MessageThreadId.HasValue)
                        {
                            var SendText = await bot.SendMessage(msg.Chat.Id, chatReply, messageThreadId: msg.MessageThreadId.Value, cancellationToken: token);
                            AppendText($"Ответ отправлен: \"{SendText.Text}\" (ID: {SendText.MessageId})");
                        }
                        else
                        {
                            var SendText = await bot.SendMessage(msg.Chat.Id, chatReply, cancellationToken: token);
                            AppendText($"Ответ отправлен: \"{SendText.Text}\" (ID: {SendText.MessageId})");
                        }
                    }
                }
                else
                {
                    AppendText($"Доступ пользователю {msg.Chat.Id} запрещен.");
                }

            }
            catch (Exception ex)
            {
                var errorText = $"Ошибка: {ex.Message}";
                if (msg.MessageThreadId.HasValue)
                {
                    await bot.SendMessage(msg.Chat.Id, errorText, messageThreadId: msg.MessageThreadId.Value, cancellationToken: token);
                    AppendText(errorText);
                }
                else
                {
                    await bot.SendMessage(msg.Chat.Id, errorText, cancellationToken: token);
                    AppendText(errorText);
                }
            }
        }

        private async Task CheckRconAsync()
        {
            await Task.Delay(1500);
            while (true)
            {
                if (rcon == null)
                {
                    var newRcon = await ConnectToRconAsync(servers[CheckEnableServer]);
                    if (newRcon != null)
                    {
                        rcon = newRcon;
                    }
                }

                if (rcon != null && servers[CheckEnableServer].RconPort != Convert.ToString(rcon.Port))
                {
                    if (rcon != null && rcon.Connected == true)
                    {
                        AppendText($"Связь с сервером {servers[CheckEnableServer].Name} ({rcon.IPAddress}:{rcon.Port}) разорвана.");
                        rcon.Dispose();
                        rcon = null;
                    }
                    var newRcon = await ConnectToRconAsync(servers[CheckEnableServer]);
                    if (newRcon != null)
                    {
                        rcon = newRcon;
                    }
                }


                await Task.Delay(3500);
            }
        }

        private async Task<RCON?> ConnectToRconCheckingServersAsync(ServerConfig config)
        {
            if (rconCheckingServers != null && rconCheckingServers.Connected == true)
            {
                rconCheckingServers.Dispose();
                rconCheckingServers = null;
            }
            try
            {

                var rconCheckingServers = new RCON(
                    IPAddress.Parse(config.Ip),
                    Convert.ToUInt16(config.RconPort),
                    config.RconPassword
                );

                await rconCheckingServers.ConnectAsync();
                await rconCheckingServers.AuthenticateAsync();
                return rconCheckingServers;
            }
            catch
            {
                return null;
            }
        }

        private async Task RconCheckingServersAsync(long chatId, int? threadId, int User)
        {
            string json = File.ReadAllText(pathServers);
            servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);

            jsonUserSettings = File.ReadAllText(pathUserSettings);
            var UserSettings = JsonSerializer.Deserialize<List<UserSettings>>(jsonUserSettings);

            string response;
            string text = "Статус серверов:\n";
            foreach (ServerConfig server in servers)
            {
                for (int i = 0; i < UserSettings[User].AllowedServers.Count; i++)
                {
                    if (server.Name == UserSettings[User].AllowedServers[i].Server)
                    {
                        var newRconCS = await ConnectToRconCheckingServersAsync(server);
                        if (newRconCS != null)
                        {
                            rconCheckingServers = newRconCS;
                        }

                        if (rconCheckingServers != null)
                        {
                            try
                            {
                                response = await rconCheckingServers.SendCommandAsync("list");
                                var match = System.Text.RegularExpressions.Regex.Match(response, @"There are (\d+) of a max of (\d+) players online");
                                if (match.Success)
                                {
                                    text += $"\n✅ Сервер {server.Name} запущен!\n" +
                                    $"Онлайн: {match.Groups[1].Value} из {match.Groups[2].Value} игроков.\n" +
                                    $"Подключение: {server.ConnectIp}:{server.Port}\n";
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            text += $"\n⚠️ Сервер {server.Name} выключен.\n";
                        }
                    }

                }
            }


            if (text == "")
            {
                text = "У вас нету доступных серверов";
            }

            if (threadId.HasValue)
            {
                await botClient.SendMessage(chatId, text, messageThreadId: threadId.Value);
            }
            else
            {

                await botClient.SendMessage(chatId, text);
            }

        }

        private async Task CheckServerReadyAsync(long chatId, int? threadId, ServerConfig serverConfig)
        {
            RCON? localRcon = null;
            string response = "";
            for (int i = 0; i <= 60; i++)
            {
                try
                {
                    if (localRcon == null)
                    {
                        localRcon = new RCON(
                            IPAddress.Parse(serverConfig.Ip),
                            Convert.ToUInt16(serverConfig.RconPort),
                            serverConfig.RconPassword
                        );
                        await localRcon.ConnectAsync();
                        await localRcon.AuthenticateAsync();
                    }
                    response = await localRcon.SendCommandAsync("list");

                    var match = Regex.Match(response, @"There are (\d+) of a max of (\d+) players online");
                    if (match.Success)
                    {
                        localRcon.Dispose();
                        string text = $"✅ Сервер {serverConfig.Name} успешно запущен!";
                        if (threadId.HasValue)
                            await botClient.SendMessage(chatId, text, messageThreadId: threadId.Value);
                        else
                            await botClient.SendMessage(chatId, text);
                        AppendText("Сервер запущен и пользователь уведомлён.");
                        return;
                    }
                }
                catch { }
                await Task.Delay(5000);
            }

            string failText = $"⚠️ Не удалось подтвердить запуск сервера {serverConfig.Name}. Возможно, он не запустился или RCON не отвечает.";
            if (threadId.HasValue)
                await botClient.SendMessage(chatId, failText, messageThreadId: threadId.Value);
            else
                await botClient.SendMessage(chatId, failText);

            AppendText($"Не удалось подтвердить запуск сервера {serverConfig.Name} .");
        }

        private void AppendText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendText), text);
                return;
            }

            string timePrefix = DateTime.Now.ToString("HH:mm:ss");
            textBox1.AppendText($"[{timePrefix}] {text.Trim()}{Environment.NewLine}");
        }

        private Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
        {
            string error = ex switch
            {
                ApiRequestException apiEx => $"Telegram API ошибка: [{apiEx.ErrorCode}] {apiEx.Message}",
                _ => ex.ToString()
            };

            AppendText($"Ответ отправлен: {error}");


            return Task.CompletedTask;
        }

        private async Task<string> RconList(int User, ServerConfig serverConfig)
        {
            try
            {
                jsonUserSettings = File.ReadAllText(pathUserSettings);
                var UserSettings = JsonSerializer.Deserialize<List<UserSettings>>(jsonUserSettings);
                string response = "";
                RCON? localRcon = null;


                for (int i = 0; i < UserSettings[User].AllowedServers.Count; i++)
                {
                    if (UserSettings[User].AllowedServers[i].Enabled == true)
                    {
                        if (UserSettings[User].AllowedServers.Any(x => x.Enabled == true))
                        {

                            if (localRcon == null)
                            {
                                localRcon = new RCON(
                                    IPAddress.Parse(serverConfig.Ip),
                                    Convert.ToUInt16(serverConfig.RconPort),
                                    serverConfig.RconPassword
                                );
                                await localRcon.ConnectAsync();
                                await localRcon.AuthenticateAsync();
                            }
                            response = await rcon.SendCommandAsync("list");
                            if (string.IsNullOrEmpty(response))
                            {
                                response = "Cервер не отвечает. Возможно сервер выключен.";
                            }
                            else
                            {
                                string pattern = @"There are (\d+) of a max of (\d+) players online(:.*)";
                                response = System.Text.RegularExpressions.Regex.Replace(response, pattern, m =>
                                    $"Сейчас онлайн {m.Groups[1].Value} из {m.Groups[2].Value} игроков{m.Groups[3].Value}"
                                );
                            }
                        }
                    }
                }

                if (response.Trim() == "")
                {
                    response = "У вас нету доступных серверов";
                }
                else
                {
                    localRcon.Dispose();
                    response.Trim();
                }
                return response;
            }
            catch
            {
                return "Не удалось получить ответ от сервера. Возможно сервер выключен."; ;
            }
        }

        private async Task<string> ServersList(int User)
        {
            string EnabledServer;
            string response = "";
            serverNumber = 1;
            try
            {
                json = File.ReadAllText("Servers.json");
                servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);

                jsonUserSettings = File.ReadAllText(pathUserSettings);
                var UserSettings = JsonSerializer.Deserialize<List<UserSettings>>(jsonUserSettings);
                foreach (ServerConfig server in servers)
                {
                    for (int i = 0; i < UserSettings[User].AllowedServers.Count; i++)
                    {
                        if (server.Name == UserSettings[User].AllowedServers[i].Server)
                        {
                            if (servers.Count != null)
                            {
                                if (UserSettings[User].AllowedServers[i].Enabled == true) { EnabledServer = "Выбран"; }
                                else { EnabledServer = "Не выбран"; }
                                response += $"\n{serverNumber}) {server.Name} - {EnabledServer}";
                                serverNumber++;
                            }
                        }
                    }
                }

                if (response.Trim() == "")
                {
                    response = "У вас нету доступных серверов";
                }
                else
                {
                    response.Trim();
                }
                return response;
            }
            catch
            {
                return "Не удалось получить ответ от сервера. Возможно сервер выключен."; ;
            }

        }

        private async Task<string> ServerEnable(int Number, int User)
        {
            string EnabledServer;
            string response = "";
            serverNumber = 1;
            if (Number > servers.Count || Number <= 0)
            {
                response = "Ошибка: Неверный номер сервера.";
                return response;
            }
            try
            {

                jsonUserSettings = File.ReadAllText(pathUserSettings);
                var UserSettings = JsonSerializer.Deserialize<List<UserSettings>>(jsonUserSettings);

                if (UserSettings[User].AllowedServers.Count != null)
                {
                    for (int i = 0; i < UserSettings[User].AllowedServers.Count; i++)
                    {
                        if (serverNumber == Number)
                        {
                            UserSettings[User].AllowedServers[serverNumber - 1].Enabled = true;
                            EnabledServer = "Выбран";
                        }
                        else
                        {
                            UserSettings[User].AllowedServers[serverNumber - 1].Enabled = false;
                            EnabledServer = "Не выбран";
                        }
                        response += $"\n{serverNumber}) {UserSettings[User].AllowedServers[i].Server} - {EnabledServer}";
                        serverNumber++;
                    }

                    string updatedJsonStr = JsonSerializer.Serialize(UserSettings, options);
                    await File.WriteAllTextAsync(pathUserSettings, updatedJsonStr);
                }
                _ = Task.Run(() => CheckRconAsync());
                return response.Trim();
            }
            catch (Exception ex)
            {
                response = "Ошибка: " + ex;
                return response;
            }

        }

        private async Task<string> RconServerStop(int User, ServerConfig serverConfig)
        {
            try
            {
                jsonUserSettings = File.ReadAllText(pathUserSettings);
                var UserSettings = JsonSerializer.Deserialize<List<UserSettings>>(jsonUserSettings);
                string response = "";
                RCON? localRcon = null;


                for (int i = 0; i < UserSettings[User].AllowedServers.Count; i++)
                {
                    if (UserSettings[User].AllowedServers[i].Enabled == true)
                    {
                        if (UserSettings[User].AllowedServers.Any(x => x.Enabled == true))
                        {
                            if (localRcon == null)
                            {
                                localRcon = new RCON(
                                    IPAddress.Parse(serverConfig.Ip),
                                    Convert.ToUInt16(serverConfig.RconPort),
                                    serverConfig.RconPassword
                                );
                                await localRcon.ConnectAsync();
                                await localRcon.AuthenticateAsync();
                            }
                            response = await localRcon.SendCommandAsync("stop");
                            if (string.IsNullOrEmpty(response))
                            {
                                response = "Не удалось получить ответ от сервера. Возможно сервер выключен.";
                            }
                            else
                            {
                                localRcon.Dispose();
                                response = "Остановка сервера.";
                            }
                        }
                    }
                }
                if (response.Trim() == "")
                {
                    response = "У вас нету доступных серверов";
                }
                else
                {
                    response.Trim();
                }
                return response;
            }
            catch
            {
                return "Не удалось получить ответ от сервера. Возможно сервер выключен.";
            }
        }

        private async Task<string> ServerRunCommand(string command)
        {
            try
            {
                string response = "";
                if (rcon == null)
                {
                    var newRcon = await ConnectToRconAsync(servers[CheckEnableServer]);
                    if (newRcon != null)
                    {
                        rcon = newRcon;
                    }
                }
                else
                {
                    response = await rcon.SendCommandAsync(command);
                }
                if (string.IsNullOrEmpty(response))
                {
                    response = "Не удалось получить ответ от сервера. Возможно неправильная команда.";
                    AppendText($"Команда ({command}): {response}");
                }
                else
                {
                    AppendText($"Команда ({command}) выполнена: {response}");
                }
                if (response.Trim() == "")
                {
                    response = "У вас нету доступных серверов";
                }
                else
                {
                    response.Trim();
                }
                return response;
            }
            catch
            {
                return "Не удалось получить ответ от сервера. Возможно сервер выключен."; ;
            }
        }
    }
}