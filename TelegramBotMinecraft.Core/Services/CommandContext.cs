using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBotMinecraft.Core.Database;

namespace TelegramBotMinecraft.Core.Services
{
    public class CommandContext
    {
        private readonly CommandRepository _commandRepository;

        private readonly Dictionary<string, ICommandStrategy> _strategies = new();

        private readonly IEnumerable<ICommandStrategy> _rawStrategies;


        public CommandContext(IEnumerable<ICommandStrategy> strategies, CommandRepository commandRepository)
        {
            _commandRepository = commandRepository;
            _rawStrategies = strategies;

            foreach (var strategy in strategies)
            {
                _strategies[strategy.CommandName.ToLower()] = strategy;
            }
        }

        public async Task InitializeAsync()
        {
            await _commandRepository.EnsureCommandsRegisteredAsync(_rawStrategies);
        }

        public async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            var messageText = message.Text?.Trim().ToLower();
            if (string.IsNullOrEmpty(messageText)) return;

            ICommandStrategy? matchedStrategy = null;

            foreach (var pair in _strategies)
            {
                if (messageText == pair.Key || messageText.StartsWith(pair.Key + " "))
                {
                    matchedStrategy = pair.Value;
                    break;
                }
            }

            if (matchedStrategy != null)
            {
                long userId = message.Chat.Id;

                bool hasAccess = await _commandRepository.HasAccessToCommandAsync(userId, matchedStrategy.CommandId);

                if (hasAccess)
                {
                    await matchedStrategy.ExecuteAsync(botClient, message, cancellationToken);
                }
                else
                {
                    await botClient.SendMessage(userId, "У вас нет прав на выполнение этой команды.", cancellationToken: cancellationToken);
                }
            }
            else
            {
                await botClient.SendMessage(message.Chat.Id, "Неизвестная команда.", cancellationToken: cancellationToken);
            }
        }
    }
}
