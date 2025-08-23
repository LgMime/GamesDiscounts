namespace GamesDiscounts.Models
{
    public interface IDataBase
    {
        void SaveGameName(long ChatId, string GameName);
        List<string> GetSavedGames(long ChatId);
        void DeleteGameName(long ChatId, string GameName);


    }
}
