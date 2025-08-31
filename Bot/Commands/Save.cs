using GamesDiscounts.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GamesDiscounts.Bot.Commands
{
    public class Save : ICommandHandler
    {
        private readonly ISavedGamesService _savedGamesService;
        public Save(ISavedGamesService savedGamesService)
        {
            _savedGamesService = savedGamesService;
        }
        public string Command => "/save";
        public bool RequiresInput => true;

        public async Task HandleCommand(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            long chatId = update.Message.Chat.Id;
            await bot.SendMessage(chatId, "Please enter the exact name of the game you want to save to your favorites.");

        }
        public async Task HandleInput(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            await SaveHandle(bot, update, token);
        }
        private async Task SaveHandle(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                long chatId = update.Message.Chat.Id;
                string gameName = update.Message.Text;
                try
                {
                    await _savedGamesService.SaveGameAsync(chatId, gameName);
                    await bot.SendMessage(chatId, "✅ Game saved successfully.");
                }
                catch (InvalidOperationException ex) when (ex.Message == "Game already saved.")
                {
                    await bot.SendMessage(chatId, "⚠ This game is already in your favorites.");
                }
                catch (Exception ex) when (ex.Message == "Game not found.")
                {
                    await bot.SendMessage(chatId, "❌ Game not found. Please check the name and try again or use exactsearch.");
                }
                catch
                {
                    await bot.SendMessage(chatId, "An error occurred while saving the game. Please try Chek games name.");
                    throw;
                }

            }
        }

    }
}
