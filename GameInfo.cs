using Newtonsoft.Json;
using static GamesDiscounts.FoundGames;


namespace GamesDiscounts
{
    internal class GameInfo
    {
        public string? discount { get; set; }
        public string? finalPrice { get; set; }
        private string priceOrDiscount { get; set; } = "0";
        private static readonly HttpClient client = new HttpClient();

        private const string SteamStoreListUrl = @"https://api.steampowered.com/ISteamApps/GetAppList/v2/";
        public async Task <List<NamesGames> > DesAllGamesAsync()
        {
            string json = await client.GetStringAsync(SteamStoreListUrl);
            var result = JsonConvert.DeserializeObject<SteamAppListResponse>(json);
            return result.applist.apps;
        }

        public async Task FoundGameAppIdAsync(string gameName)
        {
            var allGames = await DesAllGamesAsync();
            var foundGame = allGames.FirstOrDefault(games => games.Name != null && games.Name.Contains(gameName, StringComparison.OrdinalIgnoreCase));
            if (foundGame == null)
                return;
            var parse = await client.GetStringAsync($"https://store.steampowered.com/api/appdetails?appids={foundGame.AppId}");
            var gameDetails = JsonConvert.DeserializeObject<Dictionary<string, AppDeatailsResponse>>(parse);
            if (gameDetails.TryGetValue(foundGame.AppId.ToString(), out var appDetails)&& appDetails.success)
            {
                discount = appDetails.data.price_overview?.discount_percent.ToString() ?? priceOrDiscount;
                finalPrice = appDetails.data.price_overview?.final_formatted ?? priceOrDiscount;
            }
        }
    }
}

//https://api.steampowered.com/ISteamApps/GetAppList/v2/ all games
//@"https://store.steampowered.com/api/appdetails?appids={gameId}" current game info
