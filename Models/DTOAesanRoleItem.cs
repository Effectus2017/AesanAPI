namespace Api.Models;

/// <summary>
/// Rol AESAN con Name (clave), DisplayName (español) y DisplayNameEN (inglés) para GET /user/get-all-roles-from-db?aesanOnly=true.
/// </summary>
public class DTOAesanRoleItem
{
    /// <summary>Clave única del rol (para lógica y envío en API).</summary>
    public string Name { get; set; } = "";
    /// <summary>Nombre a mostrar en español.</summary>
    public string? DisplayName { get; set; }
    /// <summary>Nombre a mostrar en inglés.</summary>
    public string? DisplayNameEN { get; set; }
}
