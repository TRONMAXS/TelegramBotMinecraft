using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegramBotMinecraft.Core.Database;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ConsoleViewModel ConsoleVm { get; }
        public ServersViewModel ServersVm { get; }
        public UsersViewModel UsersVm { get; }
        public SettingsViewModel SettingsVm { get; }

        private readonly SettingsRepository? _SettingsRepository;

        [ObservableProperty]
        public int _selectPage = 0;

        bool AttemptDisplayWindow = false;

        public MainViewModel(
            ConsoleViewModel consoleVm,
            ServersViewModel serversVm,
            UsersViewModel usersVm,
            SettingsViewModel settingsVm,
            SettingsRepository settingsRepository)
        {
            ConsoleVm = consoleVm;
            ServersVm = serversVm;
            UsersVm = usersVm;
            SettingsVm = settingsVm;

            _SettingsRepository = settingsRepository;
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
    }
}
