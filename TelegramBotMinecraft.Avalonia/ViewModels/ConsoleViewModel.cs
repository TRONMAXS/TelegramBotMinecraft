using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Snippets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoreRCON.Parsers.Standard;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using TelegramBotMinecraft.Avalonia.ViewModels.Items;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;
using static TelegramBotMinecraft.Core.Models.ServerStatusModel;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class ConsoleViewModel : ObservableObject
    {
        private readonly MinecraftServerManager _MinecraftServerManager;
        private readonly ServerRepository _ServerRepository;
        private readonly ServerStatusService _ServerStatusService;
        private readonly ServerLogService _ServerLogService;
        private readonly ServerCommandService _ServerCommandService;
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
            ServerStatusService serverStatusService, 
            ServerLogService serverLogService,
            ServerCommandService serverCommandService,
            IDialogService dialogService,
            INotificationService notificationService)
        {
            _MinecraftServerManager = minecraftServerManager;
            _ServerRepository = serverRepository;
            _ServerStatusService = serverStatusService;
            _ServerLogService = serverLogService;
            _ServerCommandService = serverCommandService;
            _dialogService = dialogService;
            _notificationService = notificationService;

            _ = LoadServersAsync();
            _ = MonitorServersAsync();
        }

        private async Task LoadServersAsync()
        {
            var serversNames = await _ServerRepository.GetAllServersIdAndName();
            if (serversNames == null) return;

            foreach (var server in serversNames)
            {
                Servers.Add(new ServerStatusItemViewModel(server.Id, server.Name));
            }
        }

        [RelayCommand(CanExecute = nameof(CanStartServer))]
        private async Task StartServer()
        {
            if (SelectedServer == null) return;
            await _notificationService.ShowNotification("Запуск сервера", $"Сервер [{SelectedServer.Name}] запускается...", "Information");
            await _MinecraftServerManager.StartServer(SelectedServer.Name);
        }

        [RelayCommand(CanExecute = nameof(CanStopServer))]
        private async Task StopServer()
        {
            if (SelectedServer == null) return;

            var ServerData = await _ServerRepository.GetServerByName(SelectedServer.Name);
            if (ServerData.IdProcess == -1) return;

            var result = await _dialogService.AskConfirmationAsync($"Вы уверены, что хотите остановить сервер {SelectedServer.Name}?");

            if (result == true) 
            {
                await _notificationService.ShowNotification("Остановка сервера", $"Сервер [{SelectedServer.Name}] останавливается...", "Warning");
                await _MinecraftServerManager.StopServer(SelectedServer.Name);
            }
        }

        [RelayCommand]
        private async Task SendCommand()
        {
            if (SelectedServer == null || string.IsNullOrWhiteSpace(TextCommand)) return;

            var currentCommand = TextCommand;
            var currentServerName = SelectedServer.Name;

            TextCommand = string.Empty;

            var response = await _ServerCommandService.SendCommandToServer(currentServerName, currentCommand);

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

        private async Task UpdateStatusServer(string Name, string? status = null)
        {
            NameServer = Name;
            var item = Servers.FirstOrDefault(x => x.Name == Name);
            StatusServer = item.Status;
        }

        private async Task MonitorServersAsync()
        {
            while (true)
            {
                var statuses = await _ServerStatusService.CheckAllServers();

                UpdateServers(statuses);

                await Task.Delay(2000);
            }
        }

        private async Task UpdateServerLogsAsync(string Name)
        {
            LogsServer.Text = string.Empty;

            using var cts = new CancellationTokenSource();

            await foreach (var logLine in _ServerLogService.UpdateConsoleServer(Name, cts.Token))
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

        private void UpdateServers(List<ServerStatusInfo> statuses)
        {
            foreach (var status in statuses)
            {
                foreach (var server in Servers)
                {
                    if (server.Name != status.Server) continue;
                    switch (status.Status)
                    {
                        case ServerStatus.Online:
                            server.Status = "Online";
                            break;
                        case ServerStatus.Offline:
                            server.Status = "Offline";
                            break;
                        case ServerStatus.Starting:
                            server.Status = "Starting";
                            break;
                        case ServerStatus.Stopping:
                            server.Status = "Stopping";
                            break;
                        case ServerStatus.Warning:
                            server.Status = "Warning";
                            break;
                    }
                    if (SelectedServer != null && server.Name == SelectedServer.Name) _ = UpdateStatusServer(server.Name, server.Status);
                }
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


        partial void OnSelectedServerChanged(ServerStatusItemViewModel? value)
        {
            if (value == null) return;
            _ = UpdateStatusServer(value.Name);
            _ = UpdateServerLogsAsync(value.Name);
            UpdateLogsRconServerAsync(value.Name);
        }
    }
}
