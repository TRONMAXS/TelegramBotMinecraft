using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotMinecraft.Core.Database;
using static TelegramBotMinecraft.Core.Models.ServerStatusModel;

namespace TelegramBotMinecraft.Core.Services
{
    public class OnServerCommandStrategy : ICommandStrategy
    {
        private readonly MinecraftServerManager _minecraftServerManager;
        private readonly ServerRepository _serverRepository;


        public int CommandId => 4;
        public string CommandName => "/on_server";

        public OnServerCommandStrategy(MinecraftServerManager minecraftServerManager, ServerRepository serverRepository)
        {
            _minecraftServerManager = minecraftServerManager;
            _serverRepository = serverRepository;
        }

        public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            string text = message.Text?.Trim() ?? "";
            string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2 || !int.TryParse(parts[1], out int serverId))
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Пожалуйста, укажите номер сервера из /list",
                    cancellationToken: cancellationToken
                );
                return;
            }

            var ServerInfo = await _serverRepository.GetServerByID(serverId);
            if (ServerInfo == null || !await _serverRepository.HasAccessToServerAsync(message.Chat.Id, ServerInfo.Id))
            {
                await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Сервер не найден!",
                cancellationToken: cancellationToken
                );
                return;
            }

            var statusServer = await _minecraftServerManager.GetServerStatus(ServerInfo.Name);
            if (statusServer == ServerStatus.Online)
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"**СЕРВЕР:** `{ServerInfo.Id}` - `{ServerInfo.Name}`\n" + $"▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬\n" + $"**Статус сервера:** `{statusServer}`\n" + $"**IP для входа:** `{ServerInfo.Connected}`",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: cancellationToken
                );

                return;
            }

            bool startingServer = await _minecraftServerManager.StartServer(ServerInfo.Name);
            if (startingServer != true)
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Сервер не запустился!",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: cancellationToken
                );
                return;
            }

            var messageStarting = await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Сервер запускается...",
                cancellationToken: cancellationToken
            );

            statusServer = await _minecraftServerManager.GetServerStatus(ServerInfo.Name);

            var messageStatus = await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Статус сервера: {statusServer}",
                cancellationToken: cancellationToken
            );

            int maxAttempts = 180;
            int attempts = 0;

            var lastStatus = statusServer;

            while (statusServer != ServerStatus.Online)
            {
                if (attempts++ >= maxAttempts)
                {
                    await botClient.EditMessageText(
                        chatId: message.Chat.Id,
                        messageId: messageStatus.MessageId,
                        text: "Превышено время ожидания запуска сервера!",
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                await Task.Delay(2000, cancellationToken);

                statusServer = await _minecraftServerManager.GetServerStatus(ServerInfo.Name);

                if (statusServer == ServerStatus.Offline)
                {
                    await botClient.EditMessageText(
                        chatId: message.Chat.Id,
                        messageId: messageStatus.MessageId,
                        text: "Ошибка: сервер неожиданно перешел в офлайн во время запуска.",
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                if (statusServer != lastStatus)
                {
                    await botClient.EditMessageText(
                        chatId: message.Chat.Id,
                        messageId: messageStatus.MessageId,
                        text: $"Статус сервера: {statusServer}",
                        cancellationToken: cancellationToken
                    );

                    lastStatus = statusServer;
                }
            }

            await botClient.EditMessageText(
                chatId: message.Chat.Id,
                messageId: messageStatus.MessageId,
                text: $"**СЕРВЕР:** `{ServerInfo.Id}` - `{ServerInfo.Name}`\n" + $"▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬\n" + $"**Статус сервера:** `{statusServer}`\n" + $"**IP для входа:** `{ServerInfo.Connected}`",
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken
            );

            await botClient.DeleteMessage(
                chatId: message.Chat.Id,
                messageId: messageStarting.MessageId,
                cancellationToken: cancellationToken
            );
        }
    }
}
