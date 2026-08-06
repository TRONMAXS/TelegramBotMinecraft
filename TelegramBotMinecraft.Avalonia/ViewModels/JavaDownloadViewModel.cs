using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class JavaDownloadViewModel : ObservableObject
    {
        private readonly JavaManagerService? _JavaManagerService;

        public ObservableCollection<JavaManager>? JavaList { get; } = new();
        public ObservableCollection<JavaInfoDownload>? JavaListInfo { get; } = new();

        [ObservableProperty]
        private JavaManager? _selectedJava;

        [ObservableProperty]
        private JavaInfoDownload? _selectedJavaInfo;

        public JavaDownloadViewModel(JavaManagerService? javaManagerService)
        {
            _JavaManagerService = javaManagerService;

            LoadJavaList();
        }

        private async void LoadJavaList()
        { 
            SelectedJava = null;
            SelectedJavaInfo = null;

            List<JavaManager> javaList = await _JavaManagerService.GetAllAvailableNamesJava();
            if (javaList == null) return;

            JavaList?.Clear();

            foreach (var java in javaList)
            {
                JavaList?.Add(new JavaManager($"Java {java.Version}", java.Architecture));
            }
        }

        private async void LoadJavaInfoList()
        {
            JavaListInfo?.Clear();

            if (SelectedJava == null) return;
            if (JavaList == null) return;

            List<JavaInfoDownload> javaInfoList = await _JavaManagerService.GetAllInfoSelectedJava(SelectedJava.Architecture);
            if (javaInfoList == null) return;


            foreach (var java in javaInfoList)
            {
                JavaListInfo?.Add(new JavaInfoDownload(java.Version, java.Name, java.ReleasData, java.Type));
            }
        }

        [RelayCommand]
        private async Task Download()
        {
            if (SelectedJava == null || SelectedJavaInfo == null) return;
            if (JavaList == null || JavaListInfo == null) return;

            await _JavaManagerService.JavaDownloader(SelectedJavaInfo.Name);
        }

        [RelayCommand]
        private void Update()
        {
            LoadJavaList();
            LoadJavaInfoList();
        }

        partial void OnSelectedJavaChanged(JavaManager? value)
        {
            if (value == null) return;
            LoadJavaInfoList();
        }

        partial void OnSelectedJavaInfoChanged(JavaInfoDownload? value)
        {
            if (value == null) return;
        }
    }
}
