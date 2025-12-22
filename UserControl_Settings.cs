using Microsoft.Data.Sqlite;
using System.Windows.Forms;

namespace TelegramBotMinecraft
{
    public partial class UserControl_Settings : UserControl
    {
        public UserControl_Settings()
        {
            InitializeComponent();
            this.Load += UserControl_Settings_Load;
        }

        private void UserControl_Settings_Load(object sender, EventArgs e)
        {
            LoadAllSettings();
        }

        private void LoadAllSettings()
        {
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT * FROM Settings", connection);
                SqliteDataReader reader = command.ExecuteReader();

                
                while (reader.Read())
                {
                    textBox1.Text = reader["BotToken"].ToString();
                    checkBox1.Checked = Convert.ToInt32(reader["Auto_Bot"]) == 1 ? true : false;
                    checkBox2.Checked = Convert.ToInt32(reader["TrayOnStart"]) == 1 ? true : false;
                    checkBox3.Checked = Convert.ToInt32(reader["SaveLogs"]) == 1 ? true : false;
                    checkBox4.Checked = Convert.ToInt32(reader["RunAtStartup"]) == 1 ? true : false;
                    checkBox5.Checked = Convert.ToInt32(reader["AutoReconnect"]) == 1 ? true : false;
                }
            }
        }
    }
}
