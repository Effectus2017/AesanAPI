using Api.Models;
using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de calendario de funcionamiento de sitios
/// Define las operaciones CRUD y específicas para la gestión de días de funcionamiento
/// </summary>
public interface ISiteCalendarRepository
{
    /// <summary>
    /// Obtiene todos los días de funcionamiento de un sitio específico
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <returns>Respuesta con información del sitio y sus días de funcionamiento</returns>
    Task<SiteCalendarResponse> GetOperatingDays(int siteId);

    /// <summary>
    /// Alterna el estado de funcionamiento de un día específico
    /// Inserta o actualiza un día de funcionamiento
    /// </summary>
    /// <param name="request">Datos del día de funcionamiento</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> ToggleOperatingDay(SiteOperatingDayRequest request);

    /// <summary>
    /// Actualiza un día de funcionamiento existente
    /// </summary>
    /// <param name="id">ID del día de funcionamiento</param>
    /// <param name="request">Datos actualizados del día de funcionamiento</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> UpdateOperatingDay(int id, SiteOperatingDayRequest request);

    /// <summary>
    /// Actualiza múltiples días de funcionamiento en lote
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="requests">Lista de días de funcionamiento a actualizar</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> BulkUpdateOperatingDays(int siteId, List<SiteOperatingDayRequest> requests);

    /// <summary>
    /// Elimina un día de funcionamiento
    /// </summary>
    /// <param name="id">ID del día de funcionamiento</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> DeleteOperatingDay(int id);

}
