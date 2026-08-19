using MihaZupan;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;
using static TelegramBotMinecraft.Core.Models.TgBotStatusModel;

namespace TelegramBotMinecraft.Core.Services
{
    public class TelegramBot
    {
        private readonly SettingsRepository? _settingsRepository;
        private readonly LoggerService? _loggerService;

        private readonly CommandContext _commandContext;

        private TelegramBotClient? _botClient;

        private int _consecutiveNetworkErrors = 0;

        private CancellationTokenSource? _cts;
        private CancellationTokenSource? _reconnectCts;

        private readonly SemaphoreSlim _lock = new(1, 1);

        public DateTime? BotStartTime { get; private set; }

        private TgBotStatus botStatus = TgBotStatus.Offline;

        public event Action<TgBotStatus>? TgBotStatusChanged;
        public event Action<bool>? BotCrashed;

        private readonly Timer _backgroundCheckTimer;
        private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5);

        private int restartAttempts = 0;
        private const int MaxRestartAttempts = 6;
        private const int RestartDelayMs = 10000;

        public TelegramBot(SettingsRepository settingsRepository, LoggerService loggerService, CommandContext commandContext)
        {
            _settingsRepository = settingsRepository;
            _loggerService = loggerService;
            _commandContext = commandContext;

            _backgroundCheckTimer = new Timer(async _ => await ExecutionRecoveryCheckAsync(), null, TimeSpan.Zero, _checkInterval);
        }

        private async Task ExecutionRecoveryCheckAsync()
        {
            ChangeStatus(botStatus);
        }

        public async Task BotAutostart()
        {
            try
            {
                var settings = await _settingsRepository.GetAllSettings();

                if (settings != null && settings.AutoBot == 1)
                {
                    _loggerService?.MessageBotInfo("Обнаружен флаг автостарта. Инициализация фонового запуска бота...");

                    if (string.IsNullOrWhiteSpace(settings.BotToken))
                    {
                        _loggerService?.ErrorBotInfo("Ошибка при запуске бота. Токен бота не может быть пустым");
                        return;
                    }

                    await StartBotAsync();
                }
            }
            catch (Exception ex)
            {
                _loggerService?.ErrorBotInfo($"Не удалось выполнить автостарт бота при запуске приложения: {ex.Message}");
            }
        }

        public async Task OnBotCrash(bool isCriticalError = false)
        {
            Setting? settings = await _settingsRepository.GetAllSettings();
            if (settings == null) return;


            if (settings.AutoReconnect == 0)
            {
                _loggerService?.ErrorBotInfo("Авторестарт отключен в настройках. Бот остановлен.");
                restartAttempts = 0;
                BotCrashed?.Invoke(false);
                ChangeStatus(TgBotStatus.Offline);
                return;
            }

            if (isCriticalError)
            {
                _loggerService?.ErrorBotInfo("Критическая ошибка. Автоматический перезапуск отменен. Проверьте настройки.");
                ChangeStatus(TgBotStatus.Offline);
                restartAttempts = 0;
                BotCrashed?.Invoke(false);
                return;
            }

            if (restartAttempts < MaxRestartAttempts)
            {
                ChangeStatus(TgBotStatus.Reloading);
                restartAttempts++;

                BotCrashed?.Invoke(true);

                _loggerService?.ErrorBotInfo($"Ожидание {RestartDelayMs / 1000} сек перед попыткой рестарта ({restartAttempts}/{MaxRestartAttempts})...");

                _reconnectCts?.Dispose();
                _reconnectCts = new CancellationTokenSource();

                try
                {
                    await Task.Delay(RestartDelayMs, _reconnectCts.Token);

                    BotCrashed?.Invoke(false);

                    _loggerService?.ErrorBotInfo($"Попытка автоматического перезапуска...");
                    await StartBotAsync();
                }
                catch (OperationCanceledException){ }
                finally
                {
                    _reconnectCts?.Dispose();
                    _reconnectCts = null;
                }
            }
            else
            {
                _loggerService?.ErrorBotInfo($"Превышено максимальное количество попыток рестарта ({MaxRestartAttempts}). Требуется ручное вмешательство.");
                BotCrashed?.Invoke(false);
                ChangeStatus(TgBotStatus.Offline);
                restartAttempts = 0;
            }
        }

