using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotMinecraft.Core.Database;

namespace TelegramBotMinecraft.Core.Services
{
    public class ListServersCommandStrategy : ICommandStrategy
    {
        private readonly ServerRepository _serverRepository;

        public int CommandId => 3;
        public string CommandName => "/list";

        public ListServersCommandStrategy(ServerRepository serverRepository)
        {
            _serverRepository = serverRepository;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var listServers = await _serverRepository.GetServersByUserIdAsync(message.Chat.Id);
            if (listServers == null || listServers.Count == 0)
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Доступные сервера: отсутствуют",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: cancellationToken
                );

                return;
            }

            string answerText = "";
            foreach (var server in listServers)
            {
                answerText += $"\n`{server.Id}` - `{server.Name}`";
            }

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Доступные сервера:{answerText}",
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken
            );
        }
    }
}
