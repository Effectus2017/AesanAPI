using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para operaciones con días de funcionamiento de escuelas
/// Utilizado para recibir datos del frontend y validar la entrada
/// </summary>
public class SchoolOperatingDayRequest
{
    /// <summary>
    /// ID de la escuela a la que pertenece este día de funcionamiento
    /// </summary>
    [Required(ErrorMessage = "El ID de la escuela es requerido")]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la escuela debe ser mayor a 0")]
    public int schoolId { get; set; }

    /// <summary>
    /// Fecha de funcionamiento
    /// </summary>
    [Required(ErrorMessage = "La fecha de funcionamiento es requerida")]
    public DateTime operatingDate { get; set; }

    /// <summary>
    /// Hora de inicio del funcionamiento (formato HH:MM:SS)
    /// </summary>
    public TimeSpan? startTime { get; set; }

    /// <summary>
    /// Hora de fin del funcionamiento (formato HH:MM:SS)
    /// </summary>
    public TimeSpan? endTime { get; set; }

    /// <summary>
    /// Indica si es un fin de semana que funciona por excepción
    /// </summary>
    public bool isWeekendOverride { get; set; } = false;

    /// <summary>
    /// Indica si el día está excluido del funcionamiento
    /// </summary>
    public bool isExcluded { get; set; } = false;

    /// <summary>
    /// Comentario opcional sobre el día de funcionamiento
    /// </summary>
    [MaxLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
    public string? comment { get; set; }
}
