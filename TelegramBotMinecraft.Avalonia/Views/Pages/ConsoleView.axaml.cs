using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.ViewModels;

namespace TelegramBotMinecraft.Avalonia;

public partial class ConsoleView : UserControl
{
    public ConsoleView()
    {
        InitializeComponent();

        DataContext = new ConsoleViewModel(new MinecraftServerManager());
    }
}