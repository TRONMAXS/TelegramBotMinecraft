using Avalonia;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TelegramBotMinecraft.Avalonia.ViewModels.Items;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Services;
using static TelegramBotMinecraft.Core.Models.ServerStatusModel;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class ConsoleViewModel : ObservableObject
    {
        private readonly MinecraftServerManager _minecraftServerManager;
        private readonly ServerRepository _serverRepository;
        private readonly ServerLogService _serverLogService;
        private readonly ServerCommandService _serverCommandService;
        private readonly IDialogService _dialogService;
        private readonly INotificationService _notificationService;


        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsRconEnabled))]
        [NotifyCanExecuteChangedFor(nameof(StartServerCommand))]
        [NotifyCanExecuteChangedFor(nameof(StopServerCommand))]
        public string? statusServer;

        [ObservableProperty]
        public string? nameServer;

        [ObservableProperty]
        public TextDocument? logsServer = new();

        [ObservableProperty]
        public TextDocument? logsRcon = new();

        [ObservableProperty]
        public string? textCommand;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsLogsEnabled))]
        [NotifyPropertyChangedFor(nameof(IsRconEnabled))]
        [NotifyCanExecuteChangedFor(nameof(StartServerCommand))]
        [NotifyCanExecuteChangedFor(nameof(StopServerCommand))]
        private ServerStatusItemViewModel? _selectedServer;

        public bool IsLogsEnabled => SelectedServer != null;
        public bool IsRconEnabled => SelectedServer != null && SelectedServer.Status == "Online";


        public ObservableCollection<ServerStatusItemViewModel> Servers { get; } = new();

        public Dictionary<string, string> LogsRconServers = new();

        public ConsoleViewModel(MinecraftServerManager minecraftServerManager, 
            ServerRepository serverRepository, 
            ServerLogService serverLogService,
            ServerCommandService serverCommandService,
            IDialogService dialogService,
            INotificationService notificationService)
        {
            _minecraftServerManager = minecraftServerManager;
            _serverRepository = serverRepository;
            _serverLogService = serverLogService;
            _serverCommandService = serverCommandService;
            _dialogService = dialogService;
            _notificationService = notificationService;

            _minecraftServerManager.ServerStatusChanged += OnServerStatusChanged;
            _ = LoadServersAsync();
        }

        private async Task LoadServersAsync()
        {
            var serversNames = await _serverRepository.GetAllServersIdAndName();
            if (serversNames == null) return;

            foreach (var server in serversNames)
            {
                var currentStatus = _minecraftServerManager.GetServerStatus(server.Name).ToString();
                Servers.Add(new ServerStatusItemViewModel(server.Id, server.Name) { Status = currentStatus });
            }
        }

        [RelayCommand(CanExecute = nameof(CanStartServer))]
        private async Task StartServer()
        {
            if (SelectedServer == null) return;
            await _notificationService.ShowNotification("Запуск сервера", $"Сервер [{SelectedServer.Name}] запускается...", "Information");
            await _minecraftServerManager.StartServer(SelectedServer.Name);
        }

        [RelayCommand(CanExecute = nameof(CanStopServer))]
        private async Task StopServer()
        {
            if (SelectedServer == null) return;

            var ServerData = await _serverRepository.GetServerByName(SelectedServer.Name);
            if (ServerData.IdProcess == -1) return;

            var result = await _dialogService.AskConfirmationAsync($"Вы уверены, что хотите остановить сервер {SelectedServer.Name}?");

            if (result == true) 
            {
                await _minecraftServerManager.StopServer(SelectedServer.Name);
                await _notificationService.ShowNotification("Остановка сервера", $"Сервер [{SelectedServer.Name}] останавливается...", "Warning");
            }
        }

        [RelayCommand]
        private async Task SendCommand()
        {
            if (SelectedServer == null || string.IsNullOrWhiteSpace(TextCommand)) return;

            var currentCommand = TextCommand;
            var currentServerName = SelectedServer.Name;

            TextCommand = string.Empty;

            var response = await _serverCommandService.SendCommandToServer(currentServerName, currentCommand);

            DateTime dateTime = DateTime.Now;
            string timeStr = dateTime.ToString("HH:mm:ss");

            string responseText = string.IsNullOrWhiteSpace(response) ? "Success" : response.Trim();
            string newLogEntry = $"[{timeStr}] Command: {currentCommand}\n[{timeStr}] Response: {responseText}\n";

            Dispatcher.UIThread.Post(() =>
            {
                if (SelectedServer?.Name == currentServerName)
                {
                    LogsRcon.Insert(LogsRcon.TextLength, newLogEntry);
                }
            });

            if (LogsRconServers.TryGetValue(currentServerName, out var currentLogs))
            {
                LogsRconServers[currentServerName] = currentLogs + newLogEntry;
            }
            else
            {
                LogsRconServers[currentServerName] = newLogEntry;
            }
        }

        private async Task UpdateServerLogsAsync(string Name)
        {
            LogsServer.Text = string.Empty;

            using var cts = new CancellationTokenSource();

            await foreach (var logLine in _serverLogService.UpdateConsoleServer(Name, cts.Token))
            {
                if (logLine == null) continue;
                Dispatcher.UIThread.Post(() =>
                {
                    if (logLine == "Console log clear")
                    {
                        LogsServer.Text = string.Empty;
                    }
                    else
                    {
                        LogsServer.Insert(LogsServer.TextLength, logLine);
                    }
                    
                });
            }
        }

        private void UpdateLogsRconServerAsync(string Name)
        {
            LogsRcon.Text = string.Empty;
            if (LogsRconServers.TryGetValue(Name, out var value))
            {
                LogsRcon.Insert(LogsRcon.TextLength, value);
            }
        }


        private bool CanStartServer()
        {
            if (SelectedServer != null && SelectedServer.Status == "Starting") return false;
            return SelectedServer != null && SelectedServer.Status != "Online";
        }

        private bool CanStopServer()
        {
            if (SelectedServer != null && SelectedServer.Status == "Starting") return false;
            return SelectedServer != null && SelectedServer.Status != "Offline";
        }

        private void OnServerStatusChanged(string serverName, ServerStatus status)
        {
            Dispatcher.UIThread.Post(() =>
            {
                var server = Servers.FirstOrDefault(x => x.Name == serverName);
                if (server != null)
                {
                    server.Status = status.ToString();
                    if (SelectedServer != null)
                    {
                        NameServer = serverName;
                        StatusServer = status.ToString();
                    }
                }
            });
        }

        partial void OnSelectedServerChanged(ServerStatusItemViewModel? value)
        {
            if (value == null) return;
            NameServer = value.Name;
            StatusServer = value.Status;

            _ = UpdateServerLogsAsync(value.Name);
            UpdateLogsRconServerAsync(value.Name);
        }

        public void Dispose()
        {
            _minecraftServerManager.ServerStatusChanged -= OnServerStatusChanged;
        }
    }
}
