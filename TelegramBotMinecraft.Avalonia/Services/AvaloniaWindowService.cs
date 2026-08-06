using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using TelegramBotMinecraft.Avalonia.ViewModels;
using TelegramBotMinecraft.Avalonia.Views;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia.Services
{
    public class AvaloniaWindowService : IWindowService
    {
        private readonly Func<IWindowService, JavaManagementViewModel> _viewModelJavaManagementFactory;

        private readonly Func<JavaDownloadViewModel> _viewModelJavaDownloadFactory;


        public AvaloniaWindowService(Func<IWindowService, JavaManagementViewModel> viewModelJavaManagementFactory, Func<JavaDownloadViewModel> viewModelJavaDownloadFactory)
        {
            _viewModelJavaManagementFactory = viewModelJavaManagementFactory ?? throw new ArgumentNullException(nameof(viewModelJavaManagementFactory));
            _viewModelJavaDownloadFactory = viewModelJavaDownloadFactory ?? throw new ArgumentNullException(nameof(viewModelJavaDownloadFactory));
        }

        public async Task OpenJavaManagement()
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                Window? ownerWindow = null;
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    ownerWindow = desktop.MainWindow;
                }

                if (ownerWindow == null)
                {
                    throw new InvalidOperationException("Не удалось найти главное окно приложения.");
                }

                var dialog = new JavaManagementWindow
                {
                    DataContext = _viewModelJavaManagementFactory(this)
                };

                await dialog.ShowDialog(ownerWindow);
            });
        }

        public async Task OpenJavaDownloader()
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                Window? ownerWindow = null;
                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    ownerWindow = desktop.MainWindow;
                }

                if (ownerWindow == null)
                {
                    throw new InvalidOperationException("Не удалось найти главное окно приложения.");
                }

                var dialog = new JavaDownloadWindow
                {
                    DataContext = _viewModelJavaDownloadFactory()
                };

                await dialog.ShowDialog(ownerWindow);
            });
        }
    }
}
