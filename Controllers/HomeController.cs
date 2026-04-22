using ArtGallery.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtGallery.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        // На главной показываем 3 случайные картины как анонс
        var allPaintings = await _db.Paintings.ToListAsync();
        var random = new Random();
        var featured = allPaintings.OrderBy(_ => random.Next()).Take(3).ToList();

        return View(featured);
    }
}
