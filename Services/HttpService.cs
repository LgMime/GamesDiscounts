using GamesDiscounts.Models;

namespace GamesDiscounts.Services
{
    public class HttpService
    {
        private readonly HttpClient _client = new HttpClient();

        public Task<string> GetStringAsync(string url)
        {
            return _client.GetStringAsync(url);
        }

    }
}
