using Microsoft.Data.Sqlite;
using Telegram.Bot.Types;
using Color = System.Drawing.Color;

namespace TelegramBotMinecraft
{
    public partial class UserControl_Settings : UserControl
    {

        public static TelegramBot telegramBot;

        public UserControl_Settings()
        {
            InitializeComponent();
            this.Load += UserControl_Settings_Load;
            button1.Click += SaveSettingsBOT;
            button2.Click += SaveSettings;
            button3.Click += (sender, e) =>
            {
                TelegramBot.StartBotTelegram();
                button3.Visible = false;
                button4.Visible = true;

            };
            button4.Click += (sender, e) =>
            {
                TelegramBot.StopBotTelegram();
                button4.Visible = false;
                button3.Visible = true;
            };

            App.userControl_Settings = this;
            TelegramBot.userControl_Settings = this;
        }

        public async Task SaveLogBots()
        {
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT SaveLogs FROM Settings", connection);
                if (Convert.ToInt32(command.ExecuteScalar()) == 0) return;

            }
            await File.WriteAllTextAsync(@$"logs\latest.log", richTextBox1.Text);
        }

        public void ButtonOnBotTelegram()
        {
            button3.Visible = true;
            button4.Visible = false;
        }

        public void ButtonOffBotTelegram()
        {
            button3.Visible = false;
            button4.Visible = true;
        }

        public void UserControl_Settings_Load(object sender, EventArgs e)
        {
            LoadAllSettings();
        }

        private void LoadAllSettings()
        {
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Settings", connection);
                SqliteDataReader reader = command.ExecuteReader();


                while (reader.Read())
                {
                    textBox1.Text = reader["BotToken"].ToString();
                    checkBox1.Checked = Convert.ToInt32(reader["Auto_Bot"]) == 1 ? true : false;
                    checkBox2.Checked = Convert.ToInt32(reader["TrayOnStart"]) == 1 ? true : false;
                    checkBox3.Checked = Convert.ToInt32(reader["SaveLogs"]) == 1 ? true : false;
                    checkBox4.Checked = Convert.ToInt32(reader["RunAtStartup"]) == 1 ? true : false;
                    checkBox5.Checked = Convert.ToInt32(reader["AutoReconnect"]) == 1 ? true : false;
                }
            }
        }

        private void SaveSettingsBOT(object? sender, EventArgs e)
        {
            try
            {
                string OldBotToken = "";
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand("SELECT BotToken FROM Settings", connection);
                    OldBotToken = command.ExecuteScalar().ToString();
                }

                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand("UPDATE Settings SET (BotToken, Auto_Bot, SaveLogs, AutoReconnect) = (@BotToken, @Auto_Bot, @SaveLogs, @AutoReconnect) WHERE ID == 1;", connection);
                    command.Parameters.AddWithValue("@BotToken", textBox1.Text.Trim());
                    command.Parameters.AddWithValue("@Auto_Bot", Convert.ToInt32(checkBox1.Checked));
                    command.Parameters.AddWithValue("@SaveLogs", Convert.ToInt32(checkBox3.Checked));
                    command.Parameters.AddWithValue("@AutoReconnect", Convert.ToInt32(checkBox5.Checked));
                    command.ExecuteNonQuery();

                }
                textBox1.Clear();
                checkBox1.Checked = false;
                checkBox3.Checked = false;
                checkBox5.Checked = false;
                MessageBox.Show("Настройки бота обновлены", "Настройки", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadAllSettings();

                if (OldBotToken != textBox1.Text.Trim())
                {
                    TelegramBot.StartBotTelegram();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SaveSettings(object? sender, EventArgs e)
        {
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand("UPDATE Settings SET (TrayOnStart, RunAtStartup) = (@TrayOnStart, @RunAtStartup) WHERE ID == 1;", connection);
                    command.Parameters.AddWithValue("@TrayOnStart", Convert.ToInt32(checkBox2.Checked));
                    command.Parameters.AddWithValue("@RunAtStartup", Convert.ToInt32(checkBox4.Checked));
                    command.ExecuteNonQuery();
                }
                checkBox2.Checked = false;
                checkBox4.Checked = false;
                LoadAllSettings();
                MessageBox.Show("Настройки сохранены!", "Настройки", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public async Task MessageChat(Chat chat, string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => MessageChat(chat, text)));
                return;
            }
            AppendTextColored($"[{DateTime.Now:HH:mm:ss}]", Color.Black);
            AppendTextColored($" [MSG]", Color.Green);
            AppendTextColored($": Cообщение от ", Color.Black);
            if (!string.IsNullOrEmpty(chat.Username)) AppendTextColored($"@{chat.Username} [{chat.Id}] ", Color.DarkOrange);
            else AppendTextColored($"{chat.FirstName} [{chat.Id}] ", Color.DarkOrange);
            AppendTextColored($": {text}", Color.Black);
            richTextBox1.AppendText($"{Environment.NewLine}");
            richTextBox1.ScrollToCaret();
        }

        public async Task MessageBotInfo(string Message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => MessageBotInfo(Message)));
                return;
            }
            AppendTextColored($"[{DateTime.Now:HH:mm:ss}]", Color.Black);
            AppendTextColored($" [INFO]", Color.Blue);
            AppendTextColored(Message, Color.Black);
            richTextBox1.AppendText($"{Environment.NewLine}");
            richTextBox1.ScrollToCaret();
        }

        public async Task StartBotInfo(string FirstNameBot, string UsernameBot)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => StartBotInfo(FirstNameBot, UsernameBot)));
                return;
            }
            AppendTextColored($"[{DateTime.Now:HH:mm:ss}]", Color.Black);
            AppendTextColored($" [INFO]", Color.Blue);
            AppendTextColored($": Бот ", Color.Black);
            AppendTextColored($"{FirstNameBot} [@{UsernameBot}]", Color.DarkOrange);
            AppendTextColored($" успешно авторизован и запущен.", Color.Black);
            richTextBox1.AppendText($"{Environment.NewLine}");
            richTextBox1.ScrollToCaret();
        }

        public async Task ErrorBotInfo(string Message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ErrorBotInfo(Message)));
                return;
            }
            AppendTextColored($"[{DateTime.Now:HH:mm:ss}]", Color.Black);
            AppendTextColored($" [ERROR]", Color.Red);
            AppendTextColored($": {Message}", Color.Black);
            richTextBox1.AppendText($"{Environment.NewLine}");
            richTextBox1.ScrollToCaret();
        }

        private void AppendTextColored(string text, Color color, bool bold = false)
        {
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionColor = color;

            if (bold)
                richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
            else
                richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);

            richTextBox1.AppendText(text);
            richTextBox1.SelectionColor = richTextBox1.ForeColor;
        }
    }
}
