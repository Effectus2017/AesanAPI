namespace Api.Models;

/// <summary>Rol secundario con vigencia (para listados y respuesta GetUser).</summary>
public class DTOUserSecondaryRole
{
    public string RoleId { get; set; } = "";
    public string RoleName { get; set; } = "";
    public string? Comment { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
    public bool IsActive { get; set; }
}
