namespace TelegramBotMinecraft.Core.Services
{
    public interface INotificationService
    {
        Task ShowNotification(string title, string text, string status);
    }
}
