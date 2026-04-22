using ArtGallery.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtGallery.Controllers;

public class AuthorsController : Controller
{
    private readonly AppDbContext _db;

    public AuthorsController(AppDbContext db) => _db = db;

    // GET /Authors
    public async Task<IActionResult> Index()
    {
        var authors = await _db.Authors.OrderBy(a => a.Name).ToListAsync();
        return View(authors);
    }

    // GET /Authors/Details/1
    public async Task<IActionResult> Details(int id)
    {
        var author = await _db.Authors.FindAsync(id);
        if (author == null) return NotFound();

        var paintings = await _db.Paintings
            .Where(p => p.Author == author.Name)
            .ToListAsync();

        ViewBag.Paintings = paintings;
        return View(author);
    }

    // AJAX GET /Authors/Bio?name=Ван Гог  (Сценарий 4 из ЛР1)
    [HttpGet]
    public async Task<IActionResult> Bio(string name)
    {
        var author = await _db.Authors
            .FirstOrDefaultAsync(a => a.Name == name);

        if (author == null)
            return Json(new { found = false });

        return Json(new
        {
            found   = true,
            id      = author.Id,
            name    = author.Name,
            country = author.Country,
            years   = author.Years,
            style   = author.Style,
            bio     = author.Bio,
            photoUrl= author.PhotoUrl,
        });
    }
}
