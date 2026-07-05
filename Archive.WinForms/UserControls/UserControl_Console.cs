using CoreRCON;
using Microsoft.Data.Sqlite;
using System.Diagnostics;
using System.Net;
using static System.Windows.Forms.AxHost;

namespace TelegramBotMinecraft
{
    public partial class UserControl_Console : UserControl
    {
        bool isServerSelected = false;
        string SelectedServerName;

        public static CancellationTokenSource _ctsLogUpdater;
        private static CancellationTokenSource _ctsStatusUpdater;

        public UserControl_Console()
        {
            InitializeComponent();
            this.Load += UserControl_Console_Load;
            lv_Servers.ColumnWidthChanging += listView1_ColumnWidthChanging;
            lv_Servers.SelectedIndexChanged += listView1_SelectedIndexChanged;
            btn_OnServer.Click += StartMineServer;
            btn_OffServer.Click += StopMineServer;

            MinecraftServerManager.userControl_Console = this;
            App.userControl_Console = this;

            /*int columnWidth = (lv_Servers.ClientSize.Width - 2) / lv_Servers.Columns.Count;
            foreach (ColumnHeader col in lv_Servers.Columns)
            {
                col.Width = columnWidth;
            }*/

            btn_EnterCommand.Click += (sender, e) =>
            {

            };

        }
        public static void StopLogUpdating()
        {
            _ctsLogUpdater?.Cancel();
        }

        private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true;
            e.NewWidth = lv_Servers.Columns[e.ColumnIndex].Width;
        }

        public void UserControl_Console_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            btn_OnServer.Enabled = false;
            btn_OffServer.Enabled = false;
            gb_ConsoleServer.Enabled = false;

