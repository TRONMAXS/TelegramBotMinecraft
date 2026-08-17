using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class JavaManagerWindowViewModel : ObservableObject
    {
        public JavaManagementViewModel JavaManagementVm { get; }

        public JavaDownloadViewModel JavaDownloadVm { get; }

        [ObservableProperty]
        private int _selectedTabIndex = 0;

        [ObservableProperty]
        private bool _isDownloadedEnabled = true;

        public Action? CloseAction { get; set; }

        public JavaManagerWindowViewModel(JavaManagementViewModel javaManagementViewModel, JavaDownloadViewModel javaDownloadViewModel)
        {
            JavaManagementVm = javaManagementViewModel;
            JavaDownloadVm = javaDownloadViewModel;
            JavaManagementVm.SetParent(this);
            JavaDownloadVm.SetParent(this);
        }

        [RelayCommand]
        private void OpenJavaDownloaderWindow()
        {
            SelectedTabIndex = 1;
        }

        [RelayCommand]
        private void Cancel()
        {
            SelectedTabIndex = 0;
        }

        public void CloseWindow()
        {
            CloseAction?.Invoke();
        }

        partial void OnSelectedTabIndexChanged(int value)
        {
            if (value == 0)
            {
                JavaManagementVm.LoadJavaDownloadedList();
                JavaManagementVm.UpdateJavaDb();
            }
            else if (value == 1)
            {
                JavaDownloadVm.LoadJavaList();
            }
        }
    }
}
