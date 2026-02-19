using Api.Models;

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
    Task<(IReadOnlyList<CentralLogEntryDto> Items, int TotalCount)> GetLogsPagedAsync(
        string category,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
