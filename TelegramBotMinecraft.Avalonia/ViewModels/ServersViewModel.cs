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
        private Server? _editableServer;

        [ObservableProperty]
        private Server? _selectedItem;

        private bool _addingServer;


        public ServersViewModel(ServerRepository serverRepository)
        {
            _ServerRepository = serverRepository;
            _ = LoadServersAsync();
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

        [RelayCommand]
        public async Task SaveButtonAsync()
        {
            if (SelectedItem == null) return;
            if (!_addingServer)
            {
                _ServerRepository?.UpdateServer(EditableServer);
            }
            else
            {
                _ServerRepository?.AddServer(EditableServer);
            }
            _addingServer = false;
            await LoadServersAsync();
        }

        [RelayCommand]
        public async Task CancelButtonAsync()
        {
            if (!_addingServer)
            {
                if (SelectedItem == null) return;
                await LoadSettingsServerAsync(SelectedItem.Name);
            }
            else
            {
                await LoadServersAsync();
            }
            _addingServer = false;

        }

        [RelayCommand]
        public async Task AddButton()
        {
            _addingServer = true;
            EditableServer = new Server();
            await LoadServersAsync();
        }

        partial void OnSelectedItemChanged(Server? value)
        {
            if (value == null) return;
            _ = LoadSettingsServerAsync(value.Name);
        }
    }
}