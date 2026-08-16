using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotMinecraft.Core.Database;
using static TelegramBotMinecraft.Core.Models.ServerStatusModel;

namespace TelegramBotMinecraft.Core.Services
{
    public class OffServerCommandStrategy : ICommandStrategy
    {
        private readonly MinecraftServerManager _minecraftServerManager;
        private readonly ServerRepository _serverRepository;

        public int CommandId => 5;
        public string CommandName => "/off_server";

        public OffServerCommandStrategy(MinecraftServerManager minecraftServerManager, ServerRepository serverRepository)
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
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken
                );
                return;
            }

            var statusServer = await _minecraftServerManager.GetServerStatus(ServerInfo.Name);
            if (statusServer == ServerStatus.Offline)
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"**СЕРВЕР:** `{ServerInfo.Id}` - `{ServerInfo.Name}`\n" + $"▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬\n" + $"**Статус сервера:** `{statusServer}`",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: cancellationToken
                );
                return;
            }

            bool stopingServer = await _minecraftServerManager.StopServer(ServerInfo.Name);
            if (stopingServer != true)
            {
                await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Сервер не остановлен!",
                    parseMode: ParseMode.Markdown,
                    cancellationToken: cancellationToken
                );
                return;
            }

            var messageStoping = await botClient.SendMessage(
                chatId: message.Chat.Id,
                text: $"Сервер останавливается...",
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

            while (statusServer != ServerStatus.Offline)
            {
                if (attempts++ >= maxAttempts)
                {
                    await botClient.EditMessageText(
                        chatId: message.Chat.Id,
                        messageId: messageStatus.MessageId,
                        text: "Превышено время ожидания запуска сервера!",
                        parseMode: ParseMode.Markdown,
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                await Task.Delay(2000, cancellationToken);

                statusServer = await _minecraftServerManager.GetServerStatus(ServerInfo.Name);

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
                text: $"**СЕРВЕР:** `{ServerInfo.Id}` - `{ServerInfo.Name}`\n" + $"▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬\n" + $"**Статус сервера:** `{statusServer}`",
                parseMode: ParseMode.Markdown,
                cancellationToken: cancellationToken
            );

            await botClient.DeleteMessage(
                chatId: message.Chat.Id,
                messageId: messageStoping.MessageId,
                cancellationToken: cancellationToken
            );
        }
    }
}
