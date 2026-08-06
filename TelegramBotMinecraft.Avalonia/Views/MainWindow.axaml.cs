using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.ViewModels;
using TelegramBotMinecraft.Core.Services;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Avalonia.Services;
using System.Net.Http;

namespace TelegramBotMinecraft.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        HttpClient httpClient = new HttpClient();

        var serverRepo = new ServerRepository();
        var javaRepo = new JavaRepository();

        var serverManager = new MinecraftServerManager();
        var javaManager = new JavaManagerService(httpClient, new HashService(), new FileDownloaderService(httpClient), new LzmaDecompressorService());

        var dialogService = new AvaloniaDialogService();
        var windowService = new AvaloniaWindowService((ws) => new JavaManagementViewModel(javaRepo, ws, javaManager), () => new JavaDownloadViewModel(javaRepo, javaManager));

        var sharedLogger = new LoggerService();
        var telegramBot = new TelegramBot(new SettingsRepository(), sharedLogger);


        var consoleVm = new ConsoleViewModel(serverManager, serverRepo,
            new ServerStatusService(serverRepo),
            new ServerLogService(serverRepo),
            new ServerCommandService(serverRepo, serverManager), 
            dialogService); 

        var serversVm = new ServersViewModel(serverRepo, dialogService);

        var usersVm = new UsersViewModel(serverRepo, 
            new UserRepository(), 
            new CommandRepository(), 
            dialogService);

        var settingsVm = new SettingsViewModel(new SettingsRepository(), sharedLogger, telegramBot, dialogService, windowService);

        DataContext = new MainViewModel(consoleVm, serversVm, usersVm, settingsVm);
    }
}