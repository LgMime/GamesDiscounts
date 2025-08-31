using GamesDiscounts.Models;
using Telegram.Bot;

namespace GamesDiscounts.Bot.Commands
{
    public class Start : ICommandHandler
    {
        public string Command => "/start";
        public bool RequiresInput => false;
        public async Task HandleCommand(Telegram.Bot.ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken token)
        {
            string welcomeMessage = "Welcome to the Game Discounts Bot!\n" +
                                    "Available commands:\n" +
                                    "/start - Show this welcome message\n" +
                                    "/search - Search for a game by name\n" +
                                    "/exactSearch exact search for a game by name" +
                                    "/save - Save a game to your favorites\n" +
                                    "/sale - List your saved games\n" +
                                    "/delete - Delete a game from your saved list\n\n" +
                                    "Use these commands to find and manage your favorite games with ease!";
            await bot.SendMessage(update.Message.Chat.Id, welcomeMessage, cancellationToken: token);
        }
        public async Task HandleInput(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken token)
        {
            // No input handling needed for this command
        }
    }
}
