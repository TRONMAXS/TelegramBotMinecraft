using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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
            string appDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "TelegramBotMinecraft"
            );

            if (!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }

            string dbPath = Path.Combine(appDataFolder, "Data.db");
            string backupDirPath = Path.Combine(appDataFolder, "DataBackup");
            string logsDirPath = Path.Combine(appDataFolder, "Logs");
            string javaDirPath = Path.Combine(appDataFolder, "Java");

            if (!Directory.Exists(appDataFolder)) Directory.CreateDirectory(appDataFolder);
            if (!Directory.Exists(backupDirPath)) Directory.CreateDirectory(backupDirPath);
            if (!Directory.Exists(logsDirPath)) Directory.CreateDirectory(logsDirPath);
            if (!Directory.Exists(javaDirPath)) Directory.CreateDirectory(javaDirPath);

            string connectionString = $"Data Source={dbPath}";

            var httpClient = new HttpClient();
            var sharedLogger = new LoggerService(logsDirPath);

            var dbManager = new DatabaseManager(dbPath, backupDirPath, sharedLogger);

            dbManager.Startup().GetAwaiter().GetResult();

            var serverRepo = new ServerRepository(connectionString);
            var settingsRepo = new SettingsRepository(connectionString);
            var javaRepo = new JavaRepository(connectionString);
            var commandRepo = new CommandRepository(connectionString);
            var userRepo = new UserRepository(connectionString);

            var serverManager = new MinecraftServerManager(javaRepo, serverRepo);
            var serverCommandService = new ServerCommandService(serverRepo, serverManager);
            var javaManager = new JavaManagerService(httpClient, new HashService(), new FileDownloaderService(httpClient), new LzmaDecompressorService(), javaRepo, javaDirPath);
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
            _ = commandContext.InitializeAsync();

            var telegramBot = new TelegramBot(settingsRepo, sharedLogger, commandContext);

            var consoleVm = new ConsoleViewModel(serverManager, serverRepo, new ServerLogService(serverRepo), serverCommandService, dialogService, notificationService);
            var serversVm = new ServersViewModel(serverRepo, dialogService, notificationService, javaManager, windowService, serverManager);
            var usersVm = new UsersViewModel(serverRepo, userRepo, commandRepo, dialogService, notificationService);
            var settingsVm = new SettingsViewModel(settingsRepo, sharedLogger, telegramBot, dialogService, windowService, notificationService, new StartupManager());

            var mainVm = new MainViewModel(consoleVm, serversVm, usersVm, settingsVm);

            DataContext = mainVm;

            _ = Task.Run(async () => await telegramBot.BotAutostart());

            bool hideOnStart = false;
            try
            {
                var settings = Task.Run(async () => await settingsRepo.GetAllSettings()).GetAwaiter().GetResult();
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