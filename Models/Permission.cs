namespace Api.Models;

public class Permission
{
    public string Id { get; set; } = string.Empty;
    public string ValueKey { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
