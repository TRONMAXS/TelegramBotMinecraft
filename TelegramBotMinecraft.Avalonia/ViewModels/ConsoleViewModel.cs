using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;

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
        private Server? _selectedItem;

        public ObservableCollection<Server> Servers { get; } = new();


        public ConsoleViewModel(MinecraftServerManager minecraftServerManager, ServerRepository serverRepository, ServerStatusService serverStatusService)
        {
            _MinecraftServerManager = minecraftServerManager;
            _ServerRepository = serverRepository;
            _ServerStatusService = serverStatusService;

            _ = LoadServersAsync();
        }

        private async Task LoadServersAsync()
        {
            var serversNames = await _ServerRepository.GetAllServers();
            if (serversNames == null) return;

            int i = 0;
            foreach (var server in serversNames)
            {
                Servers.Add(new Server(server.Id, server.Name));
                i++;
            }
        }

        private async Task LoadSelectedInfoServerAsync(string Name)
        {
            NameServer = Name;

            var status = await _ServerStatusService.GetStatus(Name);
            StatusServer = status.ToString();
        }



        [RelayCommand]
        private async Task StartServer()
        {
            if (SelectedItem == null) return;

            bool status = await _MinecraftServerManager.StartServer(SelectedItem.Name);

            /*if (status == true) StatusServer = "Запущен";
            else if (status == false) StatusServer = "Ошибка включения";*/
            await UpdateStatusServer(SelectedItem.Name);
        }

        [RelayCommand]
        private async Task StopServer()
        {
            if (SelectedItem == null) return;

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

                    /*if (status == true) StatusServer = "Остановлен";
                    else if (status == false) StatusServer = "Ошибка выключения";*/
                }
                await UpdateStatusServer(SelectedItem.Name);
            }
        }

        private async Task UpdateStatusServer(string Name)
        {
            var status = await _ServerStatusService.GetStatus(Name);
            StatusServer = status.ToString();
        }

        partial void OnSelectedItemChanged(Server? value)
        {
            if (value == null) return;
            _ = LoadSelectedInfoServerAsync(value.Name);
        }
    }
}
