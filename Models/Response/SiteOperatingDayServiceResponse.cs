namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para servicios de alimentación por día de funcionamiento
/// </summary>
public class SiteOperatingDayServiceResponse
{
    public int Id { get; set; }
    public int OperatingDayId { get; set; }
    public int ServiceTypeId { get; set; }
    
    /// <summary>
    /// Nombre del tipo de servicio (español)
    /// </summary>
    public string? ServiceTypeName { get; set; }
    
    /// <summary>
    /// Nombre del tipo de servicio (inglés)
    /// </summary>
    public string? ServiceTypeNameEN { get; set; }
    
    public int? ChildGroupId { get; set; }
    
    /// <summary>
    /// Nombre del grupo de niños (si aplica)
    /// </summary>
    public string? ChildGroupName { get; set; }
    
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsEnabled { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Información del día de funcionamiento
    public DateTime? OperatingDate { get; set; }
    public TimeSpan? DayStartTime { get; set; }
    public TimeSpan? DayEndTime { get; set; }
}

