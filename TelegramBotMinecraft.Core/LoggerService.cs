using Telegram.Bot.Types;
using Color = System.Drawing.Color;

namespace TelegramBotMinecraft
{
    internal static class LoggerService
    {
        private static string PathLogsBot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "bot");
        private static string PathLogsApp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app");
        //public static RichTextBox ConsoleBot { get; set; }

        public static void MessageChat(Chat chat, string text, bool new_message)
        {
            AppendTextColoredBot($"[{DateTime.Now:HH:mm:ss}]", Color.Black);
            if (new_message) AppendTextColoredBot($" [MSG]", Color.Green);
            else AppendTextColoredBot($" [MSG_OLD]", Color.Green);
            AppendTextColoredBot($": Cообщение от ", Color.Black);
            if (!string.IsNullOrEmpty(chat.Username)) AppendTextColoredBot($"@{chat.Username} [{chat.Id}] ", Color.DarkOrange);
            else AppendTextColoredBot($"{chat.FirstName} [{chat.Id}]", Color.DarkOrange);
            AppendTextColoredBot($": {text}", Color.Black);
        }

        public static void MessageBotInfo(string Message)
        {
            AppendTextColoredBot($"[{DateTime.Now:HH:mm:ss}]", Color.Black);
            AppendTextColoredBot($" [INFO]", Color.Blue);
            AppendTextColoredBot($": {Message}", Color.Black);
        }

        public static void StartBotInfo(string FirstNameBot, string UsernameBot)
        {
            AppendTextColoredBot($"[{DateTime.Now:HH:mm:ss}]", Color.Black);
            AppendTextColoredBot($" [INFO]", Color.Blue);
            AppendTextColoredBot($": Бот ", Color.Black);
            AppendTextColoredBot($"{FirstNameBot} [@{UsernameBot}]", Color.DarkOrange);
            AppendTextColoredBot($": успешно авторизован и запущен.", Color.Black);
        }

        public static void ErrorBotInfo(string Message)
        {
            AppendTextColoredBot($"[{DateTime.Now:HH:mm:ss}]", Color.Black);
            AppendTextColoredBot($" [ERROR]", Color.Red);
            AppendTextColoredBot($": {Message}", Color.Black);
        }

        private static async Task AppendTextColoredBot(string text, Color color, bool bold = false)
        {
            //if (ConsoleBot == null) return;

            //if (ConsoleBot.InvokeRequired)
            //{
            //    ConsoleBot.Invoke(new Action(async () => await AppendTextColoredBot(text, color, bold)));
            //    return;
            //}

            //ConsoleBot.SelectionStart = ConsoleBot.TextLength;
            //ConsoleBot.SelectionLength = 0;
            //ConsoleBot.SelectionColor = color;

            //if (bold)
            //    ConsoleBot.SelectionFont = new Font(ConsoleBot.Font, FontStyle.Bold);
            //else
            //    ConsoleBot.SelectionFont = new Font(ConsoleBot.Font, FontStyle.Regular);

            //ConsoleBot.AppendText(text);
            //ConsoleBot.SelectionColor = ConsoleBot.ForeColor;

            await SaveLogBot(text);
        }

        public static async void MessageAppInfo(string message)
        {
            await SaveLogApp($"[{DateTime.Now:HH:mm:ss}] [INFO]: {message}");
        }

        public static async void ErrorAppInfo(string message)
        {
            await SaveLogApp($"[{DateTime.Now:HH:mm:ss}] [ERROR]: {message}");
        }

        private static async Task SaveLogBot(string message)
        {
            Directory.CreateDirectory(PathLogsBot);

            string fileName = $"bot_{DateTime.Now:yyyy-MM-dd}.log";
            string filePath = Path.Combine(PathLogsBot, fileName);

            string logEntry = $"{message}{Environment.NewLine}";
            await File.AppendAllTextAsync(filePath, logEntry);
        }

        private static async Task SaveLogApp(string message)
        {
            Directory.CreateDirectory(PathLogsApp);

            string fileName = $"app_{DateTime.Now:yyyy-MM-dd}.log";
            string filePath = Path.Combine(PathLogsApp, fileName);

            string logEntry = $"{message}{Environment.NewLine}";
            await File.AppendAllTextAsync(filePath, logEntry);
        }
    }
}
