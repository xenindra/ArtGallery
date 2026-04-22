using System.ComponentModel.DataAnnotations;

namespace ArtGallery.Models;

public class Painting
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Название обязательно")]
    [StringLength(255)]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Автор обязателен")]
    [StringLength(255)]
    public string Author { get; set; } = "";

    [Required(ErrorMessage = "Стиль обязателен")]
    [StringLength(100)]
    public string Style { get; set; } = "";

    [Required(ErrorMessage = "Год обязателен")]
    [Range(1000, 2100, ErrorMessage = "Год должен быть от 1000 до 2100")]
    public int Year { get; set; }

    [Required(ErrorMessage = "Страна обязательна")]
    [StringLength(100)]
    public string Country { get; set; } = "";

    [StringLength(255)]
    public string Materials { get; set; } = "";

    [StringLength(100)]
    public string Size { get; set; } = "";

    [StringLength(500)]
    public string ImageUrl { get; set; } = "/images/paintings/placeholder.jpg";

    public string Description { get; set; } = "";
}
