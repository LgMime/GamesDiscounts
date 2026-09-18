using GamesDiscounts.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;
using static GamesDiscounts.Models.FoundGame;


namespace GamesDiscounts.Services
{
    public class GameInfo : IGameInfo
    {
        private readonly HttpService _hpp;
        private readonly IMemoryCache _cache;

        public GameInfo(IDataBase dataBase, HttpService hpp)
        {
            _hpp = hpp;
        }

        private const string SteamStoreListUrl = "https://api.steampowered.com/ISteamApps/GetAppList/v0002/";

        public async Task<List<GamesName>> GetAllAppsAsync()
        {
            if (!_cache.TryGetValue("AllSteamGames", out List<GamesName> cachedGames))
            {
                // Если в кэше пусто - качаем из Steam
                string json = await _hpp.GetStringAsync(SteamStoreListUrl);
                var result = JsonConvert.DeserializeObject<SteamAppListResponse>(json);
                cachedGames = result.applist.apps;

                // Сохраняем в кэш на 24 часа
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromHours(24));

                _cache.Set("AllSteamGames", cachedGames, cacheEntryOptions);
            }

            return cachedGames;
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
                    DiscountPercent = appDetails.data.price_overview?.discount_percent ?? 0,
                    FinalFormatted = appDetails.data.price_overview?.final_formatted ?? "0",
                    HeaderImage = appDetails.data.header_image,
                };
            }
            return null;
        }
    }
}

//https://api.steampowered.com/ISteamApps/GetAppList/v2/ all games
//@"https://store.steampowered.com/api/appdetails?appids={gameId}" current game info
