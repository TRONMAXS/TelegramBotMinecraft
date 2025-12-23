using Microsoft.Data.Sqlite;

namespace TelegramBotMinecraft
{
    public partial class UserControl_Users : UserControl
    {
        public UserControl_Users()
        {
            InitializeComponent();
            this.Load += UserControl_Users_Load;
            listBox1.SelectedIndexChanged += listBox1SelectedItem;
            //checkedListBox1.ItemCheck += CheckedListBox1_Servers;
            //checkedListBox2.ItemCheck += CheckedListBox2_Commands;
        }

        private void UserControl_Users_Load(object sender, EventArgs e)
        {
                LoadUserList();
        }

        private void LoadUserList()
        {
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Users", connection);
                SqliteDataReader reader = command.ExecuteReader();

                listBox1.Items.Clear();
                while (reader.Read())
                {
                    listBox1.Items.Add(reader["Name"]);
                    LoadServersListBox();
                    LoadCommandsListBox();
                }
            }
        }

        private void listBox1SelectedItem(object sender, EventArgs e)
        {
            int selectedIndex = listBox1.SelectedIndex + 1;

            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand commandToLoadServersList = new SqliteCommand($"SELECT ID_Server FROM UserServers WHERE ID_User == {selectedIndex}", connection);
                SqliteCommand commandToLoadCommandsList = new SqliteCommand($"SELECT ID_Command FROM UserCommands WHERE ID_User == {selectedIndex}", connection);
                SqliteDataReader readerToLoadServersList = commandToLoadServersList.ExecuteReader();
                SqliteDataReader readerToLoadCommandsList = commandToLoadCommandsList.ExecuteReader();

                for (int i = 1; i < checkedListBox1.Items.Count+1; i++)
                {
                    if (readerToLoadServersList.Read())
                    {
                        if (Convert.ToInt32(readerToLoadServersList["ID_Server"]) == i)
                        {
                            checkedListBox1.SetItemChecked(i-1, true);
                        }
                    }
                    else checkedListBox1.SetItemChecked(i-1, false);
                }

                for (int i = 1; i < checkedListBox2.Items.Count+1; i++)
                {
                    if (readerToLoadCommandsList.Read())
                    {
                        if (Convert.ToInt32(readerToLoadCommandsList["ID_Command"]) == i)
                        {
                            checkedListBox2.SetItemChecked(i - 1, true);
                        }
                    }
                    else checkedListBox2.SetItemChecked(i - 1, false);
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
