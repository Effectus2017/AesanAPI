using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio del dashboard AESAN
/// </summary>
public interface IAesanDashboardRepository
{
    /// <summary>
    /// Obtiene las métricas del dashboard AESAN
    /// </summary>
    /// <param name="userId">ID del usuario (opcional)</param>
    /// <returns>Métricas del dashboard</returns>
    Task<AesanDashboardResponse> GetDashboardMetrics(string userId);
}
