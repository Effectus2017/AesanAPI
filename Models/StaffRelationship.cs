namespace Api.Models;

/// <summary>
/// Representa una relación de parentesco entre dos empleados de la agencia
/// </summary>
public class StaffRelationship
{
    /// <summary>
    /// Identificador único de la relación
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID del empleado principal
    /// </summary>
    public int StaffId { get; set; }

    /// <summary>
    /// ID del empleado relacionado
    /// </summary>
    public int RelatedStaffId { get; set; }

    /// <summary>
    /// ID del tipo de parentesco (referencia a OptionSelection)
    /// </summary>
    public int RelationshipTypeId { get; set; }

    /// <summary>
    /// Indica si la relación está activa
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Fecha de creación de la relación
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Fecha de última actualización de la relación
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Comentario sobre el cambio de estado o la relación
    /// </summary>
    public string? Comment { get; set; }

    // Propiedades de navegación
    /// <summary>
    /// Empleado principal
    /// </summary>
    public Staff? Staff { get; set; }

    /// <summary>
    /// Empleado relacionado
    /// </summary>
    public Staff? RelatedStaff { get; set; }

    /// <summary>
    /// Tipo de parentesco
    /// </summary>
    public DTOOptionSelection? RelationshipType { get; set; }
}
