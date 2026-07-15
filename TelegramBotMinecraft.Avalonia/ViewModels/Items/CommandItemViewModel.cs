using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace TelegramBotMinecraft.Avalonia.ViewModels.Items
{
    public partial class CommandItemViewModel : ObservableObject
    {
        public int Id { get; }
        public string Name { get; }

        [ObservableProperty]
        private bool isChecked;

        public CommandItemViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
