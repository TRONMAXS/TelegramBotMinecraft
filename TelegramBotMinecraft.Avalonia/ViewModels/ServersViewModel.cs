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
    public class ServersViewModel : ObservableObject
    {
        private readonly ServerRepository _ServerRepository;

        public ObservableCollection<Server> Servers { get; } = new();

        public ServersViewModel(ServerRepository serverRepository)
        {
            _ServerRepository = serverRepository;

            _ = LoadServersAsync();
        }

        private async Task LoadServersAsync()
        {
            var serversNames = await _ServerRepository.GetAllServersNames();
            if (serversNames == null) return;

            foreach (var server in serversNames)
            {
                Servers.Add(new Server(0, server));
            }
        }
    }
}