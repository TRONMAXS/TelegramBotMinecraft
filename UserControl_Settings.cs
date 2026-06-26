using Microsoft.Data.Sqlite;


namespace TelegramBotMinecraft
{
    public partial class UserControl_Settings : UserControl
    {

        public static TelegramBot telegramBot;
        private bool TelegramBotStart = false;

        public UserControl_Settings()
        {
            InitializeComponent();
            this.Load += UserControl_Settings_Load;
            btn_SaveAndReconnect.Click += SaveSettingsBOT;
            btn_SaveMain.Click += SaveSettings;
            btn_OnBot.Click += (sender, e) =>
            {
                TelegramBot.StartBotTelegram();
                TelegramBotStart = true;
                btn_OnBot.Enabled = false;
                btn_OffBot.Enabled = true;

            };
            btn_OffBot.Click += (sender, e) =>
            {
                TelegramBot.StopBotTelegram();
                TelegramBotStart = false;
                btn_OffBot.Enabled = false;
                btn_OnBot.Enabled = true;
            };

            App.userControl_Settings = this;
            TelegramBot.userControl_Settings = this;
            LoggerService.ConsoleBot = this.rtb_ConsoleBot;
        }

        public void ButtonOnBotTelegram()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(ButtonOnBotTelegram));
                return;
            }
            TelegramBotStart = false;
            btn_OnBot.Enabled = true;
            btn_OffBot.Enabled = false;
        }

        public void ButtonOffBotTelegram()
        {

            TelegramBotStart = true;
            btn_OnBot.Enabled = false;
            btn_OffBot.Enabled = true;
        }

        public void UserControl_Settings_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }
            tb_BotToken.UseSystemPasswordChar = true;
            tb_ProxyPassword.UseSystemPasswordChar = true;
            btn_ShowBotToken.Text = "🔒";
            btn_ShowPassProxy.Text = "🔒";
            LoadAllSettings();
        }

        private async void LoadAllSettings()
        {
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Settings", connection);
                SqliteDataReader reader = await command.ExecuteReaderAsync();


                while (await reader.ReadAsync())
                {
                    tb_BotToken.Text = reader["BotToken"].ToString();
                    chk_AutoStartBot.Checked = Convert.ToInt32(reader["Auto_Bot"]) == 1 ? true : false;
                    chk_StartToTray.Checked = Convert.ToInt32(reader["TrayOnStart"]) == 1 ? true : false;
                    chk_AutoStartup.Checked = Convert.ToInt32(reader["RunAtStartup"]) == 1 ? true : false;
                    chk_AutoRestartBot.Checked = Convert.ToInt32(reader["AutoReconnect"]) == 1 ? true : false;
                    chk_PushNotifications.Checked = Convert.ToInt32(reader["AutoReconnect"]) == 1 ? true : false;
                    tb_ProxyHost.Text = reader["Proxy_Host"].ToString();
                    tb_ProxyPort.Text = reader["Proxy_Port"].ToString();
                    tb_ProxyUsername.Text = reader["Proxy_Username"].ToString();
                    tb_ProxyPassword.Text = reader["Proxy_Password"].ToString();
                }
            }
        }

        private async void SaveSettingsBOT(object? sender, EventArgs e)
        {
            try
            {

                string OldBotToken = "";
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand getOldTokenCmd = new SqliteCommand("SELECT BotToken FROM Settings", connection);
                    OldBotToken = getOldTokenCmd.ExecuteScalar().ToString();

                    if (TelegramBotStart == true && OldBotToken != tb_BotToken.Text.Trim())
                    {
                        TelegramBot.StopBotTelegram();
                        TelegramBotStart = false;
                        btn_OnBot.Enabled = true;
                        btn_OffBot.Enabled = false;
                    }

                    SqliteCommand updateCommand = new SqliteCommand("UPDATE Settings SET (BotToken, Auto_Bot, AutoReconnect, Proxy_Host, Proxy_Port, Proxy_Username, Proxy_Password)" +
                        " = (@BotToken, @Auto_Bot, @AutoReconnect, @Proxy_Host, @Proxy_Port, @Proxy_Username, @Proxy_Password) WHERE ID == 1;", connection);
                    updateCommand.Parameters.AddWithValue("@BotToken", tb_BotToken.Text.Trim());
                    updateCommand.Parameters.AddWithValue("@Auto_Bot", Convert.ToInt32(chk_AutoStartBot.Checked));
                    updateCommand.Parameters.AddWithValue("@AutoReconnect", Convert.ToInt32(chk_AutoRestartBot.Checked));
                    updateCommand.Parameters.AddWithValue("@Proxy_Host", tb_ProxyHost.Text.Trim());
                    updateCommand.Parameters.AddWithValue("@Proxy_Port", tb_ProxyPort.Text.Trim());
                    updateCommand.Parameters.AddWithValue("@Proxy_Username", tb_ProxyUsername.Text.Trim());
                    updateCommand.Parameters.AddWithValue("@Proxy_Password", tb_ProxyPassword.Text.Trim());
                    await updateCommand.ExecuteNonQueryAsync();

                }

                LoggerService.MessageAppInfo("Настройки бота обновлены.");

                if (TelegramBotStart == false && OldBotToken != tb_BotToken.Text.Trim())
                {
                    TelegramBot.StartBotTelegram();
                    TelegramBotStart = true;
                    btn_OnBot.Enabled = false;
                    btn_OffBot.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при сохранении настроек бота: {ex.Message}");
            }
            finally { LoadAllSettings(); }
        }

        private void SaveSettings(object? sender, EventArgs e)
        {
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand("UPDATE Settings SET (TrayOnStart, RunAtStartup, Notifications) = (@TrayOnStart, @RunAtStartup, @Notifications) WHERE ID == 1;", connection);
                    command.Parameters.AddWithValue("@TrayOnStart", Convert.ToInt32(chk_StartToTray.Checked));
                    command.Parameters.AddWithValue("@RunAtStartup", Convert.ToInt32(chk_AutoStartup.Checked));
                    command.Parameters.AddWithValue("@Notifications", Convert.ToInt32(chk_PushNotifications.Checked));
                    command.ExecuteNonQuery();
                }
                chk_StartToTray.Checked = false;
                chk_AutoStartup.Checked = false;
                LoadAllSettings();
                LoggerService.MessageAppInfo("Настройки приложения сохранены.");
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при сохранении настроек приложения: {ex.Message}");
            }
        }

        private void ShowHideTokenPass_Click(object sender, EventArgs e)
        {
            if (sender == btn_ShowBotToken)
            {
                tb_BotToken.UseSystemPasswordChar = !tb_BotToken.UseSystemPasswordChar;
                if (tb_BotToken.UseSystemPasswordChar) btn_ShowBotToken.Text = "👁";
                else btn_ShowBotToken.Text = "🔒";
            }
            else if (sender == btn_ShowPassProxy)
            {
                tb_ProxyPassword.UseSystemPasswordChar = !tb_ProxyPassword.UseSystemPasswordChar;
                if (tb_ProxyPassword.UseSystemPasswordChar) btn_ShowPassProxy.Text = "👁";
                else btn_ShowPassProxy.Text = "🔒";
            }
        }

    }
}
