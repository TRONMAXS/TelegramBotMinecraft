using Avalonia.Automation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class ServersViewModel : ObservableObject
    {
        private readonly ServerRepository _ServerRepository;

        public ObservableCollection<Server> Servers { get; } = new();


        [ObservableProperty]
        public string _textName;
        [ObservableProperty]
        public string _textIpPort;
        [ObservableProperty]
        public string _textPathToServer;
        [ObservableProperty]
        public string _textArgJava;
        [ObservableProperty]
        public string _textRconPort;
        [ObservableProperty]
        public string _textRconPass;
        [ObservableProperty]
        public bool _checkRcon = false;

        [ObservableProperty]
        private Server? _selectedItem;

        public ServersViewModel(ServerRepository serverRepository)
        {
            _ServerRepository = serverRepository;

            _ = LoadServersAsync();
        }

        private async Task LoadServersAsync()
        {
            var serversNames = await _ServerRepository.GetAllServers();
            if (serversNames == null) return;

            foreach (var server in serversNames)
            {
                Servers.Add(new Server(server.Id, server.Name));
            }
        }

        private async Task LoadSettingsServerAsync(string Name)
        {
            var serverSettings = await _ServerRepository.GetServerByName(Name);
            if (serverSettings == null || serverSettings.Count == 0) return;

            var firstServer = serverSettings[0];

            TextName = firstServer.Name ?? string.Empty;
            TextIpPort = firstServer.Connected ?? string.Empty;
            TextPathToServer = firstServer.PathServer ?? string.Empty;
            TextArgJava = firstServer.JavaArgs ?? string.Empty;
            TextRconPort = firstServer.RconPort?.ToString() ?? string.Empty;
            TextRconPass = firstServer.RconPass?.ToString() ?? string.Empty;
            CheckRcon = Convert.ToBoolean(firstServer.RconEnable);
        }

        partial void OnSelectedItemChanged(Server? value)
        {
            if (value == null) return;
            _ = LoadSettingsServerAsync(value.Name);
        }
    }
}