using System.Text.Json;

namespace TelegramBotMinecraft
{
    public partial class Form3 : Form
    {
        private Form4 form4;

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
            button2.Click += AddToListBox;
            button3.Click += DeleteItemListBox;
            button4.Click += button4_Click;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;

            pathSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
            json = File.ReadAllText(pathSettings);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            _ = JsonSettings();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (form4 == null || form4.IsDisposed)
            {
                form4 = new Form4();
            }
            form4.Show();
            form4.BringToFront();
            form4.WindowState = FormWindowState.Normal;
            form4.Update();
        }

        private void AddToListBox(object sender, EventArgs e)
        {
            var settings = JsonSerializer.Deserialize<List<SettingsConfig>>(json);
            string inputName = textBox2.Text;
            string inputId = textBox3.Text;

            if (!string.IsNullOrWhiteSpace(inputName) && !string.IsNullOrWhiteSpace(inputId))
            {
                // Удаляем пример из списка ChatIds, если он есть
                settings[0].ChatIds.RemoveAll(c =>
                    c.Identifier == "example: 646516246" || c.Name == "example: Admin");

                // Добавляем новый ChatId
                settings[0].ChatIds.Add(new ChatId { Name = inputName, Identifier = inputId });

                // Обновляем listBox1
                listBox1.Items.Remove("Пример <Имя: 646516246 ; ID: Admin>");
                listBox1.Items.Add($"Имя: {inputName} ; ID: {inputId}");
                textBox3.Clear();
                textBox2.Clear();

                // Сохраняем изменения в JSON
                string jsonStr = System.Text.Json.JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, jsonStr);

                // Обновляем переменную json для дальнейшей работы
                json = jsonStr;
            }
            else
            {
                MessageBox.Show("Введите текст перед добавлением в список.", "Ошибка");
            }
        }

        private void DeleteItemListBox(object sender, EventArgs e)
        {
            var settings = JsonSerializer.Deserialize<List<SettingsConfig>>(json);
            if (listBox1.SelectedItem != null)
            {
                int selectedIndex = listBox1.SelectedIndex;
                if (selectedIndex >= 0 && selectedIndex < settings[0].ChatIds.Count)
                {
                    settings[0].ChatIds.RemoveAt(selectedIndex);
                    listBox1.Items.RemoveAt(selectedIndex);
                }
                if (listBox1.Items.Count == 0)
                {
                    listBox1.Items.Add("Пример <Имя: 646516246 ; ID: Admin>");
                    if (settings[0].ChatIds.Count == 0)
                    {
                        settings[0].ChatIds.Add(new ChatId());
                    }
                    settings[0].ChatIds[0].Identifier = "example: 646516246";
                    settings[0].ChatIds[0].Name = "example: Admin";
                }

                string jsonStr = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, jsonStr);
                json = jsonStr;
            }
            else
            {
                MessageBox.Show("Выберите элемент для удаления.", "Ошибка");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (suppressCheckedChanged) return;
            var settings = JsonSerializer.Deserialize<List<SettingsConfig>>(json);
            if (checkBox1.Checked)
            {
                MessageBox.Show("Уведомления включены", "Настройки");
                settings[0].Notifications = true;
                string json = System.Text.Json.JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, json);
            }
            else
            {
                MessageBox.Show("Уведомления отключены", "Настройки");
                settings[0].Notifications = false;
                string json = System.Text.Json.JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, json);
            }
        }

        private async Task JsonSettings()
        {
            var settings = JsonSerializer.Deserialize<List<SettingsConfig>>(json);
            suppressCheckedChanged = true;
            checkBox1.Checked = settings[0].Notifications ?? false;
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
            for (int i = 0; i< settings[0].ChatIds.Count; i++)
            {
                if (settings[0].ChatIds[i].Identifier == "example: 646516246" || settings[0].ChatIds[i].Name == "example: Admin")
                {
                    listBox1.Items.AddRange(new object[] { $"Пример <Имя: 646516246 ; ID: Admin>" });
                }
                else if (settings[0].ChatIds[i].Identifier != null || settings[0].ChatIds[i].Name != null)
                {
                    listBox1.Items.AddRange(new object[] { $"Имя: {settings[0].ChatIds[i].Name} ; ID: {settings[0].ChatIds[i].Identifier}" });
                }
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
                string jsonStr = JsonSerializer.Serialize(settings, options);
                File.WriteAllText(pathSettings, jsonStr);
                MessageBox.Show("Токен бота успешно сохранён!", "Telegram Bot");
            }
            else
            {
                MessageBox.Show("Введите токен бота перед сохранением.", "Telegram Bot");
            }
        }


    }
    public class SettingsConfig
    {
        public bool? Notifications { get; set; }
        public string BotToken { get; set; }
        public List<ChatId> ChatIds { get; set; }
    }
    public class ChatId
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
    }
}