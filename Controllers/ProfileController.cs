using ArtGallery.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtGallery.Controllers;

public class ProfileController : Controller
{
    private readonly AppDbContext _db;
    public ProfileController(AppDbContext db) => _db = db;

    // GET /Profile
    public async Task<IActionResult> Index()
    {
        var raw = HttpContext.Session.GetString("favorites") ?? "";
        var ids = raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(int.Parse).ToList();

        var favorites = ids.Any()
            ? await _db.Paintings.Where(p => ids.Contains(p.Id)).ToListAsync()
            : new();

        // ВАЖНО: Передаём через ViewBag, а не через модель
        ViewBag.Favorites = favorites;
        return View();
    }

    // POST /Profile/RemoveFromFavorites
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveFromFavorites(int id)
    {
        var raw = HttpContext.Session.GetString("favorites") ?? "";
        var ids = raw.Split(',', StringSplitOptions.RemoveEmptyEntries)
                     .Select(int.Parse).ToList();

        if (ids.Contains(id))
        {
            ids.Remove(id);
            HttpContext.Session.SetString("favorites", string.Join(',', ids));
            TempData["SuccessMessage"] = "Картина удалена из избранного.";
        }

        return RedirectToAction("Index");
    }
}