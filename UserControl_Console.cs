using Microsoft.Data.Sqlite;

namespace TelegramBotMinecraft
{
    public partial class UserControl_Console : UserControl
    {
        public UserControl_Console()
        {
            InitializeComponent();
            this.Load += UserControl_Console_Load;
        }
        public void UserControl_Console_Load(object sender, EventArgs e)
        {

            LoadServersList();
        }

        private void LoadServersList()
        {
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                richTextBox1.Text = "База подлючена \n";
                SqliteCommand command = new SqliteCommand("SELECT * FROM Servers", connection);
                SqliteDataReader reader = command.ExecuteReader();

                listBox1.Items.Clear();
                while (reader.Read())
                {
                    listBox1.Items.Add(reader["Name"]);
                }
            }
        }
    }
}
