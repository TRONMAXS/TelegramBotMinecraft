using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.ViewModels;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();

        DataContext = new SettingsViewModel(new SettingsRepository());
    }
}