using Avalonia.Controls;
using System;
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

        var logConsole = this.FindControl<AvaloniaEdit.TextEditor>("LogConsole");
        if (logConsole != null)
        {
            logConsole.Options.AllowScrollBelowDocument = false;
            LogConsole.TextChanged += LogConsole_TextChanged;
        }

    }
    private void LogConsole_TextChanged(object? sender, EventArgs e)
    {
        LogConsole.ScrollToLine(LogConsole.LineCount);
    }
}