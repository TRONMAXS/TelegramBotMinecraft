using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using TelegramBotMinecraft.Avalonia.ViewModels;

namespace TelegramBotMinecraft.Avalonia.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();

        Loaded += SettingsView_Loaded;

        var logs = this.FindControl<AvaloniaEdit.TextEditor>("LogsBotAndProgram");
        if (logs != null)
        {
            logs.Options.AllowScrollBelowDocument = false;
            logs.TextChanged += Logs_TextChanged;
        }
    }

    private void SettingsView_Loaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel viewModel)
        {
            _ = viewModel.InitializeAsync();
        }
    }

    private void Logs_TextChanged(object? sender, EventArgs e)
    {
        LogsBotAndProgram?.ScrollToLine(LogsBotAndProgram.LineCount);
    }
}