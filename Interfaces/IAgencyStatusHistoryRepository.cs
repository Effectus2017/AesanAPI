namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de historial de estados de agencia.
/// </summary>
public interface IAgencyStatusHistoryRepository
{
    /// <summary>
    /// Obtiene el historial de estados de agencia paginado.
    /// </summary>
    Task<dynamic> GetAgencyStatusHistoryPaged(
        int take,
        int skip,
        int? agencyId,
        DateTime? from,
        DateTime? to);
}
