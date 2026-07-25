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


        public ServersViewModel(ServerRepository serverRepository)
        {
            _ServerRepository = serverRepository;
            _ = LoadServersAsync();
        }

        private async Task LoadServersAsync()
        {
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
        public async Task SaveButton()
        {
            if (SelectedItem?.Name == null) return;
            _ServerRepository?.UpdateServer(EditableServer);
            // если кнопка добавить активна то сервер добавляется, а не обновляется
        }

        partial void OnSelectedItemChanged(Server? value)
        {
            if (value == null) return;
            _ = LoadSettingsServerAsync(value.Name);
        }
    }
}