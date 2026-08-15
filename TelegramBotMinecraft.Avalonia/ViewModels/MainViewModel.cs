using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ConsoleViewModel ConsoleVm { get; }
        public ServersViewModel ServersVm { get; }
        public UsersViewModel UsersVm { get; }
        public SettingsViewModel SettingsVm { get; }

        [ObservableProperty]
        public int _selectPage = 0;

        bool AttemptDisplayWindow = false;

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

        [RelayCommand]
        private void ShowWindow()
        {
            if (AttemptDisplayWindow == true) return;
            AttemptDisplayWindow = true;

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                if (desktop.MainWindow == null && AttemptDisplayWindow == true)
                {
                    var mainWindow = new Views.MainWindow
                    {
                        DataContext = this
                    };
                    desktop.MainWindow = mainWindow;
                    mainWindow.Show();
                    AttemptDisplayWindow = false;
                }
                else
                {
                    desktop.MainWindow.Show();
                    desktop.MainWindow.WindowState = WindowState.Normal;
                    desktop.MainWindow.Activate();
                    AttemptDisplayWindow = false;
                }
            }
        }

        [RelayCommand]
        private void ShowSettings()
        {
            ShowWindow();
            SelectPage = 3;
        }

        [RelayCommand]
        private void ExitApplication()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        }

        partial void OnSelectPageChanged(int value)
        {
            if (value == null) return;

            if (value == 0)
            {
                _ = ConsoleVm.Reload();
            }
            else if (value == 1)
            {
                _ = ServersVm.Reload();
            }
            else if (value == 2)
            {
                _ = UsersVm.Reload();
            }
            else if (value == 3)
            {
                _ = SettingsVm.Reload();
            }
        }
    }
}
