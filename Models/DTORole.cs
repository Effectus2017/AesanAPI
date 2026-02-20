namespace Api.Models;

/// <summary>
/// Modelo simplificado de rol (solo Id, Name e IsAesanRole).
/// </summary>
public class DTORole
{
    /// <summary>Identificador único del rol (GUID).</summary>
    public string Id { get; set; } = "";
    
    /// <summary>
    /// Clave técnica del rol (ej: "administrator", "program_coordinator").
    /// Se usa como identificador en lógica de negocio. NO mostrar directamente al usuario.
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>Indica si el rol pertenece a AESAN (true) o a una agencia (false).</summary>
    public bool IsAesanRole { get; set; }
}