        public void CancelReconnection()
        {
            if (_reconnectCts != null && !_reconnectCts.IsCancellationRequested)
            {
                _reconnectCts.Cancel();
                _loggerService?.ErrorBotInfo("Автоматический перезапуск отменен пользователем.");

                restartAttempts = 0;
                ChangeStatus(TgBotStatus.Offline);

                BotCrashed?.Invoke(false);
            }
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();

                ChangeStatus(TgBotStatus.Offline);
                BotCrashed?.Invoke(false);
            }
        }

        public async Task StartBotTelegram()
        {
            await StartBotAsync();
        }

        public async Task StopBotTelegram()
        {
            await _lock.WaitAsync();
            try
            {
                if (_cts == null) return;

                _loggerService?.MessageBotInfo("Остановка Telegram бота...");
                ChangeStatus(TgBotStatus.Stopping);

                SilentCleanup();

                ChangeStatus(TgBotStatus.Offline);
                _loggerService?.MessageBotInfo("Telegram бот успешно остановлен!");
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task StartBotAsync()
        {
            bool shouldTriggerCrash = false;
            bool isCritical = false;

            await _lock.WaitAsync();
            try
            {
                if (_botClient != null || _cts != null) return;

                BotStartTime = DateTime.UtcNow;
                _cts = new CancellationTokenSource();
                ChangeStatus(TgBotStatus.Starting);

                var settings = await _settingsRepository.GetTokenAndProxySettings();
                if (settings == null)
                {
                    ChangeStatus(TgBotStatus.Offline);
                    return;
                }

                HttpClient? httpClient = null;
                if (!string.IsNullOrWhiteSpace(settings.ProxyHost) && !string.IsNullOrWhiteSpace(settings.ProxyPort))
                {
                    int port = Convert.ToInt32(settings.ProxyPort);
                    var proxy = string.IsNullOrWhiteSpace(settings.ProxyUsername)
                        ? new HttpToSocks5Proxy(settings.ProxyHost, port)
                        : new HttpToSocks5Proxy(settings.ProxyHost, port, settings.ProxyUsername, settings.ProxyPassword);

                    httpClient = new HttpClient(new HttpClientHandler { Proxy = proxy });
                }

                var options = new TelegramBotClientOptions(settings.BotToken);
                _botClient = httpClient != null
                    ? new TelegramBotClient(options, httpClient)
                    : new TelegramBotClient(options);

                var me = await _botClient.GetMe(_cts.Token);

                await _botClient.DeleteWebhook(false, _cts.Token);

                _botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    errorHandler: HandleErrorAsync,
                    receiverOptions: new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() },
                    cancellationToken: _cts.Token
                );

                BotCrashed?.Invoke(false);
                restartAttempts = 0;
                _consecutiveNetworkErrors = 0;

                _loggerService?.StartBotInfo(me.FirstName, me.Username);
                ChangeStatus(TgBotStatus.Online);
            }
            catch (ArgumentException ex)
            {
                _loggerService?.ErrorBotInfo($"Неверный формат токена: {ex.Message}");
                SilentCleanup();
                ChangeStatus(TgBotStatus.Offline);
                shouldTriggerCrash = false;
                isCritical = true;
            }
            catch (ApiRequestException apiEx)
            {
                isCritical = apiEx.ErrorCode == 401;

                _loggerService?.ErrorBotInfo(isCritical ? $"Критическая ошибка API: {apiEx.Message}" : "Ошибка API");

                SilentCleanup();
                ChangeStatus(isCritical ? TgBotStatus.Offline : TgBotStatus.Warning);
                shouldTriggerCrash = !isCritical;
            }
            catch (RequestException reqEx)
            {
                _loggerService?.ErrorBotInfo($"Ошибка сети/прокси при запуске бота: {reqEx.Message}");

                SilentCleanup();
                ChangeStatus(TgBotStatus.Offline);
                shouldTriggerCrash = true;
                isCritical = false;
            }
            catch (Exception ex)
            {
                _loggerService?.ErrorBotInfo($"Непредвиденная ошибка: {ex.Message}");
                SilentCleanup();
                ChangeStatus(TgBotStatus.Warning);
                shouldTriggerCrash = true;
                isCritical = false;
            }
            finally
            {
                _lock.Release();
            }

            if (shouldTriggerCrash)
            {
                BotCrashed?.Invoke(!isCritical);
                await OnBotCrash(isCritical);
            }
        }

        private void SilentCleanup()
        {
            try
            {
                if (_cts != null && !_cts.IsCancellationRequested)
                {
                    _cts.Cancel();
                }
            }
            catch { }
            finally
            {
                _cts?.Dispose();
                _cts = null;
                _botClient = null;
                BotStartTime = null;
            }
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type != UpdateType.Message || update.Message?.Type != MessageType.Text)
                return;

            var msg = update.Message;
            string text = msg.Text.Trim();

            var messageAge = DateTime.UtcNow - msg.Date.ToUniversalTime();

            bool isNewMessage = messageAge.TotalSeconds <= 20 && messageAge.TotalSeconds >= -5;

            _loggerService?.MessageChat(msg.Chat, msg.Text.Trim(), isNewMessage);

            if (isNewMessage)
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _commandContext.HandleMessageAsync(bot, msg, token);
                    }
                    catch (Exception ex)
                    {
                        _loggerService?.ErrorBotInfo($"Ошибка при обработке команды: {ex.Message}");
                    }
                }, token);
            }
        }

        private async Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            if (ex is RequestException reqEx)
            {
                Interlocked.Increment(ref _consecutiveNetworkErrors);

                if (_consecutiveNetworkErrors <= 4)
                {
                    _loggerService?.ErrorBotInfo($"[Сеть] Временный сбой соединения #{_consecutiveNetworkErrors} (ожидание авто-реконнекта): {reqEx.Message}");

                    ChangeStatus(TgBotStatus.Warning);

                    int delaySeconds = _consecutiveNetworkErrors * 3;
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds), token);

                    return;
                }

                _loggerService?.ErrorBotInfo($"[Сеть] Прокси или сеть недоступны длительное время ({_consecutiveNetworkErrors} сбоев подряд). Бот останавливается для полной перезагрузки.");

                _ = Task.Run(async () =>
                {
                    await StopBotTelegram();
                    await OnBotCrash(false);
                });
                return;
            }

            if (ex is not RequestException)
            {
                Interlocked.Exchange(ref _consecutiveNetworkErrors, 0);
            }

            if (ex is ApiRequestException apiEx)
            {
                if (apiEx.ErrorCode == 409)
                {
                    var runtime = DateTime.UtcNow - (BotStartTime ?? DateTime.UtcNow);

                    if (runtime.TotalSeconds < 15)
                    {
                        _loggerService?.ErrorBotInfo("Ошибка 409: Обнаружен активно работающий сервер бота. Этот экземпляр (копия) будет остановлен.");

                        _ = Task.Run(async () =>
                        {
                            BotCrashed?.Invoke(false);
                            await StopBotTelegram();
                        });
                        return;
                    }
                    else
                    {
                        _loggerService?.ErrorBotInfo("Предупреждение: Кто-то попытался запустить копию бота.");
                        return;
                    }
                }

                if (apiEx.ErrorCode == 401)
                {
                    _loggerService?.ErrorBotInfo($"Критическая ошибка API [401 Unauthorized]. Токен отозван. Бот останавливается.");
                    _ = Task.Run(async () =>
                    {
                        await StopBotTelegram();
                        BotCrashed?.Invoke(false);
                        await OnBotCrash(true);
                    });
                    return;
                }
            }

            string error = ex switch
            {
                ApiRequestException apiExCustom => $"Telegram API ошибка: [{apiExCustom.ErrorCode}] {apiExCustom.Message}",
                _ => ex.ToString()
            };

            _loggerService?.ErrorBotInfo($"[Критический сбой] {error}");

            _ = Task.Run(async () =>
            {
                await StopBotTelegram();
                await OnBotCrash(false);
            });

        }

        private void ChangeStatus(TgBotStatus newStatus)
        {
            botStatus = newStatus;
            TgBotStatusChanged?.Invoke(newStatus);
        }

        public void Dispose()
        {
            _backgroundCheckTimer?.Dispose();
        }
    }
}
