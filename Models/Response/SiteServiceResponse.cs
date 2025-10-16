using System;

namespace Api.Models.Response;

public class SiteServiceResponse
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public SiteChildGroupResponse? ChildGroup { get; set; }
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
