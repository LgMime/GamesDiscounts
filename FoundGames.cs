namespace GamesDiscounts
{
    internal class FoundGames
    {

        public class Rootobject
        {
            public _10 _10 { get; set; }
        }

        public class _10
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
            public Pc_Requirements pc_requirements { get; set; }
            public Mac_Requirements mac_requirements { get; set; }
            public Linux_Requirements linux_requirements { get; set; }
            public string[] developers { get; set; }
            public string[] publishers { get; set; }
            public Price_Overview price_overview { get; set; }
            public int[] packages { get; set; }
            public Package_Groups[] package_groups { get; set; }
            public Platforms platforms { get; set; }
            public Metacritic metacritic { get; set; }
            public Category[] categories { get; set; }
            public Genre[] genres { get; set; }
            public Screenshot[] screenshots { get; set; }
            public Recommendations recommendations { get; set; }
            public Release_Date release_date { get; set; }
            public Support_Info support_info { get; set; }
            public string background { get; set; }
            public string background_raw { get; set; }
            public Content_Descriptors content_descriptors { get; set; }
            public Ratings ratings { get; set; }
        }

        public class Pc_Requirements
        {
            public string minimum { get; set; }
        }

        public class Mac_Requirements
        {
            public string minimum { get; set; }
        }

        public class Linux_Requirements
        {
            public string minimum { get; set; }
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

        public class Platforms
        {
            public bool windows { get; set; }
            public bool mac { get; set; }
            public bool linux { get; set; }
        }

        public class Metacritic
        {
            public int score { get; set; }
            public string url { get; set; }
        }

        public class Recommendations
        {
            public int total { get; set; }
        }

        public class Release_Date
        {
            public bool coming_soon { get; set; }
            public string date { get; set; }
        }

        public class Support_Info
        {
            public string url { get; set; }
            public string email { get; set; }
        }

        public class Content_Descriptors
        {
            public int[] ids { get; set; }
            public string notes { get; set; }
        }

        public class Ratings
        {
            public Usk usk { get; set; }
            public Dejus dejus { get; set; }
            public Steam_Germany steam_germany { get; set; }
        }

        public class Usk
        {
            public string rating { get; set; }
        }

        public class Dejus
        {
            public string rating_generated { get; set; }
            public string rating { get; set; }
            public string required_age { get; set; }
            public string banned { get; set; }
            public string use_age_gate { get; set; }
            public string descriptors { get; set; }
        }

        public class Steam_Germany
        {
            public string rating_generated { get; set; }
            public string rating { get; set; }
            public string required_age { get; set; }
            public string banned { get; set; }
            public string use_age_gate { get; set; }
            public string descriptors { get; set; }
        }

        public class Package_Groups
        {
            public string name { get; set; }
            public string title { get; set; }
            public string description { get; set; }
            public string selection_text { get; set; }
            public string save_text { get; set; }
            public int display_type { get; set; }
            public string is_recurring_subscription { get; set; }
            public Sub[] subs { get; set; }
        }

        public class Sub
        {
            public int packageid { get; set; }
            public string percent_savings_text { get; set; }
            public int percent_savings { get; set; }
            public string option_text { get; set; }
            public string option_description { get; set; }
            public string can_get_free_license { get; set; }
            public bool is_free_license { get; set; }
            public int price_in_cents_with_discount { get; set; }
        }

        public class Category
        {
            public int id { get; set; }
            public string description { get; set; }
        }

        public class Genre
        {
            public string id { get; set; }
            public string description { get; set; }
        }

        public class Screenshot
        {
            public int id { get; set; }
            public string path_thumbnail { get; set; }
            public string path_full { get; set; }
        }

    }
}
