namespace GamesDiscounts.Models
{
    public interface IGameInfo
    {
       
        Task<List<GamesName>> GetAllAppsAsync();
        Task<GameDetailsDto> FindGameByNameAsync(string gameName, bool SearchEquals);
    }
}
