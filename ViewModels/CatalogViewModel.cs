using ArtGallery.Models;

namespace ArtGallery.ViewModels;

public class CatalogViewModel
{
    public List<Painting> Paintings { get; set; } = new();
    public List<string> AllStyles { get; set; } = new();
    public List<string> AllCountries { get; set; } = new();

    // Текущие фильтры
    public string? Search { get; set; }
    public List<string> SelectedStyles { get; set; } = new();
    public List<string> SelectedCountries { get; set; } = new();
    public int? YearFrom { get; set; }
    public int? YearTo { get; set; }
    public string Sort { get; set; } = "name-asc";

    // Пагинация
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalCount { get; set; } = 0;
    public int PageSize { get; set; } = 9;

    // ID избранных (для текущей сессии)
    public List<int> FavoriteIds { get; set; } = new();
}

public class PaintingCardDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Style { get; set; } = "";
    public int Year { get; set; }
    public string Country { get; set; } = "";
    public string Materials { get; set; } = "";
    public string Size { get; set; } = "";
    public string ImageUrl { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsFavorite { get; set; }
}

