using Avalonia;
using Avalonia.Automation;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
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
using TelegramBotMinecraft.Avalonia.Services;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Models;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class ServersViewModel : ObservableObject
    {
        private readonly ServerRepository _ServerRepository;

        private readonly IDialogService _dialogService;

        public ObservableCollection<Server> Servers { get; } = new();


        [ObservableProperty]
        private Server? _editableServer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEditorEnabled))]
        [NotifyCanExecuteChangedFor(nameof(DeleteButtonCommand))]
        private Server? _selectedServer;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEditorEnabled))]
        [NotifyCanExecuteChangedFor(nameof(DeleteButtonCommand))]
        private bool _isAddingNewServer;

        public bool IsEditorEnabled => SelectedServer != null || IsAddingNewServer;

        public ServersViewModel(ServerRepository serverRepository, IDialogService dialogService)
        {
            _ServerRepository = serverRepository;
            _dialogService = dialogService;
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
        private void AddButton()
        {
            SelectedServer = null;
            EditableServer = new Server();
            IsAddingNewServer = true;
        }

        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task DeleteButton()
        {
            if (SelectedServer == null) return;

            var result = await _dialogService.AskConfirmationAsync($"Вы уверены, что хотите удалить сервер {SelectedServer.Name}?");


            if (result == true)
            {
                await _ServerRepository.DeleteServer(SelectedServer.Id);

                SelectedServer = null;
                await LoadServersAsync();
            }
        }

        [RelayCommand]
        private async Task SaveButton()
        {
            if (EditableServer == null) return;

            if (IsAddingNewServer) await _ServerRepository.AddServer(EditableServer);
            else await _ServerRepository.UpdateServer(EditableServer);

            IsAddingNewServer = false;
            SelectedServer = null;
            await LoadServersAsync();
        }

        [RelayCommand]
        private async Task CancelButton()
        {
            IsAddingNewServer = false;

            if (SelectedServer != null) await LoadSettingsServerAsync(SelectedServer.Name);
            else EditableServer = new Server();
        }

        [RelayCommand]
        private async Task SelectButton()
        {
            var result = await _dialogService.SelectFolderAsync($"Путь к папке сервера");
            if (result == null) return;

            if (EditableServer != null)
            {
                EditableServer.PathServer = result;

                var temp = EditableServer;
                EditableServer = null;
                EditableServer = temp;
            }
        }


        private bool CanDelete() => SelectedServer != null && !IsAddingNewServer;

        partial void OnSelectedServerChanged(Server? value)
        {
            if (value == null) return;
            _ = LoadSettingsServerAsync(value.Name);
        }
    }
}