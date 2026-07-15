using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly SettingsRepository _SettingsRepository;

        [ObservableProperty]
        public string? _textBotToken;
        [ObservableProperty]
        public bool _checkAutoBot;
        [ObservableProperty]
        public bool _checkTrayOnStart;
        [ObservableProperty]
        public bool _checkRunAtStartup;
        [ObservableProperty]
        public bool _checkAutoReconnect;
        [ObservableProperty]
        public bool _checkNotifications;
        [ObservableProperty]
        public string? _textProxyHost;
        [ObservableProperty]
        public string? _textProxyPort;
        [ObservableProperty]
        public string? _textProxyUsername;
        [ObservableProperty]
        public string? _textProxyPassword;


        public SettingsViewModel(SettingsRepository settingsRepository)
        {
            _SettingsRepository = settingsRepository;

            _ = LoadSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            var settings = await _SettingsRepository.GetAllSettings();
            if (settings == null || settings.Count == 0) return;

            var setting = settings[0];

            TextBotToken = setting.BotToken;
            TextProxyHost = setting.ProxyHost;
            TextProxyPort = setting.ProxyPort;
            TextProxyUsername = setting.ProxyUsername;
            TextProxyPassword = setting.ProxyPassword;

            CheckAutoBot = setting.AutoBot == 1;
            CheckTrayOnStart = setting.TrayOnStart == 1;
            CheckRunAtStartup = setting.RunAtStartup == 1;
            CheckAutoReconnect = setting.AutoReconnect == 1;
            CheckNotifications = setting.Notifications == 1;
        }
    }
}
    
