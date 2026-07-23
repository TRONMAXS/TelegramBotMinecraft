using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.ViewModels;

namespace TelegramBotMinecraft.Avalonia;

public partial class ConsoleView : UserControl
{
    public ConsoleView()
    {
        InitializeComponent();

        DataContext = new ConsoleViewModel(new MinecraftServerManager(), 
            new Core.Database.ServerRepository(), 
            new Core.Services.ServerStatusService(new Core.Database.ServerRepository()),
            new Core.Services.ServerLogService(new Core.Database.ServerRepository()));
    }
}