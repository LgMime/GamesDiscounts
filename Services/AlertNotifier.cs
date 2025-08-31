using GamesDiscounts.Bot;
using GamesDiscounts.Models;

namespace GamesDiscounts.Services
{
    public class AlertNotifier
    {

        private readonly BotMessage _message;

        public AlertNotifier(BotMessage message)
        {
            _message = message;
        }
        public async Task SendDailyAlertsAsync(long chatId, List<GameDetailsDto> games)
        {
            foreach (var game in games)
            {
                await _message.SendMessageAsync(chatId, game);
            }
        }
    }
}
