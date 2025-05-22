using CoreRCON.Parsers.Csgo;
using System.Text.Json;
using Telegram.Bot.Types;
using static System.Net.Mime.MediaTypeNames;

namespace TelegramBotMinecraft
{
    public partial class Form3 : Form
    {
        private string BotToken;
        private string pathSettings;
        private string json;
        private bool suppressCheckedChanged = false;
        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
        public Form3()
        {
            InitializeComponent();
            textBox1.UseSystemPasswordChar = true;
            textBox1.MouseEnter += (s, e) => textBox1.UseSystemPasswordChar = false;
            textBox1.MouseLeave += (s, e) => textBox1.UseSystemPasswordChar = true;
            this.Load += Form3_Load;
            button1.Click += SaveBotToken;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;

            pathSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
            json = File.ReadAllText(pathSettings);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            _ = JsonSettings();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (suppressCheckedChanged) return;
            var settings = JsonSerializer.Deserialize<List<SettingsConfig>>(json);
            if (checkBox1.Checked)
            {
                MessageBox.Show("Уведомления включены");
                settings[0].Notifications = true;
                string json = System.Text.Json.JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, json);
            }
            else
            {
                MessageBox.Show("Уведомления отключены");
                settings[0].Notifications = false;
                string json = System.Text.Json.JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, json);
            }
        }

        private async Task JsonSettings()
        {
            var settings = JsonSerializer.Deserialize<List<SettingsConfig>>(json);
            suppressCheckedChanged = true;
            checkBox1.Checked = settings[0].Notifications;
            if (settings[0].BotToken == null)
            {
                BotToken = "Your bot token (example:  123456789:ABCdefGHIjklMNOpqrSTUvwxYZ)";
                settings[0].BotToken = BotToken;
                string json = System.Text.Json.JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, json);
            }
            else
            {
                textBox1.Text = settings[0].BotToken;
            }
            suppressCheckedChanged = false;
        }

        private void SaveBotToken(object sender, EventArgs e)
        {
            var settings = JsonSerializer.Deserialize<List<SettingsConfig>>(json);
            string input = textBox1.Text;
            if (!string.IsNullOrWhiteSpace(input))
            {
                BotToken = input;
                settings[0].BotToken = BotToken;
                string jsonStr = System.Text.Json.JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, jsonStr);
                MessageBox.Show("Токен бота успешно сохранён!");
            }
            else
            {
                MessageBox.Show("Введите токен бота перед сохранением.");
            }
        }
    }
    public class SettingsConfig
    {
        public bool Notifications { get; set; }
        public string BotToken { get; set; }
    }
}