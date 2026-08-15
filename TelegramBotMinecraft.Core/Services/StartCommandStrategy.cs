using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBotMinecraft.Core.Services
{
    public class StartCommandStrategy : ICommandStrategy
    {
        public int CommandId => 1;
        public string CommandName => "/start";

        public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Привет! Добро пожаловать в бота. Отправьте /help чтобы узнать больше команд.",
                    cancellationToken: cancellationToken
            );
        }
    }
}
