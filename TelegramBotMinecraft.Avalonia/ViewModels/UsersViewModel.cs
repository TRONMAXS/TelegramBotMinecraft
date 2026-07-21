using Avalonia.Automation;
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

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public partial class UsersViewModel : ObservableObject
    {

        private readonly ServerRepository _ServerRepository;
        private readonly UserRepository _UserRepository;
        private readonly CommandRepository _CommandRepository;

        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<ServerItemViewModel> Servers { get; } = new();
        public ObservableCollection<CommandItemViewModel> Commands { get; } = new();


        [ObservableProperty]
        private User? _selectedItem;

        public UsersViewModel(ServerRepository serverRepository, UserRepository userRepository, CommandRepository commandRepository)
        {
            _ServerRepository = serverRepository;
            _UserRepository = userRepository;
            _CommandRepository = commandRepository;

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
            var userServers = await _ServerRepository.GetServersByUserIdAsync(userId);
            var userCommands = await _CommandRepository.GetCommandsByUserIdAsync(userId);
            if (userServers == null || userServers.Count == 0) return;
            if (userCommands == null || userCommands.Count == 0) return;

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

        partial void OnSelectedItemChanged(User? value)
        {
            if (value == null) return;
            _ = LoadPermissionsUserAsync(value.Id);
        }
    }
}
