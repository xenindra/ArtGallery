using ArtGallery.Data;
using ArtGallery.Models;
using ArtGallery.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ArtGallery.Controllers;

// ЛР5: Добавление картины с двойной валидацией (клиент + сервер)
public class AddPaintingController : Controller
{
    private readonly AppDbContext _db;

    // Фиксированные списки для выбора
    private static readonly List<string> Styles = new()
    {
        "Возрождение", "Импрессионизм", "Кубизм", "Поп-арт",
        "Постимпрессионизм", "Реализм", "Современное искусство",
        "Супрематизм", "Сюрреализм", "Экспрессионизм"
    };

    private static readonly List<string> Countries = new()
    {
        "Германия", "Италия", "Испания", "Нидерланды",
        "Норвегия", "Россия", "США", "Франция", "Япония"
    };

    public AddPaintingController(AppDbContext db) => _db = db;

    // ── GET /AddPainting ─────────────────────────────────────────
    [HttpGet]
    public IActionResult Index()
    {
        var vm = new AddPaintingViewModel
        {
            AllStyles = Styles,
            AllCountries = Countries,
        };
        return View(vm);
    }

    // ── POST /AddPainting ────────────────────────────────────────
    // Сервер-сайд валидация перед записью в БД
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(AddPaintingViewModel vm)
    {
        // Заполняем списки обратно — они не приходят из формы
        vm.AllStyles = Styles;
        vm.AllCountries = Countries;

        // Убираем проверку ModelState для ImageUrl, т.к. оно может быть пустым при загрузке файла
        if (ModelState.ContainsKey("ImageUrl"))
            ModelState.Remove("ImageUrl");

        // Проверяем уникальность: нет ли уже такой картины
        if (!string.IsNullOrWhiteSpace(vm.Title) && !string.IsNullOrWhiteSpace(vm.Author))
        {
            var exists = await _db.Paintings.AnyAsync(p =>
                p.Title == vm.Title.Trim() && p.Author == vm.Author.Trim());

            if (exists)
                ModelState.AddModelError("Title",
                    "Картина с таким названием и автором уже есть в базе данных");
        }

        // Проверяем что стиль из разрешённого списка
        if (!string.IsNullOrWhiteSpace(vm.Style) && !Styles.Contains(vm.Style))
            ModelState.AddModelError("Style", "Выберите стиль из списка");

        // Проверяем что страна из разрешённого списка
        if (!string.IsNullOrWhiteSpace(vm.Country) && !Countries.Contains(vm.Country))
            ModelState.AddModelError("Country", "Выберите страну из списка");

        // Год не в будущем
        if (vm.Year.HasValue && vm.Year.Value > DateTime.Now.Year)
            ModelState.AddModelError("Year",
                $"Год не может быть больше текущего ({DateTime.Now.Year})");

        if (!ModelState.IsValid)
            return View(vm);

        // Обработка загруженного файла
        string imageUrl = vm.ImageUrl?.Trim() ?? "";

        if (vm.ImageFile != null && vm.ImageFile.Length > 0)
        {
            // Генерируем уникальное имя файла
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "paintings", fileName);

            // Создаём папку, если её нет
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            // Сохраняем файл
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await vm.ImageFile.CopyToAsync(stream);
            }

            imageUrl = "/images/paintings/" + fileName;
        }

        // Если ни файл, ни URL не указаны — ставим заглушку
        if (string.IsNullOrWhiteSpace(imageUrl))
            imageUrl = "/images/placeholder.jpg";

        var painting = new Painting
        {
            Title = vm.Title.Trim(),
            Author = vm.Author.Trim(),
            Style = vm.Style.Trim(),
            Year = vm.Year!.Value,
            Country = vm.Country.Trim(),
            Materials = (vm.Materials ?? "").Trim(),
            Size = (vm.Size ?? "").Trim(),
            ImageUrl = imageUrl,
            Description = (vm.Description ?? "").Trim(),
        };

        _db.Paintings.Add(painting);
        await _db.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"Картина «{painting.Title}» успешно добавлена в каталог!";

        return RedirectToAction("Index", "Catalog");
    }

    // ── AJAX: проверка названия в реальном времени ───────────────
    // GET /AddPainting/CheckTitle?title=Мона+Лиза&author=Леонардо
    [HttpGet]
    public async Task<IActionResult> CheckTitle(string title, string author)
    {
        if (string.IsNullOrWhiteSpace(title))
            return Json(new { exists = false });

        var exists = await _db.Paintings.AnyAsync(p =>
            p.Title == title.Trim() &&
            (string.IsNullOrWhiteSpace(author) || p.Author == author.Trim()));

        return Json(new { exists });
    }
}