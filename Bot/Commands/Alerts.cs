using GamesDiscounts.Models;
using GamesDiscounts.Services;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace GamesDiscounts.Bot.Commands
{
    public class Alerts : ICommandHandler
    {
        private readonly IAlert _alert;
        private readonly SavedGamesService _savedGamesService;

        public Alerts(IAlert alert, GameAlerts gameAlerts, AlertsNotifier _alertsNotifier, SavedGamesService savedGamesService)
        {
            _alert = alert;
            gameAlerts.OnTimerElapsed -= _alertsNotifier.SendDailyAlertsAsync;
            gameAlerts.OnTimerElapsed += _alertsNotifier.SendDailyAlertsAsync;
            _savedGamesService = savedGamesService;
     
        }
        public string Command => "/alerts";
        public bool RequiresInput => true;
        public async Task HandleCommand(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            long chatId = update.Message.Chat.Id;

            await bot.SendMessage(chatId, "Please enter the minimum discount percentage (0-100) for which you want to receive alerts about your saved games.");
            await bot.SendMessage(chatId, "For example, if you enter 20, you'll get alerts for games with at least a 20% discount.", cancellationToken: token);
        }
        public async Task HandleInput(ITelegramBotClient bot, Update update, CancellationToken token)
        {
            await AlertsHandlerAsync(bot, update, token, update.Message.Text);
        }

        private async Task AlertsHandlerAsync(ITelegramBotClient bot, Update update, CancellationToken token, string DiscountPrecent)
        {
            if (update.Type == UpdateType.Message && update.Message.Text != null)
            {

                await bot.SendMessage(update.Message.Chat.Id, "Ready i'll notify you every day at .... if your game have discount");
                int finaleDiscountPrecent = int.Parse(DiscountPrecent);
                if (finaleDiscountPrecent < 0 || finaleDiscountPrecent > 100)
                {
                    await bot.SendMessage(update.Message.Chat.Id, "Please enter a valid discount percentage between 0 and 100.");
                 
                    return;
                }
                else
                {
                    await _savedGamesService.SaveAlertState(update.Message.Chat.Id, true, finaleDiscountPrecent);
                    _alert.SetTimer(update.Message.Chat.Id, finaleDiscountPrecent);
                }
            }
        }

    }
}
