using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ArtGallery.ViewModels;

// ViewModel для формы добавления картины (ЛР5)
public class AddPaintingViewModel
{
    public IFormFile? ImageFile { get; set; }
    [Required(ErrorMessage = "Введите название картины")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Название: от 2 до 255 символов")]
    [RegularExpression(@"^[А-ЯA-Z«\(].+",
        ErrorMessage = "Первый символ должен быть заглавной буквой или «")]
    public string Title { get; set; } = "";

    [Required(ErrorMessage = "Введите имя автора")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Имя автора: от 2 до 255 символов")]
    [RegularExpression(@"^[А-ЯA-Z].+",
        ErrorMessage = "Первый символ должен быть заглавной буквой")]
    public string Author { get; set; } = "";

    [Required(ErrorMessage = "Выберите стиль")]
    public string Style { get; set; } = "";

    [Required(ErrorMessage = "Введите год создания")]
    [Range(1000, 2100, ErrorMessage = "Год должен быть от 1000 до 2100")]
    public int? Year { get; set; }

    [Required(ErrorMessage = "Выберите страну")]
    public string Country { get; set; } = "";

    [StringLength(255)]
    public string Materials { get; set; } = "";

    // Формат: "73.7 × 92.1 см"
    [RegularExpression(@"^\d+(\.\d+)?\s?[×xх]\s?\d+(\.\d+)?\s?см$",
        ErrorMessage = "Формат: 73.7 × 92.1 см")]
    [StringLength(100)]
    public string Size { get; set; } = "";

    [StringLength(500)]
    public string ImageUrl { get; set; } = "";

    public string Description { get; set; } = "";

    // Для заполнения выпадающих списков
    public List<string> AllStyles    { get; set; } = new();
    public List<string> AllCountries { get; set; } = new();
}
