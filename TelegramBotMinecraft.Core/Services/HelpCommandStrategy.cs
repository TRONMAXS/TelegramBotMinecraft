using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotMinecraft.Core.Database;

namespace TelegramBotMinecraft.Core.Services
{
    public class HelpCommandStrategy : ICommandStrategy
    {
        private readonly CommandRepository _commandRepository;

        public int CommandId => 2;
        public string CommandName => "/help";

        public HelpCommandStrategy(CommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var listCommand = await _commandRepository.GetCommandsByUserIdAsync(message.Chat.Id);
            if (listCommand == null || listCommand.Count == 0) 
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Доступные команды: отсутствуют",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken
                );

                return;
            }

            string answerText = "";
            foreach (var command in listCommand)
            {
                answerText += $"\n{command.CommandText}";

            }

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text:$"Доступные команды: {answerText}",
                cancellationToken: cancellationToken
            );
        }
    }
}
