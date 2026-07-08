using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.ViewModels;
using System;

namespace TelegramBotMinecraft.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var consoleVm = new ConsoleViewModel(new MinecraftServerManager());
        var serversVm = new ServersViewModel();
        var usersVm = new UsersViewModel();
        var settingsVm = new SettingsViewModel();

        DataContext = new MainViewModel(consoleVm, serversVm, usersVm, settingsVm);
    }
}