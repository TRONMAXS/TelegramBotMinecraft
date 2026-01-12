using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBotMinecraft
{
    public class TelegramBot
    {
        private TelegramBotClient botClient;
        private CancellationTokenSource cts;

        private static TelegramBot _currentInstance;

        public static UserControl_Settings userControl_Settings;

        private string BotToken;

        public static void StartBotTelegram()
        {
            if (_currentInstance != null)
            {
                _currentInstance.cts?.Cancel();
                _currentInstance.cts?.Dispose();
            }
            _currentInstance = new TelegramBot();

            Task.Run(() => _currentInstance.StartBotAsync());
        }

        public static void StopBotTelegram()
        {
            if (_currentInstance != null)
            {
                _currentInstance.cts?.Cancel();
                _currentInstance.cts?.Dispose();
                _currentInstance = null;
                userControl_Settings.MessageBotInfo("Telegram бот остановлен!");
            }
        }

        private async Task StartBotAsync()
        {
            cts = new CancellationTokenSource();
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    connection.Open();
                    SqliteCommand command = new SqliteCommand("SELECT BotToken FROM Settings", connection);
                    SqliteDataReader reader ;
                    BotToken = command.ExecuteScalar().ToString();
                }
                botClient = new TelegramBotClient(BotToken);
                var me = await botClient.GetMe();

                botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    errorHandler: HandleErrorAsync,
                    receiverOptions: new ReceiverOptions { AllowedUpdates = { } },
                    cancellationToken: cts.Token
                );

                await userControl_Settings.StartBotInfo(me.FirstName, me.Username);

                userControl_Settings.ButtonOffBotTelegram();

                await Task.Delay(Timeout.Infinite, cts.Token);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unauthorized")
                {
                    await userControl_Settings.ErrorBotInfo("Токен бота неактивен либо указан неправильно. \n Пожалуйста, перепроверьте настройки вашего бота. ");
                }
            }

        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type != UpdateType.Message || update.Message?.Type != MessageType.Text)
                return;

            var msg = update.Message;
            string text = msg.Text.Trim();
            await userControl_Settings.MessageChat(msg.Chat, text);
        }

        private async Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
        {
            string error = ex switch
            {
                ApiRequestException apiEx => $"Telegram API ошибка: [{apiEx.ErrorCode}] {apiEx.Message}",
                _ => ex.ToString()
            };
            await userControl_Settings.ErrorBotInfo(error);
            return;
        }
    }
}
