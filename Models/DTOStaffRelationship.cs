namespace Api.Models;

/// <summary>
/// DTO para respuestas de relaciones entre empleados
/// </summary>
public class DTOStaffRelationship
{
    /// <summary>
    /// Identificador único de la relación
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Información del empleado principal
    /// </summary>
    public DTOStaffForRelationship? Staff { get; set; }

    /// <summary>
    /// Información del empleado relacionado
    /// </summary>
    public DTOStaffForRelationship? RelatedStaff { get; set; }

    /// <summary>
    /// Tipo de parentesco en español
    /// </summary>
    public string? RelationshipType { get; set; }

    /// <summary>
    /// Tipo de parentesco en inglés
    /// </summary>
    public string? RelationshipTypeEn { get; set; }

    /// <summary>
    /// ID del tipo de parentesco
    /// </summary>
    public int RelationshipTypeId { get; set; }

    /// <summary>
    /// Indica si la relación está activa
    /// </summary>
    public bool IsActive { get; set; }

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
}

/// <summary>
/// Información esencial del empleado para relaciones
/// </summary>
public class DTOStaffForRelationship
{
    /// <summary>
    /// ID del empleado
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nombre completo del empleado (FirstName + MiddleName + FatherLastName + MotherLastName)
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Posición del empleado
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Tipo de empleado
    /// </summary>
    public string? StaffType { get; set; }

    /// <summary>
    /// Email del empleado
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Indica si el empleado está activo
    /// </summary>
    public bool IsActive { get; set; }
}
