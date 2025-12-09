using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio del dashboard de agencia
/// </summary>
public interface IAgencyDashboardRepository
{
    /// <summary>
    /// Obtiene las métricas del dashboard de agencia
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>Métricas del dashboard</returns>
    Task<AgencyDashboardResponse> GetDashboardMetrics(int agencyId);
}

