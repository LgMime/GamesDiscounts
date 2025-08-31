namespace GamesDiscounts.Models
{
    public interface ISavedGamesService
    {
        Task SaveGameAsync(long chatId, string gameName);
        Task<List<string>> GetSavedGamesAsync(long chatId);
        Task DeleteSavedGameAsync(long chatId, string gameName);
        Task<List<GameDetailsDto>> SaleGameAsync(long chatId);
        Task<List<GameDetailsDto>> GetAlertGamesAsync(long chatId, int PrecentDiscount = 0);
        Task SaveAlertState(long chatId, bool alertsEnabled, int minDiscount = 0);
        Task<List<AlertSettings>> GetAlertStateAsync(long chatId);
   
    }
}
