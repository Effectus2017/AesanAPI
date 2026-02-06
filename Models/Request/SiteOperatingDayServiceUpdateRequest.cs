using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para actualizar solo los campos editables de un servicio de alimentación por día de funcionamiento.
/// No incluye ChildGroupId, OperatingDayId ni ServiceTypeId porque no se modifican en la actualización.
/// </summary>
public class SiteOperatingDayServiceUpdateRequest
{
    /// <summary>
    /// Hora de inicio del servicio (debe estar dentro del rango del día de funcionamiento)
    /// </summary>
    [Required(ErrorMessage = "La hora de inicio es requerida")]
    public TimeSpan StartTime { get; set; }

    /// <summary>
    /// Hora de fin del servicio (debe estar dentro del rango del día de funcionamiento)
    /// </summary>
    [Required(ErrorMessage = "La hora de fin es requerida")]
    public TimeSpan EndTime { get; set; }

    /// <summary>
    /// Indica si el servicio está habilitado para este día
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Comentarios sobre el servicio
    /// </summary>
    [MaxLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
    public string? Comment { get; set; }
}
