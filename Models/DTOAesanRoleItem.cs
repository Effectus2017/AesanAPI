namespace Api.Models;

/// <summary>
/// Rol AESAN con Name (español) y NameEN (inglés) para el endpoint /auth/aesan-roles.
/// </summary>
public class DTOAesanRoleItem
{
    public string Name { get; set; } = "";
    public string? NameEN { get; set; }
}
