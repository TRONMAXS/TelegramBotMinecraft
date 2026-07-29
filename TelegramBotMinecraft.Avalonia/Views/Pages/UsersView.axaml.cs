using Avalonia.Controls;
using TelegramBotMinecraft.Avalonia.Services;
using TelegramBotMinecraft.Avalonia.ViewModels;
using TelegramBotMinecraft.Core.Database;
using TelegramBotMinecraft.Core.Services;

namespace TelegramBotMinecraft.Avalonia;

public partial class UsersView : UserControl
{
    public UsersView()
    {
        InitializeComponent();

        DataContext = new UsersViewModel(new ServerRepository(), new UserRepository(), new CommandRepository(), new AvaloniaDialogService());
    }
}