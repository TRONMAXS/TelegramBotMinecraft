using Microsoft.Data.Sqlite;

namespace TelegramBotMinecraft
{
    public partial class UserControl_Servers : UserControl
    {

        private bool FlagAddServer = false;

        public UserControl_Servers()
        {
            InitializeComponent();
            lb_Servers.MouseClick += LoadServersValues;
            this.Load += UserControl_Servers_Load;
            btn_DellServer.Click += DeleteServer;
            btn_AddServer.Click += AddORCancle;
            btn_CancleServer.Click += AddORCancle;
            btn_SaveServer.Click += SaveServer;
            btn_UpdateServer.Click += UpdateServer;

            tb_NameServer.TextChanged += TextBoxAll_TextChanged;
            tb_IpAndPortServer.TextChanged += TextBoxAll_TextChanged;
            tb_PathToServer.TextChanged += TextBoxAll_TextChanged;
            tb_JavaArgsServer.TextChanged += TextBoxAll_TextChanged;
            tb_IdProcessServer.TextChanged += TextBoxAll_TextChanged;
            tb_RconPort.TextChanged += TextBoxAll_TextChanged;
            tb_RconPassword.TextChanged += TextBoxAll_TextChanged;

            App.userControl_Servers = this;
        }

        private void TextBoxAll_TextChanged(object? sender, EventArgs e)
        {
            if (FlagAddServer == false)
            {
                btn_UpdateServer.Enabled = true;
            }
            btn_CancleServer.Enabled = true;
        }

        public void UserControl_Servers_Load(object sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }
            FlagAddServer = false;
            gb_SettingsServer.Enabled = false;
            btn_UpdateServer.Enabled = false;
            btn_SaveServer.Enabled = false;
            btn_CancleServer.Enabled = false;
            btn_AddServer.Enabled = true;
            btn_DellServer.Enabled = false;

            LoadServersList();
        }

        private void AddORCancle(object? sender, EventArgs e)
        {
            if (sender == btn_CancleServer && FlagAddServer == false)
            {
                LoadServersValues(null, null);
                return;
            }

            tb_NameServer.Clear();
            tb_IpAndPortServer.Clear();
            tb_PathToServer.Clear();
            tb_JavaArgsServer.Clear();
            tb_IdProcessServer.Clear();
            btn_OnOffRconSettings.Checked = false;
            tb_RconPort.Clear();
            tb_RconPassword.Clear();
            lb_Servers.ClearSelected();

            if (sender == btn_AddServer)
            {
                FlagAddServer = true;
                gb_SettingsServer.Enabled = true;
                btn_CancleServer.Enabled = true;
                btn_AddServer.Enabled = false;
                btn_DellServer.Enabled = false;
                btn_UpdateServer.Enabled = false;
                btn_SaveServer.Enabled = true;
            }
            else if (sender == btn_CancleServer)
            {
                FlagAddServer = false;
                gb_SettingsServer.Enabled = false;
                btn_CancleServer.Enabled = false;
                btn_DellServer.Enabled = false;
                btn_AddServer.Enabled = true;
                btn_UpdateServer.Enabled = false;
                btn_SaveServer.Enabled = false;
            }
        }

