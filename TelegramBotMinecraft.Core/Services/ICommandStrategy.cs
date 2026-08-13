using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotMinecraft.Core.Services
{
    public interface ICommandStrategy
    {
        int CommandId { get; }
        string CommandName { get; }

        Task ExecuteAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken);
    }
}
