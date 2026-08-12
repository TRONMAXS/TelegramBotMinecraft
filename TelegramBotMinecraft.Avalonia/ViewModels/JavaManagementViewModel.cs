using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class JavaManagementViewModel : ObservableObject
    {
        private readonly JavaManagerService? _javaManagerService;
        private readonly IDialogService? _dialogService;

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
                UpdateJavaDb();
            }
        }

        [RelayCommand]
        private void UpdateJavaList()
        {
            LoadJavaDownloadedList();
            UpdateJavaDb();
        }

        [RelayCommand]
        private async Task Ok()
        {
            UpdateJavaDb();
            Cancel();
        }

        [RelayCommand]
        private void Cancel()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var currentWindow = desktop.Windows.FirstOrDefault(w => w.DataContext == this);
                currentWindow?.Close();
            }
        }

        public async void UpdateJavaDb()
        {
            await _javaManagerService.UpdateJavaInDb();
        }

        partial void OnSelectedJavaChanged(JavaManager? value)
        {
            if (value == null) return;
        }
    }
}
