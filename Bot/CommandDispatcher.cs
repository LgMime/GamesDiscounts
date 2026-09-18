using GamesDiscounts.Bot.Commands;
using GamesDiscounts.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GamesDiscounts.Bot
{
    public class CommandDispatcher
    {
        private readonly Dictionary<string, ICommandHandler> _commands;
        private readonly Dictionary<long, string> _userState = new();

        public CommandDispatcher(IEnumerable<ICommandHandler> commandHandlers)
        {
            _commands = commandHandlers.ToDictionary(cmd => cmd.Command, StringComparer.OrdinalIgnoreCase);
        }
        public async Task Dispatch(ITelegramBotClient bot, Update update, CancellationToken token)
        {

            var chatId = update.Message.Chat.Id;
            var message = update.Message.Text;

            if (message != null && message.StartsWith("/"))
            {
                var command = message;
                if (_commands.TryGetValue(command, out var handler))
                {
                    _userState[chatId] = command;
                    await handler.HandleCommand(bot, update, token);
                }
            }
            else
            {
                if (_userState.TryGetValue(chatId, out var state) && _commands.TryGetValue(state, out var handler))
                {
                    if (handler.RequiresInput)
                    {
                        await handler.HandleInput(bot, update, token);
                        _userState.Remove(chatId);
                    }
                }
            }
        }
    }
}
