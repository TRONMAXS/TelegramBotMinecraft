using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using TelegramBotMinecraft.Avalonia.ViewModels;

namespace TelegramBotMinecraft.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var consoleVm = new ConsoleViewModel(new MinecraftServerManager(), new Core.Database.ServerRepository());
        var serversVm = new ServersViewModel(new Core.Database.ServerRepository());
        var usersVm = new UsersViewModel(new Core.Database.ServerRepository(), new Core.Database.UserRepository(), new Core.Database.CommandRepository());
        var settingsVm = new SettingsViewModel();

        DataContext = new MainViewModel(consoleVm, serversVm, usersVm, settingsVm);
    }
}