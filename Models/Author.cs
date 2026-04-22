using System.ComponentModel.DataAnnotations;

namespace ArtGallery.Models;

public class Author
{
    public int Id { get; set; }

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = "";

    [StringLength(100)]
    public string Country { get; set; } = "";

    [StringLength(50)]
    public string Years { get; set; } = "";

    [StringLength(100)]
    public string Style { get; set; } = "";

    public string Bio { get; set; } = "";

    [StringLength(500)]
    public string PhotoUrl { get; set; } = "/images/authors/placeholder.jpg";
}
