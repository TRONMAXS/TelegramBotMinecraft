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

        var consoleVm = new ConsoleViewModel(new MinecraftServerManager(),
            new ServerRepository(),
            new ServerStatusService(new ServerRepository()),
            new ServerLogService(new ServerRepository()),
            new ServerCommandService(new ServerRepository(), new MinecraftServerManager()),
            new AvaloniaDialogService());

        var serversVm = new ServersViewModel(new ServerRepository(), new AvaloniaDialogService());

        var usersVm = new UsersViewModel(new ServerRepository(), 
            new UserRepository(), 
            new CommandRepository(), new AvaloniaDialogService());

        var settingsVm = new SettingsViewModel(new SettingsRepository());

        DataContext = new MainViewModel(consoleVm, serversVm, usersVm, settingsVm);
    }
}