using System.Drawing.Printing;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace TelegramBotMinecraft
{
    public partial class Form4 : Form
    {
        private readonly string pathServers;
        private string jsonServers;
        private readonly string pathUserSettings;
        private string jsonUserSettings;
        private readonly string pathSettings;
        private string jsonSettings;

        JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };

        private readonly List<string> AllowedCommands = new List<string>()
        { "/servers_list", "/server_enable <password> <number>",
          "/bot_server_start", "/bot_servers_check", "/bot_server_list",
          "/bot_server_stop", "/bot_server_command <password> <command>",
          "/bot_world_delete", "/help" 
        };
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
            listBox1.SelectedIndexChanged += ListBox1_SelectUsersSettings;
            checkedListBox1.ItemCheck += CheckedListBox1_ItemCheck;
            checkedListBox2.ItemCheck += CheckedListBox2_ItemCheck;

        }

        private void Form4_Load(object sender, EventArgs e)
        {
            LoadServersToList(GetCheckedListBox1());
            LoadCommandsToList(GetCheckedListBox2());
            LoadUsersToList();
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
                checkedListBox1.Enabled = false;
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
                checkedListBox2.Items.Clear();
                checkedListBox2.Enabled = false;
                int indexCommand;
                foreach (var Command in AllowedCommands)
                {
                    indexCommand = checkedListBox2.Items.Add(Command);
                    checkedListBox2.SetItemChecked(indexCommand, false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке команд: " + ex.Message, "Ошибка");
            }
        }

        private async void LoadUsersToList()
        {
            try
            {
                jsonUserSettings = File.ReadAllText(pathUserSettings);
                var UserSettings = JsonSerializer.Deserialize<List<UserSettings>>(jsonUserSettings);

                jsonSettings = File.ReadAllText(pathSettings);
                var Settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);

                listBox1.Items.Clear();
                
                int indexUser;
                foreach (var Setting in Settings)
                {
                    foreach (var User in Setting.ChatIds)
                    {
                        listBox1.Items.Add($"Имя: {User.Name} ; ID: {User.Identifier}");

                        if (!UserSettings.Any(s => s.Identifier == User.Identifier) && !UserSettings.Any(s => s.Name == User.Name))
                        {
                            UserSettings.Add(new UserSettings
                            {
                                Identifier = User.Identifier,
                                Name = User.Name,
                                AllowedServers = new List<AllowedServers>(),
                                AllowedCommands = new List<AllowedCommands>()
                            });
                        }
                        if (UserSettings.Any(s => s.Identifier == User.Identifier) &&
                            !Setting.ChatIds.Any(s => s.Identifier == User.Identifier))
                        {
                            var userToRemove = UserSettings.FirstOrDefault(s => s.Identifier == User.Identifier);
                            if (userToRemove != null)
                            {
                                UserSettings.Remove(userToRemove);
                            }
                        }
                    }
                }

                var identifiersInSettings = Settings
                    .SelectMany(setting => setting.ChatIds)
                    .Select(chatId => chatId.Identifier)
                    .ToHashSet();

                UserSettings.RemoveAll(user => !identifiersInSettings.Contains(user.Identifier));

                string updatedJsonStr = JsonSerializer.Serialize(UserSettings, options);
                await File.WriteAllTextAsync(pathUserSettings, updatedJsonStr);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке пользователей: " + ex.Message, "Ошибка");
            }
        }

        private async void ListBox1_SelectUsersSettings(object sender, EventArgs e)
        {
            try
            {
                jsonUserSettings = File.ReadAllText(pathUserSettings);
                var UserSettings = JsonSerializer.Deserialize<List<UserSettings>>(jsonUserSettings);
                if (UserSettings == null || listBox1.SelectedItem == null) return;

                jsonSettings = File.ReadAllText(pathSettings);
                var Settings = JsonSerializer.Deserialize<List<SettingsConfig>>(jsonSettings);

                int selectedIndex;
                string selectedIndexStr;

                checkedListBox1.Enabled = true;
                checkedListBox2.Enabled = true;

                if (listBox1.SelectedItem != null)
                {
                    selectedIndex = listBox1.SelectedIndex;
                    selectedIndexStr = listBox1.Items[selectedIndex].ToString();
                    string[] parts = selectedIndexStr.Split(' ');
                    foreach (var user in UserSettings.Select((value, index) => new { value, index }))
                    {
                        if (user.value.Identifier == parts[4])
                        {
                            selectedIndex = user.index;
                            break;
                        }
                    }


                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        string serverName = checkedListBox1.Items[i].ToString();
                        bool isAllowed = UserSettings[selectedIndex].AllowedServers.Any(s => s.Server == serverName);
                        checkedListBox1.SetItemChecked(i, isAllowed);
                    }

                    for (int i = 0; i < checkedListBox2.Items.Count; i++)
                    {
                        string Comand = checkedListBox2.Items[i].ToString();
                        bool isAllowed = UserSettings[selectedIndex].AllowedCommands.Any(s => s.Command == Comand);
                        checkedListBox2.SetItemChecked(i, isAllowed);
                    }

                }
            }
            catch (Exception ex)
            {
                
                MessageBox.Show("Ошибка при загрузки настроек пользователей: " + ex.Message, "Ошибка");
            }
            await Task.Delay(1000);

        }

        private async void CheckedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {
                jsonUserSettings = File.ReadAllText(pathUserSettings);
                var UserSettings = JsonSerializer.Deserialize<List<UserSettings>>(jsonUserSettings);

                int selectedIndexUsers = listBox1.SelectedIndex;

                if (selectedIndexUsers < 0 || UserSettings == null || UserSettings.Count == 0)
                    return;

                string ListBox1Server = checkedListBox1.Items[e.Index].ToString();
                var currentUserSettings = UserSettings[selectedIndexUsers].AllowedServers;


                if (e.NewValue == CheckState.Checked)
                {
                    if (!currentUserSettings.Any(s => s.Server == ListBox1Server))
                    {
                        currentUserSettings.Add(new AllowedServers { Server = ListBox1Server });
                    }
                }
                else
                {
                    var serverToRemove = currentUserSettings.FirstOrDefault(s => s.Server == ListBox1Server);
                    if (serverToRemove != null)
                    {
                        currentUserSettings.Remove(serverToRemove);
                    }
                }
                string updatedJsonStr = JsonSerializer.Serialize(UserSettings, options);
                await File.WriteAllTextAsync(pathUserSettings, updatedJsonStr);

                checkedListBox1.ClearSelected();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных пользователей: " + ex.Message, "Ошибка");
            }
        }

        private async void CheckedListBox2_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {
                jsonUserSettings = File.ReadAllText(pathUserSettings);
                var UserSettings = JsonSerializer.Deserialize<List<UserSettings>>(jsonUserSettings);

                int selectedIndexUsers = listBox1.SelectedIndex;

                if (selectedIndexUsers < 0 || UserSettings == null || UserSettings.Count == 0)
                    return;

                string ListBox2Server = checkedListBox2.Items[e.Index].ToString();
                var currentUserSettings = UserSettings[selectedIndexUsers].AllowedCommands;


                if (e.NewValue == CheckState.Checked)
                {
                    if (!currentUserSettings.Any(s => s.Command == ListBox2Server))
                    {
                        currentUserSettings.Add(new AllowedCommands { Command = ListBox2Server });
                    }
                }
                else
                {
                    var serverToRemove = currentUserSettings.FirstOrDefault(s => s.Command == ListBox2Server);
                    if (serverToRemove != null)
                    {
                        currentUserSettings.Remove(serverToRemove);
                    }
                }
                string updatedJsonStr = JsonSerializer.Serialize(UserSettings, options);
                await File.WriteAllTextAsync(pathUserSettings, updatedJsonStr);

                checkedListBox1.ClearSelected();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении данных пользователей: " + ex.Message, "Ошибка");
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
