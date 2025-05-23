using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace TelegramBotMinecraft
{
    public partial class Form2 : Form
    {
        private List<ServerConfig> servers = new List<ServerConfig>();
        private string jsonFilePath;

        public Form2()
        {
            InitializeComponent();
            this.Load += Form2_Load;
            button1.Click += ReadJson;
            button2.Click += SaveJson;
            checkedListBox1.ItemCheck += checkedListBox1_ItemCheck;
            openFileDialog1.Filter = "Json files (*.json)|*.json";
            saveFileDialog1.Filter = "Json files (*.json)|*.json";
        }

        void Form2_Load(object sender, EventArgs e)
        {
            jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Servers.json");

            string json = File.ReadAllText("Servers.json");
            var servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);
            textBox1.Text = json;

            LoadServersToList();
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
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string filename = saveFileDialog1.FileName;
            File.WriteAllText(filename, textBox1.Text);
            MessageBox.Show("Файл сохранен");
            LoadServersToList();
        }

        private void LoadServersToList()
        {
            try
            {
                string json = File.ReadAllText(jsonFilePath);
                servers = JsonSerializer.Deserialize<List<ServerConfig>>(json);

                
                checkedListBox1.Items.Clear();
                foreach (var server in servers)
                {
                    int index = checkedListBox1.Items.Add(server.Name);
                    checkedListBox1.SetItemChecked(index, server.Enabled);
                }

                textBox1.Text = json;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке серверов: " + ex.Message, "Ошибка");
            }
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Checked)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    if (i != e.Index)
                        checkedListBox1.SetItemChecked(i, false);
                }
            }

            BeginInvoke(new Action(() =>
            {
                if (e.Index >= 0 && e.Index < servers.Count)
                {
                    servers[e.Index].Enabled = checkedListBox1.GetItemChecked(e.Index);
                    SaveServersToFile();
                }
            }));
        }

        private void SaveServersToFile()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(servers, options);
            File.WriteAllText(jsonFilePath, json);
            textBox1.Text = json;
        }
    }

    public class ServerConfig
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Ip { get; set; }
        public string RconPort { get; set; }
        public string RconPassword { get; set; }
        public bool Enabled { get; set; }
    }
}
