namespace Api.Models;

public class DTOUserRole
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string? Description { get; set; } = "";
    public string? NormalizedName { get; set; } = "";
    public string? ConcurrencyStamp { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
