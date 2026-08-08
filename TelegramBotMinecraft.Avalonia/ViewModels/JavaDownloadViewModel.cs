using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
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


        [ObservableProperty]
        private bool? _isDownloading = false;

        [ObservableProperty]
        private string? _downloadProgressString;

        [ObservableProperty]
        private int? _downloadProgressValue;

        [ObservableProperty]
        private string? _downloadFilesProgressString;

        private CancellationTokenSource? _cts;

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

            IsDownloading = true;
            bool isSuccess = false;
            DownloadProgressValue = 0;
            int countDownloadedFiles = 0;
            _cts = new CancellationTokenSource();



            DownloadProgressString = "Проверка...";
            DownloadFilesProgressString = "Файлы: 0/0";

            bool validate = await _JavaManagerService.ValidateDownloadedJava(SelectedJavaInfo.Name);
            if (validate)
            {
                DownloadProgressString = "Среда Java уже установлена!";
                DownloadFilesProgressString = "Успешно!";
                return;
            }


            DownloadProgressString = "Подготовка к скачиванию...";
            DownloadFilesProgressString = "Файлы: 0/0";

            try
            {
                await foreach (var currentProgress in _JavaManagerService.JavaDownloader(SelectedJavaInfo.Name, _cts.Token))
                {
                    if (currentProgress.Progress == -1)
                    {
                        DownloadProgressString = "Ошибка: файлы повреждены или не скачались. Попробуйте снова.";
                        DownloadProgressValue = 0;
                        countDownloadedFiles = 0;
                        isSuccess = false;
                        break;
                    }

                    DownloadProgressValue = currentProgress.Progress;
                    countDownloadedFiles = currentProgress.completedFiles;
                    DownloadProgressString = $"Скачивание и установка Java... {currentProgress.Progress}%";
                    DownloadFilesProgressString = $"Файлы: {countDownloadedFiles}/{currentProgress.AllFiles}";

                    if (currentProgress.Progress == 100) isSuccess = true;
                }
                if (isSuccess)
                {
                    DownloadProgressString = "Среда Java успешно установлена и проверена!";
                    DownloadFilesProgressString = "Готово!";
                }
            }
            catch (OperationCanceledException) 
            { 
                DownloadProgressString = "Загрузка успешно отменена.";
                DownloadFilesProgressString = "Остановлено";
                DownloadProgressValue = 0;

            }
            catch { DownloadProgressString = "Ошибка при установке Java компонентов."; }
            finally 
            { 
                IsDownloading = false; 
                _cts?.Dispose(); 
                _cts = null; 
            }
        }

        [RelayCommand]
        private void Update()
        {
            LoadJavaList();
            LoadJavaInfoList();
        }

        [RelayCommand]
        private void CancelDownload()
        {
            _cts?.Cancel();

            DownloadProgressString = "Скачивание отменено пользователем.";
            IsDownloading = false;
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
