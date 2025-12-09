using System.Text.Json.Serialization;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para las métricas del dashboard de agencia
/// </summary>
public class AgencyDashboardResponse
{
    /// <summary>
    /// Total de escuelas de la agencia
    /// </summary>
    [JsonPropertyName("totalSchools")]
    public int TotalSchools { get; set; }

    /// <summary>
    /// Total de sitios de la agencia
    /// </summary>
    [JsonPropertyName("totalSites")]
    public int TotalSites { get; set; }

    /// <summary>
    /// Fecha de última actualización
    /// </summary>
    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; }

    // Propiedades para futuro (comentadas por ahora)
    // /// <summary>
    // /// Presupuesto aprobado de la agencia
    // /// </summary>
    // [JsonPropertyName("approvedBudget")]
    // public decimal? ApprovedBudget { get; set; }

    // /// <summary>
    // /// Raciones servidas por mes
    // /// </summary>
    // [JsonPropertyName("rationsByMonth")]
    // public List<RationByMonth>? RationsByMonth { get; set; }

    // /// <summary>
    // /// Visitas coordinadas por tipo
    // /// </summary>
    // [JsonPropertyName("coordinatedVisits")]
    // public List<CoordinatedVisit>? CoordinatedVisits { get; set; }
}

