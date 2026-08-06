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
        private readonly Func<JavaManagerWindowViewModel> _viewModelFactory;

        public AvaloniaWindowService(Func<JavaManagerWindowViewModel> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
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
                    DataContext = _viewModelFactory()
                };

                await dialog.ShowDialog(ownerWindow);
            });
        }
    }
}
