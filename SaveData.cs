namespace Save
{
    public class SaveEntry //my jsoon structure for saving data
    {
        public long Chat { get; set; }
        public string? Name { get; set; }
        public int? discount_percent { get; set; }
        public string? final_formatted { get; set; }
        public string? header_image { get; set; }
    }
}
