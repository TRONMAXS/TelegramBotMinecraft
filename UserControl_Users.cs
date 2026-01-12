using Microsoft.Data.Sqlite;
using System.Data;
using System.Windows.Forms;

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
            dataGridView1.CellClick += DataGridView1SelectedRow;
            button4.Click += AddUser;
            button3.Click += DeleteUser;
            button2.Click += EditUser;
            button5.Click += SaveUser;
            button1.Click += SavePermissions;

            dataTable.Columns.Add("Имя", typeof(string));
            dataTable.Columns.Add("ID Telegram", typeof(int));

            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.AllowUserToAddRows = false;

            App.userControl_Users = this;
        }
        public void UserControl_Users_Load(object? sender, EventArgs e)
        {
            groupBox1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button4.Enabled = true;

            LoadUserList();
        }

        public void UserControl_Load()
        {
            groupBox1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button4.Enabled = true;

            LoadUserList();
        }

        private void DeleteUser(object? sender, EventArgs e)
        {
            int selectedIndex = GetSelectedUserID();
            if (selectedIndex < 0) return;

            string sqlDeleteUser = "DELETE FROM Users WHERE ID = @UserID;";
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(sqlDeleteUser, connection);
                    command.Parameters.AddWithValue("@UserID", selectedIndex);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Пользователь удален", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                UserControl_Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddUser(object? sender, EventArgs e)
        {
            if (!ValidateUserInputs())
                return;
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand("INSERT INTO Users (Name, ID_TG) VALUES (@textBox1, @textBox2);", connection);
                    command.Parameters.AddWithValue("@textBox1", textBox1.Text.Trim());
                    command.Parameters.AddWithValue("@textBox2", Convert.ToInt32(textBox2.Text.Trim()));
                    command.ExecuteNonQuery();
                    MessageBox.Show("Пользователь добавлен", "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                textBox1.Clear();
                textBox2.Clear();
                UserControl_Load();
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EditUser(object? sender, EventArgs e)
        {
            int selectedIndex = GetSelectedUserID();
            if(selectedIndex < 0) return;

            ToEditMode = true;
            button5.Enabled = true;
            button4.Enabled = false;
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand commandToGetUser = new SqliteCommand("SELECT * FROM Users WHERE ID = @UserID;", connection);
                commandToGetUser.Parameters.AddWithValue("@UserID", selectedIndex);
                SqliteDataReader reader = commandToGetUser.ExecuteReader();
                while (reader.Read())
                {
                    textBox1.Text = reader["Name"].ToString();
                    textBox2.Text = reader["ID_TG"].ToString();
                }
            }
            UserControl_Load();
        }

        private void SaveUser(object? sender, EventArgs e)
        {
            if (ToEditMode)
            {
                if (!ValidateUserInputs())
                    return;
                string sqlUpdateUser = "UPDATE Users SET Name = @UserName, ID_TG = @UserIDTG WHERE ID = @UserID;";
                int selectedIndex = GetSelectedUserID();
                if (selectedIndex == -1) return;
                try
                {
                    using (var connection = new SqliteConnection("Data Source=Data.db"))
                    {
                        connection.Open();
                        SqliteCommand command = new SqliteCommand(sqlUpdateUser, connection);
                        command.Parameters.AddWithValue("@UserName", textBox1.Text.Trim());
                        command.Parameters.AddWithValue("@UserIDTG", Convert.ToInt32(textBox2.Text.Trim()));
                        command.Parameters.AddWithValue("@UserID", selectedIndex);
                        command.ExecuteNonQuery();
                        MessageBox.Show("Пользователь обновлен", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    textBox1.Clear();
                    textBox2.Clear();
                    UserControl_Load();
                    ToEditMode = false;
                    button5.Enabled = false;
                    button4.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void SavePermissions(object? sender, EventArgs e)
        {
            int selectedIndex = GetSelectedUserID();
            if (selectedIndex == -1) return;
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();

                    var userServers = new HashSet<string>();
                    using (var cmd = new SqliteCommand("SELECT s.Name" +
                                                   "\r\nFROM Servers s" +
                                                   "\r\nJOIN UserServers us ON s.ID = us.ID_Server" +
                                                   "\r\nJOIN Users u ON us.ID_User = u.ID" +
                                                   "\r\nWHERE u.ID = @UserID;", connection))
                    {
                        cmd.Parameters.AddWithValue("@UserID", selectedIndex);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                userServers.Add(reader["Name"].ToString());
                        }
                    }

                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        string serverName = checkedListBox1.Items[i].ToString();
                        bool isChecked = checkedListBox1.GetItemChecked(i);
                        bool hasPermission = userServers.Contains(serverName);
                        int IDServer;

                        using (var GetIDServer = new SqliteCommand("SELECT ID FROM Servers WHERE Name = @ServerName;", connection))
                        {
                            GetIDServer.Parameters.AddWithValue("@ServerName", serverName);
                            IDServer = Convert.ToInt32(GetIDServer.ExecuteScalar());
                        }

                        if (isChecked && !hasPermission)
                        {
                            using var AddServer = new SqliteCommand("INSERT OR IGNORE INTO UserServers (ID_User, ID_Server) VALUES (@UserID, @ServerID);", connection);
                            AddServer.Parameters.AddWithValue("@UserID", selectedIndex);
                            AddServer.Parameters.AddWithValue("@ServerID", IDServer);
                            AddServer.ExecuteNonQuery();
                        }
                        else if (!isChecked && hasPermission)
                        {
                            using var DelServer = new SqliteCommand("DELETE FROM UserServers WHERE ID_Server = @ServerID AND ID_User = @UserID;", connection);
                            DelServer.Parameters.AddWithValue("@ServerID", IDServer);
                            DelServer.Parameters.AddWithValue("@UserID", selectedIndex);
                            DelServer.ExecuteNonQuery();
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
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                                userCommands.Add(reader["Command"].ToString());
                        }
                    }

                    for (int i = 0; i < checkedListBox2.Items.Count; i++)
                    {
                        string commandName = checkedListBox2.Items[i].ToString();
                        bool isChecked = checkedListBox2.GetItemChecked(i);
                        bool hasPermission = userCommands.Contains(commandName);
                        int IDCommand;

                        using (var GetIDCommand = new SqliteCommand("SELECT ID FROM Commands WHERE Command = @CommandName;", connection))
                        {
                            GetIDCommand.Parameters.AddWithValue("@CommandName", commandName);
                            IDCommand = Convert.ToInt32(GetIDCommand.ExecuteScalar());
                        }

                        if (isChecked && !hasPermission)
                        {
                            using var AddCommand = new SqliteCommand("INSERT OR IGNORE INTO UserCommands (ID_User, ID_Command) VALUES (@UserID, @CommandID);", connection);
                            AddCommand.Parameters.AddWithValue("@UserID", selectedIndex);
                            AddCommand.Parameters.AddWithValue("@CommandID", IDCommand);
                            AddCommand.ExecuteNonQuery();
                        }
                        else if (!isChecked && hasPermission)
                        {
                            using var DelCommand = new SqliteCommand("DELETE FROM UserCommands WHERE ID_Command = @CommandID AND ID_User = @UserID;", connection);
                            DelCommand.Parameters.AddWithValue("@CommandID", IDCommand);
                            DelCommand.Parameters.AddWithValue("@UserID", selectedIndex);
                            DelCommand.ExecuteNonQuery();
                        }
                    }
                }
                checkedListBox1.ClearSelected();
                checkedListBox2.ClearSelected();
                UserControl_Load();
                MessageBox.Show("Разрешения сохранены!", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadUserList()
        {
            dataTable.Clear();

            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Users", connection);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    dataTable.Rows.Add(reader["Name"], reader["ID_TG"]);
                }
                dataGridView1.DataSource = dataTable;
                dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
                dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;

                LoadServersListBox();
                LoadCommandsListBox();
            }
            dataGridView1.ClearSelection();
            groupBox1.Enabled = false;
        }

        private void DataGridView1SelectedRow(object? sender, EventArgs e)
        {
            int selectedIndex = GetSelectedUserID();
            if(selectedIndex == -1) return;

            groupBox1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;

            string sqlServersList = "SELECT ID_Server FROM UserServers WHERE ID_User = @UserID;";
            string sqlCommandsList = "SELECT ID_Command FROM UserCommands WHERE ID_User = @UserID;";

            string sqlGetNameServer = "SELECT Name FROM Servers WHERE ID = @ServerID;";
            string sqlGetNameCommand = "SELECT Command FROM Commands WHERE ID = @CommandID;";
            string selectedNameServer;
            string selectedNameCommand;


            for (int i = 0; i < checkedListBox1.Items.Count; i++) checkedListBox1.SetItemChecked(i, false);
            for (int i = 0; i < checkedListBox2.Items.Count; i++) checkedListBox2.SetItemChecked(i, false);

            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();

                SqliteCommand commandToLoadServersList = new SqliteCommand(sqlServersList, connection);
                commandToLoadServersList.Parameters.AddWithValue("@UserID", selectedIndex);

                using (var reader = commandToLoadServersList.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SqliteCommand commandToGetNameServer = new SqliteCommand(sqlGetNameServer, connection);
                        commandToGetNameServer.Parameters.AddWithValue("@ServerID", Convert.ToInt32(reader["ID_Server"]));
                        selectedNameServer = commandToGetNameServer.ExecuteScalar().ToString();

                        int index = checkedListBox1.Items.IndexOf(selectedNameServer);
                        checkedListBox1.SetItemChecked(index, true);
                    }
                }

                SqliteCommand commandToLoadCommandsList = new SqliteCommand(sqlCommandsList, connection);
                commandToLoadCommandsList.Parameters.AddWithValue("@UserID", selectedIndex);

                using (var reader = commandToLoadCommandsList.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SqliteCommand commandToGetNameCommand = new SqliteCommand(sqlGetNameCommand, connection);
                        commandToGetNameCommand.Parameters.AddWithValue("@CommandID", Convert.ToInt32(reader["ID_Command"]));
                        selectedNameCommand = commandToGetNameCommand.ExecuteScalar().ToString();

                        int index = checkedListBox2.Items.IndexOf(selectedNameCommand);
                        checkedListBox2.SetItemChecked(index, true);
                    }
                }
            }
        }

        private void LoadServersListBox()
        {
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Servers", connection);
                SqliteDataReader reader = command.ExecuteReader();

                checkedListBox1.Items.Clear();
                while (reader.Read())
                {
                    checkedListBox1.Items.Add(reader["Name"]);
                }
            }
        }

        private void LoadCommandsListBox()
        {
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Commands", connection);
                SqliteDataReader reader = command.ExecuteReader();

                checkedListBox2.Items.Clear();
                while (reader.Read())
                {
                    checkedListBox2.Items.Add(reader["Command"]);
                }
            }
        }

        private int GetSelectedUserID()
        {
            string sqlGetIDUsers = "SELECT ID FROM Users WHERE Name = @UserName;";
            string SelectedUserNameDataGrid = "";
            int selectedIndex;

            if (dataGridView1.SelectedRows.Count > 0)
                SelectedUserNameDataGrid = (dataGridView1.SelectedRows[0].Cells[0].Value).ToString();
            else return -1;

            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand commandToGetIDUser = new SqliteCommand(sqlGetIDUsers, connection);
                commandToGetIDUser.Parameters.AddWithValue("@UserName", SelectedUserNameDataGrid);
                selectedIndex = Convert.ToInt32(commandToGetIDUser.ExecuteScalar());
            }

            return selectedIndex;
        }

        private bool ValidateUserInputs()
        {
            string name = textBox1.Text.Trim();
            string idText = textBox2.Text.Trim();
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
