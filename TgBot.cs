using Save;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GamesDiscounts
{

    public class TgBot
    {
        private readonly ITelegramBotClient _bot;
        private static readonly Dictionary<long?, string> userState = new();
        private static GameAlerts alerts = new GameAlerts(new GameInfo()); // для одного экземпляра GameInfo 

        static TelegramBotClient bot = new TelegramBotClient("8002657900:AAEu1_d3RQ2ZJS15stf23n3ywJqYNdZRYIY");
        public TgBot()
        {
            _bot = bot;
        }

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
            var id = update.Message.Chat.Id;

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
                        await SaleHandlerAsync(bot, update, token);
                        break;
                    case "/delete":
                        await bot.SendMessage(id, "Type a exact game's name for delete:");
                        userState[id] = "delete";
                        break;
                    case "/search":
                        await bot.SendMessage(id, "Type a game's name for seacrh:");
                        userState[id] = "search";
                        break;
                    case "/Alerts":
                        await bot.SendMessage(id, "Write discount's precent 0 to 100");
                        userState[id] = "Alerts";
                        break;
                    case "/AlertsOff":
                        await bot.SendMessage(id, "Alert was disabled");
                        alerts.StopTimer(id);
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
                                    await SaveHandlerAsync(bot, update, token);
                                    userState.Remove(id);
                                    break;
                                case "search":
                                    await SearchHandlerAsync(bot, update, token, someText, false);
                                    userState.Remove(id);
                                    break;
                                case "exactsearch":
                                    await SearchHandlerAsync(bot, update, token, someText, true);
                                    userState.Remove(id);
                                    break;
                                case "Alerts":
                                    AlertsHandlerAsync(bot, update, token, someText);
                                    userState.Remove(id);
                                    break;
                                case "delete":
                                    await DeletHandlerAsync(bot, update, token, someText);
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

        private static async Task DeletHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token, string someText)
        {
            await bot.SendMessage(update.Message.Chat.Id, "Deleting game...");
            GameInfo gameInfo = new GameInfo();
            try
            {
                await gameInfo.DeleteFromSave(update.Message.Chat.Id, someText);
                await bot.SendMessage(update.Message.Chat.Id, "Game deleted successfully.");
            }
            catch (Exception)
            {
                await bot.SendMessage(update.Message.Chat.Id, "An error occurred while deleting the game. Please try again.");
                throw;
            }
        }

        private static async Task SaveHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {

                GameInfo gameInfo = new GameInfo();
                SaveDB saveDB = new SaveDB();

                long id = update.Message.Chat.Id;
                string gameName = update.Message.Text;

                try
                {
                    foreach (var game in saveDB.GetSavedGames(id))
                    {
                        if (game.Equals(gameName, StringComparison.OrdinalIgnoreCase))
                        {
                            await bot.SendMessage(id, "This game is already saved.");
                            return;
                        }
                    }
                    await gameInfo.FoundGameAppIdAsync(gameName, true);
                    if (gameInfo.Name == null || gameInfo.HeaderImage == null)
                    {
                        await bot.SendMessage(id, "Game not found. Please check the name and try again.");
                        return;
                    }
                    else
                    {
                        saveDB.SaveGameName(id, gameName);
                    }
                    await bot.SendMessage(id, "Game saved successfully.");
                }
                catch (Exception)
                {
                    await bot.SendMessage(id, "An error occurred while saving the game. Please try Chek games name.");
                    throw;
                }

            }
        }

        private static async Task SaleHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                GameInfo gameInfo = new GameInfo();
                SaveDB saveDB = new SaveDB();
                long id = update.Message.Chat.Id;


                foreach (var game in saveDB.GetSavedGames(id))
                {
                    await gameInfo.FoundGameAppIdAsync(game, true);
                    await bot.SendPhoto(chatId: update.Message.Chat.Id, photo: gameInfo.HeaderImage.Media,
                    caption: $"Name: {gameInfo.Name}\nFinal price: {gameInfo.FinalPrice}\nDiscount: {gameInfo.Discount}%");
                }
            }
        }
        private static void AlertsHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token, string SomeDiscountPrecent)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                bot.SendMessage(update.Message.Chat.Id, "Setting up alerts...");
                bot.SendMessage(update.Message.Chat.Id, "Ready i'll notify you every day at .... if your game have discount");
                int finaleDiscountPrecent = Int32.Parse(SomeDiscountPrecent);
                if (finaleDiscountPrecent < 0 || finaleDiscountPrecent > 100)
                {
                    bot.SendMessage(update.Message.Chat.Id, "Please enter a valid discount percentage between 0 and 100.");
                    return;
                }
                else
                {
                    alerts.SetTimer(update.Message.Chat.Id, true, finaleDiscountPrecent);
                }
            }
        }
        public async Task SendDailyAlertsAsync(long chatId, List<SaveEntry> games)
        {
            foreach (var game in games)
            {
                await _bot.SendPhoto(
                    chatId: chatId,
                    photo: game.header_image,
                    caption: $"Name: {game.Name}\nPrice: {game.final_formatted}\nDiscount: {game.discount_percent}%"
                );
            }
        }
        private static async Task SearchHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token, string nameGame, bool searchEquals)
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
