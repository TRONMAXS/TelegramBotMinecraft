using System;
using Avalonia;
using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.ViewModels;

namespace TelegramBotMinecraft.Avalonia.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        this.DataContextChanged += OnSettingsViewDataContextChanged;

        var logs = this.FindControl<AvaloniaEdit.TextEditor>("LogsBotAndProgram");
        if (logs != null)
        {
            logs.Options.AllowScrollBelowDocument = false;
            logs.TextChanged += Logs_TextChanged;
        }
    }

    private async void OnSettingsViewDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is SettingsViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (DataContext is SettingsViewModel viewModel)
        {
            viewModel.Dispose();
        }
        this.DataContextChanged -= OnSettingsViewDataContextChanged;
    }

    private void Logs_TextChanged(object? sender, EventArgs e)
    {
        LogsBotAndProgram.ScrollToLine(LogsBotAndProgram.LineCount);
    }
}