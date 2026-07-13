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
    public class UsersViewModel : ObservableObject
    {
        private readonly ServerRepository _ServerRepository;
        private readonly UserRepository _UserRepository;
        private readonly CommandRepository _CommandRepository;

        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<Server> Servers { get; } = new();
        public ObservableCollection<Command> Commands { get; } = new();


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
            var serversNames = await _ServerRepository.GetAllServersNames();
            if (serversNames == null) return;

            foreach (var server in serversNames)
            {
                Servers.Add(new Server(0, server));
            }
        }
        private async Task LoadUsersAsync()
        {
            var usersNames = await _UserRepository.GetAllUserNamesAndId();
            if (usersNames == null) return;

            foreach (var user in usersNames)
            {
                Users.Add(new User(user.Name, user.Id));
            }
        }
        private async Task LoadCommandsAsync()
        {
            var commandsNames = await _CommandRepository.GetAllCommand();
            if (commandsNames == null) return;

            foreach (var command in commandsNames)
            {
                Commands.Add(new Command(0, command));
            }
        }
    }
}
