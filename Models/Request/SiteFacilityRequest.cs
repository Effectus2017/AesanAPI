using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para operaciones con instalaciones de sitios
/// Utilizado para recibir datos del frontend y validar la entrada
/// </summary>
public class SiteFacilityRequest
{
    /// <summary>
    /// ID del sitio al que pertenece esta instalación
    /// </summary>
    [Required(ErrorMessage = "El ID del sitio es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del sitio debe ser mayor a 0")]
    public int SiteId { get; set; }

    /// <summary>
    /// ID del tipo de instalación
    /// </summary>
    [Required(ErrorMessage = "El tipo de instalación es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El tipo de instalación debe ser mayor a 0")]
    public int FacilityTypeId { get; set; }

    /// <summary>
    /// Descripción opcional de la instalación
    /// </summary>
    [MaxLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
    public string? Description { get; set; }

    /// <summary>
    /// Indica si la instalación está activa
    /// </summary>
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Modelo de request para actualizar una instalación existente
/// </summary>
public class UpdateSiteFacilityRequest
{
    /// <summary>
    /// ID del tipo de instalación
    /// </summary>
    [Required(ErrorMessage = "El tipo de instalación es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El tipo de instalación debe ser mayor a 0")]
    public int FacilityTypeId { get; set; }

    /// <summary>
    /// Descripción opcional de la instalación
    /// </summary>
    [MaxLength(255, ErrorMessage = "La descripción no puede exceder 255 caracteres")]
    public string? Description { get; set; }

    /// <summary>
    /// Indica si la instalación está activa
    /// </summary>
    public bool IsActive { get; set; } = true;
}
