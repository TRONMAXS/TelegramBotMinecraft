using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.ViewModels;

namespace TelegramBotMinecraft.Avalonia;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();

        DataContext = new UsersViewModel(new Core.Database.ServerRepository(), new Core.Database.UserRepository(), new Core.Database.CommandRepository());
    }
}