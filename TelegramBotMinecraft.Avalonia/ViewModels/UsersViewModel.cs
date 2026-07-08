using Avalonia.Automation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using TelegramBotMinecraft.Avalonia.Models;

namespace TelegramBotMinecraft.Avalonia.ViewModels
{
    public class UsersViewModel : ObservableObject
    {
        public ObservableCollection<User> Users { get; } = new();

        public UsersViewModel()
        {

            for (int i = 0; i < 10; i++)
            {
                Users.Add(new User($"User{i}", i.ToString()));

            }

        }
    }
}
