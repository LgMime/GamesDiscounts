using GamesDiscounts.Models;

namespace GamesDiscounts.Services
{
    public class GameAlerts : IAlert
    {

        private readonly ISavedGamesService _savedGamesService;
        private CancellationTokenSource _cts = new CancellationTokenSource();
        private Dictionary<long, (Timer Timer, CancellationTokenSource Cts)> _timers = new();
        public event Func<long, List<GameDetailsDto>, Task> OnTimerElapsed;

        public GameAlerts(ISavedGamesService savedGamesService)
        {

            _savedGamesService = savedGamesService;

        }
        public void SetTimer(long СhatId, int PrecentDiscount = 0)
        {

            DailyAlerts(21, 00, СhatId, PrecentDiscount);
        }

        public void DailyAlerts(int hour, int minute, long chatId, int PrecentDiscount = 0)
        {

            var state = _savedGamesService.GetAlertStateAsync(chatId);

            foreach (var alertState in state.Result)
            {
                if (!alertState.IsEnabled)
                {
                    Console.WriteLine($"[INFO] Оповещения отключены для чата {chatId}. Таймер не будет запущен.");
                    return;
                }
            }

            DateTime currentTime = DateTime.Now;
            DateTime nextRun = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, hour, minute, 0);

            if (currentTime > nextRun)
                nextRun = nextRun.AddDays(1);

            TimeSpan timeToGo = nextRun - currentTime;
            TimeSpan period = TimeSpan.FromMinutes(1);

            Console.WriteLine($"[INFO] Запускаем таймер для чата {chatId} на {timeToGo.TotalMinutes} минут");

            _cts = new CancellationTokenSource();

            Timer _timer = new Timer(async state =>
            {
                if (_cts.IsCancellationRequested)
                {
                    Console.WriteLine("[INFO] Таймер остановлен");
                    return;
                }
                long chatIdFromState = (long)state;
                await EHandler(chatIdFromState);
            }, chatId, timeToGo, period);
            _timers[chatId] = (_timer, _cts);

        }
        public async Task EHandler(long chatId)
        {
            var handler = OnTimerElapsed;
            var alertSettingsList = await _savedGamesService.GetAlertStateAsync(chatId);
            var alertState = alertSettingsList.FirstOrDefault();

            if (alertState != null && alertState.IsEnabled && handler != null)
            {
                var games = await _savedGamesService.GetAlertGamesAsync(chatId, alertState.DiscountPercent);

                var tasks = handler
                    .GetInvocationList()
                    .Cast<Func<long, List<GameDetailsDto>, Task>>()
                    .Select(h => h(chatId, games));

                await Task.WhenAll(tasks);

            }

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
