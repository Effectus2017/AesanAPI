using Api.Models;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Servicio de consulta del centro de logs (solo lectura).
/// Devuelve entradas paginadas en formato unificado por categoría.
/// </summary>
public interface ILogsQueryService
{
    /// <summary>
    /// Obtiene entradas de log paginadas para una categoría.
    /// Categorías: Audit, Email, Job, Application.
    /// </summary>
    Task<PagedResult<CentralLogEntryDto>> GetLogsPagedAsync(
        string category,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
