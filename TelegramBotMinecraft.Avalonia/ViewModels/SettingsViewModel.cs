using Avalonia.Threading;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using TelegramBotMinecraft.Avalonia.Views;
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


        [ObservableProperty]
        private Setting? _settings;

        [ObservableProperty]
        private string? _statusWorkTGBot = "Включить бота";

        [ObservableProperty]
        private TextDocument? logsBotAndProgram = new();


        public SettingsViewModel(SettingsRepository settingsRepository, LoggerService loggerService, 
            TelegramBot telegramBot, IDialogService dialogService, IWindowService windowService)
        {
            _SettingsRepository = settingsRepository;
            _LoggerService = loggerService;
            _TelegramBot = telegramBot;
            _dialogService = dialogService;
            _windowService = windowService;

            _ = LoadSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            Setting settings = await _SettingsRepository.GetAllSettings();
            if (settings != null) Settings = settings;
        }

        [RelayCommand]
        private async Task SaveSettings()
        {
            await _SettingsRepository.SaveSettings(Settings);
        }

        [RelayCommand]
        private async Task OnOffBotTelegram()
        {
            if (Settings == null) return;

            if (StatusWorkTGBot == "Включить бота")
            {
                _TelegramBot.StartBotTelegram();
                StatusWorkTGBot = "Выключить бота";

                _ = UpdateLogsAsync();
            }
            else
            {
                var result = await _dialogService.AskConfirmationAsync($"Вы уверены, что хотите остановить Telegram бота?");

                if (result == true)
                {
                    _TelegramBot.StopBotTelegram();
                    StatusWorkTGBot = "Включить бота";
                }
            }
        }

        [RelayCommand]
        private void OpenJavaManagementWindow()
        {
            _windowService.OpenJavaManagement();
        }

        private async Task UpdateLogsAsync()
        {
            using var cts = new CancellationTokenSource();

            try
            {
                await foreach (var logLine in _LoggerService.UpdateLogsBotAndProgram(cts.Token))
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
    }
}
    
