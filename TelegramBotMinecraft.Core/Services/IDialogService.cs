namespace TelegramBotMinecraft.Core.Services
{
    public interface IDialogService
    {
        Task<bool?> AskConfirmationAsync(string title);
    }
}
