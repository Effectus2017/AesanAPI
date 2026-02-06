using System;
using System.Collections.Generic;

namespace Api.Models.Response;

/// <summary>
/// Fecha de operación asociada a un slot de servicio (para formato "Jueves(6)").
/// From y To son las horas específicas de ese día (desde SiteOperatingDayService), formato "HH:mm:ss".
/// </summary>
public class ServiceSlotOperatingDateDto
{
    public string DayName { get; set; } = string.Empty;
    public int DayOfMonth { get; set; }
    public DateTime? Date { get; set; }
    /// <summary>Hora inicio para este día (cuando difiere por día), formato "HH:mm:ss".</summary>
    public string? From { get; set; }
    /// <summary>Hora fin para este día (cuando difiere por día), formato "HH:mm:ss".</summary>
    public string? To { get; set; }
    /// <summary>True si el día es feriado (para mostrar color distinto en la UI).</summary>
    public bool IsHoliday { get; set; }
    /// <summary>True si el día es fin de semana (para mostrar color distinto en la UI).</summary>
    public bool IsWeekend { get; set; }
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
