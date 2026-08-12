using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

        public JavaManagerWindowViewModel(JavaManagementViewModel javaManagementViewModel, JavaDownloadViewModel javaDownloadViewModel)
        {
            JavaManagementVm = javaManagementViewModel;
            JavaDownloadVm = javaDownloadViewModel;
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
