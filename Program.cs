using System.Diagnostics;
using Microsoft.Data.Sqlite;

namespace TelegramBotMinecraft
{
    static class Program
    {
        static string PathMain = "Data.db";
        static string PathBackup = @"D:\PC\Documents\Visual Studio 2022\Code Snippets\Visual C#\My Code Snippets\TelegramBotMinecraft\bin\Debug\net8.0-windows7.0\";
        static void Main()
        {
            string appName = "TelegramBotMinecraft";
            if (IsAppAlreadyRunning(appName))
            {
                MessageBox.Show("Бот уже запущен!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            BackupDataBase();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new App());
        }

        private static void  BackupDataBase()
        {
            using (var connection = new SqliteConnection($"Data Source={PathMain}"))
            {
                connection.Open();
                PathBackup += "DB-" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".db";
                using (var connectionBackup = new SqliteConnection($"Data Source={PathBackup}"))
                {
                    connection.BackupDatabase(connectionBackup);
                }
                MessageBox.Show("Бекап прошел успешно" + PathBackup);
            }
        }

        public static bool IsAppAlreadyRunning(string appName)
        {
            Process[] processes = Process.GetProcessesByName(appName);
            return processes.Length > 1;
        }
    }
}
