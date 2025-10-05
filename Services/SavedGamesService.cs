using GamesDiscounts.Models;

namespace GamesDiscounts.Services
{
    public class SavedGamesService : ISavedGamesService
    {
        private readonly IDataBase _dataBase;
        private readonly IGameInfo _gameInfo;

        public SavedGamesService(IDataBase dataBase, IGameInfo gameInfo)
        {
            _dataBase = dataBase;
            _gameInfo = gameInfo;
        }

        public async Task<List<string>> GetSavedGamesAsync(long chatId)
        {
            return await _dataBase.GetSavedGames(chatId);
        }

        public async Task SaveGameAsync(long chatId, string gameName)
        {
            var savedGames = await GetSavedGamesAsync(chatId);
            if (savedGames.Any(game => game.Equals(gameName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Game already saved.");
            var gameDetails = await _gameInfo.FindGameByNameAsync(gameName, true);
            if (gameDetails == null)
                throw new InvalidOperationException("Game not found.");

            await _dataBase.SaveGameNameAsync(chatId, gameName);
        }
        public async Task<List<GameDetailsDto>> SaleGameAsync(long chatId)
        {
            var savedGames = await GetSavedGamesAsync(chatId);
            if (savedGames.Count == 0 || savedGames == null)
            { return new List<GameDetailsDto>(); }


            var tasks = savedGames.Select(game => _gameInfo.FindGameByNameAsync(game, true));
            var games = await Task.WhenAll(tasks);
            return games.Where(game => game != null).ToList();

        }


        public async Task SaveAlertState(long chatId, bool alertsEnabled, int minDiscount = 0)
        {
            await _dataBase.SetAlertsAsync(chatId, alertsEnabled, minDiscount);
        }
        public async Task<List<AlertSettings>> GetAlertStateAsync(long chatId)
        {
            return await _dataBase.GetAllEnabledAlertsAsync();
        }
        public async Task<List<GameDetailsDto>> GetAlertGamesAsync(long chatId,int minDiscount = 0)
        {
            var savedGames = await GetSavedGamesAsync(chatId);
            if (savedGames == null || savedGames.Count == 0)
                return new List<GameDetailsDto>();

            // Получаем все игры параллельно
            var tasks = savedGames.Select(game => _gameInfo.FindGameByNameAsync(game, true));
            var games = await Task.WhenAll(tasks);

            // Фильтруем по скидке
            return games.Where(game => game != null && game.discount_percent >= minDiscount).ToList();

        }



        public Task<List<AlertSettings>> GetAllAlertsAsync()
        {
            // вернуть список всех чатов, где включены алерты
            throw new NotImplementedException();
        }




        public async Task DeleteSavedGameAsync(long chatId, string gameNameToDelete)
        {
            await _dataBase.DeleteGameName(chatId, gameNameToDelete);
        }
    }
}
