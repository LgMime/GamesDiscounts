using System.Threading.Tasks;

namespace GamesDiscounts.Models
{
    public interface IGameInfo
    {
       
        Task<List<NamesGames>> GetAllAppsAsync();
        Task<GameDetailsDto> FindGameByNameAsync(string gameName, bool SearchEquals);
    }
}
