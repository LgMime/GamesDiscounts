using Newtonsoft.Json;



namespace Save
{
    public class SaveService
    {
        public static string FilePath { get; set; } = "SaveData.json";

        public static List<SaveEntry> Load()
        {
            if (!File.Exists(FilePath))
                return new List<SaveEntry>();

            string json = File.ReadAllText(FilePath);
            return JsonConvert.DeserializeObject<List<SaveEntry>>(json) ?? new List<SaveEntry>();
        }
        public static void Save(List<SaveEntry> item)
        {
            string json = JsonConvert.SerializeObject(item, Formatting.Indented);
            File.WriteAllText(FilePath, json);
        }
        public static void AddItem(SaveEntry newItem)
        {
            var item = Load();
            item.Add(newItem);
            Save(item);
        }
    }
}
