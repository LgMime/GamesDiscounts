using Save;

namespace GamesDiscounts
{
    public class GameAlerts
    {
        private Timer _timer;
        private readonly GameInfo _gameInfo;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public GameAlerts(GameInfo gameInfo)
        {
            _gameInfo = gameInfo;
        }
        public void SetTimer(long ChatId, bool TurnOn)
        {
            DailyAlerts(19, 21, ChatId, TurnOn);
        }

        public void DailyAlerts(int hour, int minute, long chatId, bool TurnOn)
        {
            DateTime currentTime = DateTime.Now;
            DateTime nextRun = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hour, minute, 0);

            if (currentTime > nextRun)
                nextRun = nextRun.AddMinutes(1);

            TimeSpan timeToGo = nextRun - currentTime;
            TimeSpan period = TimeSpan.FromMinutes(1);

            Console.WriteLine($"[INFO] Запускаем таймер для чата {chatId} на {timeToGo.TotalSeconds} секунд");

            _cts = new CancellationTokenSource();

            _timer = new Timer(async state =>
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
                List<SaveEntry> alertGames = await _gameInfo.FoundAlertGames(chatIdFromState);
                await _tgBot.SendDailyAlerts(chatIdFromState, alertGames);
            }, chatId, timeToGo, period);
        }
        public void StopTimer()
        {
            Console.WriteLine("[INFO] Timer stopped");
            _cts?.Cancel();   // говорит всем колбэкам «остановиться»
            _timer?.Dispose();
            _timer = null;
        }
    }
}
