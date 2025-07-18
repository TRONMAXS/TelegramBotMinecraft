using System.Text.Encodings.Web;
using System.Text.Json;

namespace TelegramBotMinecraft
{
    public partial class Form2 : Form
    {
        private List<ServerConfig> servers = new List<ServerConfig>();
        private string jsonFilePath;

        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        public Form2()
        {
            InitializeComponent();
            this.Load += Form2_Load;
            button1.Click += ReadJson;
            button2.Click += SaveJson;
            openFileDialog1.Filter = "Json files (*.json)|*.json";
            saveFileDialog1.Filter = "Json files (*.json)|*.json";
        }

        void Form2_Load(object sender, EventArgs e)
        {
            try
            {
                jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Servers.json");
                string json = File.ReadAllText("Servers.json");
                var servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);
                textBox1.Text = json;
                LoadServersToList();

            }
            catch
            {
                servers = new List<ServerConfig>
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
                File.WriteAllText(jsonFilePath, textBox1.Text);
                LoadServersToList();
            }
        }

        void ReadJson(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = openFileDialog1.FileName;
            string fileText = File.ReadAllText(filename);
            textBox1.Text = fileText;
            LoadServersToList();
        }

        void SaveJson(object sender, EventArgs e)
        {
            string json = JsonSerializer.Serialize(servers, options);
            File.WriteAllText(jsonFilePath, textBox1.Text);
            MessageBox.Show("Файл сохранен", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadServersToList();
        }

        private void LoadServersToList()
        {
            try
            {
                string json = File.ReadAllText(jsonFilePath);
                servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);

                if (servers[0].Name == "Name Server (example:  Vanilla(Survival) - 1.20.1)")
                {
                    listBox1.Items.Clear();
                    return;
                }

                listBox1.Items.Clear();
                foreach (var server in servers)
                {
                    int index = listBox1.Items.Add(server.Name);
                }

                textBox1.Text = json;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке серверов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                servers = new List<ServerConfig>
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
                File.WriteAllText(jsonFilePath, json);
                textBox1.Text = json;
                LoadServersToList();
            }
        }
    }

    public class ServerConfig
    {
        public string Name { get; set; }
        public string StartupPath { get; set; }
        public string FolderPath { get; set; }
        public string Ip { get; set; }
        public string RconPort { get; set; }
        public string RconPassword { get; set; }
        public string ConnectIp { get; set; }
        public string Port { get; set; }
    }
}
