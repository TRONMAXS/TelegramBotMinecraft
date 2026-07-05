using Microsoft.Data.Sqlite;
using System.Data;

namespace TelegramBotMinecraft
{
    public partial class UserControl_Users : UserControl
    {
        DataTable dataTable = new DataTable();
        private bool ToEditMode = false;

        public UserControl_Users()
        {
            InitializeComponent();
            this.Load += UserControl_Users_Load;
            dgv_Users.CellClick += DataGridView1SelectedRow;
            btn_AddUsers.Click += AddUser;
            btn_DellUsers.Click += DeleteUser;
            btn_EditUsers.Click += EditUser;
            btn_SaveUsers.Click += SaveUser;
            btn_SavePerm.Click += SavePermissions;

            dataTable.Columns.Add("Имя", typeof(string));
            dataTable.Columns.Add("ID Telegram", typeof(int));

            dgv_Users.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv_Users.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv_Users.AllowUserToAddRows = false;

            App.userControl_Users = this;
        }
        public void UserControl_Users_Load(object? sender, EventArgs e)
        {
            if (this.DesignMode)
            {
                return;
            }

            gb_PermUsers.Enabled = false;
            btn_EditUsers.Enabled = false;
            btn_DellUsers.Enabled = false;
            btn_SaveUsers.Enabled = false;
            btn_AddUsers.Enabled = true;
            dgv_Users.Enabled = true;
            tb_IdUser.Clear();
            tb_NameUser.Clear();

            LoadUserList();
        }

        public void UserControl_Load()
        {
            if (this.DesignMode)
            {
                return;
            }

            gb_PermUsers.Enabled = false;
            btn_EditUsers.Enabled = false;
            btn_DellUsers.Enabled = false;
            btn_SaveUsers.Enabled = false;
            btn_AddUsers.Enabled = true;
            dgv_Users.Enabled = true;

            LoadUserList();
        }

