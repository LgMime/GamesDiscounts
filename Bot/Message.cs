using GamesDiscounts.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace GamesDiscounts.Bot
{
    public class Message
    {
        private readonly ITelegramBotClient _bot;
        public Message(ITelegramBotClient bot)
        {
            _bot = bot;
        }
        public async Task SendMessageAsync(long chatId, GameDetailsDto gameDetails)
        {
            var photo = new InputMediaPhoto(gameDetails.header_image);
            await _bot.SendPhoto(chatId,
                   photo: photo.Media,
                   caption: $"Name: {gameDetails.Name}\nFinal price: {gameDetails.final_formatted}\nDiscount: {gameDetails.discount_percent}%");

        }
    }
}
