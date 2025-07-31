using Save;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GamesDiscounts
{

    public class TgBot
    {
        
        private static readonly Dictionary<long?, string> userState = new();

        static TelegramBotClient bot = new TelegramBotClient("8002657900:AAEu1_d3RQ2ZJS15stf23n3ywJqYNdZRYIY");

        public static void Main(string[] args)
        {
            var receiverOptions = new ReceiverOptions { AllowedUpdates = new UpdateType[] { UpdateType.Message, }, };
            bot.StartReceiving(updateHandler, errorHandler, receiverOptions);
            Console.ReadLine();
        }

        private static async Task errorHandler(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            throw new Exception($"An error occurred in the bot: {exception.Message}", exception);
        }

        private static async Task updateHandler(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            string someText = UpdateType.Message.ToString();
            var id = update.Message?.Chat.Id;

            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                someText = update.Message.Text;
                string chatId = update.Message.Chat.Id.ToString();

            }

            if (update.Type == UpdateType.Message)
            {

                switch (someText)
                {
                    case "/start":
                        await bot.SendMessage(id, "Welcome!\n Command to Use👾\n😍/Save Add a game to your favorites\n🤔/Sale Show your saved games,\n☠/Delete Remove a game from your favorites\n🤔/Search Find a game by name\n🤔/ExactSearch Search with exact match");
                        break;
                    case "/save":
                        await bot.SendMessage(id, "Type a exact game's name for save:");
                        userState[id] = "save";
                        break;
                    case "/sale":
                        await bot.SendMessage(id, "Showing all saved games:");
                        await SaleHandler(bot, update, token);
                        break;
                    case "/delete":
                        await bot.SendMessage(id, "Type a exact game's name for delete:");
                        userState[id] = "delete";
                        break;
                    case "/search":
                        await bot.SendMessage(id, "Type a game's name for seacrh:");
                        userState[id] = "search";
                        break;
                    case "/exactsearch":
                        await bot.SendMessage(id, "Type the exact game's name for search:");
                        userState[id] = "exactsearch";
                        break;

                    default:
                        if (userState.TryGetValue(id, out var state))
                        {
                            switch (state)
                            {
                                case "save":
                                    await SaveHandler(bot, update, token);
                                    userState.Remove(id);
                                    break;
                                case "search":
                                    await SearchHandler(bot, update, token, someText, false);
                                    userState.Remove(id);
                                    break;
                                case "exactsearch":
                                    await SearchHandler(bot, update, token, someText, true);
                                    userState.Remove(id);
                                    break;
                                case "delete":
                                    await DeletHandler(bot, update, token, someText);
                                    userState.Remove(id);
                                    break;
                                default:
                                    await bot.SendMessage(id, "Unknown command. Please use /start to see available commands.");
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        private static async Task DeletHandler(ITelegramBotClient bot, Update update, CancellationToken token, string someText)
        {
            await bot.SendMessage(update.Message.Chat.Id, "Deleting game...");
            GameInfo gameInfo = new GameInfo();
            try
            {
                await gameInfo.DeleteFromSave(someText, update.Message.Chat.Id);
                await bot.SendMessage(update.Message.Chat.Id, "Game deleted successfully.");
            }
            catch (Exception)
            {
                await bot.SendMessage(update.Message.Chat.Id, "An error occurred while deleting the game. Please try again.");
                throw;
            }
        }

        private static async Task SaveHandler(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                GameInfo gameInfo = new GameInfo();
                string gameName = update.Message.Text;

                try
                {
                    await gameInfo.FoundGameAppIdAsync(gameName , true);
                    if (gameInfo.Name == null || gameInfo.HeaderImage == null)
                    {
                        await bot.SendMessage(update.Message.Chat.Id, "Game not found. Please check the name and try again.");
                        return;
                    }
                    var entry = new SaveEntry
                    {
                        Chat = update.Message.Chat.Id,
                        Name = gameName,
                    };
                    Save.SaveService.AddItem(entry);
                    await bot.SendMessage(update.Message.Chat.Id, "Game saved successfully.");
                }
                catch (Exception)
                {
                    await bot.SendMessage(update.Message.Chat.Id, "An error occurred while saving the game. Please try Chek games name.");
                    throw;
                }

            }
        }

        private static async Task SaleHandler(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                GameInfo gameInfo = new GameInfo();
                long id = update.Message.Chat.Id;
                var savedGames = await gameInfo.FoundSavedGames(id);

                foreach (var game in savedGames)
                {
                    var gameDetails = await gameInfo.FoundGameAppIdAsync(game.Name, true);
                    Console.WriteLine($"game.Name: {game.Name}, final_formatted: {game.final_formatted}, discount_percent: {game.discount_percent}");
                    if (game.Name != null && game.final_formatted != null && game.discount_percent != null)
                    {
                        await bot.SendPhoto(chatId: update.Message.Chat.Id,
                            photo: gameDetails.header_image,
                            caption: $"Name: {gameDetails.Name}\nFinal price: {gameDetails.final_formatted}\nDiscount: {gameDetails.discount_percent}%");
                    }
                    else
                    {
                        await bot.SendMessage(update.Message.Chat.Id, "No saved games found.");
                    }
                }
            }
        }
        private static async Task SearchHandler(ITelegramBotClient bot, Update update, CancellationToken token, string nameGame, bool searchEquals)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                GameInfo gameInfo = new GameInfo();
                await bot.SendMessage(update.Message.Chat.Id, "Searching for the game...");
                await gameInfo.FoundGameAppIdAsync(nameGame, searchEquals);
                if (gameInfo.Name != null && gameInfo.HeaderImage != null)
                {
                    await bot.SendPhoto(chatId: update.Message.Chat.Id,
                    photo: gameInfo.HeaderImage.Media,
                    caption: $"Name: {gameInfo.Name}\nFinal price: {gameInfo.FinalPrice}\nDiscount: {gameInfo.Discount}%");
                }
                else
                {
                    await bot.SendMessage(update.Message.Chat.Id, "Game not found. Please check the name and try again.");
                }
            }
            else
            {
                await bot.SendMessage(update.Message.Chat.Id, "Please enter a valid game name to search.");
            }
        }
        
    }
}
