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
            LoadUserList();
        }

        private void DeleteUser(object? sender, EventArgs e)
        {
            string sqlDeleteUser = "DELETE FROM Users WHERE ID = @UserID;";
            int selectedIndex = GetSelectedUserID();
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(sqlDeleteUser, connection);
                    command.Parameters.AddWithValue("@UserID", selectedIndex);
                    int number = command.ExecuteNonQuery();
                    MessageBox.Show("Удаленно пользователей:" + number, "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                LoadUserList();
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
                    int number = command.ExecuteNonQuery();
                    MessageBox.Show("Добавленно пользователей:" + number, "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                textBox1.Clear();
                textBox2.Clear();
                LoadUserList();
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EditUser(object? sender, EventArgs e)
        {
            ToEditMode = true;
            button5.Enabled = true;
            button4.Enabled = false;
            int selectedIndex = GetSelectedUserID();
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
        }

        private void SaveUser(object? sender, EventArgs e)
        {
            if (ToEditMode)
            {
                if (!ValidateUserInputs())
                    return;
                string sqlUpdateUser = "UPDATE Users SET Name = @UserName, ID_TG = @UserIDTG WHERE ID = @UserID;";
                int selectedIndex = GetSelectedUserID();
                try
                {
                    using (var connection = new SqliteConnection("Data Source=Data.db"))
                    {
                        connection.Open();
                        SqliteCommand command = new SqliteCommand(sqlUpdateUser, connection);
                        command.Parameters.AddWithValue("@UserName", textBox1.Text.Trim());
                        command.Parameters.AddWithValue("@UserIDTG", Convert.ToInt32(textBox2.Text.Trim()));
                        command.Parameters.AddWithValue("@UserID", selectedIndex);
                        int number = command.ExecuteNonQuery();
                        MessageBox.Show("Обновленно пользователей:" + number, "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    textBox1.Clear();
                    textBox2.Clear();
                    LoadUserList();
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
            List<string> tableName = new List<string> { "UserServers", "UserCommands" };
            List<string> columnName = new List<string> { "ID_Server", "ID_Command" };

            string sqlServersList = "SELECT ID_Server FROM UserServers WHERE ID_User = @UserID;";
            string sqlCommandsList = "SELECT ID_Command FROM UserCommands WHERE ID_User = @UserID;";

            int selectedIndex = GetSelectedUserID();
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();

                    SqliteCommand commandToLoadServersList = new SqliteCommand(sqlServersList, connection);
                    commandToLoadServersList.Parameters.AddWithValue("@UserID", selectedIndex);

                    using (var reader = commandToLoadServersList.ExecuteReader())
                    {
                        for (int i = 0; i < checkedListBox1.Items.Count; i++)
                        {
                            if (checkedListBox1.GetItemChecked(i) == true && reader.Read() == false)
                            {
                                SqliteCommand commandAddUserPermissions = new SqliteCommand($@"INSERT OR IGNORE INTO [{tableName[0]}] (ID_User, [{columnName[0]}]) VALUES (@UserID, @Value);", connection);
                                commandAddUserPermissions.Parameters.AddWithValue("@UserID", selectedIndex);
                                commandAddUserPermissions.Parameters.AddWithValue("@Value", i + 1);
                                int number = commandAddUserPermissions.ExecuteNonQuery();

                            }
                            else if (checkedListBox1.GetItemChecked(i) == false && reader.Read() == true)
                            {
                                SqliteCommand commandDelUserPermissions = new SqliteCommand($@"DELETE FROM [{tableName[0]}] WHERE ID_User = @UserID;", connection);
                                commandDelUserPermissions.Parameters.AddWithValue("@UserID", selectedIndex);
                                int number = commandDelUserPermissions.ExecuteNonQuery();
                            }
                        }
                    }

                    SqliteCommand commandToLoadCommandsList = new SqliteCommand(sqlCommandsList, connection);
                    commandToLoadCommandsList.Parameters.AddWithValue("@UserID", selectedIndex);

                    using (var reader = commandToLoadCommandsList.ExecuteReader())
                    {
                        for (int i = 0; i < checkedListBox2.Items.Count; i++)
                        {
                            if (checkedListBox2.GetItemChecked(i) == true && reader.Read() == false)
                            {
                                SqliteCommand commandAddUserPermissions = new SqliteCommand($@"INSERT OR IGNORE INTO [{tableName[1]}] (ID_User, [{columnName[1]}]) VALUES (@UserID, @Value);", connection);
                                commandAddUserPermissions.Parameters.AddWithValue("@UserID", selectedIndex);
                                commandAddUserPermissions.Parameters.AddWithValue("@Value", i + 1);
                                int number = commandAddUserPermissions.ExecuteNonQuery();
                            }
                            else if (checkedListBox2.GetItemChecked(i) == false && reader.Read() == true)
                            {
                                SqliteCommand commandDelUserPermissions = new SqliteCommand($@"DELETE FROM [{tableName[1]}] WHERE ID_User = @UserID;", connection);
                                commandDelUserPermissions.Parameters.AddWithValue("@UserID", selectedIndex);
                                int number = commandDelUserPermissions.ExecuteNonQuery();
                            }
                        }
                    }
                }
                checkedListBox1.ClearSelected();
                checkedListBox2.ClearSelected();
                LoadUserList();
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

                LoadServersListBox();
                LoadCommandsListBox();
            }
            dataGridView1.ClearSelection();
            groupBox1.Enabled = false;
        }

        private void DataGridView1SelectedRow(object? sender, EventArgs e)
        {
            groupBox1.Enabled = true;
            string sqlServersList = "SELECT ID_Server FROM UserServers WHERE ID_User = @UserID;";
            string sqlCommandsList = "SELECT ID_Command FROM UserCommands WHERE ID_User = @UserID;";
            int selectedIndex = GetSelectedUserID();

            for (int i = 0; i < checkedListBox1.Items.Count; i++) checkedListBox1.SetItemChecked(i, false);
            for (int i = 0; i < checkedListBox2.Items.Count; i++) checkedListBox2.SetItemChecked(i, false);

            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();

                SqliteCommand commandToLoadServersList = new SqliteCommand(sqlServersList, connection);
                commandToLoadServersList.Parameters.AddWithValue("@UserID", selectedIndex);

                using (var reader = commandToLoadServersList.ExecuteReader())
                {
                    while (reader.Read()) checkedListBox1.SetItemChecked(Convert.ToInt32(reader["ID_Server"]) - 1, true);
                }

                SqliteCommand commandToLoadCommandsList = new SqliteCommand(sqlCommandsList, connection);
                commandToLoadCommandsList.Parameters.AddWithValue("@UserID", selectedIndex);

                using (var reader = commandToLoadCommandsList.ExecuteReader())
                {
                    while (reader.Read()) checkedListBox2.SetItemChecked(Convert.ToInt32(reader["ID_Command"]) - 1, true);
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
