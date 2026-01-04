using Microsoft.Data.Sqlite;
using System.Windows.Forms;

namespace TelegramBotMinecraft
{
    public partial class UserControl_Servers : UserControl
    {
        public UserControl_Servers()
        {
            InitializeComponent();
            listBox1.SelectedIndexChanged += LoadServersValues;
            this.Load += UserControl_Servers_Load;
            button4.Click += DeleteUser;
            button3.Click += AddServer;
            button1.Click += SaveServer;
            button2.Click += Cancle;


            App.userControl_Servers = this;
        }

        public void UserControl_Servers_Load(object sender, EventArgs e)
        {
            LoadServersList();
        }

        private void AddServer(object? sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            checkBox1.Checked = false;
            textBox6.Clear();
            textBox7.Clear();
            groupBox1.Enabled = true;
            button2.Enabled = true;
        }

        private void Cancle(object? sender, EventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            checkBox1.Checked = false;
            textBox6.Clear();
            textBox7.Clear();
            groupBox1.Enabled = false;
            button2.Enabled = false;
            listBox1.ClearSelected();
        }

        private void SaveServer(object? sender, EventArgs e)
        {
            string sqlAddServer = "INSERT INTO Servers (Name, Connected, Path_Server, Java_args, ID_Process, Rcon_Enable, Rcon_Port, Rcon_Pass) " +
                                  "VALUES (@textBox1, @textBox2, @textBox3, @textBox4, @textBox5, @checkBox1, @textBox6, @textBox7);";
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
                    command.Parameters.AddWithValue("@textBox5", Convert.ToInt32(textBox5.Text.Trim()));
                    command.Parameters.AddWithValue("@checkBox1", Convert.ToInt32(checkBox1.Checked ? 1 : 0));
                    command.Parameters.AddWithValue("@textBox6", Convert.ToInt32(textBox6.Text.Trim()));
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

                LoadServersList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteUser(object? sender, EventArgs e)
        {
            string sqlDeleteServer = "DELETE FROM Servers WHERE ID = @ID;";
            int selectedIndex = GetSelectedServerIndex();
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
            listBox1.ClearSelected();
            groupBox1.Enabled = false;
        }

        private void LoadServersValues(object sender, EventArgs e)
        {
            groupBox1.Enabled = true;
            int selectedIndex = GetSelectedServerIndex();
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
        }

        private int GetSelectedServerIndex()
        {
            string sqlGetIDUsers = "SELECT ID FROM Servers WHERE Name = @ServerName;";
            string SelectedServerNameListBox = "";
            int selectedIndex;

            SelectedServerNameListBox = listBox1.Items[listBox1.SelectedIndex].ToString();

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
