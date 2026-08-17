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
        private readonly JavaManagerService? _javaManagerService;
        private readonly IDialogService? _dialogService;

        private JavaManagerWindowViewModel? _parent;

        public ObservableCollection<JavaManager>? DownloadedJavaList { get; } = new();

        [ObservableProperty]
        private JavaManager? _selectedJava;

        public JavaManagementViewModel(JavaManagerService? javaManagerService, IDialogService? dialogService)
        {
            _javaManagerService = javaManagerService;
            _dialogService = dialogService;

            LoadJavaDownloadedList();
            UpdateJavaDb();
        }
        public void SetParent(JavaManagerWindowViewModel parent)
        {
            _parent = parent;
        }

        public async void LoadJavaDownloadedList()
        {
            List<JavaManager> javaList = await _javaManagerService.GetAllDownloadedJava();
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
                await _javaManagerService.DeletingJavaFolder(SelectedJava.Name);

                LoadJavaDownloadedList();
                await UpdateJavaDb();
            }
        }

        [RelayCommand]
        private async Task UpdateJavaList()
        {
            LoadJavaDownloadedList();
            await UpdateJavaDb();
        }

        [RelayCommand]
        private async Task Ok()
        {
            await UpdateJavaDb();

            _parent?.CloseWindow();
        }

        public async Task UpdateJavaDb()
        {
            await _javaManagerService.UpdateJavaInDb();
        }

        partial void OnSelectedJavaChanged(JavaManager? value)
        {
            if (value == null) return;
        }
    }
}
