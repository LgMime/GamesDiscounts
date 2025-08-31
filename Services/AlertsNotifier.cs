using GamesDiscounts.Bot;
using GamesDiscounts.Models;

namespace GamesDiscounts.Services
{
    public class AlertsNotifier
    {

        private readonly Message _message;

        public AlertsNotifier(Message message)
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
