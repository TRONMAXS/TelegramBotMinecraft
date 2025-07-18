using System.Diagnostics;

namespace TelegramBotMinecraft
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
            labelVersion.Text = "Версия: v0.4.1";
            labelBuild.Text = "Дата сборки: 18.07.2025";
        }

        private void buttonGitHub_Click(object sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/TRONMAXS/TelegramBotMinecraft",
                UseShellExecute = true
            });
        }
    }
}
