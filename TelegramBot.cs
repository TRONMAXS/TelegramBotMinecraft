using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using MihaZupan;

namespace TelegramBotMinecraft
{
    public class TelegramBot
    {
        private TelegramBotClient botClient;
        private CancellationTokenSource cts;

        private static TelegramBot _currentInstance;

        public static UserControl_Settings userControl_Settings;

        private HttpToSocks5Proxy proxy;

        private string BotToken;
        private string Proxy_Host;
        private string Proxy_Port;
        private string Proxy_Username;
        private string Proxy_Password;

        public static DateTime BotStartTime { get; set; }

        public static void StartBotTelegram()
        {
            if (_currentInstance != null)
            {
                var oldInstance = _currentInstance;
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(150); 
                        oldInstance.cts?.Dispose();
                    }
                    catch {}
                });
            }
            _currentInstance = new TelegramBot();
            _ = _currentInstance.StartBotAsync();
        }

        public static void StopBotTelegram()
        {
            if (_currentInstance != null)
            {
                _currentInstance.cts?.Cancel();
                _currentInstance = null;
                LoggerService.MessageBotInfo("Telegram бот остановлен!");
                userControl_Settings.ButtonOnBotTelegram();
            }
        }

        private async Task StartBotAsync()
        {
            TelegramBot.BotStartTime = DateTime.UtcNow;

            cts = new CancellationTokenSource();
            proxy = null;
            try
            {
                using (var connection = new SqliteConnection("Data Source=Data.db"))
                {
                    await connection.OpenAsync();
                    SqliteCommand command = new SqliteCommand("SELECT BotToken, Proxy_Host, Proxy_Port, Proxy_Username, Proxy_Password FROM Settings ", connection);
                    SqliteDataReader reader = await command.ExecuteReaderAsync();

                    while (await reader.ReadAsync())
                    {
                        BotToken = reader["BotToken"].ToString();
                        Proxy_Host = reader["Proxy_Host"].ToString();
                        Proxy_Port = reader["Proxy_Port"].ToString();
                        Proxy_Username = reader["Proxy_Username"].ToString();
                        Proxy_Password = reader["Proxy_Password"].ToString();
                    }
                }
                if (string.IsNullOrWhiteSpace(Proxy_Username) && string.IsNullOrWhiteSpace(Proxy_Password) &&
                    !string.IsNullOrWhiteSpace(Proxy_Host) && !string.IsNullOrWhiteSpace(Proxy_Port))
                {
                    proxy = new HttpToSocks5Proxy(Proxy_Host, Convert.ToInt32(Proxy_Port));
                    botClient = new TelegramBotClient(BotToken, new HttpClient(new HttpClientHandler { Proxy = proxy }));
                }
                else if (!string.IsNullOrWhiteSpace(Proxy_Username) && !string.IsNullOrWhiteSpace(Proxy_Password) &&
                    !string.IsNullOrWhiteSpace(Proxy_Host) && !string.IsNullOrWhiteSpace(Proxy_Port))
                {
                    proxy = new HttpToSocks5Proxy(Proxy_Host, Convert.ToInt32(Proxy_Port), Proxy_Username, Proxy_Password);
                    botClient = new TelegramBotClient(BotToken, new HttpClient(new HttpClientHandler { Proxy = proxy }));
                }
                else
                {
                    botClient = new TelegramBotClient(BotToken);
                }


                var me = await botClient.GetMe();

                botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    errorHandler: HandleErrorAsync,
                    receiverOptions: new ReceiverOptions { AllowedUpdates = { } },
                    cancellationToken: cts.Token
                );

                LoggerService.StartBotInfo(me.FirstName, me.Username);

                userControl_Settings.ButtonOffBotTelegram();

                await Task.Delay(Timeout.Infinite, cts.Token);
            }
            catch (OperationCanceledException) {}
            catch (HttpRequestException netEx)
            {
                LoggerService.ErrorBotInfo("Попытка установить соединение была безуспешной.\nПожалуйста, перепроверьте настройки сети или прокси.");
                 ExceptionStartBot();
            }
            catch (RequestException apiEx)
            {
                if (apiEx.InnerException is HttpRequestException || apiEx.InnerException?.InnerException is System.Net.Sockets.SocketException)
                {
                    LoggerService.ErrorBotInfo("Попытка установить соединение была безуспешной.\nПожалуйста, перепроверьте настройки сети или прокси.");
                }
                else
                {
                    LoggerService.ErrorBotInfo("Неверный токен бота или ошибка API: " + apiEx.Message);
                }
                 ExceptionStartBot();
            }
            catch (Exception ex)
            {
                LoggerService.ErrorBotInfo($"Непредвиденная ошибка: {ex.Message}");
                 ExceptionStartBot();
            }
        }

        private void ExceptionStartBot()
        {
            if (_currentInstance != null)
            {
                _currentInstance.cts?.Cancel();
                _currentInstance.cts?.Dispose();
                _currentInstance = null;
                userControl_Settings.ButtonOnBotTelegram();
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type != UpdateType.Message || update.Message?.Type != MessageType.Text)
                return;

            var msg = update.Message;
            var messageAge = DateTime.UtcNow - msg.Date.ToUniversalTime();

            string text = msg.Text.Trim();

            bool isNewMessage = msg.Date >= BotStartTime;
            LoggerService.MessageChat(msg.Chat, msg.Text.Trim(), isNewMessage);
        }

        private async Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
        {
            string error = ex switch
            {
                ApiRequestException apiEx => $"Telegram API ошибка: [{apiEx.ErrorCode}] {apiEx.Message}",
                _ => ex.ToString()
            };
            LoggerService.ErrorBotInfo(error);
            if (ex is ApiRequestException ApiEx)
            {
                if (ApiEx.ErrorCode == 409)
                {
                    LoggerService.ErrorBotInfo("Обнаружена копия бота! Завершение работы бота...");
                    StopBotTelegram();
                }
            }
            return;
        }
    }
}
