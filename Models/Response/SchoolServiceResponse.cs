using System;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para los servicios de alimentación de las escuelas
/// </summary>
public class SchoolServiceResponse
{
    public int Id { get; set; }
    public int SchoolId { get; set; }

    /// <summary>
    /// Información del grupo de niños. NULL = servicio general, NOT NULL = servicio específico por grupo
    /// </summary>
    public DTOOptionSelection? ChildGroup { get; set; }

    // Servicios de alimentación
    public bool? Breakfast { get; set; }
    public TimeSpan? BreakfastFrom { get; set; }
    public TimeSpan? BreakfastTo { get; set; }

    public bool? Lunch { get; set; }
    public TimeSpan? LunchFrom { get; set; }
    public TimeSpan? LunchTo { get; set; }

    public bool? SnackAM { get; set; }
    public TimeSpan? SnackAMFrom { get; set; }
    public TimeSpan? SnackAMTo { get; set; }

    public bool? Dinner { get; set; }
    public TimeSpan? DinnerFrom { get; set; }
    public TimeSpan? DinnerTo { get; set; }

    public bool? SnackPM { get; set; }
    public TimeSpan? SnackPMFrom { get; set; }
    public TimeSpan? SnackPMTo { get; set; }

    public bool? SnackNight { get; set; }
    public TimeSpan? SnackNightFrom { get; set; }
    public TimeSpan? SnackNightTo { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
