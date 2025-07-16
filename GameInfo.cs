using Newtonsoft.Json;
using Telegram.Bot.Types;
using static GamesDiscounts.FoundGames;


namespace GamesDiscounts
{
    internal class GameInfo
    {
        public string? Discount { get; set; }
        public string? FinalPrice { get; set; }
        public InputMediaPhoto? HeaderImage { get; set; }
        private string _priceOrDiscount { get; set; } = "0";
        private static readonly HttpClient _client = new HttpClient();

        private const string SteamStoreListUrl = "https://api.steampowered.com/ISteamApps/GetAppList/v2/";
        public async Task<List<NamesGames>> DesAllGamesAsync()
        {
            string json = await _client.GetStringAsync(SteamStoreListUrl);
            var result = JsonConvert.DeserializeObject<SteamAppListResponse>(json);
            return result.applist.apps;
        }

        public async Task FoundGameAppIdAsync(string gameName)
        {
            var allGames = await DesAllGamesAsync();
            var foundGame = allGames.FirstOrDefault(games => games.Name != null && games.Name.Contains(gameName, StringComparison.OrdinalIgnoreCase));
            if (foundGame == null)
                return;
            var parse = await _client.GetStringAsync($"https://store.steampowered.com/api/appdetails?appids={foundGame.AppId}&cc=ua");//&cc=ua Ukranian region, if i want to get languange Ukrainian i need paste this &cc=ua&l=ukrainian  
            var gameDetails = JsonConvert.DeserializeObject<Dictionary<string, AppDeatailsResponse>>(parse);
            if (gameDetails.TryGetValue(foundGame.AppId.ToString(), out var appDetails) && appDetails.success)
            {
                Discount = appDetails.data.price_overview?.discount_percent.ToString() ?? _priceOrDiscount;
                FinalPrice = appDetails.data.price_overview?.final_formatted ?? _priceOrDiscount;
                HeaderImage = new InputMediaPhoto {  Media = appDetails.data.header_image };
            }
        }
    }
}

//https://api.steampowered.com/ISteamApps/GetAppList/v2/ all games
//@"https://store.steampowered.com/api/appdetails?appids={gameId}" current game info
