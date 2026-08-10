using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Notifications;
using System;
using System.Threading.Tasks;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia.Services
{
    public class AvaloniaNotificationService : INotificationService
    {
        private WindowNotificationManager? _notificationManager;
        private readonly SettingsRepository? _settingsRepository;


        public AvaloniaNotificationService(SettingsRepository settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public void Initialize(TopLevel topLevel)
        {
            _notificationManager = new WindowNotificationManager(topLevel)
            {
                Position = NotificationPosition.BottomRight,
                MaxItems = 3
            };
        }

        public async Task ShowNotification(string title, string text, string status)
        {
            var settings = await _settingsRepository.GetAllSettings();
            if (settings != null && settings.Notifications == 0) return;

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                if (_notificationManager == null) return;

                NotificationType typeNoti = status switch
                {
                    "Success" => NotificationType.Success,
                    "Error" => NotificationType.Error,
                    "Warning" => NotificationType.Warning,
                    _ => NotificationType.Information
                };

                _notificationManager.Show(new Notification(
                    title: title,
                    message: text,
                    type: typeNoti,
                    expiration: TimeSpan.FromSeconds(5)
                ));

            }
        }
    }
}
