using Microsoft.Data.Sqlite;

namespace TelegramBotMinecraft
{
    public partial class UserControl_Servers : UserControl
    {
        public UserControl_Servers()
        {
            InitializeComponent();
            listBox1.MouseClick += LoadServersValues;
            this.Load += UserControl_Servers_Load;
            button4.Click += DeleteUser;
            button3.Click += AddORCancle;
            button2.Click += AddORCancle;
            button1.Click += SaveServer;
            button8.Click += UpdateServer;

            textBox1.TextChanged += TextBoxAll_TextChanged;
            textBox2.TextChanged += TextBoxAll_TextChanged;
            textBox3.TextChanged += TextBoxAll_TextChanged;
            textBox4.TextChanged += TextBoxAll_TextChanged;
            textBox5.TextChanged += TextBoxAll_TextChanged;
            textBox6.TextChanged += TextBoxAll_TextChanged;
            textBox7.TextChanged += TextBoxAll_TextChanged;

            App.userControl_Servers = this;
        }

        private void TextBoxAll_TextChanged(object? sender, EventArgs e)
        {
            if(button8.Enabled == true)
                button2.Enabled = true;
        }

        public void UserControl_Servers_Load(object sender, EventArgs e)
        {
            groupBox1.Enabled = false;
            button8.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = false;

            LoadServersList();
        }

        private void AddORCancle(object? sender, EventArgs e)
        {
            if (sender == button2 && button8.Enabled == true)
            {
                LoadServersValues(null, null);
                return;
            }

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            checkBox1.Checked = false;
            textBox6.Clear();
            textBox7.Clear();
            listBox1.ClearSelected();

            if (sender == button3)
            {
                groupBox1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = false;
                button4.Enabled = false;
                button8.Enabled = false;
                button1.Enabled = true;
            }
            else if (sender == button2)
            {
                groupBox1.Enabled = false;
                button2.Enabled = false;
                button4.Enabled = true;
                button3.Enabled = true;
                button8.Enabled = true;
                button1.Enabled = false;
            }
        }

