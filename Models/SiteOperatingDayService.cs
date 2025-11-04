namespace Api.Models;

/// <summary>
/// Modelo para servicios de alimentación relacionados con días de funcionamiento
/// </summary>
public class SiteOperatingDayService
{
    public int Id { get; set; }
    public int OperatingDayId { get; set; }
    public int ServiceTypeId { get; set; }
    public int? ChildGroupId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsEnabled { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

