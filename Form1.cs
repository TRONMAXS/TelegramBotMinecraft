using CoreRCON;
using System.Diagnostics;
using System.Net;
using System.Text.Json;
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
        private string TextBoxAdmin;
        private readonly string pathServers;
        private string json;
        private readonly string pathSettings;
        private string jsonSettings;
        private List<SettingsConfig> settings = new List<SettingsConfig>();
        private List<ServerConfig> servers = new List<ServerConfig>();
        //private bool isServerRunning = false;
        private string BotToken;
        private int CheckEnableServer = -1;
        private bool flagStartCheck = true;
        private bool StartCheck = false;
        private static readonly HashSet<long> AllowedUsers = new HashSet<long>{};
        readonly JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        
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

        private ContextMenuStrip notifyIconMenu;
        private ToolStripMenuItem exitMenuItem;


        public Form1()
        {
            InitializeComponent();
            pathServers = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Servers.json");
            json = File.ReadAllText("Servers.json");
            servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);
            serversRunning = new List<bool>(new bool[servers.Count]);

            pathSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
            jsonSettings = File.ReadAllText("Settings.json");
            settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);

            this.Load += Form1_Load;
            this.Resize += Form1_Resize;

            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;
            button5.Click += button5_Click;
            button4.Click += RunСommand;

            // Скрываем окно
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide(); // спрятать форму

            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Открыть", null, (s, e) =>
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.BringToFront();
            });
            contextMenu.Items.Add("Выход", null, (s, e) => Application.Exit());
            notifyIcon1.ContextMenuStrip = contextMenu;


            // Уведомление при запуске
            _ = ShowBalloonTip("Приложение запущено", "Ожидает команд через Telegram.");


            AutoCompleteStringCollection source = new AutoCompleteStringCollection()
            {
            "start",
            "stop",
            "list"
            };
            textBox2.AutoCompleteCustomSource = source;
            textBox2.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            textBox2.AutoCompleteSource = AutoCompleteSource.CustomSource;


        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                e.Cancel = true;
            }
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

        private void button1_Click(object sender, EventArgs e)
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

        private void button2_Click(object sender, EventArgs e)
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

        private async void button3_Click(object sender, EventArgs e)
        {
            try
            {
                jsonSettings = File.ReadAllText("Settings.json");
                settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);

            }
            catch
            {
                settings = new List<SettingsConfig>
                    {
                        new SettingsConfig
                        {
                            Notifications = true,
                            BotToken = "Your bot token (example:  123456789:ABCdefGHIjklMNOpqrSTUvwxYZ)",
                            ChatIds = new List<ChatId>
                            {
                                new ChatId { Identifier = "example: 646516246", Name = "example: Admin" },
                            }
                        }
                    };
                string jsonStr = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, jsonStr);
                AppendText($"Settings.json был перезаписан из-за ошибки. Проверьте настройки.");
            }
            try
            {
                if (cts != null)
                {
                    cts.Cancel();
                    cts.Dispose();
                    cts = null;
                }
                Form1_Load(this, EventArgs.Empty);

            }
            catch { Form1_Load(this, EventArgs.Empty); }
        }

        private async void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string json = File.ReadAllText(pathServers);
                servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);
                AppendText("В файле Servers.json нет включённых серверов. Пожалуйста, включите хотя бы один, чтобы можно было с ним работать.");
            }
            catch
            {
                servers = new List<ServerConfig>
                {
                    new ServerConfig
                    {
                        Name = "Name Server (example:  Vanilla(Survival) - 1.20.1)",
                        Path = @"Path to the server folder (example:  G:\MinecraftServers\Vanilla(Survival) - 1.20.1)",
                        Ip = "server ip (example:  127.0.0.1)",
                        RconPort = "rcon port (example:  25565)",
                        RconPassword = "rcon password(example:  12345)",
                        ConnectIp = "connect ip (example: 127.0.0.1)",
                        Port = "server port (example:  25565)",
                        Enabled = true
                    }
                };
                string json = JsonSerializer.Serialize(servers, options);
                File.WriteAllText(pathServers, json);
                AppendText($"Servers.json был перезаписан из-за ошибки. Проверьте настройки.");
            }
            try
            {
                Form1_Load(this, EventArgs.Empty);
            }
            catch { Form1_Load(this, EventArgs.Empty); }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                jsonSettings = File.ReadAllText("Settings.json");
                settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);
            }
            catch (Exception ex) 
            {
                settings = new List<SettingsConfig>
                    {
                        new SettingsConfig
                        {
                            Notifications = true,
                            BotToken = "Your bot token (example:  123456789:ABCdefGHIjklMNOpqrSTUvwxYZ)",
                            ChatIds = new List<ChatId>
                            {
                                new ChatId { Identifier = "example: 646516246", Name = "example: Admin" },
                            }
                        }
                    };
                string jsonStr = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, jsonStr);
                AppendText($"Settings.json был перезаписан из-за ошибки. Проверьте настройки."); 
            }
            if (settings[0].BotToken == "Your bot token (example:  123456789:ABCdefGHIjklMNOpqrSTUvwxYZ)" || string.IsNullOrWhiteSpace(settings[0].BotToken))
            {
                AppendText($"Перейдите в настройки и введите токен бота");
                MessageBox.Show("Не указан токен бота в файле Settings.json!", "Ошибка в файле Settings.json");
                this.Invoke(new Action(() =>
                {
                    button3.Enabled = true;
                    button3.Visible = true;

                }));
                return;
            }

            if (settings[0].ChatIds == null ||
                settings[0].ChatIds.Any(c => c.Identifier == "example: 646516246" || c.Name == "example: Admin") ||
                settings[0].ChatIds.Any(c => string.IsNullOrEmpty(c.Identifier) || string.IsNullOrEmpty(c.Name)) ||
                settings[0].ChatIds.Count == 0)
            {
                if (settings[0].ChatIds.Count == 0)
                {
                    BotToken = settings[0].BotToken;
                    bool? Notifications = settings[0].Notifications;
                    settings = new List<SettingsConfig>
                    {
                        new SettingsConfig
                        {
                            Notifications = Notifications,
                            BotToken = BotToken,
                            ChatIds = new List<ChatId>
                            {
                                new ChatId { Identifier = "example: 646516246", Name = "example: Admin" },
                            }
                        }
                    };
                }
                string jsonStr = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, jsonStr);
                AppendText($"Перейдите в настройки и введите ID нужных групп или чатов.");
                MessageBox.Show("Не указаны ID групп или чатов в файле Settings.json!", "Ошибка в файле Settings.json");

                this.Invoke(new Action(() =>
                {
                    button3.Enabled = true;
                    button3.Visible = true;

                }));
                return;
            }
            try
            {
                StartBot = true;
                for (int i = 0; i < settings[0].ChatIds.Count; i++)
                {

                    if (long.TryParse(settings[0].ChatIds[i].Identifier, out long chatId))
                    {
                        AllowedUsers.Add(chatId);
                    }
                }

                this.Invoke(new Action(() =>
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = false;
                    button3.Visible = false;
                    button4.Enabled = true;
                    button5.Enabled = false;
                    button5.Visible = false;
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;
                    textBox3.Enabled = true;
                    label1.Enabled = true;
                }));
                bool CheckSettings = false;
                bool CheckServers = false;
                while (true)
                {
                    CheckSettings = await CheckJsonSettings();
                    CheckServers = await CheckJsonServers();
                    if (CheckSettings == true && CheckServers == true)
                    {
                        if (!botStartedForm1_Load)
                        {
                            StartBot = true;
                            botStartedForm1_Load = true;
                            _ = RunBotLoopAsync();
                        }
                        break;
                    }
                    else
                    {
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
                        rcon = newRcon; // только если подключение удалось
                    }
                }
                StartCheck = true;
                _ = Task.Run(() => CheckJson());

                _ = Task.Run(() => CheckServerAsync());
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
            while (true)
            {
                try
                {
                    await StartBotAsync();
                    // Если StartBotAsync завершился без ошибок — выходим из цикла
                    break;
                }
                catch (Exception ex)
                {
                    AppendText($"Бот завершил работу: {ex.Message}.");
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

            // Можно добавить ожидание завершения работы, если нужно
            await Task.Delay(Timeout.Infinite, cts.Token);
        }

        private async Task<bool> CheckJsonServers()
        {
            while (true)
            {

                string json = File.ReadAllText(pathServers);
                servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);
                int count = 0;
                int countFalse = 0;

                if (rcon != null)
                {
                    if (flagStartCheck == false && servers[CheckEnableServer].RconPort != Convert.ToString(rcon.Port))
                    {
                        if (rcon != null && rcon.Connected == true)
                        {
                            AppendText($"Связь с сервером {servers[CheckEnableServer].Name} ({rcon.IPAddress}:{rcon.Port}) разорвана.");
                            rcon.Dispose();
                            rcon = null;
                        }
                        if (CheckEnableServer != -1)
                        {
                            var newRcon = await ConnectToRconAsync(servers[CheckEnableServer]);
                            if (newRcon != null)
                            {
                                rcon = newRcon; // только если подключение удалось
                            }
                        }
                    }
                }

                try
                {

                    for (int i = 0; i < servers.Count; i++)
                    {
                        if (servers[i].Enabled == false)
                        {
                            countFalse++;
                        }
                    }

                    if (servers[0].Path == "Path to the server folder (example:  G:\\MinecraftServers\\Vanilla(Survival) - 1.20.1)" ||
                        countFalse == servers.Count || servers.Any(c => string.IsNullOrEmpty(c.Name) || string.IsNullOrEmpty(c.Path)) ||
                        servers.Any(c => string.IsNullOrEmpty(c.Ip) || string.IsNullOrEmpty(c.RconPort)) ||
                        servers.Any(c => string.IsNullOrEmpty(c.RconPassword) || string.IsNullOrEmpty(c.ConnectIp)) ||
                        servers.Any(c => string.IsNullOrEmpty(c.Port)))
                    {
                        
                        if (MessageShow == true)
                        {
                            MessageShow = false;
                            if (cts != null)
                            {
                                cts.Cancel();
                                cts.Dispose();
                                cts = null;
                            }
                            if (servers[0].Path == "Path to the server folder (example:  G:\\MinecraftServers\\Vanilla(Survival) - 1.20.1)")
                            {
                                MessageBox.Show($"В файле Servers.json присутсвует неправильный путь. Пожалуйста, измените на правильный.", "Ошибка в файле Servers.json");

                            }
                            if (countFalse == servers.Count)
                            {
                                MessageBox.Show($"В файле Servers.json нет включённых серверов. Пожалуйста, включите хотя бы один, чтобы можно было с ним работать.", "Ошибка в файле Servers.json");
                            }
                            if (servers.Any(c => string.IsNullOrEmpty(c.Name) || string.IsNullOrEmpty(c.Path)) ||
                                servers.Any(c => string.IsNullOrEmpty(c.Ip) || string.IsNullOrEmpty(c.RconPort)) ||
                                servers.Any(c => string.IsNullOrEmpty(c.RconPassword) || string.IsNullOrEmpty(c.ConnectIp)) ||
                                servers.Any(c => string.IsNullOrEmpty(c.Port)))
                            {
                                MessageBox.Show($"В файле Servers.json присутствуют пустые поля. Пожалуйста, измените на правильные.", "Ошибка в файле Servers.json");
                            }
                        }

                        StartBot = false;
                        this.Invoke(new Action(() =>
                        {
                            button1.Enabled = false;
                            button2.Enabled = true;
                            button3.Enabled = false;
                            button3.Visible = false;
                            button4.Enabled = false;
                            button5.Enabled = true;
                            button5.Visible = true;
                            textBox1.Enabled = true;
                            textBox2.Enabled = false;
                            textBox3.Enabled = false;
                            label1.Enabled = false;
                        }));
                        return false;
                    }
                    for (int i = 0; i < servers.Count; i++)
                    {
                        if (servers[i].Enabled == true)
                        {
                            CheckEnableServer = i;
                            this.Invoke(new Action(() =>
                            {
                                textBox3.Text = servers[i].Name;
                            }));

                            if (rcon != null && rcon.Connected == false)
                            {
                                var newRcon = await ConnectToRconAsync(servers[CheckEnableServer]);
                                if (newRcon != null)
                                {
                                    rcon = newRcon; // только если подключение удалось
                                }
                            }
                        }
                    }

                    if (countFalse != servers.Count && MessageShow == false)
                    {
                        MessageShow = true;
                        this.Invoke(new Action(() =>
                        {
                            button1.Enabled = true;
                            button2.Enabled = true;
                            button3.Enabled = false;
                            button3.Visible = false;
                            button5.Enabled = false;
                            button5.Visible = false;
                            button4.Enabled = true;
                            textBox1.Enabled = true;
                            textBox2.Enabled = true;
                            textBox3.Enabled = true;
                            label1.Enabled = true;
                        }));
                    }

                    for (int i = 0; i < servers.Count; i++)
                    {
                        if (servers[i].Enabled == true)
                        {
                            count++;
                        }
                    }

                    if (count > 1)
                    {
                        for (int i = 0; i < servers.Count; i++)
                        {
                            servers[i].Enabled = false;
                        }
                        var options = new JsonSerializerOptions { WriteIndented = true };
                        string jsonStr = JsonSerializer.Serialize(servers, options);
                        File.WriteAllText(pathServers, jsonStr);

                        MessageBox.Show("Можно включить только один сервер. Все серверы были отключены. Включите нужный сервер.", "Ошибка в файле Servers.json");
                        this.Invoke(new Action(() =>
                        {
                            textBox3.Text = "";
                        }));
                    }

                    await Task.Delay(2500);
                    flagStartCheck = false;
                }
                catch (Exception ex)
                {
                    AppendText($"Ошибка при проверке серверов: {ex.Message}");
                    return false;
                }
                await Task.Delay(2500);
                return true;
            }
        }

        private async Task<bool> CheckJsonSettings()
        {
            while (true)
            {
                try
                {
                    settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);
                    if (settings[0].ChatIds.Any(c => string.IsNullOrEmpty(c.Identifier) || string.IsNullOrEmpty(c.Name)))
                    {
                        settings[0].ChatIds.RemoveAll(c => string.IsNullOrEmpty(c.Identifier) || string.IsNullOrEmpty(c.Name));
                        string jsonStr = JsonSerializer.Serialize(settings, options);
                        File.WriteAllText(pathSettings, jsonStr);
                    }

                    if (settings[0].ChatIds == null ||
                        settings[0].ChatIds.Any(c => c.Identifier == "example: 646516246" || c.Name == "example: Admin") ||
                        settings[0].ChatIds.Any(c => string.IsNullOrEmpty(c.Identifier) || string.IsNullOrEmpty(c.Name)) ||
                        settings[0].ChatIds.Count == 0)
                    {
                        this.Invoke(new Action(() =>
                        {
                            button1.Enabled = true;
                            button2.Enabled = false;
                            button3.Enabled = true;
                            button3.Visible = true;
                            button4.Enabled = false;
                            textBox2.Enabled = false;
                            textBox3.Enabled = false;
                            label1.Enabled = false;
                        }));

                        if (cts != null)
                        {
                            cts.Cancel();
                            cts.Dispose();
                            cts = null;
                        }
                        Form1_Load(this, EventArgs.Empty);
                        return true;
                    }
                }
                catch{}
                try
                {
                    jsonSettings = File.ReadAllText("Settings.json");
                    settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);
                }
                catch (Exception ex)
                {
                    settings = new List<SettingsConfig>
                    {
                        new SettingsConfig
                        {
                            Notifications = true,
                            BotToken = "Your bot token (example:  123456789:ABCdefGHIjklMNOpqrSTUvwxYZ)",
                            ChatIds = new List<ChatId>
                            {
                                new ChatId { Identifier = "example: 646516246", Name = "example: Admin" },
                            }
                        }
                    };
                    string jsonStr = JsonSerializer.Serialize(settings, options);
                    File.WriteAllText(pathSettings, jsonStr);
                    AppendText($"Settings.json был перезаписан из-за ошибки. Проверьте настройки.");
                    if (cts != null)
                    {
                        cts.Cancel();
                        cts.Dispose();
                        cts = null;
                    }
                    return false;
                }
                return true;
            }
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

        private void RunСommand(object sender, EventArgs e)
        {
            TextBoxAdmin = textBox2.Text;
            textBox2.Text = null;
            if (!string.IsNullOrWhiteSpace(TextBoxAdmin))
            {
                try
                {
                    switch (TextBoxAdmin)
                    {
                        case "start":
                            if (serversRunning[CheckEnableServer] == false)
                            {
                                serversRunning[CheckEnableServer] = true;

                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = "start.cmd",
                                    UseShellExecute = false,
                                    WorkingDirectory = @$"{servers[CheckEnableServer].Path}"
                                });
                                AppendText($"Сервер {servers[CheckEnableServer].Name} запущен!");
                                _ = ShowBalloonTip("Сервер", "Minecraft-сервер успешно запущен!");
                            }
                            else
                            {
                                AppendText($"Сервер уже запущен!");
                            }
                            break;
                        case "stop":
                            if (rcon != null)
                            {
                                _ = Task.Run(() => RconServerAnyAsync("stop"));
                                _ = ShowBalloonTip("Сервер", "Minecraft-сервер остановлен!");
                                AppendText($"Сервер {servers[CheckEnableServer].Name} остановлен!");
                            }
                            else
                            {
                                AppendText($"Сервер уже остановлен!");
                            }
                            break;
                        case "list":
                            _ = Task.Run(() => RconServerAnyAsync("list"));
                            break;

                        default:
                            _ = Task.Run(() => RconServerAnyAsync(TextBoxAdmin));
                            break;

                    }
                }
                catch { }
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            json = File.ReadAllText("Servers.json");
            servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);
            

            if (update.Type != UpdateType.Message || update.Message?.Type != MessageType.Text)
                return;

            var msg = update.Message;
            string text = msg.Text.Trim();
            string chatReply = "";

            AppendText($"Получено сообщение от {msg.Chat.Id}: {msg.Text}{Environment.NewLine}");


            if (!AllowedUsers.Contains(msg.Chat.Id))
            {
                if (msg.MessageThreadId.HasValue)
                {
                    var SendText = await bot.SendMessage(msg.Chat.Id, "У вас нет доступа к этой команде.", cancellationToken: token);
                    AppendText(SendText.ToString());
                }
                else
                {
                    var SendText = await bot.SendMessage(msg.Chat.Id, "У вас нет доступа к этой команде.", cancellationToken: token);
                    AppendText(SendText.ToString());
                }
                return;
            }

            try
            {
                switch (text.ToLower())
                {
                    case "/start":
                        chatReply = "Привет! напиши /help для списка всех команд";
                        break;

                    case "/help":
                    case "/help@xy8zjr4tqbot":
                        chatReply =
                            "Доступные команды:\n" +
                            "/servers_list — показывает список доступных серверов Minecraft\n" +
                            "/server_enable — включает указанный сервер Minecraft\n" +
                            "/bot_server_start — запускает выбранный сервер\n" +
                            "/bot_servers_check — проверяет статус серверов\n" +
                            "/bot_server_list — показывает количество игроков на выбранном сервере\n" +
                            "/bot_server_stop — останавливает выбранный сервер\n" +
                            "/bot_world_delete — удаляет мир на выбранном сервере";
                        break;

                    case "/bot_servers_check":
                    case "/bot_servers_check@xy8zjr4tqbot":
                        _ = Task.Run(() => RconCheckingServersAsync(msg.Chat.Id, msg.MessageThreadId));
                        break;

                    case "/bot_server_start":
                    case "/bot_server_start@xy8zjr4tqbot":

                        if (serversRunning[CheckEnableServer] == false)
                        {
                            serversRunning[CheckEnableServer] = true;
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "start.cmd",
                                UseShellExecute = false,
                                WorkingDirectory = @$"{servers[CheckEnableServer].Path}"
                            });
                            _= ShowBalloonTip("Сервер", "Minecraft - сервер запускается!");
                            chatReply = "Сервер запускается...";
                            await Task.Delay(5000);
                            _ = Task.Run(() => CheckServerReadyAsync(msg.Chat.Id, msg.MessageThreadId));
                        }
                        else
                        {
                            chatReply = "Сервер запущен!";
                        }
                            break;

                    case "/bot_world_delete":
                    case "/bot_world_delete@xy8zjr4tqbot":
                        try
                        {
                            Directory.Delete(@$"{servers[CheckEnableServer].Path}world", true);
                            chatReply = "Мир успешно удалён!";
                            _ = ShowBalloonTip("Сервер", "Мир успешно удалён!");
                        }
                        catch
                        {
                            chatReply = "Мир уже удалён!";
                        }
                        break;

                    case "/bot_server_list":
                    case "/bot_server_list@xy8zjr4tqbot":
                        chatReply = await RconList();
                        break;

                    case "/servers_list":
                    case "/servers_list@xy8zjr4tqbot":
                        chatReply = await ServersList();
                        break;
                    case "/server_enable@xy8zjr4tqbot":
                        chatReply = "Укажите номер сервера: /server_enable <номер> . Можно включить только один сервер.";
                        break;
                    case string s when s.StartsWith("/server_enable"):
                        {
                            var parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length == 2 && int.TryParse(parts[1], out int serverIndex))
                            {
                                chatReply = await ServerEnable(Convert.ToInt32(parts[1]));
                            }
                            else
                            {
                                chatReply = "Укажите номер сервера: /server_enable <номер> . Можно включить только один сервер.";
                            }
                            break;
                        }

                    case "/bot_server_stop":
                    case "/bot_server_stop@xy8zjr4tqbot":
                        chatReply = await RconServerStop();
                        _ = ShowBalloonTip("Сервер", "Minecraft-сервер остановлен!");
                        break;

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

        private async Task CheckServerAsync()
        {
            while (true)
            {
                try
                {
                    if (rcon != null && CheckEnableServer >= 0 && CheckEnableServer < serversRunning.Count)
                    {
                        string response = await rcon.SendCommandAsync("list");
                        var match = System.Text.RegularExpressions.Regex.Match(response, @"There are (\d+) of a max of (\d+) players online");

                        this.Invoke(new Action(() =>
                        {
                            if (match.Success)
                            {
                                serversRunning[CheckEnableServer] = true;
                                notifyIcon1.Icon = new Icon("Icons/server_start.ico");
                                label1.Text = "Сервер - работает";
                            }
                            else
                            {
                                serversRunning[CheckEnableServer] = false;
                                notifyIcon1.Icon = new Icon("Icons/server_stop.ico");
                                label1.Text = "Сервер - выключен";
                            }
                        }));
                    }
                    else
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (CheckEnableServer >= 0 && CheckEnableServer < serversRunning.Count)
                                serversRunning[CheckEnableServer] = false;
                            notifyIcon1.Icon = new Icon("Icons/server_stop.ico");
                            label1.Text = "Сервер - выключен";
                        }));
                    }
                }
                catch (Exception)
                {
                    this.Invoke(new Action(() =>
                    {
                        if (CheckEnableServer >= 0 && CheckEnableServer < serversRunning.Count)
                            serversRunning[CheckEnableServer] = false;
                        notifyIcon1.Icon = new Icon("Icons/server_stop.ico");
                        label1.Text = "Сервер - выключен";
                    }));
                }

                await Task.Delay(2500);
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

        private async Task RconCheckingServersAsync(long chatId, int? threadId)
        {
            string json = File.ReadAllText(pathServers);
            servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);
            string response;
            string text = "Статус серверов:\n";
            foreach (ServerConfig server in servers)
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
            if (threadId.HasValue)
            {
                await botClient.SendMessage(chatId, text, messageThreadId: threadId.Value);
            }
            else
            {
                await botClient.SendMessage(chatId, text);
            }

        }

        private async Task CheckServerReadyAsync(long chatId, int? threadId)
        {
            string response = "";
            _ = Task.Run(() => CheckJsonServers());
            for (int i = 0; i <= 15; i++) // макс 15 попыток
            {
                try
                {
                    if (rcon != null)
                    {
                        response = await rcon.SendCommandAsync("list");
                    }

                    AppendText($"RCON ответ при проверке сервера(попытка-{i}): " + response);

                    var match = System.Text.RegularExpressions.Regex.Match(response, @"There are (\d+) of a max of (\d+) players online");

                    if (match.Success)
                    {
                        string text = $"✅ Сервер успешно запущен!";
                        if (threadId.HasValue)
                        {
                            await botClient.SendMessage(chatId, text, messageThreadId: threadId.Value);
                        }
                        else
                        {
                            await botClient.SendMessage(chatId, text);
                        }

                        AppendText("Сервер запущен и пользователь уведомлён.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    AppendText($"Ошибка при попытке подключения к RCON: {ex.Message}");
                }

                await Task.Delay(1500); // подождать 1,5 секунды перед повтором
            }

            // Если не удалось за 90 секунд — сообщаем
            string failText = "⚠️ Не удалось подтвердить запуск сервера. Возможно, он не запустился или RCON не отвечает.";
            if (threadId.HasValue)
            {
                await botClient.SendMessage(chatId, failText, messageThreadId: threadId.Value);
            }
            else
            {
                await botClient.SendMessage(chatId, failText);
            }

            AppendText("Не удалось подтвердить запуск сервера.");
        }

        private void AppendText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendText), text);
                return;
            }

            string timePrefix = DateTime.Now.ToString("HH:mm:ss"); // формат времени ЧЧ:мм:сс
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

        private async Task<string> RconList()
        {
            string response = "";
            if (rcon != null)
            {
                 response = await rcon.SendCommandAsync("list");
            }

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
            return response.Trim();
        }

        private async Task<string> ServersList()
        {
            string EnabledServer;
            string response = "";
            serverNumber = 1;
            
            try
            {
                json = File.ReadAllText("Servers.json");
                servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);
                if (servers.Count != null)
                {
                    foreach (ServerConfig server in servers)
                    {
                        if (server.Enabled == true) { EnabledServer = "Выбран"; }
                        else { EnabledServer = "Не выбран"; }
                        response += $"\n{serverNumber}) {server.Name} - {EnabledServer}";
                        serverNumber++;    
                    }
                }
                return response.Trim();
            }
            catch(Exception ex)
            {
                response = "Ошибка: " + ex;
                return response;
            }
            
        }

        private async Task<string> ServerEnable(int Number)
        {
            await ServersList();
            string EnabledServer;
            string response = "";
            serverNumber = 1;
            if(Number > servers.Count || Number <= 0)
            {
                response = "Ошибка: Неверный номер сервера.";
                return response;
            }
            try
            {
                json = File.ReadAllText("Servers.json");
                servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);

                if (servers.Count != null)
                {
                    foreach (ServerConfig server in servers)
                    {
                        if(serverNumber == Number) 
                        {
                            EnabledServer = "Выбран";
                            servers[serverNumber - 1].Enabled = true;
                        }
                        else
                        {
                            servers[serverNumber - 1].Enabled = false;
                            EnabledServer = "Не выбран";
                        }
                        response += $"\n{serverNumber}) {server.Name} - {EnabledServer}";
                        serverNumber++;
                    }
                    string json = JsonSerializer.Serialize(servers, options);
                    File.WriteAllText(pathServers, json);
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

        private async Task<string> RconServerStop()
        {
            string response = "";
            if (rcon != null)
            {
                response = await rcon.SendCommandAsync("stop");
            }
            if (string.IsNullOrEmpty(response))
            {
                response = "Не удалось получить ответ от сервера. Возможно сервер выключен.";
            }
            else
            {
                response = "Остановка сервера.";
            }
            return response.Trim();
        }

        private async Task RconServerAnyAsync(string command)
        {
            string response = "";
            if (rcon != null)
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
        }
    }
}