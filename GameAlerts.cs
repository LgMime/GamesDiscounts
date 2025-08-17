using Save;

namespace GamesDiscounts
{
    public class GameAlerts
    {
        private readonly GameInfo _gameInfo;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private Dictionary<long, (Timer Timer, CancellationTokenSource Cts)>  _timers = new Dictionary<long, (Timer, CancellationTokenSource)>();

        public GameAlerts(GameInfo gameInfo)
        {
            _gameInfo = gameInfo;
        }
        public void SetTimer(long ChatId, bool TurnOn, int PrecentDiscount = 0)
        {
            DailyAlerts(21, 00, ChatId, TurnOn, PrecentDiscount);
        }

        public void DailyAlerts(int hour, int minute, long chatId, bool TurnOn, int PrecentDiscount = 0)
        {
            DateTime currentTime = DateTime.Now;
            DateTime nextRun = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hour, minute, 0);

            if (currentTime > nextRun)
                nextRun = nextRun.AddDays(1);

            TimeSpan timeToGo = nextRun - currentTime;
            TimeSpan period = TimeSpan.FromDays(1);

            Console.WriteLine($"[INFO] Запускаем таймер для чата {chatId} на {timeToGo.TotalSeconds} секунд");

            _cts = new CancellationTokenSource();

          Timer _timer = new Timer(async state =>
            {
                if (_cts.IsCancellationRequested)
                {
                    Console.WriteLine("[INFO] Таймер остановлен");
                    return;
                }
                TgBot _tgBot = new TgBot();
                if (_tgBot == null)
                {
                    _tgBot = new TgBot();
                }
                long chatIdFromState = (long)state;
                List<SaveEntry> alertGames = await _gameInfo.FoundAlertGames(chatIdFromState, PrecentDiscount);
                await _tgBot.SendDailyAlertsAsync(chatIdFromState, alertGames);
            }, chatId, timeToGo, period);
            _timers[chatId] = (_timer, _cts);
        }
        public void StopTimer(long chatId)
        {
            if (_timers.TryGetValue(chatId, out var entry))
            {
                entry.Cts.Cancel();
                entry.Timer.Dispose();
                _timers.Remove(chatId);
            }
        }
    }
}
