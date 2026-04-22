using ArtGallery.Data;
using ArtGallery.Models;
using ArtGallery.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtGallery.Controllers;

public class CatalogController : Controller
{
    private readonly AppDbContext _db;
    private const int PageSize = 8;

    public CatalogController(AppDbContext db)
    {
        _db = db;
    }

    // ЛР4 Сценарий №1: Первоначальная загрузка каталога ─────────────────
    // GET /Catalog
    public async Task<IActionResult> Index(
        string? search,
        List<string>? styles,
        List<string>? countries,
        int? yearFrom,
        int? yearTo,
        string sort = "name-asc",
        int page = 1)
    {
        var favoriteIds = GetFavoriteIds();

        var vm = new CatalogViewModel
        {
            Search = search,
            SelectedStyles = styles ?? new(),
            SelectedCountries = countries ?? new(),
            YearFrom = yearFrom,
            YearTo = yearTo,
            Sort = sort,
            CurrentPage = page,
            PageSize = PageSize,
            AllStyles = await _db.Paintings.Select(p => p.Style).Distinct().OrderBy(s => s).ToListAsync(),
            AllCountries = await _db.Paintings.Select(p => p.Country).Distinct().OrderBy(c => c).ToListAsync(),
            FavoriteIds = favoriteIds,
        };

        var query = BuildQuery(search, styles, countries, yearFrom, yearTo, sort);
        vm.TotalCount = await query.CountAsync();
        vm.TotalPages = (int)Math.Ceiling(vm.TotalCount / (double)PageSize);
        vm.Paintings = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

        return View(vm);
    }

    // ── ЛР3/ЛР4 AJAX: Живой счётчик  ──────────────────
    // GET /Catalog/Count
    [HttpGet]
    public async Task<IActionResult> Count(
        string? search, List<string>? styles, List<string>? countries,
        int? yearFrom, int? yearTo)
    {
        var count = await BuildQuery(search, styles, countries, yearFrom, yearTo, "name-asc").CountAsync();
        return Json(new { count });
    }

    // ── ЛР3/ЛР4 AJAX: Умный поиск с подсказками ──────
    [HttpGet]
    public async Task<IActionResult> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 1)
            return Json(new List<object>());

        // Используем EF.Functions.Like для корректной работы с кириллицей
        var results = await _db.Paintings
            .Where(p => EF.Functions.Like(p.Title, $"%{q}%") || EF.Functions.Like(p.Author, $"%{q}%"))
            .Select(p => new {
                p.Id,
                label = p.Title + " — " + p.Author,
                p.Title,
                p.Author,
                priority = p.Title.StartsWith(q) ? 1 : (p.Author.StartsWith(q) ? 2 : 3)
            })
            .OrderBy(p => p.priority)
            .ThenBy(p => p.Title)
            .Take(8)
            .Select(p => new { p.Id, p.label, p.Title, p.Author })
            .ToListAsync();

        return Json(results);
    }

    // ── ЛР3/ЛР4 AJAX: Карточка картины ───────────────
    // GET /Catalog/PaintingInfo
    [HttpGet]
    public async Task<IActionResult> PaintingInfo(int id)
    {
        var p = await _db.Paintings.FindAsync(id);
        if (p == null) return NotFound();

        return Json(new PaintingCardDto
        {
            Id = p.Id,
            Title = p.Title,
            Author = p.Author,
            Style = p.Style,
            Year = p.Year,
            Country = p.Country,
            Materials = p.Materials,
            Size = p.Size,
            ImageUrl = p.ImageUrl,
            Description = p.Description,
            IsFavorite = GetFavoriteIds().Contains(p.Id),
        });
    }

    // ── ЛР3 AJAX: Добавление/удаление из избранного ──
    // POST /Catalog/ToggleFavorite
    [HttpPost]
    public IActionResult ToggleFavorite([FromBody] FavoriteRequest req)
    {
        var favorites = GetFavoriteIds();

        if (favorites.Contains(req.PaintingId))
        {
            favorites.Remove(req.PaintingId);
            SaveFavoriteIds(favorites);
            return Json(new { status = "removed", message = "Картина удалена из избранного" });
        }
        else
        {
            favorites.Add(req.PaintingId);
            SaveFavoriteIds(favorites);
            return Json(new { status = "added", message = "Картина добавлена в избранное!" });
        }
    }

    // ── ЛР3/ЛР4 AJAX: Partial view — обновление сетки без перезагрузки ────
    // GET /Catalog/Grid
    [HttpGet]
    public async Task<IActionResult> Grid(
        string? search, List<string>? styles, List<string>? countries,
        int? yearFrom, int? yearTo, string sort = "name-asc", int page = 1)
    {
        var favoriteIds = GetFavoriteIds();
        var query = BuildQuery(search, styles, countries, yearFrom, yearTo, sort);
        var total = await query.CountAsync();
        var paintings = await query.Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();

        var vm = new CatalogViewModel
        {
            Paintings = paintings,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)PageSize),
            CurrentPage = page,
            PageSize = PageSize,
            FavoriteIds = favoriteIds,
            Sort = sort,
        };

        return PartialView("_PaintingsGrid", vm);
    }

    // ── Хелперы ────────────────────────────────────────────────────────────

    private IQueryable<Painting> BuildQuery(
        string? search, List<string>? styles, List<string>? countries,
        int? yearFrom, int? yearTo, string sort)
    {
        var q = _db.Paintings.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {

            q = q.Where(p => EF.Functions.Like(p.Title, $"%{search}%") ||
                             EF.Functions.Like(p.Author, $"%{search}%"));
        }

        // ЛР4 Сценарий №2: фильтр по году
        if (yearFrom.HasValue)
            q = q.Where(p => p.Year >= yearFrom.Value);
        if (yearTo.HasValue)
            q = q.Where(p => p.Year <= yearTo.Value);

        // ЛР4 Сценарий №3: фильтр по стилю
        if (styles != null && styles.Count > 0)
            q = q.Where(p => styles.Contains(p.Style));

        // ЛР4 Сценарий №4: комбинированный — стиль + страна
        if (countries != null && countries.Count > 0)
            q = q.Where(p => countries.Contains(p.Country));

        q = sort switch
        {
            "name-desc" => q.OrderByDescending(p => p.Title),
            "newest" => q.OrderByDescending(p => p.Year),
            "oldest" => q.OrderBy(p => p.Year),
            _ => q.OrderBy(p => p.Title),
        };

        return q;
    }

    private List<int> GetFavoriteIds()
    {
        var raw = HttpContext.Session.GetString("favorites");
        if (string.IsNullOrEmpty(raw)) return new List<int>();
        return raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                  .Select(int.Parse).ToList();
    }

    private void SaveFavoriteIds(List<int> ids)
    {
        HttpContext.Session.SetString("favorites", string.Join(',', ids));
    }
}

public class FavoriteRequest
{
    public int PaintingId { get; set; }
}