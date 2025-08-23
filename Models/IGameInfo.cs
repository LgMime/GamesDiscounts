using System.Threading.Tasks;

namespace GamesDiscounts.Models
{
    public interface IGameInfo
    {
       
        Task<List<NamesGames>> GetAllAppsAsync();
        Task<SaveEntry> FindGameByNameAsync(string gameName, bool SearchEquals);
        Task<List<SaveEntry>> GetAlertGamesAsync(long chatId, int PrecentDiscount = 0);
        Task DeleteSavedGameAsync(long chatId, string gameNameToDelete);
        string Name { get; set; }
        string? Discount { get; set; }
        string? FinalPrice { get; set; }
        string? HeaderImage { get; set; }
        bool SearchEquals { get; set; }
        string _priceOrDiscount { get; set; }
    }
}
