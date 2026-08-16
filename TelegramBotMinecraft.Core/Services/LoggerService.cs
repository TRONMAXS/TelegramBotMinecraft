using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Telegram.Bot.Types;

namespace TelegramBotMinecraft.Core.Services
{
    public class LoggerService
    {
        private readonly string _logsPath;


        private readonly Channel<string> _logChannel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true
        });

        public LoggerService(string logsPath)
        {
            _logsPath = logsPath;
        }

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
            string MessageText = $"[{DateTime.Now:HH:mm:ss}] [BOT]";

            if (new_message) MessageText += " [MSG] ";
            else MessageText += " [MSG_OLD] ";

            if (!string.IsNullOrEmpty(chat.Username)) MessageText += $"@{chat.Username} [{chat.Id}]";
            else MessageText += $"{chat.FirstName} [{chat.Id}]";

            MessageText += $": {text}";
            Log(MessageText);
        }

        public void MessageBotInfo(string Message)
        {
            Log($"[{DateTime.Now:HH:mm:ss}] [BOT] [INFO]: {Message}");
        }

        public void StartBotInfo(string FirstNameBot, string UsernameBot)
        {
            Log($"[{DateTime.Now:HH:mm:ss}] [BOT] [INFO]: Бот {FirstNameBot} [@{UsernameBot}]: успешно авторизован и запущен");
        }

        public void ErrorBotInfo(string Message)
        {
            Log($"[{DateTime.Now:HH:mm:ss}] [BOT] [ERROR]: {Message}");
        }

        public void MessageAppInfo(string message)
        {
            Log($"[{DateTime.Now:HH:mm:ss}] [APP] [INFO]: {message}");
        }

        public void ErrorAppInfo(string message)
        {
            Log($"[{DateTime.Now:HH:mm:ss}] [APP] [ERROR]: {message}");
        }


        private async Task SaveLog(string message)
        {
            Directory.CreateDirectory(_logsPath);

            string fileName = $"{DateTime.Now:yyyy-MM-dd}.log";
            string filePath = Path.Combine(_logsPath, fileName);

            string logEntry = $"{message}{Environment.NewLine}";
            await File.AppendAllTextAsync(filePath, logEntry);
        }
    }
}
