using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.Services;
using TelegramBotMinecraft.Avalonia.ViewModels;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia;

public partial class ServersView : UserControl
{
    public ServersView()
    {
        InitializeComponent();

        DataContext = new ServersViewModel(new ServerRepository(), new AvaloniaDialogService());
    }
}