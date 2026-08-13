using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
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
            var javaRepo = new JavaRepository();
            var commandRepo = new CommandRepository();
            var userRepo = new UserRepository();

            var serverManager = new MinecraftServerManager(javaRepo, serverRepo);
            var serverCommandService = new ServerCommandService(serverRepo, serverManager);
            var javaManager = new JavaManagerService(httpClient, new HashService(), new FileDownloaderService(httpClient), new LzmaDecompressorService(), javaRepo);
            var dialogService = new AvaloniaDialogService();
            var windowService = new AvaloniaWindowService( () => new JavaManagerWindowViewModel( new JavaManagementViewModel(javaManager, dialogService), 
                                                                 new JavaDownloadViewModel(javaManager)),
                                                                 (server) => new JavaArgumentManagerViewModel(serverRepo, server));
            var notificationService = new AvaloniaNotificationService(settingsRepo);

            var strategies = new List<ICommandStrategy>
            {
                new StartCommandStrategy(),
                new OnServerCommandStrategy(serverManager, serverRepo),
                new OffServerCommandStrategy(serverManager, serverRepo),
                new HelpCommandStrategy(commandRepo),
                new ListServersCommandStrategy(serverRepo),
            };
            var commandContext = new CommandContext(strategies, commandRepo);

            var sharedLogger = new LoggerService();
            var telegramBot = new TelegramBot(settingsRepo, sharedLogger, commandContext);

            var consoleVm = new ConsoleViewModel(serverManager, serverRepo, new ServerLogService(serverRepo), serverCommandService, dialogService, notificationService);
            var serversVm = new ServersViewModel(serverRepo, dialogService, notificationService, javaManager, windowService);
            var usersVm = new UsersViewModel(serverRepo, userRepo, commandRepo, dialogService, notificationService);
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