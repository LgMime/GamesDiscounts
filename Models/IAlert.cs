namespace GamesDiscounts.Models
{
    public interface IAlert
    {
        void SetTimer(long ChatId, bool TurnOn, int PrecentDiscount = 0);
        void StopTimer(long chatId);

    }
}
