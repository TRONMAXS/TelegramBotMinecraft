using System.Collections.Concurrent;
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

        private static readonly ConcurrentDictionary<int, SemaphoreSlim> _serverLocks = new();

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
                    text: $"Пожалуйста, укажите номер сервера из /list\n" +
                          $"Пример: <code>/off_server 1</code>",
                    parseMode: ParseMode.Html,
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

            var serverLock = _serverLocks.GetOrAdd(ServerInfo.Id, _ => new SemaphoreSlim(1, 1));

            if (!await serverLock.WaitAsync(0))
            {
                var watchStoping = await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: "Этот сервер уже находится в процессе остановки! Пожалуйста, подождите.",
                    cancellationToken: cancellationToken
                );

                var currentStatus = await _minecraftServerManager.GetServerStatus(ServerInfo.Name);
                var watchStatusMessage = await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Статус сервера: <code>{currentStatus}</code>",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken
                );

                int watchAttempts = 0;
                var lastWatchStatus = currentStatus;

                while (currentStatus != ServerStatus.Offline && serverLock.CurrentCount == 0)
                {
                    if (watchAttempts++ >= 180) break;

                    await Task.Delay(3000, cancellationToken);
                    currentStatus = await _minecraftServerManager.GetServerStatus(ServerInfo.Name);

                    if (currentStatus != lastWatchStatus)
                    {
                        await botClient.EditMessageText(
                            chatId: message.Chat.Id,
                            messageId: watchStatusMessage.MessageId,
                            text: $"Статус сервера: <code>{currentStatus}</code>",
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken
                        );
                        lastWatchStatus = currentStatus;
                    }
                }

                if (currentStatus == ServerStatus.Offline)
                {
                    await botClient.EditMessageText(
                        chatId: message.Chat.Id,
                        messageId: watchStatusMessage.MessageId,
                        text: $"СЕРВЕР: <code>{ServerInfo.Id}</code> - <code>{ServerInfo.Name}</code>\n" +
                              $"▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬\n" +
                              $"Статус сервера: <code>{currentStatus}</code>",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken
                    );
                }
                else
                {
                    await botClient.EditMessageText(
                        chatId: message.Chat.Id,
                        messageId: watchStatusMessage.MessageId,
                        text: $"Не удалось дождаться остановки сервера или произошла ошибка.",
                        cancellationToken: cancellationToken
                    );
                }

                await botClient.DeleteMessage(
                    chatId: message.Chat.Id,
                    messageId: watchStoping.MessageId,
                    cancellationToken: cancellationToken
                );

                return;
            }
            try
            {
                var statusServer = await _minecraftServerManager.GetServerStatus(ServerInfo.Name);
                if (statusServer == ServerStatus.Offline)
                {
                    await botClient.SendMessage(
                        chatId: message.Chat.Id,
                        text: $"СЕРВЕР: <code>{ServerInfo.Id}</code> - <code>{ServerInfo.Name}</code>\n" +
                              $"▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬\n" +
                              $"Статус сервера: <code>{statusServer}</code>",
                        parseMode: ParseMode.Html,
                        cancellationToken: cancellationToken
                    );
                    return;
                }

                if (statusServer == ServerStatus.Online)
                {
                    bool stopingServer = await _minecraftServerManager.StopServer(ServerInfo.Name);
                    if (!stopingServer)
                    {
                        await botClient.SendMessage(
                            chatId: message.Chat.Id,
                            text: $"Сервер не остановлен!",
                            cancellationToken: cancellationToken
                        );
                        return;
                    }
                }

                var messageStoping = await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Сервер останавливается...",
                    cancellationToken: cancellationToken
                );

                statusServer = await _minecraftServerManager.GetServerStatus(ServerInfo.Name);
                var messageStatus = await botClient.SendMessage(
                    chatId: message.Chat.Id,
                    text: $"Статус сервера: <code>{statusServer}</code>",
                    parseMode: ParseMode.Html,
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
                            text: "Превышено время ожидания остановки сервера!",
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
                            text: $"Статус сервера: <code>{statusServer}</code>",
                            parseMode: ParseMode.Html,
                            cancellationToken: cancellationToken
                        );

                        lastStatus = statusServer;
                    }
                }

                await botClient.EditMessageText(
                    chatId: message.Chat.Id,
                    messageId: messageStatus.MessageId,
                    text: $"СЕРВЕР: <code>{ServerInfo.Id}</code> - <code>{ServerInfo.Name}</code>\n" +
                          $"▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬\n" +
                          $"Статус сервера: <code>{statusServer}</code>",
                    parseMode: ParseMode.Html,
                    cancellationToken: cancellationToken
                );

                await botClient.DeleteMessage(
                    chatId: message.Chat.Id,
                    messageId: messageStoping.MessageId,
                    cancellationToken: cancellationToken
                );
            }
            finally
            {
                serverLock.Release();
            }
        }
    }
}
