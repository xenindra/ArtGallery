using Microsoft.EntityFrameworkCore;
using ArtGallery.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// PostgreSQL подключение (работает и локально, и на Railway)
var connectionString = Environment.GetEnvironmentVariable("PG_CONNECTION_STRING")
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Сессии (для хранения избранного)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Применяем миграции при запуске (важно для Railway!)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "painting_details",
    pattern: "Paintings/{id:int}",
    defaults: new { controller = "Paintings", action = "Details" });

app.MapControllerRoute(
    name: "author_details",
    pattern: "Authors/{id:int}",
    defaults: new { controller = "Authors", action = "Details" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();