        private void SaveServer(object? sender, EventArgs e)
        {
            if (textBox1.Text == string.Empty)
            {
                MessageBox.Show("Пажалуйста, укажите имя сервера", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string sqlAddServer = "INSERT INTO Servers (Name, Connected, Path_Server, Java_args, Rcon_Port, Rcon_Pass) " +
                                  "VALUES (@textBox1, @textBox2, @textBox3, @textBox4, @textBox6, @textBox7);";
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(sqlAddServer, connection);
                    command.Parameters.AddWithValue("@textBox1", textBox1.Text.Trim());
                    command.Parameters.AddWithValue("@textBox2", textBox2.Text.Trim());
                    command.Parameters.AddWithValue("@textBox3", textBox3.Text.Trim());
                    command.Parameters.AddWithValue("@textBox4", textBox4.Text.Trim());
                    command.Parameters.AddWithValue("@textBox6", textBox6.Text.Trim());
                    command.Parameters.AddWithValue("@textBox7", textBox7.Text.Trim());

                    int number = command.ExecuteNonQuery();
                    MessageBox.Show("Добавленно серверов:" + number, "Добавление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                checkBox1.Checked = false;
                textBox6.Clear();
                textBox7.Clear();
                listBox1.ClearSelected();
                button3.Enabled = true;

                LoadServersList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // +  сделать новую кнопку для обновления 
        // +  кнопку сохранени использовать только после нажатия на кнопку добавить 
        // -  сделать новую кнопку для дублирования серверов(SaveServer уже делает дублирование, только надо просить ввести другое имя)
        // -  попробывать сделать чтобы кнопка сохранить была активной, только после изменения чего либо в настройках сервера

        private void DeleteUser(object? sender, EventArgs e)
        {
            string sqlDeleteServer = "DELETE FROM Servers WHERE ID = @ID;";
            int selectedIndex = GetSelectedServerIndex();
            if (selectedIndex == -1) return;
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(sqlDeleteServer, connection);
                    command.Parameters.AddWithValue("@ID", selectedIndex);
                    int number = command.ExecuteNonQuery();
                    MessageBox.Show("Удаленно серверов:" + number, "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                LoadServersList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateServer(object? sender, EventArgs e)
        {
            int selectedIndex = GetSelectedServerIndex();
            if (selectedIndex == -1) return;

            string sqlUpdateServer = "UPDATE Servers SET Name = @textBox1, " +
                                             "\r\n    Connected = @textBox2, " +
                                             "\r\n    Path_Server = @textBox3, " +
                                             "\r\n    Java_args = @textBox4, " +
                                             "\r\n    Rcon_Port = @textBox6, " +
                                             "\r\n    Rcon_Pass = @textBox7" +
                                             "\r\nWHERE ID = @SelectId;";
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand(sqlUpdateServer, connection);
                    command.Parameters.AddWithValue("@textBox1", textBox1.Text.Trim());
                    command.Parameters.AddWithValue("@textBox2", textBox2.Text.Trim());
                    command.Parameters.AddWithValue("@textBox3", textBox3.Text.Trim());
                    command.Parameters.AddWithValue("@textBox4", textBox4.Text.Trim());
                    command.Parameters.AddWithValue("@textBox6", textBox6.Text.Trim());
                    command.Parameters.AddWithValue("@textBox7", textBox7.Text.Trim());
                    command.Parameters.AddWithValue("@SelectId", selectedIndex);

                    int number = command.ExecuteNonQuery();
                    MessageBox.Show("Обновденно серверов:" + number, "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
                checkBox1.Checked = false;
                textBox6.Clear();
                textBox7.Clear();
                listBox1.ClearSelected();

                LoadServersList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoadServersList()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            checkBox1.Checked = false;
            textBox6.Clear();
            textBox7.Clear();

            listBox1.ClearSelected();
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Servers", connection);
                SqliteDataReader reader = command.ExecuteReader();

                listBox1.Items.Clear();
                while (reader.Read())
                {
                    listBox1.Items.Add(reader["Name"]);
                }
            }
            groupBox1.Enabled = false;
        }

        private void LoadServersValues(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            if (index == -1) return;

            groupBox1.Enabled = true;
            button8.Enabled = true;
            button4.Enabled = true;

            int selectedIndex = GetSelectedServerIndex();
            if (selectedIndex == -1) return;

            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Servers", connection);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["ID"]) == selectedIndex)
                    {
                        textBox1.Text = reader["Name"].ToString();
                        textBox2.Text = reader["Connected"].ToString();
                        textBox3.Text = reader["Path_Server"].ToString();
                        textBox4.Text = reader["Java_args"].ToString();
                        textBox5.Text = reader["ID_Process"].ToString();

                        checkBox1.Checked = Convert.ToInt32(reader["Rcon_Enable"]) == 1 ? true : false;

                        textBox6.Text = reader["Rcon_Port"].ToString();
                        textBox7.Text = reader["Rcon_Pass"].ToString();
                    }
                }
            }
            button2.Enabled = false;
        }

        private int GetSelectedServerIndex()
        {
            string sqlGetIDUsers = "SELECT ID FROM Servers WHERE Name = @ServerName;";
            string SelectedServerNameListBox = "";
            int selectedIndex;

            if (listBox1.SelectedItems.Count > 0)
                SelectedServerNameListBox = listBox1.Items[listBox1.SelectedIndex].ToString();
            else return -1;


            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand commandToGetIDUser = new SqliteCommand(sqlGetIDUsers, connection);
                commandToGetIDUser.Parameters.AddWithValue("@ServerName", SelectedServerNameListBox);
                selectedIndex = Convert.ToInt32(commandToGetIDUser.ExecuteScalar());
            }
            return selectedIndex;
        }
    }
}
