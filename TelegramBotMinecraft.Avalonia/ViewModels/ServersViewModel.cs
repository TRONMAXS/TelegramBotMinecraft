using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TelegramBotMinecraft.Avalonia.Services;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class ServersViewModel : ObservableObject
    {
        private readonly ServerRepository _ServerRepository;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;
        private readonly JavaManagerService? _JavaManagerService;
        private readonly IWindowService _windowService;


        public ObservableCollection<Server> Servers { get; } = new();
        public ObservableCollection<string> Javas { get; } = new();


        [ObservableProperty]
        private Server? _editableServer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEditorEnabled))]
        [NotifyCanExecuteChangedFor(nameof(DeleteButtonCommand))]
        private Server? _selectedServer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEditorEnabled))]
        [NotifyCanExecuteChangedFor(nameof(DeleteButtonCommand))]
        private bool _isAddingNewServer;

        public bool IsEditorEnabled => SelectedServer != null || IsAddingNewServer;

        public ServersViewModel(ServerRepository serverRepository, IDialogService dialogService, 
            INotificationService notificationService, JavaManagerService? javaManagerService,
            IWindowService windowService)
        {
            _ServerRepository = serverRepository;
            _dialogService = dialogService;
            _notificationService = notificationService;
            _JavaManagerService = javaManagerService;
            _windowService = windowService;

            _ = LoadServersAsync();
            LoadJavaDownloadedList();
        }

        private async Task LoadServersAsync()
        {
            Servers.Clear();
            EditableServer = new Server();
            var serversNames = await _ServerRepository.GetAllServersIdAndName();
            if (serversNames == null) return;

            foreach (var server in serversNames)
            {
                Servers.Add(new Server(server.Id, server.Name));
            }
        }

        private async Task LoadSettingsServerAsync(string Name)
        {
            Server serverSettings = await _ServerRepository.GetServerByName(Name);
            if (serverSettings != null) EditableServer = serverSettings;
        }

        private async void LoadJavaDownloadedList()
        {
            List<JavaManager> javaList = await _JavaManagerService.GetAllDownloadedJava();
            if (javaList == null && javaList.Count > 0) return;

            Javas?.Clear();

            foreach (var java in javaList)
            {
                Javas?.Add(java.Version);
            }
        }

        [RelayCommand]
        private void AddButton()
        {
            SelectedServer = null;
            EditableServer = new Server();
            IsAddingNewServer = true;
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task DeleteButton()
        {
            if (SelectedServer == null) return;

            var result = await _dialogService.AskConfirmationAsync($"Вы уверены, что хотите удалить сервер {SelectedServer.Name}?");

            if (result == true)
            {
                await _notificationService.ShowNotification("Удаление", $"Сервер [{SelectedServer.Name}] был успешно удален", "Warning");

                await _ServerRepository.DeleteServer(SelectedServer.Id);

                SelectedServer = null;
                await LoadServersAsync();
            }
        }

        [RelayCommand]
        private async Task SaveButton()
        {
            if (EditableServer == null) return;

            if (IsAddingNewServer) 
            {
                await _notificationService.ShowNotification("Новый сервер", $"Сервер [{EditableServer.Name}] успешно добавлен в список", "Success");
                await _ServerRepository.AddServer(EditableServer);
            }
            else
            {
                await _notificationService.ShowNotification("Настройки сервера", $"Изменения конфигурации сервера [{EditableServer.Name}] успешно сохранены", "Success");
                await _ServerRepository.UpdateServer(EditableServer);
            }

            IsAddingNewServer = false;
            SelectedServer = null;
            await LoadServersAsync();
        }

        [RelayCommand]
        private async Task CancelButton()
        {
            IsAddingNewServer = false;

            if (SelectedServer != null) await LoadSettingsServerAsync(SelectedServer.Name);
            else EditableServer = new Server();
        }

        [RelayCommand]
        private async Task SelectButton()
        {
            var result = await _dialogService.SelectFolderAsync($"Путь к папке сервера");
            if (result == null) return;

            if (EditableServer != null)
            {
                await _notificationService.ShowNotification("Путь к серверу", $"Корневая папка сервера успешно привязана", "Success");

                EditableServer.PathServer = result;
                var temp = EditableServer;
                EditableServer = null;
                EditableServer = temp;
            }
        }

        [RelayCommand]
        private async Task OpenJavaArgumentsWindow()
        {
            if(EditableServer == null) return;
            await _windowService.OpenJavaToServerArgumentManagement(EditableServer);

            await LoadSettingsServerAsync(EditableServer.Name);
        }


        private bool CanDelete() => SelectedServer != null && !IsAddingNewServer;

        partial void OnSelectedServerChanged(Server? value)
        {
            if (value == null) return;
            _ = LoadSettingsServerAsync(value.Name);
        }
    }
}