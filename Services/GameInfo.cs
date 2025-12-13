using GamesDiscounts.Models;
using Newtonsoft.Json;
using static GamesDiscounts.Models.FoundGame;


namespace GamesDiscounts.Services
{
    public class GameInfo: IGameInfo
    {
        private readonly HttpService _hpp;
        public GameInfo(IDataBase dataBase, HttpService hpp)
        {
            _hpp = hpp;
        }
            
        private const string SteamStoreListUrl = "https://api.steampowered.com/ISteamApps/GetAppList/v0002/";

        public async Task<List<GamesName>> GetAllAppsAsync()
        {
            string json = await _hpp.GetStringAsync(SteamStoreListUrl);
            var result = JsonConvert.DeserializeObject<SteamAppListResponse>(json);
            return result.applist.apps;
        }
        public async Task<GameDetailsDto> FindGameByNameAsync(string gameName, bool SearchEquals)
        {
            var allGames = await GetAllAppsAsync();
            GamesName? foundGame = null;

            if (SearchEquals == false)
                foundGame = allGames.FirstOrDefault(games => games.Name != null && games.Name.StartsWith(gameName, StringComparison.OrdinalIgnoreCase));
            else
            {
                foundGame = allGames.FirstOrDefault(games => games.Name != null && games.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase));
            }

            if (foundGame == null || foundGame.Name == $"{gameName} Trailer")
            {
                return null;
            }

            var parse = await _hpp.GetStringAsync($"https://store.steampowered.com/api/appdetails?appids={foundGame.AppId}&cc=ua");//&cc=ua Ukranian region. If i want to get languange Ukrainian i need paste this &cc=ua&l=ukrainian  

            var gameDetails = JsonConvert.DeserializeObject<Dictionary<string, AppDeatailsResponse>>(parse);
            if (gameDetails.TryGetValue(foundGame.AppId.ToString(), out var appDetails) && appDetails.success)
            {
                return new GameDetailsDto
                {
                    Name = appDetails.data.name,
                    discount_percent = appDetails.data.price_overview?.discount_percent ?? 0,
                    final_formatted = appDetails.data.price_overview?.final_formatted ?? "0",
                    header_image = appDetails.data.header_image,

                };
            }
            return null;
        }    
    }
}

//https://api.steampowered.com/ISteamApps/GetAppList/v2/ all games
//@"https://store.steampowered.com/api/appdetails?appids={gameId}" current game info
