using System;

namespace Api.Models.Request;

public class SiteServiceRequest
{
    public int? Id { get; set; }
    public int? ChildGroupId { get; set; }
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

    // Servicios adicionales para PACNA
    public bool? DinnerExtended { get; set; }
    public TimeSpan? DinnerExtendedFrom { get; set; }
    public TimeSpan? DinnerExtendedTo { get; set; }
    public bool? DinnerAtRisk { get; set; }
    public TimeSpan? DinnerAtRiskFrom { get; set; }
    public TimeSpan? DinnerAtRiskTo { get; set; }
    public bool? SnackExtended { get; set; }
    public TimeSpan? SnackExtendedFrom { get; set; }
    public TimeSpan? SnackExtendedTo { get; set; }
    public bool? SnackAtRisk { get; set; }
    public TimeSpan? SnackAtRiskFrom { get; set; }
    public TimeSpan? SnackAtRiskTo { get; set; }
}
