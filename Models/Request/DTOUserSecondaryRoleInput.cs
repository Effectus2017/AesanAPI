namespace Api.Models.Request;

/// <summary>Entrada para asignar un rol secundario (nombre + vigencia + comentario opcional).</summary>
public class DTOUserSecondaryRoleInput
{
    public string RoleName { get; set; } = "";
    public string? Comment { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime ValidTo { get; set; }
}
