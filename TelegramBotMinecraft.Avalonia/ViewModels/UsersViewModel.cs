using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly INotificationService _notificationService;


        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<ServerItemViewModel> Servers { get; } = new();
        public ObservableCollection<CommandItemViewModel> Commands { get; } = new();

        [ObservableProperty]
        private User? _selectedUser;

        [ObservableProperty]
        private bool _isEditingUser;

        public bool IsPermissionsEnabled => SelectedUser != null && !IsEditingUser;
        public bool IsListUsersEnabled => !IsEditingUser;

        [ObservableProperty]
        private string? _userName;

        [ObservableProperty]
        private long? _userId;

        private long? _userOldId;

        public UsersViewModel(ServerRepository serverRepository, UserRepository userRepository, 
            CommandRepository commandRepository, IDialogService dialogService, INotificationService notificationService)
        {
            _ServerRepository = serverRepository;
            _UserRepository = userRepository;
            _CommandRepository = commandRepository;
            _dialogService = dialogService;
            _notificationService = notificationService;

            _ = LoadServersAsync();
            _ = LoadUsersAsync();
            _ = LoadCommandsAsync();
        }

        public async Task Reload()
        {
            await LoadServersAsync();
            await LoadUsersAsync();
            await LoadCommandsAsync();
        }

        private async Task LoadServersAsync()
        {
            var servers = await _ServerRepository.GetAllServersIdAndName();
            if (servers == null) return;

            Servers.Clear();

            foreach (var server in servers)
            {
                Servers.Add(new ServerItemViewModel(server.Id, server.Name));
            }
        }

        private async Task LoadUsersAsync()
        {
            var users = await _UserRepository.GetAllUserNamesAndId();
            if (users == null) return;

            SelectedUser = null;

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

            Commands.Clear();

            foreach (var command in commands)
            {
                Commands.Add(new CommandItemViewModel(command.Id, command.CommandText));
            }
        }

        private async Task LoadPermissionsUserAsync(long userId)
        {
            if (SelectedUser == null) return;

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


        [RelayCommand(CanExecute = nameof(CanAdd))]
        private async Task AddUser()
        {
            if(UserName != null) UserName = UserName.Trim();

            if (string.IsNullOrWhiteSpace(UserName) || UserId == null)
            {
                await _notificationService.ShowNotification("Ошибка при сохранении пользователя",
                    $"Имя или ID пользователя не могут быть пустыми.",
                    "Error");
                return;
            }

            try
            {
                await _UserRepository.AddUser(UserName, (long)UserId);

                await _notificationService.ShowNotification("Пользователь добавлен", 
                    $"Пользователь [{UserName}] (ID: [{UserId}]) успешно добавлен", 
                    "Success");

                UserName = null;
                UserId = null;

                await Reload();
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 19 || ex.SqliteExtendedErrorCode == 2067)
            {
                await _notificationService.ShowNotification($"Ошибка при сохранении пользователя", 
                    "Пользователь с таким Telegram ID уже зарегистрирован. Пожалуйста, укажите другой id.", 
                    "Error");
            }
            catch (Exception ex)
            {
                await _notificationService.ShowNotification(
                    "Непредвиденная ошибка",
                    $"Что-то пошло не так: {ex.Message}",
                    "Error"
                );
            }
        }


        [RelayCommand(CanExecute = nameof(CanDelete))]
        private async Task DeleteUser()
        {
            if (SelectedUser == null) return;

            var result = await _dialogService.AskConfirmationAsync($"Вы уверены, что хотите удалить пользователя [{SelectedUser.Name}]:[{SelectedUser.Id}]?");

            if (result == true)
            {
                await _notificationService.ShowNotification("Пользователь удален", $"Пользователь [{SelectedUser.Name}] успешно удален", "Warning");

                await _UserRepository.DeleteUser(SelectedUser.Id);

                await LoadUsersAsync();
                await RefreshListServersAndCommads();
            }
        }


        [RelayCommand(CanExecute = nameof(CanNotEdit))]
        private void EditUser()
        {
            if (SelectedUser == null) return;
            IsEditingUser = true;

            UserName = SelectedUser.Name;
            UserId = SelectedUser.Id;
            _userOldId = SelectedUser.Id;
        }


        [RelayCommand(CanExecute = nameof(CanEdit))]
        private async Task SaveUser()
        {
            if (UserName != null) UserName = UserName.Trim();

            if (string.IsNullOrWhiteSpace(UserName) || UserId == null || _userOldId == null)
            {
                await _notificationService.ShowNotification("Ошибка при сохранении пользователя",
                    $"Имя или ID пользователя не могут быть пустыми.",
                    "Error");
                return;
            }

            try
            {
                await _UserRepository.UpdateUser(UserName, (long)UserId, (long)_userOldId);

                await _notificationService.ShowNotification("Пользователь обновлен", 
                    $"Пользователь [{UserName}] успешно обновлен", 
                    "Success");

                UserName = null;
                UserId = null;
                _userOldId = null;
                IsEditingUser = false;

                await LoadUsersAsync();
                await RefreshListServersAndCommads();
            }
            catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 19 || ex.SqliteExtendedErrorCode == 2067)
            {
                await _notificationService.ShowNotification($"Ошибка при сохранении пользователя",
                    "Пользователь с таким Telegram ID уже зарегистрирован. Пожалуйста, укажите другой id.", 
                    "Error");
            }
            catch (Exception ex)
            {
                await _notificationService.ShowNotification(
                    "Непредвиденная ошибка",
                    $"Что-то пошло не так: {ex.Message}",
                    "Error"
                );
            }
        }


        [RelayCommand]
        private async Task SavePermissions()
        {
            if(SelectedUser == null) return;

            List<int> selectedServers = Servers
                                                .Where(s => s.IsChecked)
                                                .Select(s => s.Id)
                                                .ToList();

            List<int> selectedCommands = Commands
                                                .Where(c => c.IsChecked)
                                                .Select(c => c.Id)
                                                .ToList();

            await _notificationService.ShowNotification("Права доступа", $"Разрешения для серверов и команд успешно обновлены", "Success");

            await _ServerRepository.SaveUserServersAsync(SelectedUser.Id, selectedServers);       
            await _CommandRepository.SaveUserCommandsAsync(SelectedUser.Id, selectedCommands);

            SelectedUser = null;
            await RefreshListServersAndCommads();
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


        private async Task RefreshListServersAndCommads()
        {
            foreach (var server in Servers)
            {
                server.IsChecked = false;
            }
            foreach (var command in Commands)
            {
                command.IsChecked = false;
            }
        }


        private bool CanDelete() => SelectedUser != null && !IsEditingUser;
        private bool CanEdit() => SelectedUser != null && IsEditingUser;
        private bool CanNotEdit() => SelectedUser != null && !CanEdit();
        private bool CanAdd() => !IsEditingUser;

        private void RefreshUserUI()
        {
            OnPropertyChanged(nameof(IsPermissionsEnabled));
            OnPropertyChanged(nameof(IsListUsersEnabled));

            AddUserCommand.NotifyCanExecuteChanged();
            DeleteUserCommand.NotifyCanExecuteChanged();
            EditUserCommand.NotifyCanExecuteChanged();
            SaveUserCommand.NotifyCanExecuteChanged();
        }

        partial void OnIsEditingUserChanged(bool value) => RefreshUserUI();
        partial void OnSelectedUserChanged(User? value)
        {
            RefreshUserUI();

            if (value == null) return;
            _ = LoadPermissionsUserAsync(value.Id);
        }
    }
}
