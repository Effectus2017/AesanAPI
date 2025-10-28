using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para operaciones con días de funcionamiento de sitios
/// Utilizado para recibir datos del frontend y validar la entrada
/// </summary>
public class SiteOperatingDayRequest
{
    /// <summary>
    /// ID del día de funcionamiento (opcional, solo para actualizaciones)
    /// </summary>
    public int? Id { get; set; }

    /// <summary>
    /// ID del sitio al que pertenece este día de funcionamiento
    /// </summary>
    [Required(ErrorMessage = "El ID del sitio es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del sitio debe ser mayor a 0")]
    public int SiteId { get; set; }

    /// <summary>
    /// Fecha de funcionamiento
    /// </summary>
    [Required(ErrorMessage = "La fecha de funcionamiento es requerida")]
    public DateTime OperatingDate { get; set; }

    /// <summary>
    /// Hora de inicio del funcionamiento (formato HH:MM:SS)
    /// </summary>
    public TimeSpan? StartTime { get; set; }

    /// <summary>
    /// Hora de fin del funcionamiento (formato HH:MM:SS)
    /// </summary>
    public TimeSpan? EndTime { get; set; }

    /// <summary>
    /// Indica si es un fin de semana que funciona por excepción
    /// </summary>
    public bool IsWeekendOverride { get; set; } = false;

    /// <summary>
    /// Indica si el día está excluido del funcionamiento
    /// </summary>
    public bool IsExcluded { get; set; } = false;

    /// <summary>
    /// Comentario opcional sobre el día de funcionamiento
    /// </summary>
    [MaxLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
    public string? Comment { get; set; }
}
