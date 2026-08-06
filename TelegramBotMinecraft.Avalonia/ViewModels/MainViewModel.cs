using CommunityToolkit.Mvvm.ComponentModel;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ConsoleViewModel ConsoleVm { get; }

        public ServersViewModel ServersVm { get; }

        public UsersViewModel UsersVm { get; }

        public SettingsViewModel SettingsVm { get; }

        public MainViewModel(
            ConsoleViewModel consoleVm,
            ServersViewModel serversVm,
            UsersViewModel usersVm,
            SettingsViewModel settingsVm)
        {
            ConsoleVm = consoleVm;
            ServersVm = serversVm;
            UsersVm = usersVm;
            SettingsVm = settingsVm;
        }
    }
}
