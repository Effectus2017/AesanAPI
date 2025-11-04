using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para crear o actualizar servicios de alimentación por día de funcionamiento
/// </summary>
public class SiteOperatingDayServiceRequest
{
    /// <summary>
    /// ID del día de funcionamiento (requerido para crear)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "El ID del día de funcionamiento debe ser mayor a 0")]
    public int? OperatingDayId { get; set; }

    /// <summary>
    /// ID del tipo de servicio (FK a OptionSelection con OptionKey='service-type')
    /// </summary>
    [Required(ErrorMessage = "El ID del tipo de servicio es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del tipo de servicio debe ser mayor a 0")]
    public int ServiceTypeId { get; set; }

    /// <summary>
    /// ID del grupo de niños (opcional, para Day Care Homes)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "El ID del grupo de niños debe ser mayor a 0")]
    public int? ChildGroupId { get; set; }

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

