using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para operaciones con días de funcionamiento de sitios (múltiples días)
/// Utilizado para recibir datos del frontend y validar la entrada
/// </summary>
public class SiteOperatingDaysRequest
{
    /// <summary>
    /// ID del sitio al que pertenecen estos días de funcionamiento
    /// </summary>
    [Required(ErrorMessage = "El ID del sitio es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del sitio debe ser mayor a 0")]
    public int SiteId { get; set; }

    /// <summary>
    /// Fecha de inicio del período de funcionamiento
    /// </summary>
    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    public DateTime OperatingFromDate { get; set; }

    /// <summary>
    /// Fecha de fin del período de funcionamiento
    /// </summary>
    [Required(ErrorMessage = "La fecha de fin es requerida")]
    public DateTime OperatingToDate { get; set; }

    /// <summary>
    /// Hora de inicio por defecto para todos los días
    /// </summary>
    public TimeSpan? DefaultStartTime { get; set; }

    /// <summary>
    /// Hora de fin por defecto para todos los días
    /// </summary>
    public TimeSpan? DefaultEndTime { get; set; }

    /// <summary>
    /// Comentario por defecto para todos los días
    /// </summary>
    [MaxLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
    public string? DefaultComment { get; set; }

    /// <summary>
    /// Lista de días específicos con configuraciones personalizadas
    /// </summary>
    public List<SiteOperatingDayRequest>? SpecificDays { get; set; }
}
