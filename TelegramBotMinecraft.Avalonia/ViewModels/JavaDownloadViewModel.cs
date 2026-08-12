using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class JavaDownloadViewModel : ObservableObject
    {
        private readonly JavaManagerService _JavaManagerService;
        private JavaManagerWindowViewModel? _parent;

        public ObservableCollection<JavaManager>? JavaList { get; } = new();
        public ObservableCollection<JavaInfoDownload>? JavaListInfo { get; } = new();

        [ObservableProperty]
        private JavaManager? _selectedJava;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDownloadedEnabled))]
        [NotifyCanExecuteChangedFor(nameof(DownloadCommand))]
        [NotifyCanExecuteChangedFor(nameof(UpdateCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelDownloadCommand))]
        private JavaInfoDownload? _selectedJavaInfo;

        [ObservableProperty]
        private string? _downloadProgressString;

        [ObservableProperty]
        private int? _downloadProgressValue;

        [ObservableProperty]
        private string? _downloadFilesProgressString;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsDownloadedEnabled))]
        [NotifyCanExecuteChangedFor(nameof(DownloadCommand))]
        [NotifyCanExecuteChangedFor(nameof(UpdateCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelDownloadCommand))]
        private bool _isDownloading;
        public bool IsDownloadedEnabled => !IsDownloading;

        


        private CancellationTokenSource? _cts;

        public JavaDownloadViewModel(JavaManagerService javaManagerService)
        {
            _JavaManagerService = javaManagerService;
            LoadJavaList();
        }

        public void SetParent(JavaManagerWindowViewModel parent)
        {
            _parent = parent;
            UpdateParentStatus();
        }
        private void UpdateParentStatus()
        {
            if (_parent != null)
            {
                _parent.IsDownloadedEnabled = this.IsDownloadedEnabled;
            }
        }

        public async void LoadJavaList()
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

            DownloadProgressString = string.Empty;
            DownloadFilesProgressString = string.Empty;
            DownloadProgressValue = 0;
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

        [RelayCommand(CanExecute = nameof(CanUpload))]
        private async Task Download()
        {
            if (SelectedJava == null || SelectedJavaInfo == null) return;
            if (JavaList == null || JavaListInfo == null) return;


            DownloadProgressString = "Проверка...";
            DownloadFilesProgressString = "Файлы: 0/0";

            bool validate = await _JavaManagerService.ValidateDownloadedJava(SelectedJavaInfo.Name);
            if (validate)
            {
                DownloadProgressString = "Среда Java уже установлена!";
                DownloadFilesProgressString = "Успешно!";
                return;
            }


            IsDownloading = true;
            bool isSuccess = false;
            DownloadProgressValue = 0;
            int countDownloadedFiles = 0;
            _cts = new CancellationTokenSource();

            DownloadProgressString = "Подготовка к скачиванию...";
            DownloadFilesProgressString = "Файлы: 0/0";

            try
            {
                await Task.Run(async () =>
                {
                    if(SelectedJavaInfo == null) return;
                    var downloader = _JavaManagerService.JavaDownloader(SelectedJavaInfo.Name, _cts.Token)
                                    .ConfigureAwait(false);

                    await foreach (var currentProgress in downloader)
                    {
                        if (currentProgress.Progress == -1)
                        {
                            await Dispatcher.UIThread.InvokeAsync(() =>
                            {
                                DownloadProgressString = "Ошибка: файлы повреждены или не скачались. Попробуйте снова.";
                                DownloadProgressValue = 0;
                                isSuccess = false;
                            });
                            break;
                        }

                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            DownloadProgressValue = currentProgress.Progress;
                            int countDownloadedFiles = currentProgress.completedFiles;
                            DownloadProgressString = $"Скачивание и установка Java... {currentProgress.Progress}%";
                            DownloadFilesProgressString = $"Файлы: {countDownloadedFiles}/{currentProgress.AllFiles}";
                        });

                        if (currentProgress.Progress == 100) isSuccess = true;
                    }
                });
                if (isSuccess)
                {
                    DownloadProgressString = "Среда Java успешно установлена и проверена!";
                    DownloadFilesProgressString = "Готово!";
                    await _JavaManagerService.UpdateJavaInDb();
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

        [RelayCommand(CanExecute = nameof(CanUpdate))]
        private void Update()
        {
            LoadJavaList();
            LoadJavaInfoList();
        }

        [RelayCommand(CanExecute = nameof(CanDownloading))]
        private void CancelDownload()
        {
            if (!IsDownloading || _cts == null) return;
            _cts?.Cancel();
            DownloadProgressString = "Скачивание отменено пользователем.";
        }

        private bool CanDownloading() => IsDownloading;

        private bool CanUpload () => !IsDownloading && SelectedJavaInfo != null;

        private bool CanUpdate () => !IsDownloading;


        partial void OnSelectedJavaChanged(JavaManager? value)
        {
            if (value == null) return;
            LoadJavaInfoList();
            DownloadProgressString = string.Empty;
            DownloadFilesProgressString = string.Empty;
            DownloadProgressValue = 0;
        }

        partial void OnSelectedJavaInfoChanged(JavaInfoDownload? value)
        {
            if (value == null) return;
        }

        partial void OnIsDownloadingChanged(bool value)
        {
            UpdateParentStatus();
        }
    }
}
