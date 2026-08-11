using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using TelegramBotMinecraft.Avalonia.ViewModels;
using TelegramBotMinecraft.Avalonia.Views;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia.Services
{
    public class AvaloniaWindowService : IWindowService
    {
        private readonly Func<JavaManagerWindowViewModel> _viewModelJavaManagerFactory;
        private readonly Func<Server?, JavaArgumentManagerViewModel> _viewModelJavaArgumentManagerFactory;


        public AvaloniaWindowService(Func<JavaManagerWindowViewModel> viewModelJavaManagerFactory, Func<Server?, JavaArgumentManagerViewModel> viewModelJavaArgumentManagerFactory)
        {
            _viewModelJavaManagerFactory = viewModelJavaManagerFactory ?? throw new ArgumentNullException(nameof(viewModelJavaManagerFactory));
            _viewModelJavaArgumentManagerFactory = viewModelJavaArgumentManagerFactory ?? throw new ArgumentException(nameof(viewModelJavaArgumentManagerFactory));
        }

        public async Task OpenJavaManagement()
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                {
                    throw new InvalidOperationException("Приложение запущено не в десктопном режиме.");
                }

                Window? ownerWindow = desktop.MainWindow;
                if (ownerWindow == null)
                {
                    throw new InvalidOperationException("Не удалось найти главное окно приложения.");
                }

                var dialog = new JavaManagerWindow
                {
                    DataContext = _viewModelJavaManagerFactory()
                };

                await dialog.ShowDialog(ownerWindow);
            });
        }

        public async Task OpenJavaToServerArgumentManagement(Server? server)
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
                {
                    throw new InvalidOperationException("Приложение запущено не в десктопном режиме.");
                }

                Window? ownerWindow = desktop.MainWindow;
                if (ownerWindow == null)
                {
                    throw new InvalidOperationException("Не удалось найти главное окно приложения.");
                }

                var dialog = new JavaArgumentManagerWindow
                {
                    DataContext = _viewModelJavaArgumentManagerFactory(server)
                };

                await dialog.ShowDialog(ownerWindow);
            });
        }
    }
}
