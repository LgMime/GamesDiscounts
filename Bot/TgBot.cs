using GamesDiscounts.Services;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GamesDiscounts.Bot
{

    public class TgBot
    {

        private readonly ITelegramBotClient _bot;
        private readonly CommandDispatcher _commandDispatcher;
        private readonly GameAlerts _gameAlerts;
        private readonly SqlSaveDB _saveDB;
        public TgBot(CommandDispatcher commandDispatcher, ITelegramBotClient bot, GameAlerts gameAlerts, SqlSaveDB saveDB)
        {
            _commandDispatcher = commandDispatcher;
            _bot = bot;
            _gameAlerts = gameAlerts;
            _saveDB = saveDB;
        }
        public async Task RunAsync()
        {
            HashSet<long> activeChats = new HashSet<long>();
            var chatIds = await _saveDB.GetAllEnabledAlertsAsync();
            foreach (var chat in chatIds)
            {
                if (chat.IsEnabled && !activeChats.Contains(chat.ChatId))
                {
                    _gameAlerts.SetTimer(chat.ChatId, chat.DiscountPercent);
                    Console.WriteLine($"[INFO] Timer set for chat ID: {chat.ChatId} with minimum discount: {chat.DiscountPercent}%");
                    activeChats.Add(chat.ChatId);
                }
            }
            var receiverOptions = new ReceiverOptions { AllowedUpdates = new UpdateType[] { UpdateType.Message, }, };
            _bot.StartReceiving(updateHandler, errorHandler, receiverOptions);
            await Task.Delay(-1);
        }

        private static async Task errorHandler(ITelegramBotClient bot, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            throw new Exception($"An error occurred in the bot: {exception.Message}", exception);
        }

        private async Task updateHandler(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            string someText = UpdateType.Message.ToString();
            var id = update.Message.Chat.Id;
            
            Console.WriteLine($"[INFO] Received update of type: {update.Type} from chat ID: {id}");
            await _commandDispatcher.Dispatch(bot, update, token);

        }

    }
}
