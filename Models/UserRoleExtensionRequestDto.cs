namespace Api.Models;

/// <summary>DTO para solicitud de extensión de rol secundario.</summary>
public class UserRoleExtensionRequestDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string RoleId { get; set; } = "";
    public string RoleName { get; set; } = "";
    public DateTime RequestedValidTo { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "";
    public DateTime RequestedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public string? ProcessedBy { get; set; }
    public string? UserEmail { get; set; }
    public string? UserName { get; set; }
}
