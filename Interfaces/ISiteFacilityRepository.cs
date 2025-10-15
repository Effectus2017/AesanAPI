using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de instalaciones de sitios
/// Define las operaciones CRUD para la gestión de instalaciones específicas de sitios
/// </summary>
public interface ISiteFacilityRepository
{
    /// <summary>
    /// Obtiene todas las instalaciones de un sitio específico
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <returns>Lista de instalaciones del sitio</returns>
    Task<List<SiteFacilityResponse>> GetFacilitiesBySite(int siteId);

    /// <summary>
    /// Actualiza las instalaciones de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="facilityTypeIds">Lista de IDs de tipos de instalación</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> UpdateSiteFacilities(int siteId, List<int> facilityTypeIds);

    /// <summary>
    /// Actualiza el estado activo de las instalaciones de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="isActive">Estado activo</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> UpdateSiteFacilityIsActive(int siteId, bool isActive);

    /// <summary>
    /// Elimina todas las instalaciones de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <returns>True si la operación fue exitosa</returns>
    Task<bool> DeleteSiteFacilities(int siteId);
}
