using System.Drawing.Printing;
using System.Text.Json;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TelegramBotMinecraft
{
    public partial class Form4 : Form
    {
        private string Identifier;
        private string Name;
        private string AllowedServer;
        private string AllowedCommand;

        private string pathServers;
        private string jsonServers;
        private string pathUserSettings;
        private string jsonUserSettings;
        private string pathSettings;
        private string jsonSettings;

        //public List<AllowedCommands> AllowedCommands = new List<AllowedCommands>() {};
        public Form4()
        {
            InitializeComponent();

            pathServers = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Servers.json");
            jsonServers = File.ReadAllText(pathServers);

            pathUserSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserSettings.json");
            jsonUserSettings = File.ReadAllText(pathUserSettings);

            pathSettings = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.json");
            jsonSettings = File.ReadAllText(pathSettings);

            this.Load += Form4_Load;
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            LoadServersToList(GetCheckedListBox1());
            LoadCommandsToList(GetCheckedListBox2());
            LoadUsersToList();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private CheckedListBox GetCheckedListBox1()
        {
            return checkedListBox1;
        }

        private CheckedListBox GetCheckedListBox2()
        {
            return checkedListBox2;
        }

        private void LoadServersToList(CheckedListBox checkedListBox1)
        {
            try
            {
                jsonServers = File.ReadAllText(pathServers);
                var servers = JsonSerializer.Deserialize<List<ServerConfig>>(jsonServers);

                checkedListBox1.Items.Clear();
                int indexServer;
                foreach (var server in servers)
                {
                    indexServer = checkedListBox1.Items.Add(server.Name);
                    checkedListBox1.SetItemChecked(indexServer, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке серверов: " + ex.Message, "Ошибка");
            }
        }

        private void LoadCommandsToList(CheckedListBox checkedListBox2)
        {
            try
            {
                jsonUserSettings = File.ReadAllText(pathUserSettings);
                var Commands = JsonSerializer.Deserialize<List<UserSettings>>(jsonUserSettings);

                checkedListBox2.Items.Clear();
                int indexCommand;
                foreach (var ListCommands in Commands)
                {
                    foreach (var Allowed in ListCommands.AllowedCommands)
                    {
                        indexCommand = checkedListBox2.Items.Add(Allowed.Command);
                        checkedListBox2.SetItemChecked(indexCommand, false);

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке команд: " + ex.Message, "Ошибка");
            }
        }

        private void LoadUsersToList()
        {
            try
            {
                jsonSettings = File.ReadAllText(pathSettings);
                var Settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);

                listBox1.Items.Clear();
                int indexUser;
                foreach (var Setting in Settings)
                {
                    foreach (var User in Setting.ChatIds)
                    {
                        listBox1.Items.Add($"Имя: {User.Name} ; ID: {User.Identifier}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке пользователей: " + ex.Message, "Ошибка");
            }
        }

    }

    public class UserSettings
    {
        public string Identifier { get; set; }
        public string Name { get; set; }
        public List<AllowedServers> AllowedServers { get; set; }
        public List<AllowedCommands> AllowedCommands { get; set; }
    }
    public class AllowedServers
    {
        public string Server { get; set; }
    }
    public class AllowedCommands
    {
        public string Command { get; set; }
    }
}
