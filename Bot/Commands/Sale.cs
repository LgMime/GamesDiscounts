using GamesDiscounts.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GamesDiscounts.Bot.Commands
{
    public class Sale : ICommandHandler
    {
        private readonly ISavedGamesService _savedGamesService;
        private readonly BotMessage _message;
        public Sale(ISavedGamesService savedGamesService, BotMessage message)
        {
            _savedGamesService = savedGamesService;
            _message = message;
        }
        public string Command => "/sale";
        public bool RequiresInput => false;

        public async Task HandleCommand(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            long chatId = update.Message.Chat.Id;
            var games = await _savedGamesService.SaleGameAsync(chatId);
            if (games.Count == 0)
                await bot.SendMessage(chatId, "You have no saved games. Use /save to add some.");

            foreach (var game in games)
            {
                await _message.SendMessageAsync(chatId, game);
            }
        }
        public async Task HandleInput(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            // No input handling needed for this command
        }
    }
}
