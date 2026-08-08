using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class JavaManagementViewModel : ObservableObject
    {
        private readonly JavaManagerService? _JavaManagerService;
        private readonly IDialogService? _dialogService;

        public ObservableCollection<JavaManager>? DownloadedJavaList { get; } = new();


        [ObservableProperty]
        private JavaManager? _selectedJava;

        public JavaManagementViewModel(JavaManagerService? javaManagerService, IDialogService? dialogService)
        {
            _JavaManagerService = javaManagerService;
            _dialogService = dialogService;

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
        private async Task DeleteSelectedJava()
        {
            if(DownloadedJavaList == null) return;
            if(SelectedJava == null) return;

            var result = await _dialogService.AskConfirmationAsync($"Вы уверены, что хотите удалить эту версию Java?");


            if (result == true)
            {
                await _JavaManagerService.DeletingJavaFolder(SelectedJava.Name);

                LoadJavaDownloadedList();
            }
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
