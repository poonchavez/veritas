using Microsoft.EntityFrameworkCore;
using Veritas.Web.Data;
using Veritas.Web.Models;

var builder = WebApplication.CreateBuilder(args);
var dbPath = Environment.GetEnvironmentVariable("INTENTIONS_DB_PATH") ?? "data/intentions.sqlite3";
var fullDbPath = Path.GetFullPath(dbPath);
Directory.CreateDirectory(Path.GetDirectoryName(fullDbPath)!);
builder.Services.AddDbContext<IntentionsDb>(options => options.UseSqlite($"Data Source={fullDbPath}"));
builder.Services.AddRazorPages();
builder.Services.AddAntiforgery();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IntentionsDb>();
    db.Database.EnsureCreated();
    await db.Database.ExecuteSqlRawAsync("CREATE INDEX IF NOT EXISTS IX_intentions_language_created_at ON intentions(language, created_at)");
}

app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapGet("/api/health/", () => Results.Ok(new { status = "ok" }));
app.MapGet("/api/intentions/", async (IntentionsDb db, string? month, string? language) =>
{
    month ??= DateTime.UtcNow.ToString("yyyy-MM");
    language ??= "en";
    if (!MonthKey.IsValid(month) || !SupportedLanguages.Contains(language))
        return Results.BadRequest(new { error = "Month must use YYYY-MM format and language must be 'en' or 'zh-TW'." });
    var intentions = await db.Intentions.AsNoTracking()
        .Where(i => i.Language == language && i.CreatedAt.StartsWith(month))
        .OrderByDescending(i => i.CreatedAt)
        .Select(i => new IntentionDto(i.Id, i.Text, i.Name, i.Location, i.Language, i.CreatedAt))
        .ToListAsync();
    var months = await db.Intentions.AsNoTracking().Where(i => i.Language == language)
        .Select(i => i.CreatedAt.Substring(0, 7)).Distinct().OrderByDescending(m => m).ToListAsync();
    return Results.Ok(new { month, language, months, intentions });
});
app.MapPost("/api/intentions/", async (CreateIntentionRequest request, IntentionsDb db) =>
{
    var name = request.Name?.Trim(); var location = request.Location?.Trim(); var text = request.Text?.Trim();
    if (string.IsNullOrWhiteSpace(text) || text.Length > 500) return Results.BadRequest(new { error = "Please write an intention of 1 to 500 characters." });
    if (string.IsNullOrWhiteSpace(name) || name.Length > 100) return Results.BadRequest(new { error = "Please enter a name of 1 to 100 characters." });
    if (string.IsNullOrWhiteSpace(location) || location.Length > 100) return Results.BadRequest(new { error = "Please enter a location of 1 to 100 characters." });
    if (request.Consent != true) return Results.BadRequest(new { error = "Consent is required before publishing an intention." });
    if (!SupportedLanguages.Contains(request.Language ?? "")) return Results.BadRequest(new { error = "Language must be 'en' or 'zh-TW'." });
    var item = new Intention { Id = Guid.NewGuid().ToString(), Text = text, Name = name, Location = location, Language = request.Language!, CreatedAt = DateTime.UtcNow.ToString("O") };
    db.Intentions.Add(item);
    await db.SaveChangesAsync();
    return Results.Json(new { intention = new IntentionDto(item.Id, item.Text, item.Name, item.Location, item.Language, item.CreatedAt) }, statusCode: 201);
});
app.MapFallbackToPage("/Index");
app.Run();

static class SupportedLanguages { public static readonly string[] Values = ["en", "zh-TW"]; public static bool Contains(string value) => Values.Contains(value, StringComparer.Ordinal); }
static class MonthKey { public static bool IsValid(string value) => DateTime.TryParseExact(value, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out _); }
