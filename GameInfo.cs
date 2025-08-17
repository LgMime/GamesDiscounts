using Newtonsoft.Json;
using Save;
using Telegram.Bot.Types;
using static GamesDiscounts.FoundGames;


namespace GamesDiscounts
{
    public class GameInfo
    {

        public string Name { get; set; } = string.Empty;
        public string? Discount { get; set; }
        public string? FinalPrice { get; set; }
        public InputMediaPhoto? HeaderImage { get; set; }
        public bool SearchEquals { get; set; } = false;
        private string _priceOrDiscount { get; set; } = "0";
        private static readonly HttpClient _client = new HttpClient();

        private const string SteamStoreListUrl = "https://api.steampowered.com/ISteamApps/GetAppList/v2/";

        public async Task<List<NamesGames>> DesAllGamesAsync()
        {
            string json = await _client.GetStringAsync(SteamStoreListUrl);
            var result = JsonConvert.DeserializeObject<SteamAppListResponse>(json);
            return result.applist.apps;
        }

        public async Task<SaveEntry> FoundGameAppIdAsync(string gameName, bool SearchEquals)
        {

            var allGames = await DesAllGamesAsync();
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

            var parse = await _client.GetStringAsync($"https://store.steampowered.com/api/appdetails?appids={foundGame.AppId}&cc=ua");//&cc=ua Ukranian region. If i want to get languange Ukrainian i need paste this &cc=ua&l=ukrainian  

            var gameDetails = JsonConvert.DeserializeObject<Dictionary<string, AppDeatailsResponse>>(parse);
            if (gameDetails.TryGetValue(foundGame.AppId.ToString(), out var appDetails) && appDetails.success)
            {
                Name = appDetails.data.name;
                Discount = appDetails.data.price_overview?.discount_percent.ToString() ?? _priceOrDiscount;
                FinalPrice = appDetails.data.price_overview?.final_formatted ?? _priceOrDiscount;
                HeaderImage = new InputMediaPhoto { Media = appDetails.data.header_image };

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
            SaveDB saveDB = new SaveDB();

            foreach (var game in saveDB.GetSavedGames(chatId))
            {
                await FoundGameAppIdAsync(game, true);
            }
        }

        public async Task<List<SaveEntry>> FoundAlertGames(long chatId, int PrecentDiscount = 0)
        {
            SaveDB saveDB = new SaveDB();
            var result = new List<SaveEntry>();
            if (saveDB.GetSavedGames != null)
            {               
                foreach (var game in saveDB.GetSavedGames(chatId))
                {
                    var entry = await FoundGameAppIdAsync(game, true);
                    if (entry != null && entry.discount_percent >= PrecentDiscount)
                    {
                        result.Add(entry);
                    }
                }
            }
            return result;
        }



        public async Task DeleteFromSave(long chatId, string gameNameToDelete)
        {
            SaveDB saveDB = new SaveDB();

            saveDB.DeleteGameName(chatId, gameNameToDelete);
        }
    }
}

//https://api.steampowered.com/ISteamApps/GetAppList/v2/ all games
//@"https://store.steampowered.com/api/appdetails?appids={gameId}" current game info
