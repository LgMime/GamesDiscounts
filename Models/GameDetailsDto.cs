using Newtonsoft.Json;

namespace GamesDiscounts.Models
{
    public class GameDetailsDto
    {
        public string? Name { get; set; }

        [JsonProperty("discount_percent")]
        public int DiscountPercent { get; set; }

        [JsonProperty("final_formatted")]
        public string? FinalFormatted { get; set; }

        [JsonProperty("header_image")]
        public string? HeaderImage { get; set; }
    }
}
