using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotMinecraft.Core.Database;

namespace TelegramBotMinecraft.Core.Services
{
    public class ListServersCommandStrategy : ICommandStrategy
    {
        private readonly ServerRepository _serverRepository;

        public int CommandId => 5;
        public string CommandName => "/list";

        public ListServersCommandStrategy(ServerRepository serverRepository)
        {
            _serverRepository = serverRepository;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var listServers = await _serverRepository.GetAllServers();
            if (listServers == null || listServers.Count == 0) return;

            string answerText = "";
            foreach (var server in listServers)
            {
                if (await _serverRepository.HasAccessToServerAsync(server.Id))
                {
                    answerText += $"`{server.Id}` - `{server.Name}`\n";
                }
            }

            await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Доступные сервера:\n{answerText}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: cancellationToken
            );
        }
    }
}
