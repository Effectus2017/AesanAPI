using Microsoft.AspNetCore.Identity;

namespace Api.Models;

/// <summary>
/// Entidad de rol extendida de IdentityRole.
/// Hereda: Id (GUID), Name (clave técnica), NormalizedName, ConcurrencyStamp.
/// </summary>
public class Role : IdentityRole
{
    /// <summary>Indica si el rol pertenece a AESAN (true) o a una agencia (false).</summary>
    public bool IsAesanRole { get; set; }
    
    /// <summary>
    /// Nombre legible del rol en español para mostrar al usuario (ej: "Administrador", "Coordinador de Programa").
    /// </summary>
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// Nombre legible del rol en inglés para mostrar al usuario (ej: "Administrator", "Program Coordinator").
    /// </summary>
    public string? DisplayNameEN { get; set; }
    
    /// <summary>Relación con usuarios que tienen este rol.</summary>
    public ICollection<UserRole> UserRoles { get; set; }
}