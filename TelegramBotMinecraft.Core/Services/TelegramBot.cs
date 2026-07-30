using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Core.Services
{
    public class TelegramBot
    {
        private readonly SettingsRepository? _SettingsRepository;
        private readonly LoggerService? _LoggerService;

        private TelegramBotClient? botClient;
        private HttpToSocks5Proxy? proxy;

        private CancellationTokenSource? cts;

        public DateTime? BotStartTime;

        public TelegramBot(SettingsRepository settingsRepository, LoggerService loggerService)
        {
            _SettingsRepository = settingsRepository;
            _LoggerService = loggerService;
        }

        public void StartBotTelegram()
        {
            if (botClient == null && cts == null) _ = StartBotAsync();
        }

        public void StopBotTelegram()
        {
            ExceptionStartBotOrStop();
        }

        private async Task StartBotAsync()
        {
            BotStartTime = DateTime.UtcNow;

            ExceptionStartBotOrStop();

            cts = new CancellationTokenSource();
            proxy = null;

            try
            {
                Setting? settings = await _SettingsRepository.GetTokenAndProxySettings();
                if (settings == null) return;

                HttpClient? httpClient = null;

                if (!string.IsNullOrWhiteSpace(settings.ProxyHost) && !string.IsNullOrWhiteSpace(settings.ProxyPort))
                {
                    int port = Convert.ToInt32(settings.ProxyPort);

                    if (string.IsNullOrWhiteSpace(settings.ProxyUsername) && string.IsNullOrWhiteSpace(settings.ProxyPassword))
                    {
                        proxy = new HttpToSocks5Proxy(settings.ProxyHost, port);
                    }
                    else
                    {
                        proxy = new HttpToSocks5Proxy(settings.ProxyHost, port, settings.ProxyUsername, settings.ProxyPassword);
                    }

                    var handler = new HttpClientHandler { Proxy = proxy };
                    httpClient = new HttpClient(handler);
                }

                if (httpClient != null)
                {
                    var options = new TelegramBotClientOptions(settings.BotToken);
                    botClient = new TelegramBotClient(options, httpClient);
                }
                else
                {
                    botClient = new TelegramBotClient(settings.BotToken);
                }

                var me = await botClient.GetMe(cts.Token);

                botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    errorHandler: HandleErrorAsync,
                    receiverOptions: new ReceiverOptions { AllowedUpdates = [] },
                    cancellationToken: cts.Token
                );

                _LoggerService?.StartBotInfo(me.FirstName, me.Username);

                try
                {
                    await Task.Delay(Timeout.Infinite, cts.Token);
                }
                catch (OperationCanceledException) { }
            }
            catch (HttpRequestException netEx)
            {
                _LoggerService?.ErrorBotInfo("Попытка установить соединение была безуспешной.\nПожалуйста, перепроверьте настройки сети или прокси.");
                ExceptionStartBotOrStop();
            }
            catch (RequestException apiEx)
            {
                if (apiEx.InnerException is HttpRequestException || apiEx.InnerException?.InnerException is System.Net.Sockets.SocketException)
                {
                    _LoggerService?.ErrorBotInfo("Попытка установить соединение была безуспешной.\nПожалуйста, перепроверьте настройки сети или прокси.");
                }
                else
                {
                    _LoggerService?.ErrorBotInfo("Неверный токен бота или ошибка API: " + apiEx.Message);
                }
                ExceptionStartBotOrStop();
            }
            catch (Exception ex)
            {
                _LoggerService?.ErrorBotInfo($"Непредвиденная ошибка: {ex.Message}");
                 ExceptionStartBotOrStop();
            }
        }

        private void ExceptionStartBotOrStop()
        {
            if (botClient == null && cts == null) return;

            _LoggerService?.MessageBotInfo("Остановка Telegram бота...");
            try
            {
                cts?.Cancel();
            }
            catch (Exception ex) { _LoggerService?.MessageBotInfo($"Ошибка при закрытии сессии: {ex.Message}"); }
            finally
            {
                cts?.Dispose();
                cts = null;
                botClient = null;
                proxy = null;
                BotStartTime = null;
                _LoggerService?.MessageBotInfo("Telegram бот успешно остановлен!");
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
            _LoggerService?.MessageChat(msg.Chat, msg.Text.Trim(), isNewMessage);
        }

        private async Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
        {
            string error = ex switch
            {
                ApiRequestException apiEx => $"Telegram API ошибка: [{apiEx.ErrorCode}] {apiEx.Message}",
                _ => ex.ToString()
            };
            _LoggerService?.ErrorBotInfo(error);
            if (ex is ApiRequestException ApiEx)
            {
                if (ApiEx.ErrorCode == 409)
                {
                    _LoggerService?.ErrorBotInfo("Обнаружена копия бота! Завершение работы бота...");
                    ExceptionStartBotOrStop();
                }
            }
            return;
        }
    }
}
