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
        private readonly Func<AboutViewModel> _viewModelAboutViewModel;


        public AvaloniaWindowService(
            Func<JavaManagerWindowViewModel> viewModelJavaManagerFactory, 
            Func<Server?, JavaArgumentManagerViewModel> viewModelJavaArgumentManagerFactory, 
            Func<AboutViewModel> viewModelAboutViewModel)
        {
            _viewModelJavaManagerFactory = viewModelJavaManagerFactory ?? throw new ArgumentNullException(nameof(viewModelJavaManagerFactory));
            _viewModelJavaArgumentManagerFactory = viewModelJavaArgumentManagerFactory ?? throw new ArgumentException(nameof(viewModelJavaArgumentManagerFactory));
            _viewModelAboutViewModel = viewModelAboutViewModel;
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

                var dialog = new JavaManagerWindow();
                var viewModel = _viewModelJavaManagerFactory();

                if (viewModel is JavaManagerWindowViewModel javaVm)
                {
                    javaVm.CloseAction = () => dialog.Close();
                }

                dialog.DataContext = viewModel;

                await dialog.ShowDialog(ownerWindow);
            });
        }

        public async Task<string> OpenJavaToServerArgumentManagement(Server? server)
        {
            return await Dispatcher.UIThread.InvokeAsync(async () =>
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

                return await dialog.ShowDialog<string>(ownerWindow) ?? string.Empty;
            });
        }

        public async Task OpenAboutDialogAsync()
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

                var dialog = new AboutWindow();
                var viewModel = _viewModelAboutViewModel();

                if (viewModel is AboutViewModel aboutVm)
                {
                    aboutVm.CloseAction = () => dialog.Close();
                }

                dialog.DataContext = viewModel;

                await dialog.ShowDialog(ownerWindow);
            });
        }
    }
}
