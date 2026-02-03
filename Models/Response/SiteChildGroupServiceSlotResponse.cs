using System;
using System.Collections.Generic;

namespace Api.Models.Response;

/// <summary>
/// Fecha de operación asociada a un slot de servicio (para formato "Jueves(6)").
/// </summary>
public class ServiceSlotOperatingDateDto
{
    public string DayName { get; set; } = string.Empty;
    public int DayOfMonth { get; set; }
    public DateTime? Date { get; set; }
}

public class SiteChildGroupServiceSlotResponse
{
    public int Id { get; set; }
    public int ChildGroupId { get; set; }
    public int ServiceTypeId { get; set; }
    public bool IsOffered { get; set; }
    public TimeSpan? FromTime { get; set; }
    public TimeSpan? ToTime { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ServiceTypeName { get; set; }
    public string? ServiceTypeNameEN { get; set; }
    /// <summary>
    /// Fechas de operación donde este servicio está cargado (calendario).
    /// </summary>
    public List<ServiceSlotOperatingDateDto>? OperatingDates { get; set; }
}
