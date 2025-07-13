using Newtonsoft.Json;

public class SteamAppListResponse
{
    public AppList applist { get; set; }
}

public class AppList
{
    public List<NamesGames> apps { get; set; }
}

public class NamesGames
{
    [JsonProperty("appid")]
    public int AppId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }
}