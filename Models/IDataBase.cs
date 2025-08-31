namespace GamesDiscounts.Models
{
    public interface IDataBase
    {
        Task SaveGameNameAsync(long ChatId, string GameName);
        Task SetAlertsAsync(long chatId, bool AlertState, int DiscountPrecent);
        Task<List<string>> GetSavedGames(long ChatId);
        Task<List<AlertSettings>> GetAllEnabledAlertsAsync();
        Task DeleteGameName(long ChatId, string GameName);

    }
}
