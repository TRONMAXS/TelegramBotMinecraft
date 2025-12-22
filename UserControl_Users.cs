using Microsoft.Data.Sqlite;

namespace TelegramBotMinecraft
{
    public partial class UserControl_Users : UserControl
    {
        public UserControl_Users()
        {
            InitializeComponent();
            this.Load += UserControl_Users_Load;
        }

        private void UserControl_Users_Load(object sender, EventArgs e)
        {
                LoadUserList();
        }

        private void LoadUserList()
        {

            int selectedIndex = listBox1.SelectedIndex;
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Users", connection);
                SqliteDataReader reader = command.ExecuteReader();

                listBox1.Items.Clear();
                if (Convert.ToInt32(reader["ID"]) == selectedIndex + 1)
                {
                    while (reader.Read())
                    {
                        listBox1.Items.Add(reader["Name"]);
                        LoadServersListBox();
                        LoadCommandsListBox();
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
    }
}