        private async void DeleteUser(object? sender, EventArgs e)
        {
            string SelectedUserName = "";

            int selectedIndex = await GetSelectedUserID();
            if (selectedIndex < 0) return;

            if (dgv_Users.SelectedRows.Count > 0)
            {
                SelectedUserName = (dgv_Users.SelectedRows[0].Cells[0].Value).ToString();
            }

            string sqlDeleteUser = "DELETE FROM Users WHERE ID = @UserID;";
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand(sqlDeleteUser, connection);
                    command.Parameters.AddWithValue("@UserID", selectedIndex);
                    await command.ExecuteNonQueryAsync();
                    LoggerService.MessageAppInfo($"Пользователь [{SelectedUserName}] удален");
                }
                UserControl_Load();
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при удалении пользователя [{SelectedUserName}] : {ex.Message}");
            }
        }

        private async void AddUser(object? sender, EventArgs e)
        {
            string SelectedUserName = "";

            if (dgv_Users.SelectedRows.Count > 0)
            {
                SelectedUserName = (dgv_Users.SelectedRows[0].Cells[0].Value).ToString();
            }

            if (!ValidateUserInputs())
                return;
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("INSERT INTO Users (Name, ID_TG) VALUES (@textBox1, @textBox2);", connection);
                    command.Parameters.AddWithValue("@textBox1", tb_NameUser.Text.Trim());
                    command.Parameters.AddWithValue("@textBox2", Convert.ToInt32(tb_IdUser.Text.Trim()));
                    await command.ExecuteNonQueryAsync();
                    LoggerService.MessageAppInfo($"Пользователь [{SelectedUserName}] добавлен");
                }
                tb_NameUser.Clear();
                tb_IdUser.Clear();
                UserControl_Load();
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при добавлении пользователя [{SelectedUserName}] : {ex.Message}");
            }
        }

        private async void EditUser(object? sender, EventArgs e)
        {
            string SelectedUserName = "";
            if (dgv_Users.SelectedRows.Count > 0)
            {
                SelectedUserName = (dgv_Users.SelectedRows[0].Cells[0].Value).ToString();
            }

            int selectedIndex = await GetSelectedUserID();
            if (selectedIndex < 0) return;

            ToEditMode = true;
            btn_DellUsers.Enabled = false;
            btn_EditUsers.Enabled = false;
            dgv_Users.Enabled = false;
            btn_SaveUsers.Enabled = true;
            btn_AddUsers.Enabled = false;
            gb_PermUsers.Enabled = false;
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand commandToGetUser = new SqliteCommand("SELECT * FROM Users WHERE ID = @UserID;", connection);
                    commandToGetUser.Parameters.AddWithValue("@UserID", selectedIndex);
                    SqliteDataReader reader = await commandToGetUser.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        tb_NameUser.Text = reader["Name"].ToString();
                        tb_IdUser.Text = reader["ID_TG"].ToString();
                    }
                }
                //UserControl_Load();
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при изменении пользователя [{SelectedUserName}] : {ex.Message}");
            }
        }

        private async void SaveUser(object? sender, EventArgs e)
        {
            if (ToEditMode)
            {
                if (!ValidateUserInputs())
                    return;
                string sqlUpdateUser = "UPDATE Users SET Name = @UserName, ID_TG = @UserIDTG WHERE ID = @UserID;";
                int selectedIndex = await GetSelectedUserID();
                if (selectedIndex == -1) return;

                string SelectedUserName = "";
                if (dgv_Users.SelectedRows.Count > 0)
                {
                    SelectedUserName = (dgv_Users.SelectedRows[0].Cells[0].Value).ToString();
                }
                try
                {
                    using (var connection = new SqliteConnection("Data Source=Data.db"))
                    {
                        await connection.OpenAsync();
                        SqliteCommand command = new SqliteCommand(sqlUpdateUser, connection);
                        command.Parameters.AddWithValue("@UserName", tb_NameUser.Text.Trim());
                        command.Parameters.AddWithValue("@UserIDTG", Convert.ToInt32(tb_IdUser.Text.Trim()));
                        command.Parameters.AddWithValue("@UserID", selectedIndex);
                        await command.ExecuteNonQueryAsync();
                    }
                    tb_NameUser.Clear();
                    tb_IdUser.Clear();
                    ToEditMode = false;

                    UserControl_Load();

                    LoggerService.MessageAppInfo($"Пользователь [{SelectedUserName}] обновлен");
                }
                catch (Exception ex)
                {
                    LoggerService.ErrorAppInfo($"Ошибка при обновление пользователя [{SelectedUserName}] : {ex.Message}");
                }
            }
        }

        private async void SavePermissions(object? sender, EventArgs e)
        {
            int selectedIndex = await GetSelectedUserID();
            if (selectedIndex == -1) return;

            string SelectedUserName = "";
            if (dgv_Users.SelectedRows.Count > 0)
            {
                SelectedUserName = (dgv_Users.SelectedRows[0].Cells[0].Value).ToString();
            }

            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();

                    var userServers = new HashSet<string>();
                    using (var cmd = new SqliteCommand("SELECT s.Name" +
                                                   "\r\nFROM Servers s" +
                                                   "\r\nJOIN UserServers us ON s.ID = us.ID_Server" +
                                                   "\r\nJOIN Users u ON us.ID_User = u.ID" +
                                                   "\r\nWHERE u.ID = @UserID;", connection))
                    {
                        cmd.Parameters.AddWithValue("@UserID", selectedIndex);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                                userServers.Add(reader["Name"].ToString());
                        }
                    }

                    for (int i = 0; i < clb_Servers.Items.Count; i++)
                    {
                        string serverName = clb_Servers.Items[i].ToString();
                        bool isChecked = clb_Servers.GetItemChecked(i);
                        bool hasPermission = userServers.Contains(serverName);
                        int IDServer;

                        using (var GetIDServer = new SqliteCommand("SELECT ID FROM Servers WHERE Name = @ServerName;", connection))
                        {
                            GetIDServer.Parameters.AddWithValue("@ServerName", serverName);
                            IDServer = Convert.ToInt32(await GetIDServer.ExecuteScalarAsync());
                        }

                        if (isChecked && !hasPermission)
                        {
                            using var AddServer = new SqliteCommand("INSERT OR IGNORE INTO UserServers (ID_User, ID_Server) VALUES (@UserID, @ServerID);", connection);
                            AddServer.Parameters.AddWithValue("@UserID", selectedIndex);
                            AddServer.Parameters.AddWithValue("@ServerID", IDServer);
                            await AddServer.ExecuteNonQueryAsync();
                        }
                        else if (!isChecked && hasPermission)
                        {
                            using var DelServer = new SqliteCommand("DELETE FROM UserServers WHERE ID_Server = @ServerID AND ID_User = @UserID;", connection);
                            DelServer.Parameters.AddWithValue("@ServerID", IDServer);
                            DelServer.Parameters.AddWithValue("@UserID", selectedIndex);
                            await DelServer.ExecuteNonQueryAsync();
                        }
                    }

                    var userCommands = new HashSet<string>();
                    using (var cmd = new SqliteCommand("SELECT c.Command" +
                                                   "\r\nFROM Commands c" +
                                                   "\r\nJOIN UserCommands uc ON c.ID = uc.ID_Command" +
                                                   "\r\nJOIN Users u ON uc.ID_User = u.ID" +
                                                   "\r\nWHERE u.ID = @UserID", connection))
                    {
                        cmd.Parameters.AddWithValue("@UserID", selectedIndex);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                                userCommands.Add(reader["Command"].ToString());
                        }
                    }

                    for (int i = 0; i < clb_Commands.Items.Count; i++)
                    {
                        string commandName = clb_Commands.Items[i].ToString();
                        bool isChecked = clb_Commands.GetItemChecked(i);
                        bool hasPermission = userCommands.Contains(commandName);
                        int IDCommand;

                        using (var GetIDCommand = new SqliteCommand("SELECT ID FROM Commands WHERE Command = @CommandName;", connection))
                        {
                            GetIDCommand.Parameters.AddWithValue("@CommandName", commandName);
                            IDCommand = Convert.ToInt32(await GetIDCommand.ExecuteScalarAsync());
                        }

                        if (isChecked && !hasPermission)
                        {
                            using var AddCommand = new SqliteCommand("INSERT OR IGNORE INTO UserCommands (ID_User, ID_Command) VALUES (@UserID, @CommandID);", connection);
                            AddCommand.Parameters.AddWithValue("@UserID", selectedIndex);
                            AddCommand.Parameters.AddWithValue("@CommandID", IDCommand);
                            await AddCommand.ExecuteNonQueryAsync();
                        }
                        else if (!isChecked && hasPermission)
                        {
                            using var DelCommand = new SqliteCommand("DELETE FROM UserCommands WHERE ID_Command = @CommandID AND ID_User = @UserID;", connection);
                            DelCommand.Parameters.AddWithValue("@CommandID", IDCommand);
                            DelCommand.Parameters.AddWithValue("@UserID", selectedIndex);
                            await DelCommand.ExecuteNonQueryAsync();
                        }
                    }
                }
                clb_Servers.ClearSelected();
                clb_Commands.ClearSelected();
                UserControl_Load();
                LoggerService.MessageAppInfo($"Разрешения пользователя [{SelectedUserName}] сохранены");
            }
            catch (Exception ex)
            {
                LoggerService.ErrorAppInfo($"Ошибка при удалении пользователя [{SelectedUserName}] : {ex.Message}");
            }
        }

        private async void LoadUserList()
        {
            dataTable.Clear();

            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Users", connection);
                SqliteDataReader reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    dataTable.Rows.Add(reader["Name"], reader["ID_TG"]);
                }
                dgv_Users.DataSource = dataTable;
                dgv_Users.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                dgv_Users.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

                LoadServersListBox();
                LoadCommandsListBox();
            }
            dgv_Users.ClearSelection();
            gb_PermUsers.Enabled = false;
        }

        private async void DataGridView1SelectedRow(object? sender, EventArgs e)
        {
            int selectedIndex = await GetSelectedUserID();
            if (selectedIndex == -1) return;

            gb_PermUsers.Enabled = true;
            btn_EditUsers.Enabled = true;
            btn_DellUsers.Enabled = true;

            string sqlServersList = "SELECT ID_Server FROM UserServers WHERE ID_User = @UserID;";
            string sqlCommandsList = "SELECT ID_Command FROM UserCommands WHERE ID_User = @UserID;";

            string sqlGetNameServer = "SELECT Name FROM Servers WHERE ID = @ServerID;";
            string sqlGetNameCommand = "SELECT Command FROM Commands WHERE ID = @CommandID;";


            for (int i = 0; i < clb_Servers.Items.Count; i++) clb_Servers.SetItemChecked(i, false);
            for (int i = 0; i < clb_Commands.Items.Count; i++) clb_Commands.SetItemChecked(i, false);

            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();

                SqliteCommand commandToLoadServersList = new SqliteCommand(sqlServersList, connection);
                commandToLoadServersList.Parameters.AddWithValue("@UserID", selectedIndex);

                using (var reader = await commandToLoadServersList.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        SqliteCommand commandToGetNameServer = new SqliteCommand(sqlGetNameServer, connection);
                        commandToGetNameServer.Parameters.AddWithValue("@ServerID", Convert.ToInt32(reader["ID_Server"]));
                        var result = await commandToGetNameServer.ExecuteScalarAsync();

                        if (result != null && result != DBNull.Value)
                        {
                            string selectedNameServer = result.ToString();
                            int index = clb_Servers.FindStringExact(selectedNameServer);
                            if (index != -1)
                            {
                                clb_Servers.SetItemChecked(index, true);
                            }
                        }
                    }
                }

                SqliteCommand commandToLoadCommandsList = new SqliteCommand(sqlCommandsList, connection);
                commandToLoadCommandsList.Parameters.AddWithValue("@UserID", selectedIndex);

                using (var reader = await commandToLoadCommandsList.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        SqliteCommand commandToGetNameCommand = new SqliteCommand(sqlGetNameCommand, connection);
                        commandToGetNameCommand.Parameters.AddWithValue("@CommandID", Convert.ToInt32(reader["ID_Command"]));
                        var result = await commandToGetNameCommand.ExecuteScalarAsync();

                        if (result != null && result != DBNull.Value)
                        {
                            string selectedNameCommand = result.ToString();
                            int index = clb_Commands.FindStringExact(selectedNameCommand);
                            if (index != -1)
                            {
                                clb_Commands.SetItemChecked(index, true);
                            }
                        }
                    }
                }
            }
        }

        private async void LoadServersListBox()
        {
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Servers", connection);
                SqliteDataReader reader = await command.ExecuteReaderAsync();

                clb_Servers.Items.Clear();
                while (await reader.ReadAsync())
                {
                    clb_Servers.Items.Add(reader["Name"]);
                }
            }
        }

        private async void LoadCommandsListBox()
        {
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Commands", connection);
                SqliteDataReader reader = await command.ExecuteReaderAsync();

                clb_Commands.Items.Clear();
                while (await reader.ReadAsync())
                {
                    clb_Commands.Items.Add(reader["Command"]);
                }
            }
        }

        private async Task<int> GetSelectedUserID()
        {
            string sqlGetIDUsers = "SELECT ID FROM Users WHERE Name = @UserName;";
            string SelectedUserNameDataGrid = "";
            int selectedIndex;

            if (dgv_Users.SelectedRows.Count > 0)
                SelectedUserNameDataGrid = (dgv_Users.SelectedRows[0].Cells[0].Value).ToString();
            else return -1;

            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();
                SqliteCommand commandToGetIDUser = new SqliteCommand(sqlGetIDUsers, connection);
                commandToGetIDUser.Parameters.AddWithValue("@UserName", SelectedUserNameDataGrid);
                selectedIndex = Convert.ToInt32(await commandToGetIDUser.ExecuteScalarAsync());
            }

            return selectedIndex;
        }

        private bool ValidateUserInputs()
        {
            string name = tb_NameUser.Text.Trim();
            string idText = tb_IdUser.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Имя не должно быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[a-zA-Zа-яА-ЯёЁ0-9\s]+$"))
            {
                MessageBox.Show("Имя должно содержать только буквы, цифры и пробелы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (string.IsNullOrWhiteSpace(idText))
            {
                MessageBox.Show("Id не должен быть пустым.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(idText, @"^[0-9]+$"))
            {
                MessageBox.Show("Id должен содержать только цифры.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if (!int.TryParse(idText, out int id) || id <= 0)
            {
                MessageBox.Show("Id должен быть положительным числом.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}
