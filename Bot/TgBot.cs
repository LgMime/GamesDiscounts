using GamesDiscounts.Models;
using GamesDiscounts.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GamesDiscounts.Bot
{

    public class TgBot
    {
        private readonly IGameInfo _gameInfo;
        private readonly ITelegramBotClient _bot;
        private readonly IDataBase _dataBase;
        private readonly IAlert _alert;
        private readonly GameAlerts _gameAlerts;
        private static readonly Dictionary<long?, string> userState = new();

        static TelegramBotClient bot = new TelegramBotClient("8002657900:AAEu1_d3RQ2ZJS15stf23n3ywJqYNdZRYIY");

        public TgBot(IGameInfo gameInfo, IDataBase dataBase, IAlert alert, GameAlerts gameAlerts)
        {
            _gameInfo = gameInfo;
            _bot = bot;
            _dataBase = dataBase;
            _alert = alert;
            gameAlerts.OnTimerElapsed += SendDailyAlertsAsync;
        }

        public void Main(string[] args)
        {
            var receiverOptions = new ReceiverOptions { AllowedUpdates = new UpdateType[] { UpdateType.Message, }, };
            bot.StartReceiving(updateHandler, errorHandler, receiverOptions);
            Console.ReadLine();
        }

        private static async Task errorHandler(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            throw new Exception($"An error occurred in the bot: {exception.Message}", exception);
        }

        private async Task updateHandler(ITelegramBotClient bot, Update update, CancellationToken token)
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
                    case "/alerts":
                        await bot.SendMessage(id, "Write discount's precent 0 to 100");
                        userState[id] = "Alerts";
                        break;
                    case "/alertsoff":
                        await bot.SendMessage(id, "Alert was disabled");
                        _alert.StopTimer(id);
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
        private async Task DeletHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token, string someText)
        {
            await bot.SendMessage(update.Message.Chat.Id, "Deleting game...");

            try
            {
                await _gameInfo.DeleteSavedGameAsync(update.Message.Chat.Id, someText);
                await bot.SendMessage(update.Message.Chat.Id, "Game deleted successfully.");
            }
            catch (Exception)
            {
                await bot.SendMessage(update.Message.Chat.Id, "An error occurred while deleting the game. Please try again.");
                throw;
            }
        }

        private async Task SaveHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                long id = update.Message.Chat.Id;
                string gameName = update.Message.Text;

                try
                {
                    foreach (var game in _dataBase.GetSavedGames(id))
                    {
                        if (game.Equals(gameName, StringComparison.OrdinalIgnoreCase))
                        {
                            await bot.SendMessage(id, "This game is already saved.");
                            return;
                        }
                    }
                    await _gameInfo.FindGameByNameAsync(gameName, true);
                    if (_gameInfo.Name == null || _gameInfo.HeaderImage == null)
                    {
                        await bot.SendMessage(id, "Game not found. Please check the name and try again.");
                        return;
                    }
                    else
                    {
                        _dataBase.SaveGameName(id, gameName);
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

        private async Task SaleHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                long id = update.Message.Chat.Id;


                foreach (var game in _dataBase.GetSavedGames(id))
                {
                    var photo = new InputMediaPhoto(_gameInfo.HeaderImage);
                    await _gameInfo.FindGameByNameAsync(game, true);
                    await bot.SendPhoto(chatId: update.Message.Chat.Id, photo: photo.Media,
                    caption: $"Name: {_gameInfo.Name}\nFinal price: {_gameInfo.FinalPrice}\nDiscount: {_gameInfo.Discount}%");
                }
            }
        }
        private void AlertsHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token, string SomeDiscountPrecent)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                bot.SendMessage(update.Message.Chat.Id, "Setting up alerts...");
                bot.SendMessage(update.Message.Chat.Id, "Ready i'll notify you every day at .... if your game have discount");
                int finaleDiscountPrecent = int.Parse(SomeDiscountPrecent);
                if (finaleDiscountPrecent < 0 || finaleDiscountPrecent > 100)
                {
                    bot.SendMessage(update.Message.Chat.Id, "Please enter a valid discount percentage between 0 and 100.");
                    return;
                }
                else
                {
                    _alert.SetTimer(update.Message.Chat.Id, true, finaleDiscountPrecent);
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
        private async Task SearchHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token, string nameGame, bool searchEquals)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {
                await bot.SendMessage(update.Message.Chat.Id, "Searching for the game...");
                await _gameInfo.FindGameByNameAsync(nameGame, searchEquals);
                if (_gameInfo.Name != null && _gameInfo.HeaderImage != null)
                {

                    var photo = new InputMediaPhoto(_gameInfo.HeaderImage);
                    await bot.SendPhoto(chatId: update.Message.Chat.Id,
                    photo: photo.Media,
                    caption: $"Name: {_gameInfo.Name}\nFinal price: {_gameInfo.FinalPrice}\nDiscount: {_gameInfo.Discount}%");
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
