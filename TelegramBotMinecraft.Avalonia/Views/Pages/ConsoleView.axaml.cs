using Avalonia.Controls;
using System;
using TelegramBotMinecraft.Avalonia.ViewModels;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia;

public partial class ConsoleView : UserControl
{
    public ConsoleView()
    {
        InitializeComponent();

        DataContext = new ConsoleViewModel(new MinecraftServerManager(),
            new ServerRepository(),
            new ServerStatusService(new ServerRepository()),
            new ServerLogService(new ServerRepository()),
            new ServerCommandService(new ServerRepository(), new MinecraftServerManager()));

        var logConsole = this.FindControl<AvaloniaEdit.TextEditor>("LogConsole");
        if (logConsole != null)
        {
            logConsole.Options.AllowScrollBelowDocument = false;
            LogConsole.TextChanged += LogConsoleAndRcon_TextChanged;
        }

        var logRcon = this.FindControl<AvaloniaEdit.TextEditor>("LogRcon");
        if (logRcon != null)
        {
            logRcon.Options.AllowScrollBelowDocument = false;
            LogRcon.TextChanged += LogConsoleAndRcon_TextChanged;
        }
    }
    private void LogConsoleAndRcon_TextChanged(object? sender, EventArgs e)
    {
        if (sender == LogConsole)
        {
            LogConsole.ScrollToLine(LogConsole.LineCount);
        }
        else if (sender == LogRcon)
        {
            LogRcon.ScrollToLine(LogRcon.LineCount);

        }
    }
}