            LoadServersList();
        }

        private async void LoadServersList()
        {
            lv_Servers.Items.Clear();
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Servers", connection);
                SqliteDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Text = reader["Name"].ToString();
                    lvi.ImageIndex = 0;
                    lv_Servers.Items.Add(lvi);
                }

            }
        }

        private async void listView1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lv_Servers.SelectedItems.Count == 0)
            {
                isServerSelected = false;

                gb_ConsoleServer.Enabled = false;
                btn_OnServer.Enabled = false;
                btn_OffServer.Enabled = false;
                return;
            }
            isServerSelected = true;

            gb_ConsoleServer.Enabled = true;
            btn_OnServer.Enabled = true;
            btn_OffServer.Enabled = true;

            if (lv_Servers.SelectedItems.Count > 0)
                SelectedServerName = lv_Servers.SelectedItems[0].Text;
            else return;
            _ctsStatusUpdater?.Cancel();
            _ = LoadInfoServer(SelectedServerName, 2);
            await UpdateConsoleServer(SelectedServerName);
        }

        public void TriggerServerInfoRefresh(string ServerName, int ServerState)
        {
            _ctsStatusUpdater?.Cancel();
            _ = LoadInfoServer(ServerName, ServerState);
        }

        private async Task LoadInfoServer(string ServerName, int ServerState)
        {
            tb_NameServer.Clear();
            tb_StatusServer.Clear();

            int IDProcessServer = -1;
            string ServerDataName = "";
            bool RconEnable = false;
            int RconPort = 0;
            string RconPass = "";

            bool ProcessEnabledCheck = false;
            bool RconEnabledCheck = false;

            int maxAttempts = 0;
            string statusText = "";

            _ctsStatusUpdater?.Cancel();
            _ctsStatusUpdater?.Dispose();

            _ctsStatusUpdater = new CancellationTokenSource();
            var token = _ctsStatusUpdater.Token;

            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT Name, ID_Process, Rcon_Enable, Rcon_Port, Rcon_Pass" +
                                                              " FROM Servers WHERE Name = @ServerName", connection);
                    command.Parameters.AddWithValue("@ServerName", ServerName);
                    SqliteDataReader reader = await command.ExecuteReaderAsync(token);

                    while (await reader.ReadAsync(token))
                    {
                        ServerDataName = reader["Name"].ToString();
                        IDProcessServer = Convert.ToInt32(reader["ID_Process"]);
                        RconEnable = Convert.ToBoolean(reader["Rcon_Enable"]);
                        RconPort = Convert.ToInt32(reader["Rcon_Port"]);
                        RconPass = reader["Rcon_Pass"].ToString();
                    }
                }
            }
            catch (SqliteException ex)
            {
                MessageBox.Show($"Ошибка базы данных: не удалось найти данные сервера.\nДетали: {ex.Message}", "Ошибка БД");
                return;
            }
            catch (OperationCanceledException)
            { return; }

            if (token.IsCancellationRequested) return;

            tb_NameServer.Text = ServerDataName;

            maxAttempts = ServerState == 2 ? 5 : 120;
            statusText = ServerState == 0 ? "Выключается" : (ServerState == 1 ? "Запускается" : "Проверка");

            for (int i = 0; i <= maxAttempts; i++)
            {
                if (token.IsCancellationRequested) return;

                if (ServerState == 2 && !isServerSelected) return;

                tb_StatusServer.Text = statusText + new string('.', i % 4);

                try
                {
                    if (IDProcessServer != -1)
                    {
                        using (var process = Process.GetProcessById(IDProcessServer))
                        {
                            if (process.ProcessName.Equals("java", StringComparison.OrdinalIgnoreCase))
                            {
                                ProcessEnabledCheck = true;
                            }
                            else ProcessEnabledCheck = false;
                        }
                    }
                }
                catch { ProcessEnabledCheck = false; }

                RconEnabledCheck = RconEnable && await CheckServerStatus(RconPort, RconPass);

                if (token.IsCancellationRequested) return;

                if (ServerState == 0)
                {
                    if (!ProcessEnabledCheck && !RconEnabledCheck)
                    {
                        tb_StatusServer.Text = "Выключен";
                        return;
                    }
                }
                else
                {
                    if (ProcessEnabledCheck && (!RconEnable || RconEnabledCheck))
                    {
                        tb_StatusServer.Text = "Запущен";
                        return;
                    }
                }

                await Task.Delay(1000, token);
            }
            if (token.IsCancellationRequested) return;
            tb_StatusServer.Text = ServerState == 0 ? "Запущен" : "Выключен";
        }

        private async Task<bool> CheckServerStatus(int Port, string Pass)
        {
            try
            {
                if (Port != 0 && !string.IsNullOrEmpty(Pass))
                {
                    using (RCON rcon = new RCON(IPAddress.Parse("127.0.0.1"), Convert.ToUInt16(Port), Pass))
                    {
                        await rcon.ConnectAsync();
                        await rcon.AuthenticateAsync();
                        string response = await rcon.SendCommandAsync("list");
                        return !string.IsNullOrEmpty(response);
                    }
                }
                else return false;
            }
            catch { return false; }
        }

        private async void StartMineServer(object? sender, EventArgs e)
        {
            if (lv_Servers.SelectedItems.Count > 0)
                SelectedServerName = lv_Servers.SelectedItems[0].Text;
            else return;

            if (await MinecraftServerManager.Start(SelectedServerName))
                await UpdateConsoleServer(SelectedServerName);
            btn_OffServer.Enabled = true;
            btn_OnServer.Enabled = false;
        }

        private async void StopMineServer(object? sender, EventArgs e)
        {
            if (lv_Servers.SelectedItems.Count > 0)
                SelectedServerName = lv_Servers.SelectedItems[0].Text;
            else return;
            if (await MinecraftServerManager.Stop(SelectedServerName))
                await UpdateConsoleServer(SelectedServerName);
            btn_OffServer.Enabled = false;
            btn_OnServer.Enabled = true;
        }

        private async Task UpdateConsoleServer(string ServerName)
        {
            _ctsLogUpdater?.Cancel();
            _ctsLogUpdater?.Dispose();

            _ctsLogUpdater = new CancellationTokenSource();
            var token = _ctsLogUpdater.Token;

            string pathToLogsServer = null;
            long lastPosition = 0;

            if (!isServerSelected) return;

            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT Path_Server FROM Servers WHERE Name = @ServerName", connection);
                    command.Parameters.AddWithValue("@ServerName", ServerName);
                    var result = await command.ExecuteScalarAsync();
                    if (result == null) return;
                    pathToLogsServer = Path.Combine(result.ToString(), "logs", "latest.log");
                }
            }
            catch (SqliteException ex)
            {
                MessageBox.Show($"Ошибка базы данных: не удалось найти путь к серверу.\nДетали: {ex.Message}", "Ошибка БД");
                return;
            }
            rtb_ConsoleBot.Clear();

            if (!File.Exists(pathToLogsServer)) return;

            try
            {
                using (var fs = new FileStream(pathToLogsServer, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var reader = new StreamReader(fs, System.Text.Encoding.UTF8, true, 1024, leaveOpen: true))
                    {
                        string initialContent = await reader.ReadToEndAsync();
                        if (!string.IsNullOrEmpty(initialContent))
                        {
                            rtb_ConsoleBot.Invoke(new Action(() =>
                            {
                                rtb_ConsoleBot.AppendText(initialContent);
                                rtb_ConsoleBot.ScrollToCaret();
                            }));
                        }
                        lastPosition = fs.Position;
                    }


                    string ServerNameSelectedOld = ServerName;
                    while (ServerNameSelectedOld == ServerName && !token.IsCancellationRequested)
                    {
                        FileInfo fi = new FileInfo(pathToLogsServer);
                        long currentLength = fi.Length;

                        if (fs.Length < lastPosition)
                        {
                            lastPosition = 0;
                            await Task.Delay(1000);
                            rtb_ConsoleBot.Invoke(new Action(() =>
                            {
                                rtb_ConsoleBot.Clear();
                            }));
                        }

                        if (fs.Length > lastPosition)
                        {
                            fs.Position = lastPosition;

                            using (var reader = new StreamReader(fs, System.Text.Encoding.UTF8, true, 1024, leaveOpen: true))
                            {
                                string content = await reader.ReadToEndAsync();
                                if (!string.IsNullOrEmpty(content) && !token.IsCancellationRequested)
                                {
                                    rtb_ConsoleBot.Invoke(new Action(() =>
                                    {
                                        rtb_ConsoleBot.AppendText(content);
                                        rtb_ConsoleBot.ScrollToCaret();
                                    }));
                                }
                                lastPosition = fs.Position;
                            }

                        }
                        await Task.Delay(1000, token);
                    }
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Не удалось прочитать файл логов.\nДетали: {ex.Message}", "Ошибка чтения файла");
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                MessageBox.Show($"Непредвиденная ошибка: {ex.Message}");
            }
        }
    }
}
