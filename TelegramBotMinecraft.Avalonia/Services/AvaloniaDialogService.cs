using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Threading.Tasks;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia.Services
{
    public class AvaloniaDialogService : IDialogService
    {
        public AvaloniaDialogService() { }

        public async Task<string?> SelectFolderAsync(string title)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
                if (topLevel == null) return null;

                var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
                {
                    Title = title,
                    AllowMultiple = false
                });

                if (folders != null && folders.Count > 0)
                {
                    var folder = folders[0];
                    string? path = folder.TryGetLocalPath();
                    if (string.IsNullOrEmpty(path) && folder.Path != null)
                    {
                        path = folder.Path.IsAbsoluteUri
                            ? folder.Path.LocalPath
                            : Uri.UnescapeDataString(folder.Path.OriginalString);
                    }

                    return path;
                }
            }

            return null;
        }

        public async Task<bool?> AskConfirmationAsync(string text)
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);
                if (topLevel == null) return null;

                var box = MessageBoxManager.GetMessageBoxStandard(
                    title: "Подтверждение",
                    text: text,
                    ButtonEnum.YesNo,
                    Icon.Warning);

                var result = await box.ShowWindowDialogAsync(desktop.MainWindow);

                if (result == ButtonResult.Yes) return true;
                else return false;
            }

            return null;
        }
    }

}

