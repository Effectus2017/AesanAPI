using System;
using System.Collections.Generic;

namespace Api.Models.Request;

public class SiteChildGroupServiceSlotRequest
{
    public int? Id { get; set; }
    public int ServiceTypeId { get; set; }
    public bool IsOffered { get; set; }

    /// <summary>Hora inicio. El frontend envía "fromTime" (camelCase).</summary>
    public TimeSpan? FromTime { get; set; }

    /// <summary>Hora fin. El frontend envía "toTime" (camelCase).</summary>
    public TimeSpan? ToTime { get; set; }

    /// <summary>
    /// Fechas de operación donde este servicio debe estar cargado (formato ISO o YYYY-MM-DD).
    /// Si es null o vacío para un slot ofrecido, no se insertan días (se respeta "ningún día").
    /// </summary>
    public List<DateTime>? OperatingDates { get; set; }
}
