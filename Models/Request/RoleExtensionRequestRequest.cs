namespace Api.Models.Request;

/// <summary>Request para solicitar extensión de vigencia de un rol secundario.</summary>
public class RoleExtensionRequestRequest
{
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
    public DateTime RequestedValidTo { get; set; }
    public string? Reason { get; set; }
}
