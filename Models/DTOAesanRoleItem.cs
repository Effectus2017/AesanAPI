namespace Api.Models;

/// <summary>
/// Rol AESAN con Name (clave), DisplayName (español) y DisplayNameEN (inglés) para GET /user/get-all-roles-from-db?aesanOnly=true.
/// </summary>
public class DTOAesanRoleItem
{
    /// <summary>
    /// Clave técnica del rol (ej: "administrator", "program_coordinator").
    /// Se usa como identificador único en lógica de negocio, comparaciones y como value en selects del frontend.
    /// NO se debe mostrar directamente al usuario; usar DisplayName o DisplayNameEN.
    /// </summary>
    public string Name { get; set; } = "";
    
    /// <summary>
    /// Nombre legible del rol en español para mostrar al usuario (ej: "Administrador", "Coordinador de Programa").
    /// </summary>
    public string? DisplayName { get; set; }
    
    /// <summary>
    /// Nombre legible del rol en inglés para mostrar al usuario (ej: "Administrator", "Program Coordinator").
    /// </summary>
    public string? DisplayNameEN { get; set; }
}
