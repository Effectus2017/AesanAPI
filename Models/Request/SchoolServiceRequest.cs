using System;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para los servicios de alimentación de las escuelas
/// </summary>
public class SchoolServiceRequest
{
    public int? Id { get; set; }
    public int SchoolId { get; set; }

    /// <summary>
    /// ID del grupo de niños. NULL = servicio general, NOT NULL = servicio específico por grupo
    /// </summary>
    public int? ChildGroupId { get; set; }

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
}
