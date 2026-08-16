using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;
using static TelegramBotMinecraft.Core.Models.ServerStatusModel;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class ServersViewModel : ObservableObject
    {
        private readonly ServerRepository _serverRepository;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;
        private readonly JavaManagerService _javaManagerService;
        private readonly IWindowService _windowService;
        private readonly MinecraftServerManager _minecraftServerManager;


        public ObservableCollection<Server> Servers { get; } = new();
        public ObservableCollection<JavaManager> Javas { get; } = new();


        [ObservableProperty]
        private Server? _editableServer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEditorEnabled))]
        [NotifyCanExecuteChangedFor(nameof(DeleteButtonCommand))]
        private Server? _selectedServer;

        [ObservableProperty]
        private JavaManager? _selectedJava;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEditorEnabled))]
        [NotifyCanExecuteChangedFor(nameof(DeleteButtonCommand))]
        private bool _isAddingNewServer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEditorEnabled))]
        [NotifyCanExecuteChangedFor(nameof(DeleteButtonCommand))]
        private string _statusServer = ServerStatus.Offline.ToString();

        [ObservableProperty]
        private string _textWhenServerIsRunning = string.Empty;

        public bool IsEditorEnabled =>
        (SelectedServer != null || IsAddingNewServer) &&
        (StatusServer == ServerStatus.Offline.ToString());

        public ServersViewModel(ServerRepository serverRepository, IDialogService dialogService, 
            INotificationService notificationService, JavaManagerService javaManagerService,
            IWindowService windowService, MinecraftServerManager minecraftServerManager)
        {
            _serverRepository = serverRepository;
            _dialogService = dialogService;
            _notificationService = notificationService;
            _javaManagerService = javaManagerService;
            _windowService = windowService;
            _minecraftServerManager = minecraftServerManager;

            _minecraftServerManager.ServerStatusChanged += OnServerStatusChanged;

            Task.Run(async () => await LoadServersAsync());
        }

        public async Task Reload()
        {
            await LoadServersAsync();
        }

        private async Task LoadServersAsync()
        {
            var serversNames = await _serverRepository.GetAllServersIdAndName();
            if (serversNames == null) return;

            EditableServer = new Server();
            Servers.Clear();

            foreach (var server in serversNames)
            {
                Servers.Add(new Server(server.Id, server.Name));
            }
            TextWhenServerIsRunning = string.Empty;
            StatusServer = null;
        }

        private async Task LoadSettingsServerAsync(string Name)
        {
            try
            {
                Server serverSettings = await _serverRepository.GetServerByName(Name);
                if (serverSettings == null) return;
                EditableServer = serverSettings;

                List<JavaManager> javaList = await _javaManagerService.GetAllDownloadedJava();
                if (javaList == null) return;

                Javas?.Clear();
                foreach (var java in javaList)
                {
                    Javas?.Add(java);
                }

                if (Javas != null && !string.IsNullOrEmpty(EditableServer.JavaName))
                {
                    SelectedJava = Javas.FirstOrDefault(j => j.Name == EditableServer.JavaName);
                }

                var status = await _minecraftServerManager.GetServerStatus(Name);
                StatusServer = status.ToString();

                if (status == ServerStatus.Online)
                {
                    TextWhenServerIsRunning = "Сервер активен, чтобы изменить настройки выключите его!";
                }
                else if (status == ServerStatus.Offline)
                {
                    TextWhenServerIsRunning = string.Empty;
                }
            }
            catch (Exception ex)
            {
                //($"Ошибка загрузки настроек сервера: {ex.Message}");
            }
        }

        [RelayCommand]
        private void AddButton()
        {
            SelectedServer = null;
            EditableServer = new Server();
            IsAddingNewServer = true;
            StatusServer = ServerStatus.Offline.ToString();
            TextWhenServerIsRunning = string.Empty;
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task DeleteButton()
        {
            if (SelectedServer == null) return;

            var result = await _dialogService.AskConfirmationAsync($"Вы уверены, что хотите удалить сервер {SelectedServer.Name}?");
            if (result != true) return;

            await _notificationService.ShowNotification("Удаление", $"Сервер [{SelectedServer.Name}] был успешно удален", "Warning");
            await _serverRepository.DeleteServer(SelectedServer.Id);

            Servers.Remove(SelectedServer);
            SelectedServer = null;

        }

        [RelayCommand]
        private async Task SaveButton()
        {
            if (EditableServer == null) return;
            if (SelectedJava != null) EditableServer.JavaName = SelectedJava.Name;

            if (IsAddingNewServer) 
            {
                await _notificationService.ShowNotification("Новый сервер", $"Сервер [{EditableServer.Name}] успешно добавлен в список", "Success");
                await _serverRepository.AddServer(EditableServer);
            }
            else
            {
                await _notificationService.ShowNotification("Настройки сервера", $"Изменения конфигурации сервера [{EditableServer.Name}] успешно сохранены", "Success");
                await _serverRepository.UpdateServer(EditableServer);
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
            else EditableServer = null;
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
            var result =  await _windowService.OpenJavaToServerArgumentManagement(EditableServer);
            if (result == null) return;

            if (EditableServer != null)
            {
                EditableServer.JavaArgs = result;
                var temp = EditableServer;
                EditableServer = null;
                EditableServer = temp;
            }
        }

        [RelayCommand]
        private async Task OpenJavaManagementWindow()
        {
            await _windowService.OpenJavaManagement();
        }

        private bool CanDelete() => SelectedServer != null && !IsAddingNewServer && StatusServer == ServerStatus.Offline.ToString();

        partial void OnSelectedServerChanged(Server? value)
        {
            if (value == null)
            {
                EditableServer = null;
                StatusServer = ServerStatus.Offline.ToString();
                TextWhenServerIsRunning = string.Empty;
                return;
            }
            TextWhenServerIsRunning = string.Empty;
            _ = LoadSettingsServerAsync(value.Name);
            
        }

        private void OnServerStatusChanged(string serverName, ServerStatus status)
        {
            if (SelectedServer != null && serverName == SelectedServer.Name)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    StatusServer = status.ToString();

                    TextWhenServerIsRunning = status == ServerStatus.Online
                    ? "Сервер активен, чтобы изменить настройки выключите его!"
                    : string.Empty;
                });
            }
        }

        public void Dispose()
        {
            _minecraftServerManager.ServerStatusChanged -= OnServerStatusChanged;
        }
    }
}