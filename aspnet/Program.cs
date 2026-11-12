var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages(options =>
{
    const string pages = "readings|prayer-library|rosary|holy-mass|about";
    options.Conventions.AddPageRoute("/Index", "{language:regex(^(zh-tw)$)}");
    options.Conventions.AddPageRoute("/Index", $"{{PageName:regex(^({pages})$)}}");
    options.Conventions.AddPageRoute("/Index", $"{{language:regex(^(zh-tw)$)}}/{{PageName:regex(^({pages})$)}}");
});

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapGet("/api/health/", () => Results.Ok(new { status = "ok" }));
app.Run();
