using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.ViewModels;

namespace TelegramBotMinecraft.Avalonia;

public partial class ServersView : UserControl
{
    public ServersView()
    {
        InitializeComponent();

        DataContext = new ServersViewModel();
    }
}