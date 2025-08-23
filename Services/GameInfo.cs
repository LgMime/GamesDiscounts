using GamesDiscounts.Models;
using Newtonsoft.Json;
using Telegram.Bot.Types;
using static GamesDiscounts.FoundGames;


namespace GamesDiscounts.Services
{
    public class GameInfo: IGameInfo
    {
        private readonly IDataBase _dataBase;
        private readonly IHttpService _hpp;
        public GameInfo(IDataBase dataBase, IHttpService hpp)
        {
            _dataBase = dataBase;
            _hpp = hpp;
        }

        public string Name { get; set; } = string.Empty;
        public string? Discount { get; set; }
        public string? FinalPrice { get; set; }
        public string? HeaderImage { get; set; }
        public bool SearchEquals { get; set; } = false;
        public string _priceOrDiscount { get; set; } = "0";


     

        private const string SteamStoreListUrl = "https://api.steampowered.com/ISteamApps/GetAppList/v2/";

        public async Task<List<NamesGames>> GetAllAppsAsync()
        {
            string json = await _hpp.GetStringAsync(SteamStoreListUrl);
            var result = JsonConvert.DeserializeObject<SteamAppListResponse>(json);
            return result.applist.apps;
        }
        public async Task<SaveEntry> FindGameByNameAsync(string gameName, bool SearchEquals)
        {
            var allGames = await GetAllAppsAsync();
            NamesGames? foundGame = null;

            if (SearchEquals == false)
                foundGame = allGames.FirstOrDefault(games => games.Name != null && games.Name.StartsWith(gameName, StringComparison.OrdinalIgnoreCase));
            else
            {
                foundGame = allGames.FirstOrDefault(games => games.Name != null && games.Name.Equals(gameName, StringComparison.OrdinalIgnoreCase));
            }

            if (foundGame == null)
            {
                return null;
            }

            var parse = await _hpp.GetStringAsync($"https://store.steampowered.com/api/appdetails?appids={foundGame.AppId}&cc=ua");//&cc=ua Ukranian region. If i want to get languange Ukrainian i need paste this &cc=ua&l=ukrainian  

            var gameDetails = JsonConvert.DeserializeObject<Dictionary<string, AppDeatailsResponse>>(parse);
            if (gameDetails.TryGetValue(foundGame.AppId.ToString(), out var appDetails) && appDetails.success)
            {
                Name = appDetails.data.name;
                Discount = appDetails.data.price_overview?.discount_percent.ToString() ?? _priceOrDiscount;
                FinalPrice = appDetails.data.price_overview?.final_formatted ?? _priceOrDiscount;
                HeaderImage =  appDetails.data.header_image;

            }
            if (foundGame == null || foundGame.Name == $"{gameName} Trailer")
            {
                return null;
            }
            return new SaveEntry
            {
                Name = appDetails.data.name,
                discount_percent = appDetails.data.price_overview?.discount_percent,
                final_formatted = appDetails.data.price_overview?.final_formatted,
                header_image = appDetails.data.header_image
            };
        }

        public async Task FoundSavedGames(long chatId)
        {
            foreach (var game in _dataBase.GetSavedGames(chatId))
            {
                await FindGameByNameAsync(game, true);
            }
        }

        public async Task<List<SaveEntry>> GetAlertGamesAsync(long chatId, int PrecentDiscount = 0)
        {
            var result = new List<SaveEntry>();
            if (_dataBase.GetSavedGames != null)
            {               
                foreach (var game in _dataBase.GetSavedGames(chatId))
                {
                    var entry = await FindGameByNameAsync(game, true);
                    if (entry != null && entry.discount_percent >= PrecentDiscount)
                    {
                        result.Add(entry);
                    }
                }
            }
            return result;
        }

        public async Task DeleteSavedGameAsync(long chatId, string gameNameToDelete)
        {
            _dataBase.DeleteGameName(chatId, gameNameToDelete);
        }
    }
}

//https://api.steampowered.com/ISteamApps/GetAppList/v2/ all games
//@"https://store.steampowered.com/api/appdetails?appids={gameId}" current game info
