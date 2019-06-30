namespace Models.Tags
{
    public class TagSearchInfo
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public string[] Tags { get; set; }
    }
}