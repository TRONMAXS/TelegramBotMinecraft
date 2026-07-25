using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.ViewModels;
using TelegramBotMinecraft.Core.Services;
using TelegramBotMinecraft.Core.Database;

namespace TelegramBotMinecraft.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var consoleVm = new ConsoleViewModel(new MinecraftServerManager(),
            new ServerRepository(),
            new ServerStatusService(new ServerRepository()),
            new ServerLogService(new ServerRepository()),
            new ServerCommandService(new ServerRepository(), new MinecraftServerManager()));

        var serversVm = new ServersViewModel(new ServerRepository());

        var usersVm = new UsersViewModel(new ServerRepository(), 
            new UserRepository(), 
            new CommandRepository());

        var settingsVm = new SettingsViewModel(new SettingsRepository());

        DataContext = new MainViewModel(consoleVm, serversVm, usersVm, settingsVm);
    }
}