namespace TelegramBotMinecraft.Core.Services
{
    public interface IDialogService
    {
        Task<string?> SelectFolderAsync(string title);

        Task<bool?> AskConfirmationAsync(string title);
    }
}
