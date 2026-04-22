namespace ArtGallery.Models
{
    public class CollectionViewModel
    {
        public string Name { get; set; } = "";
        public string Subtitle { get; set; } = "";
        public string Period { get; set; } = "";
        public int Count { get; set; }
        public string Category { get; set; } = "";
        public string ImageUrl { get; set; } = "";
    }
}
