using ArtGallery.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtGallery.Controllers;

public class PaintingsController : Controller
{
    private readonly AppDbContext _db;

    public PaintingsController(AppDbContext db) => _db = db;

    // GET /Paintings/5
    public async Task<IActionResult> Details(int id)
    {
        var painting = await _db.Paintings.FindAsync(id);
        if (painting == null) return NotFound();

        // Похожие картины того же стиля
        var similar = await _db.Paintings
            .Where(p => p.Style == painting.Style && p.Id != id)
            .Take(3)
            .ToListAsync();

        ViewBag.Similar = similar;

        // Статус избранного
        var raw = HttpContext.Session.GetString("favorites") ?? "";
        var favoriteIds = raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                             .Select(int.Parse).ToList();
        ViewBag.IsFavorite = favoriteIds.Contains(id);

        return View(painting);
    }
}
