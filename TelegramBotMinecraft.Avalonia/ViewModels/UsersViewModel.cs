using Avalonia.Automation;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class UsersViewModel : ObservableObject
    {

        private readonly ServerRepository _ServerRepository;
        private readonly UserRepository _UserRepository;
        private readonly CommandRepository _CommandRepository;
        private readonly IDialogService _dialogService;


        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<ServerItemViewModel> Servers { get; } = new();
        public ObservableCollection<CommandItemViewModel> Commands { get; } = new();

        [ObservableProperty]
        private User? _selectedItem;

        [ObservableProperty]
        private string? _userName;

        [ObservableProperty]
        private int? _userId;

        private int? UserOldId;

        public UsersViewModel(ServerRepository serverRepository, UserRepository userRepository, 
            CommandRepository commandRepository, IDialogService dialogService)
        {
            _ServerRepository = serverRepository;
            _UserRepository = userRepository;
            _CommandRepository = commandRepository;
            _dialogService = dialogService;

            _ = LoadServersAsync();
            _ = LoadUsersAsync();
            _ = LoadCommandsAsync();
        }

        private async Task LoadServersAsync()
        {
            var servers = await _ServerRepository.GetAllServersIdAndName();
            if (servers == null) return;

            foreach (var server in servers)
            {
                Servers.Add(new ServerItemViewModel(server.Id, server.Name));
            }
        }
        private async Task LoadUsersAsync()
        {
            var users = await _UserRepository.GetAllUserNamesAndId();
            if (users == null) return;

            SelectedItem = null;

            Users.Clear();

            foreach (var user in users)
            {
                Users.Add(new User(user.Name, user.Id));
            }
        }
        private async Task LoadCommandsAsync()
        {
            var commands = await _CommandRepository.GetAllCommandAsync();
            if (commands == null) return;

            foreach (var command in commands)
            {
                Commands.Add(new CommandItemViewModel(command.Id, command.CommandText));
            }
        }
        private async Task LoadPermissionsUserAsync(int userId)
        {
            if (SelectedItem == null) return;

            var userServers = await _ServerRepository.GetServersByUserIdAsync(userId);
            var userCommands = await _CommandRepository.GetCommandsByUserIdAsync(userId);
            if (userServers == null) return;
            if (userCommands == null) return;

            foreach (var server in Servers)
            {
                server.IsChecked = false;
            }
            foreach (var command in Commands)
            {
                command.IsChecked = false;
            }

            foreach (var server in userServers)
            {
                var item = Servers.FirstOrDefault(x => x.Id == server.Id);

                if (item != null)
                    item.IsChecked = true;
            }

            foreach (var command in userCommands)
            {
                var item = Commands.FirstOrDefault(x => x.Id == command.Id);

                if (item != null)
                    item.IsChecked = true;
            }
        }

        [RelayCommand]
        private async Task AddUser()
        {
            if(UserName == null && UserId == null) return;

            await _UserRepository.AddUser(UserName, UserId);

            UserName = null;
            UserId = null;

            await LoadUsersAsync();
        }

        [RelayCommand]
        private async Task DeleteUser()
        {
            if (SelectedItem == null) return;

            var result = await _dialogService.AskConfirmationAsync($"Вы уверены, что хотите удалить пользователя {SelectedItem.Name} : {SelectedItem.Id}?");

            if (result == true)
            {
                await _UserRepository.DeleteUser(SelectedItem.Id);
                await LoadUsersAsync();
            }
        }

        [RelayCommand]
        private void EditUser()
        {
            if (SelectedItem == null) return;
            UserName = SelectedItem.Name;
            UserId = SelectedItem.Id;
            UserOldId = SelectedItem.Id;
        }

        [RelayCommand]
        private async Task SaveUser()
        {
            await _UserRepository.UpdateUser(UserName, UserId, UserOldId);

            UserName = null;
            UserId = null;
            UserOldId = null;

            await LoadUsersAsync();
        }

        [RelayCommand]
        private async Task SavePermissions()
        {
            if(SelectedItem == null) return;

            List<int> selectedServers = Servers
                                                .Where(s => s.IsChecked)
                                                .Select(s => s.Id)
                                                .ToList();

            List<int> selectedCommands = Commands
                                                .Where(c => c.IsChecked)
                                                .Select(c => c.Id)
                                                .ToList();

            await _ServerRepository.SaveUserServersAsync(SelectedItem.Id, selectedServers);       
            await _CommandRepository.SaveUserCommandsAsync(SelectedItem.Id, selectedCommands);

            SelectedItem = null;
        }

        [RelayCommand]
        private void DisableAllServers()
        {
            foreach (var server in Servers)
            {
                server.IsChecked = false;
            }
        }

        [RelayCommand]
        private void EnableAllServers()
        {
            foreach (var server in Servers)
            {
                server.IsChecked = true;
            }
        }

        [RelayCommand]
        private void DisableAllCommands()
        {
            foreach (var command in Commands)
            {
                command.IsChecked = false;
            }
        }

        [RelayCommand]
        private void EnableAllCommands()
        {
            foreach (var command in Commands)
            {
                command.IsChecked = true;
            }
        }

        partial void OnSelectedItemChanged(User? value)
        {
            if (value == null) return;
            _ = LoadPermissionsUserAsync(value.Id);
        }
    }
}
