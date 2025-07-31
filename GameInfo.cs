using Newtonsoft.Json;
using Save;
using Telegram.Bot.Types;
using static GamesDiscounts.FoundGames;


namespace GamesDiscounts
{
    internal class GameInfo
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
                Console.WriteLine($"[ERROR] Игра '{gameName}' не найдена в списке");
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
            if (foundGame == null)
            {
                return new SaveEntry
                {
                    Name = "not found",
                    discount_percent = null,
                    final_formatted = null,
                    header_image = null
                };
            }
            return new SaveEntry
            {
                Name = appDetails.data.name,
                discount_percent = appDetails.data.price_overview?.discount_percent,
                final_formatted = appDetails.data.price_overview?.final_formatted,
                header_image = appDetails.data.header_image
            };
        }

        //to do refactor this method
        public async Task<List<SaveEntry>> FoundSavedGames(long chatId)
        {

            SaveEntry saveEntry = new SaveEntry();
            SaveService saveService = new SaveService();

            var json = File.ReadAllText(Save.SaveService.FilePath);
            var savedGames = JsonConvert.DeserializeObject<List<SaveEntry>>(json);
            var userGame = savedGames.Where(game => game.Chat == chatId).ToList();
            var result = new List<SaveEntry>();

            foreach (var game in userGame)
            {
                try
                {
                    if (game.Chat == chatId)
                    {
                        await FoundGameAppIdAsync(game.Name, true);
                        result.Add(new SaveEntry
                        {
                            Chat = game.Chat,
                            Name = game.Name,
                            discount_percent = int.TryParse(this.Discount, out var discount) ? discount : null,
                            final_formatted = this.FinalPrice,
                            header_image = this.HeaderImage?.Media.ToString()
                        });

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Error processing game {game.Name}: {ex.Message}");
                    continue; // Skip this game if an error occurs
                }
            }
            return result;
        }
        public async Task DeleteFromSave(string gameNameToDelete, long chatId)
        {
            SaveEntry saveEntry = new SaveEntry();
            SaveService saveService = new SaveService();

            var json = File.ReadAllText(Save.SaveService.FilePath);
            var savedGames = JsonConvert.DeserializeObject<List<SaveEntry>>(json) ?? new List<SaveEntry>();

            savedGames.RemoveAll(game => game.Chat == chatId && game.Name == gameNameToDelete);
            var updatedJson = JsonConvert.SerializeObject(savedGames, Formatting.Indented);
            File.WriteAllText(Save.SaveService.FilePath, updatedJson);
        }
    }
}

//https://api.steampowered.com/ISteamApps/GetAppList/v2/ all games
//@"https://store.steampowered.com/api/appdetails?appids={gameId}" current game info
