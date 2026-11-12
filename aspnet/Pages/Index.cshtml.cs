using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Veritas.Web.Data;
using Veritas.Web.Models;

namespace Veritas.Web.Pages;

public sealed class IndexModel(IntentionsDb db) : PageModel
{
    [BindProperty] public string Name { get; set; } = "";
    [BindProperty] public string Location { get; set; } = "";
    [BindProperty] public string Text { get; set; } = "";
    [BindProperty] public bool Consent { get; set; }
    [BindProperty(SupportsGet = true)] public string? Month { get; set; }
    [BindProperty(SupportsGet = true)] public string? Language { get; set; }
    [BindProperty(SupportsGet = true)] public string? PageName { get; set; }
    public IReadOnlyList<Intention> Intentions { get; private set; } = [];
    public IReadOnlyList<string> Months { get; private set; } = [];
    public string CurrentMonth { get; private set; } = DateTime.UtcNow.ToString("yyyy-MM");
    public string CurrentPage => PageName?.ToLowerInvariant() ?? "home";
    public string CurrentLanguage => string.Equals(Language, "zh-TW", StringComparison.OrdinalIgnoreCase) ? "zh-TW" : "en";
    public bool IsChinese => CurrentLanguage == "zh-TW";
    public bool IsIntentions => CurrentPage is "prayer-intentions" or "intentions";
    public string LocalePath(string language) => $"/{(language == "zh-TW" ? "zh-tw/" : "")}{(CurrentPage == "home" ? "" : CurrentPage)}";
    public string MonthLabel(string value) => DateTime.ParseExact(value, "yyyy-MM", null).ToString(IsChinese ? "yyyy年 M月" : "MMMM yyyy", IsChinese ? System.Globalization.CultureInfo.GetCultureInfo("zh-TW") : System.Globalization.CultureInfo.GetCultureInfo("en-GB"));

    public async Task OnGetAsync()
    {
        if (!IsIntentions) return;
        CurrentMonth = DateTime.TryParseExact(Month, "yyyy-MM", null, System.Globalization.DateTimeStyles.None, out var parsed) ? parsed.ToString("yyyy-MM") : DateTime.UtcNow.ToString("yyyy-MM");
        Months = await db.Intentions.AsNoTracking().Where(i => i.Language == CurrentLanguage).Select(i => i.CreatedAt.Substring(0, 7)).Distinct().OrderByDescending(m => m).ToListAsync();
        if (!Months.Contains(CurrentMonth)) Months = new[] { CurrentMonth }.Concat(Months).ToArray();
        Intentions = await db.Intentions.AsNoTracking().Where(i => i.Language == CurrentLanguage && i.CreatedAt.StartsWith(CurrentMonth)).OrderByDescending(i => i.CreatedAt).ToListAsync();
    }

    public async Task<IActionResult> OnPostShareAsync()
    {
        Language = CurrentLanguage;
        if (string.IsNullOrWhiteSpace(Name) || Name.Trim().Length > 100) ModelState.AddModelError(nameof(Name), IsChinese ? "請輸入姓名（最多100字）。" : "Enter your name (up to 100 characters).");
        if (string.IsNullOrWhiteSpace(Location) || Location.Trim().Length > 100) ModelState.AddModelError(nameof(Location), IsChinese ? "請輸入地點（最多100字）。" : "Enter your location (up to 100 characters).");
        if (string.IsNullOrWhiteSpace(Text) || Text.Trim().Length > 500) ModelState.AddModelError(nameof(Text), IsChinese ? "意向不可超過500字。" : "Your intention must be 500 characters or fewer.");
        if (!Consent) ModelState.AddModelError(nameof(Consent), IsChinese ? "分享前必須同意公開。" : "Consent is required before sharing.");
        if (!ModelState.IsValid) { PageName = "prayer-intentions"; await OnGetAsync(); return base.Page(); }
        db.Intentions.Add(new Intention { Id = Guid.NewGuid().ToString(), Name = Name.Trim(), Location = Location.Trim(), Text = Text.Trim(), Language = CurrentLanguage, CreatedAt = DateTime.UtcNow.ToString("O") });
        await db.SaveChangesAsync();
        return Redirect($"/{(IsChinese ? "zh-tw/" : "")}prayer-intentions?month={DateTime.UtcNow:yyyy-MM}");
    }
}
