using Newtonsoft.Json;


namespace GamesDiscounts
{
    internal class GameInfo
    {
        public string? discount { get; set; }
        public string? finalPrice { get; set; }
        public List<NamesGames> DesAllGames()
        {
            string SteamStoreList = @"https://api.steampowered.com/ISteamApps/GetAppList/v2/";
            List<NamesGames> namesGames = JsonConvert.DeserializeObject<List<NamesGames>>(SteamStoreList);
            return namesGames;
        }
        public List<FoundGames.Price_Overview> FoundGames(List<FoundGames.Price_Overview> price_Overview)
        {
            foreach (var foundGamesDiscount in FoundGames(price_Overview))
            {
                finalPrice = foundGamesDiscount.final_formatted.ToString();
                discount = foundGamesDiscount.discount_percent.ToString();
            }
            return FoundGames(price_Overview);
        }
        public async Task FoundGameAppId(string gameName)
        {
            foreach (var game in DesAllGames())
            {
                if (game.Name != null && game.Name.Contains(gameName, StringComparison.OrdinalIgnoreCase))
                {
                    List<FoundGames.Price_Overview> price_Overview = JsonConvert.DeserializeObject<List<FoundGames.Price_Overview>>(@$"https://store.steampowered.com/api/appdetails?appids={game.AppId}");
                    FoundGames(price_Overview);

                }
            }
            return;
        }
    }
}

//https://api.steampowered.com/ISteamApps/GetAppList/v2/ all games
//@"https://store.steampowered.com/api/appdetails?appids={gameId}" current game info
