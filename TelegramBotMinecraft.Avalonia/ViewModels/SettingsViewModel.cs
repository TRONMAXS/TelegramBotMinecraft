using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly SettingsRepository _SettingsRepository;

        [ObservableProperty]
        private Setting? _settings;

        public SettingsViewModel(SettingsRepository settingsRepository)
        {
            _SettingsRepository = settingsRepository;

            _ = LoadSettingsAsync();
        }

        private async Task LoadSettingsAsync()
        {
            Setting settings = await _SettingsRepository.GetAllSettings();
            if (settings != null) Settings = settings;
        }

        [RelayCommand]
        private async Task SaveSettings()
        {
            await _SettingsRepository.SaveSettings(Settings);
        }
    }
}
    
