using System.ComponentModel.DataAnnotations;

namespace Veritas.Web.Models;

public sealed class Intention
{
    [Key, MaxLength(36)] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Required, MaxLength(500)] public string Text { get; set; } = "";
    [Required, MaxLength(100)] public string Name { get; set; } = "";
    [Required, MaxLength(100)] public string Location { get; set; } = "";
    [Required, MaxLength(5)] public string Language { get; set; } = "en";
    [Required, MaxLength(40)] public string CreatedAt { get; set; } = DateTime.UtcNow.ToString("O");
}

public sealed record CreateIntentionRequest(string? Text, string? Name, string? Location, bool? Consent, string? Language);
public sealed record IntentionDto(string Id, string Text, string Name, string Location, string Language, string CreatedAt);
