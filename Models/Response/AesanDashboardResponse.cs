using System.Text.Json.Serialization;

namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para las métricas del dashboard AESAN
/// </summary>
public class AesanDashboardResponse
{
    /// <summary>
    /// Conteos de agencias por estado
    /// </summary>
    [JsonPropertyName("agencyStatusCounts")]
    public AgencyStatusCounts AgencyStatusCounts { get; set; } = new();

    /// <summary>
    /// Total de agencias
    /// </summary>
    [JsonPropertyName("totalAgencies")]
    public int TotalAgencies { get; set; }

    /// <summary>
    /// Fecha de última actualización
    /// </summary>
    [JsonPropertyName("lastUpdated")]
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Conteos de agencias por estado
/// </summary>
public class AgencyStatusCounts
{
    /// <summary>
    /// Agencias pendientes de validación (StatusId = 1)
    /// </summary>
    [JsonPropertyName("pendingValidationCount")]
    public int PendingValidationCount { get; set; }

    /// <summary>
    /// Agencias en orientación (StatusId = 2)
    /// </summary>
    [JsonPropertyName("orientationCount")]
    public int OrientationCount { get; set; }

    /// <summary>
    /// Agencias aprobadas (StatusId = 7)
    /// </summary>
    [JsonPropertyName("approvedCount")]
    public int ApprovedCount { get; set; }

    /// <summary>
    /// Agencias rechazadas (StatusId = 6)
    /// </summary>
    [JsonPropertyName("rejectedCount")]
    public int RejectedCount { get; set; }
}
