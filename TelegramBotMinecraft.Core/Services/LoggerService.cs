using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Telegram.Bot.Types;

namespace TelegramBotMinecraft.Core.Services
{
    public class LoggerService
    {
        private string PathLogs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");


        private readonly Channel<string> _logChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true
        });

        public async IAsyncEnumerable<string> UpdateLogsBotAndProgram([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            try
            {
                await foreach (var logLine in _logChannel.Reader.ReadAllAsync(cancellationToken))
                {
                    if (string.IsNullOrEmpty(logLine)) continue;

                    await SaveLog(logLine);

                    yield return logLine;
                }
            }
            finally { }
        }

        private void Log(string message)
        {
            _logChannel.Writer.TryWrite(message);
        }

        public void MessageChat(Chat chat, string text, bool new_message)
        {
            string MessageText = $"[{DateTime.Now:HH:mm:ss}]";

            if (new_message) MessageText += " [MSG] ";
            else MessageText += " [MSG_OLD] ";

            if (!string.IsNullOrEmpty(chat.Username)) MessageText += $"@{chat.Username} [{chat.Id}]";
            else MessageText += $"{chat.FirstName} [{chat.Id}]";

            MessageText += $": {text}";
            Log(MessageText);
        }

        public void MessageBotInfo(string Message)
        {
            Log($"[{DateTime.Now:HH:mm:ss}] [INFO]: {Message}");
        }

        public void StartBotInfo(string FirstNameBot, string UsernameBot)
        {
            Log($"[{DateTime.Now:HH:mm:ss}] [INFO]: Бот {FirstNameBot} [@{UsernameBot}]: успешно авторизован и запущен");
        }

        public async Task ErrorBotInfo(string Message)
        {
            Log($"[{DateTime.Now:HH:mm:ss}] [ERROR]: {Message}");
        }

        private async Task SaveLog(string message)
        {
            Directory.CreateDirectory(PathLogs);

            string fileName = $"{DateTime.Now:yyyy-MM-dd}.log";
            string filePath = Path.Combine(PathLogs, fileName);

            string logEntry = $"{message}{Environment.NewLine}";
            await File.AppendAllTextAsync(filePath, logEntry);
        }
    }
}
