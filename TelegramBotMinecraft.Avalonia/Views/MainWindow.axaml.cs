using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.ViewModels;
using TelegramBotMinecraft.Core.Services;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Avalonia.Services;

namespace TelegramBotMinecraft.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var serverRepo = new ServerRepository();
        var serverManager = new MinecraftServerManager();

        var dialogService = new AvaloniaDialogService();

        var sharedLogger = new LoggerService();
        var telegramBot = new TelegramBot(new SettingsRepository(), sharedLogger);


        var consoleVm = new ConsoleViewModel(serverManager, serverRepo,
            new ServerStatusService(serverRepo),
            new ServerLogService(serverRepo),
            new ServerCommandService(serverRepo, serverManager), dialogService); 

        var serversVm = new ServersViewModel(serverRepo, dialogService);

        var usersVm = new UsersViewModel(serverRepo, 
            new UserRepository(), 
            new CommandRepository(), dialogService);

        var settingsVm = new SettingsViewModel(new SettingsRepository(), sharedLogger, telegramBot, dialogService);

        DataContext = new MainViewModel(consoleVm, serversVm, usersVm, settingsVm);
    }
}