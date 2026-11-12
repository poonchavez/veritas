using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Veritas.Web.Pages;

public sealed class IndexModel : PageModel
{
    public string? PageName { get; private set; }
    public string? Language { get; private set; }

    public string CurrentPage => (PageName ?? "home").ToLowerInvariant();
    public string CurrentLanguage => string.Equals(Language, "zh-tw", StringComparison.OrdinalIgnoreCase) ? "zh-TW" : "en";
    public bool IsChinese => CurrentLanguage == "zh-TW";
    public string PageTitle => CurrentPage switch
    {
        "readings" => IsChinese ? "每日讀經" : "Daily readings",
        "prayer-library" => IsChinese ? "祈禱文庫" : "Prayer library",
        "rosary" => IsChinese ? "玫瑰經" : "The Rosary",
        "holy-mass" => IsChinese ? "彌撒常規" : "Order of Holy Mass",
        "about" => IsChinese ? "關於 Veritas" : "About Veritas",
        _ => IsChinese ? "靜心祈禱" : "A quiet place to pray"
    };

    public string PageUrl(string page)
    {
        var prefix = IsChinese ? "/zh-tw" : "";
        return page == "home" ? (IsChinese ? "/zh-tw/" : "/") : $"{prefix}/{page}";
    }

    public string LocaleUrl(string page, string locale) => page == "home"
        ? (locale == "zh-TW" ? "/zh-tw/" : "/")
        : $"{(locale == "zh-TW" ? "/zh-tw" : "")}/{page}";

    public void OnGet(string? pageName, string? language)
    {
        PageName = pageName;
        Language = language;
    }
}
