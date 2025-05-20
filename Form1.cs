using CoreRCON;
using CoreRCON.PacketFormats;
//using MinecraftServerRCON;
using System.Diagnostics;
using System.Net;
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
        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private string TextBoxAdmin;
        private string jsonFilePath;
        private static string json;
        private List<ServerConfig> servers = new List<ServerConfig>();
        private bool isServerRunning = false;
        private static readonly string BotToken = "7849309077:AAGH40mYb04wvXz2MflStS_vhl671iPgbkA";
        private static readonly HashSet<long> AllowedUsers = new HashSet<long>
        {
            903878687,         // твой ID
            -1002163535137     // групповой чат
        };

        private static readonly int ThreadId = 2258;
        RCON rcon;

        public Form1()
        {
            InitializeComponent();
            json = File.ReadAllText("Servers.json");
            servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);
            MessageBox.Show(json);

            /*rcon = RCONClient.INSTANCE;
            rcon.setupStream(servers[0].Ip, Convert.ToInt32(servers[0].RconPort), password: servers[0].RconPassword);*/



            
            this.Load += Form1_Load;
            
            this.KeyDown += Grid_KeyUp;
            this.Resize += Form1_Resize;

            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;

            /*// Скрываем окно
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Hide(); // спрятать форму*/

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
            notifyIcon1.BalloonTipTitle = "Бот запущен";
            notifyIcon1.BalloonTipText = "Ожидает команд через Telegram.";
            notifyIcon1.ShowBalloonTip(2000);


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
            Form3 form3 = new Form3();
            form3.Show();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }
        private async void button3_Click(object sender, EventArgs e)
        {
            json = File.ReadAllText("Servers.json");
            if (IsValidJson(json))
            {
                servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);
            }
            else
            {
                MessageBox.Show("Файл Servers.json содержит некорректный JSON!");
            }

            var newRcon = await ConnectToRconAsync(servers[0]);

            if (newRcon != null)
            {
                rcon = newRcon; // только если подключение удалось
                MessageBox.Show("Подключение успешно обновлено.");
            }
            MessageBox.Show(json);
            AppendText($"Json - файл серверов обновлён!");
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            botClient = new TelegramBotClient(BotToken);
            var me = await botClient.GetMe(); // проверка, что бот жив
            AppendText($"Бот {me.Username} запущен и готов к работе.");

            botClient.StartReceiving(
                new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync),
                cancellationToken: cts.Token
            );

            var newRcon = await ConnectToRconAsync(servers[0]);

            if (newRcon != null)
            {
                rcon = newRcon; // только если подключение удалось
                MessageBox.Show("Подключение успешно обновлено.");
            }
            _ = Task.Run(() => CheckServerAsync());
            _ = Task.Run(() => CheckRconAsync());
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
            if (rcon != null && rcon.Connected == true) { rcon.Dispose();}
            try
            {

                var rcon = new RCON(
                    IPAddress.Parse(config.Ip),
                    Convert.ToUInt16(config.RconPort),
                    config.RconPassword
                );

                await rcon.ConnectAsync();
                await rcon.AuthenticateAsync();

                AppendText($"Успешное подключение к серверу {config.Ip}:{config.RconPort}");
                return rcon;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TextBoxAdmin = textBox2.Text;
                textBox2.Text = null;
                if (TextBoxAdmin != null)
                {
                    switch (TextBoxAdmin)
                    {
                        case "start":
                            if (isServerRunning == false)
                            {
                                isServerRunning = true;

                                if (servers != null && servers.Count > 0)
                                {
                                    Process.Start(new ProcessStartInfo
                                    {
                                        FileName = "start.cmd",
                                        UseShellExecute = false,
                                        WorkingDirectory = @$"{servers[0].Path}"
                                    });
                                }
                                else
                                {
                                    MessageBox.Show($"servers == null {servers}");
                                }
                                notifyIcon1.BalloonTipTitle = "Сервер";
                                notifyIcon1.BalloonTipText = "Minecraft-сервер успешно запущен!";
                                notifyIcon1.ShowBalloonTip(5000); // 5 секунд
                            }
                            else
                            {
                                AppendText($"Сервер уже запущен!");
                            }
                                break;
                        case "stop":
                            _ = Task.Run(() => RconServerAnyAsync("stop"));
                            notifyIcon1.BalloonTipTitle = "Сервер";
                            notifyIcon1.BalloonTipText = "Minecraft-сервер остановлен!";
                            notifyIcon1.ShowBalloonTip(5000); // 5 секунд
                            break;
                        case "list":
                            _ = Task.Run(() => RconServerAnyAsync("list"));
                            break;

                        default:
                            _ = Task.Run(() => RconServerAnyAsync(TextBoxAdmin));
                            break;

                    }
                }
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {

            if (update.Type != UpdateType.Message || update.Message?.Type != MessageType.Text)
                return;

            var msg = update.Message;
            /*if (msg.MessageThreadId != ThreadId)
                return;*/


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
                            "/bot_server_start — запуск сервера\n" +
                            "/bot_server_check — статус сервера\n" +
                            "/bot_server_list — список игроков\n" +
                            "/bot_server_stop — остановка сервера\n" +
                            "/bot_world_delete — удалить мир";
                        break;

                    case "/bot_server_check":
                    case "/bot_server_check@xy8zjr4tqbot":
                        _ = Task.Run(() => CheckServerOperationAsync(msg.Chat.Id, msg.MessageThreadId));      
                        break;

                    case "/bot_server_start":
                    case "/bot_server_start@xy8zjr4tqbot":

                        if (isServerRunning == false)
                        {
                            isServerRunning = true;
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "start.cmd",
                                UseShellExecute = false,
                                //WorkingDirectory = @"G:\Server Minecraft - Forge - 1.20.1\"
                                WorkingDirectory = @$"{servers[0].Path}"
                            });
                            notifyIcon1.BalloonTipTitle = "Сервер";
                            notifyIcon1.BalloonTipText = "Minecraft-сервер успешно запущен!";
                            notifyIcon1.ShowBalloonTip(5000); // 5 секунд
                            chatReply = "Сервер запускается...";
                            await Task.Delay(20000);
                            _ = Task.Run(() => CheckServerReadyAsync(msg.Chat.Id, msg.MessageThreadId));
                        }
                        else
                        {
                            chatReply = "Сервер запущен!";
                        }
                            break;

                    case "/bot_world_delete":
                    case "/bot_world_delete@xy8zjr4tqbot":
                        //Directory.Delete(@"G:\Server Minecraft - Forge - 1.20.1\world", true);
                        Directory.Delete(@$"{servers[0].Path}world", true);
                        chatReply = "Мир успешно удалён!";
                        notifyIcon1.BalloonTipTitle = "Сервер";
                        notifyIcon1.BalloonTipText = "Мир успешно удалён!";
                        notifyIcon1.ShowBalloonTip(5000); // 5 секунд
                        break;

                    case "/bot_server_list":
                    case "/bot_server_list@xy8zjr4tqbot":
                        chatReply = await RconList();
                        break;

                    case "/bot_server_stop":
                    case "/bot_server_stop@xy8zjr4tqbot":
                        chatReply = await RconServerStop();
                        notifyIcon1.BalloonTipTitle = "Сервер";
                        notifyIcon1.BalloonTipText = "Minecraft-сервер остановлен!";
                        notifyIcon1.ShowBalloonTip(5000); // 5 секунд
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
                    if (rcon != null)
                    {
                        string response = await rcon.SendCommandAsync("list");
                        var match = System.Text.RegularExpressions.Regex.Match(response, @"There are (\d+) of a max of (\d+) players online");

                        this.Invoke(new Action(() =>
                        {
                            if (match.Success)
                            {
                                isServerRunning = true;
                                notifyIcon1.Icon = new Icon("Icons/server_start.ico");
                                label1.Text = "Сервер - работает";
                            }
                            else
                            {
                                isServerRunning = false;
                                notifyIcon1.Icon = new Icon("Icons/server_stop.ico");
                                label1.Text = "Сервер - выключен";
                            }
                        }));
                    }
                    else
                    {
                        // rcon == null
                        this.Invoke(new Action(() =>
                        {
                            isServerRunning = false;
                            notifyIcon1.Icon = new Icon("Icons/server_stop.ico");
                            label1.Text = "Сервер - выключен";
                        }));
                    }
                }
                catch (Exception)
                {
                    // Ошибка при попытке SendCommandAsync или других вызовах
                    this.Invoke(new Action(() =>
                    {
                        isServerRunning = false;
                        notifyIcon1.Icon = new Icon("Icons/server_stop.ico");
                        label1.Text = "Сервер - выключен";
                    }));
                }

                await Task.Delay(5000); // Ждем 5 секунд перед следующим запросом
            }
        }
        private async Task CheckRconAsync()
        {
            while (true)
            {
                if (rcon != null)
                {

                }
                else
                {
                    var newRcon = await ConnectToRconAsync(servers[0]);

                    if (newRcon != null)
                    {
                        rcon = newRcon;
                    }
                }

                await Task.Delay(5000); // Ждем 5 секунд перед следующим запросом
            }
        }

        private async Task CheckServerOperationAsync(long chatId, int? threadId)
        {
            string response = "";
            if (rcon != null)
            {
                try
                {
                    response = await rcon.SendCommandAsync("list");
                    var match = System.Text.RegularExpressions.Regex.Match(response, @"There are (\d+) of a max of (\d+) players online");
                    if (match.Success)
                    {
                        isServerRunning = true;
                        string text = $"✅ Сервер запущен!\nОнлайн: {match.Groups[1].Value} из {match.Groups[2].Value} игроков.";
                        if (threadId.HasValue)
                        {
                            await botClient.SendMessage(chatId, text, messageThreadId: threadId.Value);
                        }
                        else
                        {
                            await botClient.SendMessage(chatId, text);
                        }
                        return;
                    }
                }
                catch (Exception ex)
                {
                    string errorText = $"Ошибка при проверке состояния сервера: {ex.Message}";
                    if (threadId.HasValue)
                    {
                        await botClient.SendMessage(chatId, errorText, messageThreadId: threadId.Value);
                    }
                    else
                    {
                        await botClient.SendMessage(chatId, errorText);
                    }
                }
            }
            isServerRunning = false;
            string failText = "⚠️ Сервер выключен.";
            if (threadId.HasValue)
            {
                await botClient.SendMessage(chatId, failText, messageThreadId: threadId.Value);
            }
            else
            {
                await botClient.SendMessage(chatId, failText);
            }
        }
        private async Task CheckServerReadyAsync(long chatId, int? threadId)
        {
            if (rcon != null)
            {
                for (int i = 0; i < 15; i++) // макс 15 попыток
                {
                    try
                    {
                        string response = await rcon.SendCommandAsync("list");

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

                    await Task.Delay(3000); // подождать 3 секунды перед повтором
                }
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