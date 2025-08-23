namespace GamesDiscounts.Models
{
    public interface IHttpService
    {
        Task<string> GetStringAsync(string url);

    }
}
