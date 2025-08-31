using GamesDiscounts.Models;
using GamesDiscounts.Services;
using Telegram.Bot;

namespace GamesDiscounts.Bot.Commands
{
    public class AlertsOff: ICommandHandler
    {

        private readonly IAlert _alert;
        private readonly SavedGamesService _savedGamesService;
        public AlertsOff(IAlert alert, SavedGamesService savedGamesService)
        {
            _alert = alert;
            _savedGamesService = savedGamesService;
        }
        public string Command => "/alertsoff";
        public bool RequiresInput => false;


        public async Task HandleCommand(Telegram.Bot.ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken token)
        {
            long chatId = update.Message.Chat.Id;
            await bot.SendMessage(chatId, "Alerts turned off. You will no longer receive discount notifications for your saved games.", cancellationToken: token);
            await _savedGamesService.SaveAlertState(chatId, false, 0);
            _alert.StopTimer(chatId);
        }
        public async Task HandleInput(ITelegramBotClient bot, Telegram.Bot.Types.Update update, CancellationToken token)
        {
            // No input handling needed for this command
        }
    }
}
