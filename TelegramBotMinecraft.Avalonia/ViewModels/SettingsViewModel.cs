using Avalonia.Threading;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;
using static TelegramBotMinecraft.Core.Models.TgBotStatusModel;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class SettingsViewModel : ObservableObject, IDisposable
    {
        private readonly SettingsRepository _settingsRepository;
        private readonly LoggerService _loggerService;
        private readonly TelegramBot _telegramBot;
        private readonly IDialogService _dialogService;
        private readonly IWindowService _windowService;
        private readonly INotificationService _notificationService;
        private readonly StartupManager _startupManager;


        private CancellationTokenSource _cts;


        public Setting? _originalSettings;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsOnBot))]
        private string? _statusWorkTGBot;

        [ObservableProperty]
        private string? _buttonTGBot = "Включить бота";

        [ObservableProperty]
        private TextDocument? logsBotAndProgram = new();


        public bool IsOnBot => !(StatusWorkTGBot == TgBotStatus.Starting.ToString() || 
                                 StatusWorkTGBot == TgBotStatus.Online.ToString() || 
                                 StatusWorkTGBot == TgBotStatus.Reloading.ToString());


        #region Свойства Настроек для Связывания (UI)

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(ManagingBotTelegramCommand))]
        private string? _botToken;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelSettingsCommand))]

        private int _autoBot;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelSettingsCommand))]
        private int _trayOnStart;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelSettingsCommand))]
        private int _runAtStartup;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelSettingsCommand))]
        private int _autoReconnect;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelSettingsCommand))]
        private int? _notifications;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(ManagingBotTelegramCommand))]
        private string? _proxyHost;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(ManagingBotTelegramCommand))]
        private string? _proxyPort;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(ManagingBotTelegramCommand))]
        private string? _proxyUsername;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelSettingsCommand))]
        [NotifyCanExecuteChangedFor(nameof(ManagingBotTelegramCommand))]
        private string? _proxyPassword;

        #endregion

        public SettingsViewModel(SettingsRepository settingsRepository, 
            LoggerService loggerService, TelegramBot telegramBot, 
            IDialogService dialogService, IWindowService windowService, 
            INotificationService notificationService, StartupManager startupManager)
        {
            _settingsRepository = settingsRepository;
            _loggerService = loggerService;
            _telegramBot = telegramBot;
            _dialogService = dialogService;
            _windowService = windowService;
            _notificationService = notificationService;
            _startupManager = startupManager;

            _telegramBot.TgBotStatusChanged += OnTgBotStatusChanged;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (_cts != null)
                {
                    await _cts.CancelAsync();
                    _cts.Dispose();
                }
                _cts = new CancellationTokenSource();


                await LoadSettingsAsync();

                var token = _cts.Token;
                _ = UpdateLogsAsync(token);
            }
            catch (Exception ex)
            {
                _loggerService.ErrorBotInfo($"Ошибка инициализации настроек {ex}");
            }
        }

        public async Task Reload()
        {
            await LoadSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            _originalSettings = await _settingsRepository.GetAllSettings();
            if (_originalSettings == null) return;

            BotToken = _originalSettings.BotToken;
            AutoBot = _originalSettings.AutoBot;
            TrayOnStart = _originalSettings.TrayOnStart;
            RunAtStartup = _originalSettings.RunAtStartup;
            AutoReconnect = _originalSettings.AutoReconnect;
            Notifications = _originalSettings.Notifications;
            ProxyHost = _originalSettings.ProxyHost;
            ProxyPort = _originalSettings.ProxyPort;
            ProxyUsername = _originalSettings.ProxyUsername;
            ProxyPassword = _originalSettings.ProxyPassword;
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task SaveSettings()
        {
            if (_originalSettings == null) return;

            _originalSettings.BotToken = BotToken?.Trim() ?? string.Empty;
            _originalSettings.AutoBot = AutoBot;
            _originalSettings.TrayOnStart = TrayOnStart;
            _originalSettings.RunAtStartup = RunAtStartup;
            _originalSettings.AutoReconnect = AutoReconnect;
            _originalSettings.Notifications = Notifications;
            _originalSettings.ProxyHost = ProxyHost?.Trim() ?? string.Empty;
            _originalSettings.ProxyPort = ProxyPort?.Trim() ?? string.Empty;
            _originalSettings.ProxyUsername = ProxyUsername?.Trim() ?? string.Empty;
            _originalSettings.ProxyPassword = ProxyPassword?.Trim() ?? string.Empty;


            if (_originalSettings.Id != 1) await _settingsRepository.AddSettings(_originalSettings);

            await _settingsRepository.SaveSettings(_originalSettings);

            await _notificationService.ShowNotification("Сохранение", "Все настройки успешно применены и сохранены", "Success");

            if (_originalSettings.RunAtStartup == 1)
            {
                _startupManager.EnableStartup();
            }
            else
            {
                _startupManager.DisableStartup();
            }

            await LoadSettingsAsync();
        }


        [RelayCommand(CanExecute = nameof(CanSave))]
        private async Task CancelSettings()
        {
            if (_originalSettings == null) return;

            await LoadSettingsAsync();
        }


        [RelayCommand(CanExecute = nameof(CanStartOrStopBot))]
        private async Task ManagingBotTelegram()
        {
            if (_originalSettings == null) return;

            if (string.IsNullOrWhiteSpace(_originalSettings.BotToken))
            {
                await _notificationService.ShowNotification("Ошибка при запуске бота", 
                    "Токен бота не может быть пустым", 
                    "Error");
                return;
            }

            if (StatusWorkTGBot == TgBotStatus.Offline.ToString())
            {
                await _notificationService.ShowNotification("Telegram бот", "Бот запускается", "Information");
                await _telegramBot.StartBotTelegram();
            }
            else
            {
                var result = await _dialogService.AskConfirmationAsync($"Вы уверены, что хотите остановить Telegram бота?");

                if (result == true)
                {
                    await _notificationService.ShowNotification("Telegram бот", "Бот останавливается", "Warning");
                    await _telegramBot.StopBotTelegram();
                }
            }
        }

        [RelayCommand]
        private async Task OpenJavaManagementWindow()
        {
            await _windowService.OpenJavaManagement();
        }

        private async Task UpdateLogsAsync(CancellationToken token)
        {

            try
            {
                await foreach (var logLine in _loggerService.UpdateLogsBotAndProgram(token))
                {
                    if (logLine == null) continue;
                    Dispatcher.UIThread.Post(() =>
                    {
                        if (logLine == "Console log clear")
                        {
                            LogsBotAndProgram?.Text = string.Empty;
                        }
                        else
                        {
                            LogsBotAndProgram?.Insert(LogsBotAndProgram.TextLength, logLine + Environment.NewLine);
                        }

                    });
                }
            }
            catch { }
        }

        private bool CanSave()
        {
            if (_originalSettings == null) return false;

            return BotToken != _originalSettings.BotToken ||
                   AutoBot != _originalSettings.AutoBot ||
                   TrayOnStart != _originalSettings.TrayOnStart ||
                   RunAtStartup != _originalSettings.RunAtStartup ||
                   AutoReconnect != _originalSettings.AutoReconnect ||
                   Notifications != _originalSettings.Notifications ||
                   ProxyHost != _originalSettings.ProxyHost ||
                   ProxyPort != _originalSettings.ProxyPort ||
                   ProxyUsername != _originalSettings.ProxyUsername ||
                   ProxyPassword != _originalSettings.ProxyPassword;
        }
        private bool CanStartOrStopBot()
        {
            if (_originalSettings == null) return false;

            return BotToken == _originalSettings.BotToken &&
              ProxyHost == _originalSettings.ProxyHost &&
              ProxyPort == _originalSettings.ProxyPort &&
              ProxyUsername == _originalSettings.ProxyUsername &&
              ProxyPassword == _originalSettings.ProxyPassword;
        }

        private void OnTgBotStatusChanged(TgBotStatus status)
        {
            Dispatcher.UIThread.Post(() =>
            {
                StatusWorkTGBot = status.ToString();

                if (status == TgBotStatus.Online)
                {
                    ButtonTGBot = "Выключить бота";
                }
                else if (status == TgBotStatus.Offline)
                {
                    ButtonTGBot = "Включить бота";
                }

                ManagingBotTelegramCommand.NotifyCanExecuteChanged();
            });
        }

        public void Dispose()
         {
            _telegramBot.TgBotStatusChanged -= OnTgBotStatusChanged;

            if (_cts != null)
            {
                try
                {
                    _cts.Cancel();
                }
                catch (ObjectDisposedException) {}

                _cts.Dispose();
                _cts = null;
            }
        }
    }
}