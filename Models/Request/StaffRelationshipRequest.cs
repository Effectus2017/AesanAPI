using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

/// <summary>
/// Modelo de solicitud para crear o actualizar relaciones entre empleados
/// </summary>
public class StaffRelationshipRequest
{
    /// <summary>
    /// ID del empleado principal
    /// </summary>
    [Required(ErrorMessage = "El ID del empleado principal es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del empleado debe ser mayor a 0")]
    public int StaffId { get; set; }

    /// <summary>
    /// ID del empleado relacionado
    /// </summary>
    [Required(ErrorMessage = "El ID del empleado relacionado es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del empleado relacionado debe ser mayor a 0")]
    public int RelatedStaffId { get; set; }

    /// <summary>
    /// ID del tipo de parentesco
    /// </summary>
    [Required(ErrorMessage = "El tipo de parentesco es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El tipo de parentesco debe ser mayor a 0")]
    public int RelationshipTypeId { get; set; }
}

/// <summary>
/// Modelo de solicitud para actualizar relaciones entre empleados
/// </summary>
public class UpdateStaffRelationshipRequest
{
    /// <summary>
    /// ID de la relación a actualizar
    /// </summary>
    [Required(ErrorMessage = "El ID de la relación es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la relación debe ser mayor a 0")]
    public int Id { get; set; }

    /// <summary>
    /// ID del tipo de parentesco
    /// </summary>
    [Required(ErrorMessage = "El tipo de parentesco es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El tipo de parentesco debe ser mayor a 0")]
    public int RelationshipTypeId { get; set; }

    /// <summary>
    /// Estado activo de la relación
    /// </summary>
    [Required(ErrorMessage = "El estado activo es requerido")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Comentario sobre el cambio realizado
    /// </summary>
    public string? Comment { get; set; }
}
