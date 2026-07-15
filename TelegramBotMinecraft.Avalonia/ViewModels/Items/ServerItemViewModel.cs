using CommunityToolkit.Mvvm.ComponentModel;

namespace TelegramBotMinecraft.Avalonia.ViewModels.Items
{
    public partial class ServerItemViewModel : ObservableObject
    {
        public int Id { get; }
        public string Name { get; }

        [ObservableProperty]
        private bool isChecked;

        public ServerItemViewModel(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
