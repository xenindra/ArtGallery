using ArtGallery.Data;
using ArtGallery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtGallery.Controllers;

public class CollectionsController : Controller
{
    private readonly AppDbContext _db;

    public CollectionsController(AppDbContext db) => _db = db;

    public IActionResult Index()
    {
        var collections = new List<CollectionViewModel>
        {
            new() { Name = "Импрессионизм", Subtitle = "Искусство момента и света", Period = "1870–1920", Count = 1, Category = "style", ImageUrl = "/images/paintings/collection_impressionism.jpg" },
            new() { Name = "Сюрреализм", Subtitle = "Мир снов и подсознания", Period = "1920–1960", Count = 1, Category = "style", ImageUrl = "/images/paintings/collection_surrealism.jpg" },
            new() { Name = "Реализм", Subtitle = "Правдивое изображение жизни", Period = "1840–1900", Count = 4, Category = "style", ImageUrl = "/images/paintings/collection_realism.jpg" },
            new() { Name = "XIX век", Subtitle = "Эпоха реализма и импрессионизма", Period = "1800–1899", Count = 8, Category = "century", ImageUrl = "/images/paintings/collection_19century.jpg" },
            new() { Name = "XX век", Subtitle = "Время экспериментов и авангарда", Period = "1900–1999", Count = 5, Category = "century", ImageUrl = "/images/paintings/collection_20century.jpg" },
            new() { Name = "XXI век", Subtitle = "Современное искусство", Period = "2000–н.в.", Count = 3, Category = "century", ImageUrl = "/images/paintings/collection_21century.jpg" },
            new() { Name = "Русское искусство", Subtitle = "Шедевры русских художников", Period = "XIX–XXI вв.", Count = 6, Category = "theme", ImageUrl = "/images/paintings/collection_russian.jpg" },
            new() { Name = "Европейское искусство", Subtitle = "Классика и современность", Period = "XV–XXI вв.", Count = 7, Category = "theme", ImageUrl = "/images/paintings/collection_european.jpg" },
            new() { Name = "Современное искусство", Subtitle = "Актуальные работы", Period = "2000–н.в.", Count = 3, Category = "theme", ImageUrl = "/images/paintings/collection_modern.jpg" },
        };
        return View(collections);
    }

    public async Task<IActionResult> Single(string name)
    {
        IQueryable<Painting> query = _db.Paintings;

        if (name == "Импрессионизм")
        {
            query = query.Where(p => p.Style == "Импрессионизм");
        }
        else if (name == "Сюрреализм")
        {
            query = query.Where(p => p.Style == "Сюрреализм");
        }
        else if (name == "Реализм")
        {
            query = query.Where(p => p.Style == "Реализм");
        }
        else if (name == "XIX век")
        {
            query = query.Where(p => p.Year >= 1800 && p.Year <= 1899);
        }
        else if (name == "XX век")
        {
            query = query.Where(p => p.Year >= 1900 && p.Year <= 1999);
        }
        else if (name == "XXI век")
        {
            query = query.Where(p => p.Year >= 2000);
        }
        else if (name == "Русское искусство")
        {
            query = query.Where(p => p.Country == "Россия");
        }
        else if (name == "Европейское искусство")
        {
            query = query.Where(p => new[] { "Испания", "Франция", "Нидерланды", "Италия", "Норвегия" }.Contains(p.Country));
        }

        var paintings = await query.ToListAsync();
        ViewBag.CollectionName = name;
        ViewBag.CollectionSubtitle = GetSubtitle(name);
        ViewBag.CollectionPeriod = GetPeriod(name);
        ViewBag.CollectionDescription = GetDescription(name);

        return View(paintings);
    }

    private string GetSubtitle(string name)
    {
        if (name == "Импрессионизм") return "Искусство момента и света";
        if (name == "Сюрреализм") return "Мир снов и подсознания";
        if (name == "Реализм") return "Правдивое изображение жизни";
        if (name == "XIX век") return "Эпоха реализма и импрессионизма";
        if (name == "XX век") return "Время экспериментов и авангарда";
        if (name == "XXI век") return "Современное искусство";
        if (name == "Русское искусство") return "Шедевры русских художников";
        if (name == "Европейское искусство") return "Классика и современность";
        return "";
    }

    private string GetPeriod(string name)
    {
        if (name == "Импрессионизм") return "1870–1920";
        if (name == "Сюрреализм") return "1920–1960";
        if (name == "Реализм") return "1840–1900";
        if (name == "XIX век") return "1800–1899";
        if (name == "XX век") return "1900–1999";
        if (name == "XXI век") return "2000–н.в.";
        if (name == "Русское искусство") return "XIX–XXI вв.";
        if (name == "Европейское искусство") return "XV–XXI вв.";
        return "";
    }

    private string GetDescription(string name)
    {
        if (name == "Импрессионизм") return "Импрессионизм — художественное направление, возникшее во Франции в последней трети XIX века. Название происходит от картины Клода Моне «Впечатление. Восходящее солнце». Художники-импрессионисты стремились передать свои мимолётные впечатления от окружающего мира, уделяя особое внимание игре света и цвета. Они отказались от традиционной академической манеры письма в пользу работы на пленэре и использования чистых, ярких красок.";
        if (name == "Сюрреализм") return "Сюрреализм — направление в искусстве, сформировавшееся к началу 1920-х годов во Франции. Отличается использованием аллюзий и парадоксальных сочетаний форм. Художники-сюрреалисты вдохновлялись психоанализом Фрейда и стремились изобразить мир подсознания, снов и фантазий. Характерные черты: иррациональность, фантасмагоричность, совмещение реального и воображаемого.";
        if (name == "Реализм") return "Реализм — направление в искусстве, характеризующееся объективным изображением действительности. Возник как реакция на романтизм и академизм. Художники-реалисты стремились к точному и правдивому отображению окружающей действительности, часто обращаясь к социальным темам и жизни простых людей. Основные принципы: объективность, типизация, историческая конкретность.";
        if (name == "XIX век") return "XIX век — время кардинальных изменений в искусстве. Этот период ознаменовался переходом от классицизма к реализму и появлением новых художественных направлений. Искусство XIX века отражает социальные изменения, промышленную революцию и рост национального самосознания. Художники начинают обращаться к современным темам и жизни простых людей.";
        if (name == "XX век") return "XX век — эпоха художественных революций и радикальных экспериментов. Искусство становится более концептуальным и разнообразным по форме. Это время рождения авангарда, кубизма, сюрреализма, абстракционизма и поп-арта. Художники ломают традиционные представления о форме, цвете и композиции.";
        if (name == "XXI век") return "Искусство XXI века характеризуется глобализацией, цифровизацией и междисциплинарностью. Художники свободно экспериментируют с медиа и технологиями. Современное искусство часто обращается к актуальным социальным, политическим и экологическим проблемам. Стираются границы между высоким и массовым искусством.";
        if (name == "Русское искусство") return "Русское искусство обладает богатой историей и уникальным характером. От иконописи до авангарда — русские художники внесли значительный вклад в мировую культуру. XIX век — время расцвета реализма и деятельности передвижников. XX век ознаменовался революцией русского авангарда. Современное русское искусство продолжает развивать эти традиции.";
        if (name == "Европейское искусство") return "Европейское искусство — многовековая традиция, оказавшая огромное влияние на мировую культуру. От Возрождения до современности — европейские художники задавали тон в искусстве. Разнообразие школ и направлений: итальянское Возрождение, голландская живопись, французский импрессионизм, испанский сюрреализм, немецкий экспрессионизм.";
        return "Описание подборки";
    }
}