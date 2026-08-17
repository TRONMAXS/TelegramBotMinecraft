using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }



        [RelayCommand]
        public void OnClose() { CloseAction?.Invoke(); }
    }
}
