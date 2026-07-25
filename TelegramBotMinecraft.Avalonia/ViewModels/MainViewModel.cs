using CommunityToolkit.Mvvm.ComponentModel;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ConsoleViewModel Console { get; }

        public ServersViewModel Servers { get; }

        public UsersViewModel Users { get; }

        public SettingsViewModel Settings { get; }

        public MainViewModel(
            ConsoleViewModel console,
            ServersViewModel servers,
            UsersViewModel users,
            SettingsViewModel settings)
        {
            Console = console;
            Servers = servers;
            Users = users;
            Settings = settings;
        }
    }
}
