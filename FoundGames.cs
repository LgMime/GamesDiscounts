namespace GamesDiscounts
{
    internal class FoundGames
    {
        public class AppDeatailsResponse
        {
            public bool success { get; set; }
            public Data data { get; set; }
        }

        public class Data
        {
            public string type { get; set; }
            public string name { get; set; }
            public int steam_appid { get; set; }
            public int required_age { get; set; }
            public bool is_free { get; set; }
            public string detailed_description { get; set; }
            public string about_the_game { get; set; }
            public string short_description { get; set; }
            public string supported_languages { get; set; }
            public string header_image { get; set; }
            public string capsule_image { get; set; }
            public string capsule_imagev5 { get; set; }
            public object website { get; set; }
            public string[] developers { get; set; }
            public string[] publishers { get; set; }
            public Price_Overview price_overview { get; set; }
            public int[] packages { get; set; }
            public Screenshot[] screenshots { get; set; }
            public string background { get; set; }
            public string background_raw { get; set; }

        }


        public class Price_Overview
        {
            public string currency { get; set; }
            public int initial { get; set; }
            public int final { get; set; }
            public int discount_percent { get; set; }
            public string initial_formatted { get; set; }
            public string final_formatted { get; set; }
        }



        public class Screenshot
        {
            public int id { get; set; }
            public string path_thumbnail { get; set; }
            public string path_full { get; set; }
        }

    }
}
