namespace TelegramBotMinecraft
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.FormClosing += Form1_FormClosing;
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
