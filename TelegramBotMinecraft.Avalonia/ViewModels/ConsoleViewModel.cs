using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
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
using System.Threading.Tasks;
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


        [ObservableProperty]
        public string statusServer;

        [ObservableProperty]
        public string nameServer;

        [ObservableProperty]
        private ServerStatusItemViewModel? _selectedItem;

        public ObservableCollection<ServerStatusItemViewModel> Servers { get; } = new();


        public ConsoleViewModel(MinecraftServerManager minecraftServerManager, ServerRepository serverRepository, ServerStatusService serverStatusService)
        {
            _MinecraftServerManager = minecraftServerManager;
            _ServerRepository = serverRepository;
            _ServerStatusService = serverStatusService;

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

            bool status = await _MinecraftServerManager.StartServer(SelectedItem.Name);

            /*if (status == true) await UpdateStatusServer(SelectedItem.Name, ServerStatus.Starting);
            else if (status == false) await UpdateStatusServer(SelectedItem.Name, ServerStatus.Offline);*/
        }

        [RelayCommand]
        private async Task StopServer()
        {
            if (SelectedItem == null) return;

            var ServerData = await _ServerRepository.GetServerByName(SelectedItem.Name);
            var Data = ServerData[0];
            if (Data.IdProcess == -1) return;

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
                    bool status = await _MinecraftServerManager.StopServer(SelectedItem.Name);

                    /*if (status == true) await UpdateStatusServer(SelectedItem.Name, ServerStatus.Stopping);
                    else if (status == false) await UpdateStatusServer(SelectedItem.Name, ServerStatus.Offline);*/
                }
            }
        }

        private async Task UpdateStatusServer(string Name, string status = null)
        {
            NameServer = Name;
            var item = Servers.FirstOrDefault(x => x.Name == Name);
            StatusServer = item.Status;
            /*var status = await _ServerStatusService.GetStatus(Name, serverStatus);
            StatusServer = status.ToString();*/
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
        }
    }
}
