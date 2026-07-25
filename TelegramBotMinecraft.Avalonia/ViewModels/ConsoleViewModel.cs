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


        [ObservableProperty]
        public string statusServer;

        [ObservableProperty]
        public string nameServer;

        [ObservableProperty]
        public TextDocument logsServer = new();

        [ObservableProperty]
        public TextDocument logsRcon = new();

        [ObservableProperty]
        public string textCommand;

        [ObservableProperty]
        private ServerStatusItemViewModel? _selectedItem;

        public ObservableCollection<ServerStatusItemViewModel> Servers { get; } = new();

        public Dictionary<string, string> LogsRconServers = new();

        public ConsoleViewModel(MinecraftServerManager minecraftServerManager, 
            ServerRepository serverRepository, 
            ServerStatusService serverStatusService, 
            ServerLogService serverLogService,
            ServerCommandService serverCommandService)
        {
            _MinecraftServerManager = minecraftServerManager;
            _ServerRepository = serverRepository;
            _ServerStatusService = serverStatusService;
            _ServerLogService = serverLogService;
            _ServerCommandService = serverCommandService;

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

        [RelayCommand]
        private async Task StartServer()
        {
            if (SelectedItem == null) return;
            await _MinecraftServerManager.StartServer(SelectedItem.Name);
        }

        [RelayCommand]
        private async Task StopServer()
        {
            if (SelectedItem == null) return;

            var ServerData = await _ServerRepository.GetServerByName(SelectedItem.Name);
            if (ServerData.IdProcess == -1) return;

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    title: "Подтверждение",
                    text: $"Вы уверены, что хотите остановить сервер {SelectedItem.Name}?",
                    ButtonEnum.YesNo,
                    Icon.Warning);

                var result = await box.ShowWindowDialogAsync(desktop.MainWindow);

                if (result == ButtonResult.Yes)
                {
                    await _MinecraftServerManager.StopServer(SelectedItem.Name);
                }
            }
        }

        [RelayCommand]
        private async Task SendCommand()
        {
            if (SelectedItem == null || string.IsNullOrWhiteSpace(TextCommand)) return;

            var currentCommand = TextCommand;
            var currentServerName = SelectedItem.Name;

            TextCommand = string.Empty;

            var response = await _ServerCommandService.SendCommandToServer(currentServerName, currentCommand);

            DateTime dateTime = DateTime.Now;
            string timeStr = dateTime.ToString("HH:mm:ss");

            string responseText = string.IsNullOrWhiteSpace(response) ? "Success" : response.Trim();
            string newLogEntry = $"[{timeStr}] Command: {currentCommand}\n[{timeStr}] Response: {responseText}\n";

            Dispatcher.UIThread.Post(() =>
            {
                if (SelectedItem?.Name == currentServerName)
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
                    if (SelectedItem != null && server.Name == SelectedItem.Name) _ = UpdateStatusServer(server.Name, server.Status);
                }
            }
        }

        partial void OnSelectedItemChanged(ServerStatusItemViewModel? value)
        {
            if (value == null) return;
            _ = UpdateStatusServer(value.Name);
            _ = UpdateServerLogsAsync(value.Name);
            UpdateLogsRconServerAsync(value.Name);
        }
    }
}
