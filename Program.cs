using System.Diagnostics;

namespace TelegramBotMinecraft
{
    static class Program
    {
        static void Main()
        {
            string appName = "TelegramBotMinecraft";
            if (IsAppAlreadyRunning(appName))
            {
                MessageBox.Show("Бот уже запущен!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new App());
        }

        public static bool IsAppAlreadyRunning(string appName)
        {
            Process[] processes = Process.GetProcessesByName(appName);
            return processes.Length > 1;
        }
    }
}
