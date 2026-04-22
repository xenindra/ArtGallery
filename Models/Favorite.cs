namespace ArtGallery.Models;

public class Favorite
{
    public int Id { get; set; }
    public int PaintingId { get; set; }
    public string SessionId { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Painting? Painting { get; set; }
}
