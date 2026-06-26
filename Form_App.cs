using Microsoft.Data.Sqlite;

namespace TelegramBotMinecraft
{
    public partial class App : Form
    {
        public static UserControl_Console userControl_Console;
        public static UserControl_Servers userControl_Servers;
        public static UserControl_Users userControl_Users;
        public static UserControl_Settings userControl_Settings;

        public App()
        {
            InitializeComponent();
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            AutoStartBotTelegram();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.FormClosing += Form1_FormClosing;
        }

        private void TabControl1_SelectedIndexChanged(object? sender, EventArgs e)
        {

            if (tabControl1.SelectedTab == tabPage1 && userControl_Console != null)
            {
                userControl_Console.UserControl_Console_Load(userControl_Console, EventArgs.Empty);
            }
            else if (tabControl1.SelectedTab == tabPage2 && userControl_Servers != null)
            {
                userControl_Servers.UserControl_Servers_Load(userControl_Servers, EventArgs.Empty);
            }
            else if (tabControl1.SelectedTab == tabPage3 && userControl_Users != null)
            {
                userControl_Users.UserControl_Users_Load(userControl_Users, EventArgs.Empty);
            }
            else if (tabControl1.SelectedTab == tabPage4 && userControl_Settings != null)
            {
                userControl_Settings.UserControl_Settings_Load(userControl_Settings, EventArgs.Empty);
            }
        }

        private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            var result = MessageBox.Show("Вы действительно хотите выйти?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            e.Cancel = true;

            this.FormClosing -= Form1_FormClosing;
            this.Close();
        }

        private async void AutoStartBotTelegram()
        {
            using (var connection = new SqliteConnection("Data Source=Data.db"))
            {
                await connection.OpenAsync();
                SqliteCommand command = new SqliteCommand("SELECT Auto_Bot FROM Settings", connection);
                SqliteDataReader reader = command.ExecuteReader();
                while (await reader.ReadAsync())
                {
                    if (Convert.ToInt32(reader[0]) == 1) TelegramBot.StartBotTelegram();
                    
                    else userControl_Settings.ButtonOnBotTelegram();
                }
            }
        }
    }
}
