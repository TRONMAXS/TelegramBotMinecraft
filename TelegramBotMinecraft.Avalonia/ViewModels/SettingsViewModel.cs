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

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly SettingsRepository _SettingsRepository;
        private readonly LoggerService _LoggerService;
        private readonly TelegramBot _TelegramBot;
        private readonly IDialogService _dialogService;
        private readonly IWindowService _windowService;
        private readonly INotificationService _notificationService;
        private readonly StartupManager _startupManager;


        private readonly CancellationTokenSource _cts = new();


        public Setting? _originalSettings;

        [ObservableProperty]
        private string? _statusWorkTGBot = "Включить бота";

        [ObservableProperty]
        private TextDocument? logsBotAndProgram = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsStartingBot))]
        private bool _isOnBot;

        public bool IsStartingBot => !IsOnBot;


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
            _SettingsRepository = settingsRepository;
            _LoggerService = loggerService;
            _TelegramBot = telegramBot;
            _dialogService = dialogService;
            _windowService = windowService;
            _notificationService = notificationService;
            _startupManager = startupManager;

            _ = LoadSettingsAsync();

            _ = _TelegramBot.BotAutostart();

            _ = UpdateLogsAsync(_cts.Token);
        }

        private async Task LoadSettingsAsync()
        {
            _originalSettings = await _SettingsRepository.GetAllSettings();
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

            _originalSettings.BotToken = BotToken;
            _originalSettings.AutoBot = AutoBot;
            _originalSettings.TrayOnStart = TrayOnStart;
            _originalSettings.RunAtStartup = RunAtStartup;
            _originalSettings.AutoReconnect = AutoReconnect;
            _originalSettings.Notifications = Notifications;
            _originalSettings.ProxyHost = ProxyHost;
            _originalSettings.ProxyPort = ProxyPort;
            _originalSettings.ProxyUsername = ProxyUsername;
            _originalSettings.ProxyPassword = ProxyPassword;


            await _SettingsRepository.SaveSettings(_originalSettings);

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

            if (StatusWorkTGBot == "Включить бота")
            {
                await _notificationService.ShowNotification("Telegram бот", "Бот запускается", "Information");
                _TelegramBot.StartBotTelegram();
                StatusWorkTGBot = "Выключить бота";
                IsOnBot = true;
            }
            else
            {
                var result = await _dialogService.AskConfirmationAsync($"Вы уверены, что хотите остановить Telegram бота?");

                if (result == true)
                {
                    await _notificationService.ShowNotification("Telegram бот", "Бот останавливается", "Warning");
                    _TelegramBot.StopBotTelegram();
                    StatusWorkTGBot = "Включить бота";
                    IsOnBot = false;
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
                await foreach (var logLine in _LoggerService.UpdateLogsBotAndProgram(token))
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
    }
}
    
