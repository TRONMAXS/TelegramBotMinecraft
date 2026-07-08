using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TelegramBotMinecraft.Avalonia.Models
{
    public partial class User : ObservableObject
    {
        [ObservableProperty]
        public string _name;

        [ObservableProperty]
        private string _id;

        public User(string name, string id)
        {
            _name = name;
            _id = id;
        }
    }
}
