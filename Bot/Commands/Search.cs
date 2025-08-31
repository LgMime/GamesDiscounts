using GamesDiscounts.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GamesDiscounts.Bot.Commands
{
    public class Search : ICommandHandler
    {
        private readonly IGameInfo _gameInfo;
        private readonly Message _message;
        public string Command => "/search";
        public bool RequiresInput => true;
        public Search(IGameInfo gameInfo, Message message)
        {
            _gameInfo = gameInfo;
            _message = message;
        }
        public async Task HandleCommand(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            var chatId = update.Message.Chat.Id;
            await bot.SendMessage(chatId, "✍ Введи название игры для поиска:", cancellationToken: token);
        }
        public async Task HandleInput(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            await SearchHandlerAsync(bot, update, token, update.Message.Text, false);
        }

        private async Task SearchHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token, string gameName, bool searchEquals)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                await bot.SendMessage(update.Message.Chat.Id, "Searching for the game...");
                var gameDetails = await _gameInfo.FindGameByNameAsync(gameName, searchEquals);
                if (gameDetails.Name != null && gameDetails.header_image != null)
                {
                    await _message.SendMessageAsync(update.Message.Chat.Id, gameDetails);
                }
                else
                {
                    await bot.SendMessage(update.Message.Chat.Id, "Game not found. Please check the name and try again or use exactsearch.");
                }
            }
            else
            {
                await bot.SendMessage(update.Message.Chat.Id, "Please enter a valid game name to search.");
            }
        }
      
    }
}
