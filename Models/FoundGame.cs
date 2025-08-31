namespace GamesDiscounts.Models
{
    internal class FoundGame
    {
        public class AppDeatailsResponse
        {
            public bool success { get; set; }
            public Data? data { get; set; }
        }

        public class Data
        {
            public string? type { get; set; }
            public string? name { get; set; }
            public int? steam_appid { get; set; }
            public bool? is_free { get; set; }
            public string? header_image { get; set; }
            public Price_Overview? price_overview { get; set; }

        }

        public class Price_Overview
        {
            public string? currency { get; set; }
            public int? initial { get; set; }
            public int? final { get; set; }
            public int? discount_percent { get; set; }
            public string? initial_formatted { get; set; }
            public string? final_formatted { get; set; }
        }
    }
}
