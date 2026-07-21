using CommunityToolkit.Mvvm.ComponentModel;
using static TelegramBotMinecraft.Core.Models.ServerStatusModel;

namespace TelegramBotMinecraft.Avalonia.ViewModels.Items
{
    public partial class ServerStatusItemViewModel : ObservableObject
    {
        public int Id { get; }
        public string Name { get; }

        /*[ObservableProperty]
        private ServerStatus status;*/

        [ObservableProperty]
        private string status;

        public ServerStatusItemViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
