using GamesDiscounts.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GamesDiscounts.Bot
{
    public class BotMessage
    {
        private readonly ITelegramBotClient _bot;
        public BotMessage(ITelegramBotClient bot)
        {
            _bot = bot;
        }
        public async Task SendMessageAsync(long chatId, GameDetailsDto gameDetails)
        {
            var photo = new InputMediaPhoto(gameDetails.HeaderImage);
            await _bot.SendPhoto(chatId,
                   photo: photo.Media,
                   caption: $"Name: {gameDetails.Name}\nFinal price: {gameDetails.FinalFormatted}\nDiscount: {gameDetails.DiscountPercent}%");

        }
    }
}
