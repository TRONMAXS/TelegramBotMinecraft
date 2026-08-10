using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Net.Http;
using TelegramBotMinecraft.Avalonia.Services;
using TelegramBotMinecraft.Avalonia.ViewModels;
using TelegramBotMinecraft.Avalonia.Views;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var httpClient = new HttpClient();
            var serverRepo = new ServerRepository();
            var settingsRepo = new SettingsRepository();

            var serverManager = new MinecraftServerManager();
            var javaManager = new JavaManagerService(httpClient, new HashService(), new FileDownloaderService(httpClient), new LzmaDecompressorService());
            var dialogService = new AvaloniaDialogService();
            var windowService = new AvaloniaWindowService(() => new JavaManagerWindowViewModel(new JavaManagementViewModel(javaManager, dialogService), new JavaDownloadViewModel(javaManager)));
            var notificationService = new AvaloniaNotificationService();
            var sharedLogger = new LoggerService();
            var telegramBot = new TelegramBot(settingsRepo, sharedLogger);

            var consoleVm = new ConsoleViewModel(serverManager, serverRepo, new ServerStatusService(serverRepo), new ServerLogService(serverRepo), new ServerCommandService(serverRepo, serverManager), dialogService);
            var serversVm = new ServersViewModel(serverRepo, dialogService);
            var usersVm = new UsersViewModel(serverRepo, new UserRepository(), new CommandRepository(), dialogService);
            var settingsVm = new SettingsViewModel(settingsRepo, sharedLogger, telegramBot, dialogService, windowService, notificationService, new StartupManager());

            var mainVm = new MainViewModel(consoleVm, serversVm, usersVm, settingsVm, settingsRepo);

            DataContext = mainVm;

            bool hideOnStart = false;
            try
            {
                var settings = settingsRepo.GetAllSettings().GetAwaiter().GetResult();
                if (settings != null && settings.TrayOnStart == 1)
                {
                    hideOnStart = true;
                }
            }
            catch { }

            if (!hideOnStart)
            {
                var mainWindow = new MainWindow { DataContext = mainVm };
                desktop.MainWindow = mainWindow;

                mainWindow.Loaded += (sender, args) =>
                {
                    var topLevel = TopLevel.GetTopLevel(mainWindow);
                    if (topLevel != null)
                    {
                        notificationService.Initialize(topLevel);
                    }
                };

                mainWindow.Show();
            }
            else
            {
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}