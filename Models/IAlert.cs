
namespace GamesDiscounts.Models
{
    public interface IAlert
    {
        event Func<long, List<GameDetailsDto>, Task> OnTimerElapsed;

        void SetTimer(long ChatId, int PrecentDiscount = 0);
        void StopTimer(long chatId);

    }
}