        private async void SaveServer(object? sender, EventArgs e)
        {
            if (tb_NameServer.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Пажалуйста, укажите имя сервера", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sqlAddServer = "INSERT INTO Servers (Name, Connected, Path_Server, Java_args, Rcon_Enable, Rcon_Port, Rcon_Pass) " +
                                  "VALUES (@textBox1, @textBox2, @textBox3, @textBox4, @checkBox1, @textBox6, @textBox7);";
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand(sqlAddServer, connection);
                    command.Parameters.AddWithValue("@textBox1", tb_NameServer.Text.Trim());
                    command.Parameters.AddWithValue("@textBox2", tb_IpAndPortServer.Text.Trim());
                    command.Parameters.AddWithValue("@textBox3", tb_PathToServer.Text.Trim());
                    command.Parameters.AddWithValue("@textBox4", tb_JavaArgsServer.Text.Trim());
                    command.Parameters.AddWithValue("@checkBox1", Convert.ToInt32(btn_OnOffRconSettings.Checked));
                    command.Parameters.AddWithValue("@textBox6", tb_RconPort.Text.Trim());
                    command.Parameters.AddWithValue("@textBox7", tb_RconPassword.Text.Trim());

                    int number = await command.ExecuteNonQueryAsync();
                }
                tb_NameServer.Clear();
                tb_IpAndPortServer.Clear();
                tb_PathToServer.Clear();
                tb_JavaArgsServer.Clear();
                tb_IdProcessServer.Clear();
                btn_OnOffRconSettings.Checked = false;
                tb_RconPort.Clear();
                tb_RconPassword.Clear();
                lb_Servers.ClearSelected();
                btn_AddServer.Enabled = true;

                LoggerService.MessageAppInfo($"Сервер [{tb_NameServer.Text.Trim()}] создан");
                LoadServersList();
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteExtendedErrorCode == 2067)
                {
                    MessageBox.Show($"Сервер с таким именем уже существует.\n" +
                        $"Пожалуйста, выберите другое название.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoggerService.ErrorAppInfo($"Ошибка при создании сервера: Сервер с таким именем уже существует.\n" +
                        $"Пожалуйста, выберите другое название.");
                }
            }

        }

        // +  сделать новую кнопку для обновления 
        // +  кнопку сохранени использовать только после нажатия на кнопку добавить 
        // -  сделать новую кнопку для дублирования серверов(SaveServer уже делает дублирование, только надо просить ввести другое имя)
        // +-  попробывать сделать чтобы кнопка сохранить была активной, только после изменения чего либо в настройках сервера

        private async void DeleteServer(object? sender, EventArgs e)
        {
            string NameDeletedServer = "";
            if (lb_Servers.SelectedItems.Count > 0)
            {
                NameDeletedServer = lb_Servers.Items[lb_Servers.SelectedIndex].ToString();
            }

            string sqlDeleteServer = "DELETE FROM Servers WHERE ID = @ID;";
            int selectedIndex = await GetSelectedServerIndex();
            if (selectedIndex == -1) return;

            var result = MessageBox.Show($"Вы уверены, что хотите удалить сервер {NameDeletedServer} ?\n" +
                "Это действие приведет к удалению всех связанных данных и восстановить их будет невозможно.",
                "Удалить сервер", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if(result == DialogResult.No) return;

            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand(sqlDeleteServer, connection);
                    command.Parameters.AddWithValue("@ID", selectedIndex);
                    int number = await command.ExecuteNonQueryAsync();
                }
                LoggerService.MessageAppInfo($"Сервер [{NameDeletedServer}] удален");
                MessageBox.Show($"Сервер {NameDeletedServer} уделён", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadServersList();
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при удалении сервера [{NameDeletedServer}] : {ex.Message}");
            }
        }

        private async void UpdateServer(object? sender, EventArgs e)
        {
            int selectedIndex = await GetSelectedServerIndex();
            if (selectedIndex == -1) return;

            string NameDeletedServer = "";

            string sqlUpdateServer = "UPDATE Servers SET Name = @textBox1, " +
                                             "\r\n    Connected = @textBox2, " +
                                             "\r\n    Path_Server = @textBox3, " +
                                             "\r\n    Java_args = @textBox4, " +
                                             "\r\n    Rcon_Enable = @checkBox1, " +
                                             "\r\n    Rcon_Port = @textBox6, " +
                                             "\r\n    Rcon_Pass = @textBox7" +
                                             "\r\nWHERE ID = @SelectId;";
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand(sqlUpdateServer, connection);
                    command.Parameters.AddWithValue("@textBox1", tb_NameServer.Text.Trim());
                    command.Parameters.AddWithValue("@textBox2", tb_IpAndPortServer.Text.Trim());
                    command.Parameters.AddWithValue("@textBox3", tb_PathToServer.Text.Trim());
                    command.Parameters.AddWithValue("@textBox4", tb_JavaArgsServer.Text.Trim());
                    command.Parameters.AddWithValue("@checkBox1", Convert.ToInt32(btn_OnOffRconSettings.Checked));
                    command.Parameters.AddWithValue("@textBox6", tb_RconPort.Text.Trim());
                    command.Parameters.AddWithValue("@textBox7", tb_RconPassword.Text.Trim());
                    command.Parameters.AddWithValue("@SelectId", selectedIndex);

                    int number = await command.ExecuteNonQueryAsync();
                }
                NameDeletedServer = tb_NameServer.Text.Trim();

                tb_NameServer.Clear();
                tb_IpAndPortServer.Clear();
                tb_PathToServer.Clear();
                tb_JavaArgsServer.Clear();
                tb_IdProcessServer.Clear();
                btn_OnOffRconSettings.Checked = false;
                tb_RconPort.Clear();
                tb_RconPassword.Clear();
                lb_Servers.ClearSelected();

                LoggerService.MessageAppInfo($"Сервер [{NameDeletedServer}] обновлен");
                LoadServersList();
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при обновлении сервера [{NameDeletedServer}] : {ex.Message}");
            }
        }

        private async void LoadServersList()
        {
            FlagAddServer = false;
            tb_NameServer.Clear();
            tb_IpAndPortServer.Clear();
            tb_PathToServer.Clear();
            tb_JavaArgsServer.Clear();
            tb_IdProcessServer.Clear();
            btn_OnOffRconSettings.Checked = false;
            tb_RconPort.Clear();
            tb_RconPassword.Clear();
            lb_Servers.ClearSelected();

            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Servers", connection);
                SqliteDataReader reader = await command.ExecuteReaderAsync();

                lb_Servers.Items.Clear();
                while (await reader.ReadAsync())
                {
                    lb_Servers.Items.Add(reader["Name"]);
                }
            }
            gb_SettingsServer.Enabled = false;
            btn_DellServer.Enabled = false;
        }

        private async void LoadServersValues(object sender, EventArgs e)
        {
            int selectedIndex = await GetSelectedServerIndex();
            if (selectedIndex == -1) return;

            gb_SettingsServer.Enabled = true;
            btn_DellServer.Enabled = true;
            btn_SaveServer.Enabled = false;

            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Servers", connection);
                SqliteDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    if (Convert.ToInt32(reader["ID"]) == selectedIndex)
                    {
                        tb_NameServer.Text = reader["Name"].ToString();
                        tb_IpAndPortServer.Text = reader["Connected"].ToString();
                        tb_PathToServer.Text = reader["Path_Server"].ToString();
                        tb_JavaArgsServer.Text = reader["Java_args"].ToString();
                        tb_IdProcessServer.Text = reader["ID_Process"].ToString();

                        btn_OnOffRconSettings.Checked = Convert.ToInt32(reader["Rcon_Enable"]) == 1 ? true : false;

                        tb_RconPort.Text = reader["Rcon_Port"].ToString();
                        tb_RconPassword.Text = reader["Rcon_Pass"].ToString();
                    }
                }
            }
            btn_UpdateServer.Enabled = false;
            btn_CancleServer.Enabled = false;
        }

        private async Task<int> GetSelectedServerIndex()
        {
            string sqlGetIDUsers = "SELECT ID FROM Servers WHERE Name = @ServerName;";
            string SelectedServerNameListBox = "";
            int selectedIndex;

            if (lb_Servers.SelectedItems.Count > 0)
                SelectedServerNameListBox = lb_Servers.Items[lb_Servers.SelectedIndex].ToString();
            else return -1;


            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();
                SqliteCommand commandToGetIDUser = new SqliteCommand(sqlGetIDUsers, connection);
                commandToGetIDUser.Parameters.AddWithValue("@ServerName", SelectedServerNameListBox);
                selectedIndex = Convert.ToInt32(await commandToGetIDUser.ExecuteScalarAsync());
            }
            return selectedIndex;
        }
    }
}
