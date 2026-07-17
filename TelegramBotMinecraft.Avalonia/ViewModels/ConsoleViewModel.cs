using Avalonia.Automation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Database;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class ConsoleViewModel : ObservableObject
    {
        private readonly MinecraftServerManager _MinecraftServerManager;

        private readonly ServerRepository _ServerRepository;


        [ObservableProperty]
        public string statusServer;

        [ObservableProperty]
        public string nameServer;

        [ObservableProperty]
        private Server? _selectedItem;

        public ObservableCollection<Server> Servers { get; } = new();


        public ConsoleViewModel(MinecraftServerManager minecraftServerManager, ServerRepository serverRepository)
        {
            _MinecraftServerManager = minecraftServerManager;
            _ServerRepository = serverRepository;

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

        private async Task LoadSelectedNameServerAsync(string Name)
        {
            NameServer = Name;
        }



        [RelayCommand]
        private async Task StartServer()
        {
            StatusServer = "On";
        }

        [RelayCommand]
        private async Task StopServer()
        {
            StatusServer = "Off";
        }

        partial void OnSelectedItemChanged(Server? value)
        {
            if (value == null) return;
            _ = LoadSelectedNameServerAsync(value.Name);
        }
    }
}
