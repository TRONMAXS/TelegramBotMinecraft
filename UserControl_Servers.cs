using Microsoft.Data.Sqlite;

namespace TelegramBotMinecraft
{
    public partial class UserControl_Servers : UserControl
    {
        public UserControl_Servers()
        {
            InitializeComponent();
            listBox1.SelectedIndexChanged += LoadServersValues;
            this.Load += UserControl_Servers_Load;
        }

        public void UserControl_Servers_Load(object sender, EventArgs e)
        {
            LoadServersList();
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
        }

        private void LoadServersValues(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex;
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Servers", connection);
                SqliteDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["ID"]) == selectedIndex + 1)
                    {
                        textBox1.Text = reader["Name"].ToString();
                        textBox2.Text = reader["Connected"].ToString();
                        textBox3.Text = reader["Path_Server"].ToString();
                        textBox4.Text = reader["Java_args"].ToString();
                        textBox5.Text = reader["ID_Process"].ToString();

                        if (Convert.ToInt32(reader["Rcon_Enable"]) == 1) checkBox1.Checked = true;
                        else checkBox1.Checked = false;
                        textBox6.Text = reader["Rcon_Port"].ToString();
                        textBox7.Text = reader["Rcon_Pass"].ToString();
                    }
                }
            }
        }
    }
}
