using CommunityToolkit.Mvvm.ComponentModel;

namespace TelegramBotMinecraft.Avalonia.ViewModels.Items
{
    public partial class ServerStatusItemViewModel : ObservableObject
    {
        public int Id { get; }
        public string Name { get; }

        [ObservableProperty]
        private string status;

        public ServerStatusItemViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
