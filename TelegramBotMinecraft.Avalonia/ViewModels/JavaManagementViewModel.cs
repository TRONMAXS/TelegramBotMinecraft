using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class JavaManagementViewModel : ObservableObject
    {
        private readonly JavaRepository? _JavaRepository;
        private readonly IWindowService? _WindowService;
        private readonly JavaManagerService? _JavaManagerService;



        public ObservableCollection<JavaManager>? DownloadedJavaList { get; } = new();


        [ObservableProperty]
        private JavaManager? _selectedJava;

        public JavaManagementViewModel(JavaRepository? javaRepository, IWindowService? windowService, JavaManagerService? javaManagerService)
        {
            _JavaRepository = javaRepository;
            _WindowService = windowService;
            _JavaManagerService = javaManagerService;

            LoadJavaDownloadedList();
        }

        private async void LoadJavaDownloadedList()
        {
            List<JavaManager> javaList = await _JavaManagerService.GetAllDownloadedJava();
            if (javaList == null && javaList.Count > 0) return;

            DownloadedJavaList?.Clear();

            foreach (var java in javaList)
            {
                DownloadedJavaList?.Add(new JavaManager(java.Name, java.Version, java.Architecture, java.Path));
            }
        }

        [RelayCommand]
        private void OpenJavaDownloaderWindow()
        {
            _WindowService?.OpenJavaDownloader();
        }

        [RelayCommand]
        private async Task DeleteSelectedJava()
        {
            if(DownloadedJavaList == null) return;
            if(SelectedJava == null) return;

            await _JavaManagerService.DeletingJavaFolder(SelectedJava.Name);

            LoadJavaDownloadedList();
        }

        [RelayCommand]
        private void UpdateJavaList()
        {
            LoadJavaDownloadedList();
        }

        partial void OnSelectedJavaChanged(JavaManager? value)
        {
            if (value == null) return;
        }
    }
}
