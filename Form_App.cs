using System.Windows.Forms;

namespace TelegramBotMinecraft
{
    public partial class App : Form
    {
        public static UserControl_Users userControl_Users;
        public static UserControl_Servers userControl_Servers;


        public App()
        {
            InitializeComponent();
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.FormClosing += Form1_FormClosing;
        }

        private void TabControl1_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage3 && userControl_Users != null)
            {
                userControl_Users.UserControl_Users_Load(userControl_Users, EventArgs.Empty);
            }
            else if (tabControl1.SelectedTab == tabPage2 && userControl_Servers != null)
            {
                userControl_Servers.UserControl_Servers_Load(userControl_Servers, EventArgs.Empty);
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


            // реализовать сохрание БД

            this.FormClosing -= Form1_FormClosing;
            this.Close();
        }
    }
}
