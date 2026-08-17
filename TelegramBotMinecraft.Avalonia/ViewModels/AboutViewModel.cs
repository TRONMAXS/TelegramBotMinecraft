using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Reflection;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class AboutViewModel : ObservableObject
    {
        public Action? CloseAction { get; set; }

        [ObservableProperty]
        private string _versionProgramm;

        public AboutViewModel()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            string? informationalVersion = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            VersionProgramm = informationalVersion ?? version?.ToString() ?? "1.0.0";

            if (VersionProgramm.Contains("+"))
            {
                VersionProgramm = VersionProgramm.Split('+')[0];
            }
        }

        [RelayCommand]
        public void OnClose() { CloseAction?.Invoke(); }
    }
}
