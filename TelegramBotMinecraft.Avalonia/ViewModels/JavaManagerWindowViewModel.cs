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

        public JavaManagerWindowViewModel(JavaManagementViewModel javaManagementViewModel, JavaDownloadViewModel javaDownloadViewModel)
        {
            JavaManagementVm = javaManagementViewModel;
            JavaDownloadVm = javaDownloadViewModel;
        }
    }
}
