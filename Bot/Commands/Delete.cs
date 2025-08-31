using GamesDiscounts.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GamesDiscounts.Bot.Commands
{
    public class Delete : ICommandHandler
    {
        private readonly ISavedGamesService _savedGamesService;
        public Delete(ISavedGamesService savedGamesService)
        {
            _savedGamesService = savedGamesService;
        }
        public string Command => "/delete";
        public bool RequiresInput => true;
        public async Task HandleCommand(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            long chatId = update.Message.Chat.Id;
            await bot.SendMessage(chatId, "Please enter the name of the game you want to delete from your saved list.");

        }
        public async Task HandleInput(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            await DeletHandlerAsync(bot, update, token, update.Message.Text);
        }
        private async Task DeletHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token, string someText)
        {
            await bot.SendMessage(update.Message.Chat.Id, "Deleting game...");

            try
            {
                await _savedGamesService.DeleteSavedGameAsync(update.Message.Chat.Id, someText);
                await bot.SendMessage(update.Message.Chat.Id, "Game deleted successfully.");
            }
            catch (Exception)
            {
                await bot.SendMessage(update.Message.Chat.Id, "An error occurred while deleting the game. Please try again.");
                throw;
            }
        }
    }

}
