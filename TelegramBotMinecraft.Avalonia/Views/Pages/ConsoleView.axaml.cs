using Avalonia.Controls;
using System;

namespace TelegramBotMinecraft.Avalonia.Views;

public partial class ConsoleView : UserControl
{
    public ConsoleView()
    {
        InitializeComponent();

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