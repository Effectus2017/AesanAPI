using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de SiteStaff
/// Gestiona las operaciones de asignación de empleados a sitios
/// </summary>
public interface ISiteStaffRepository
{
    /// <summary>
    /// Obtiene todos los empleados asignados a un sitio específico
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <returns>Lista de empleados asignados al sitio</returns>
    Task<IEnumerable<SiteStaffResponse>> GetStaffBySite(int siteId);

    /// <summary>
    /// Obtiene todos los sitios asignados a un empleado específico
    /// </summary>
    /// <param name="staffId">ID del empleado</param>
    /// <returns>Lista de sitios asignados al empleado</returns>
    Task<IEnumerable<SiteStaffResponse>> GetSitesByStaff(int staffId);

    /// <summary>
    /// Asigna un empleado a un sitio
    /// </summary>
    /// <param name="request">Datos de la asignación</param>
    /// <returns>ID de la nueva asignación</returns>
    Task<int> AssignStaffToSite(SiteStaffRequest request);

    /// <summary>
    /// Desasigna un empleado de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="staffId">ID del empleado</param>
    /// <returns>True si se desasignó exitosamente</returns>
    Task<bool> UnassignStaffFromSite(int siteId, int staffId);

    /// <summary>
    /// Actualiza una asignación existente
    /// </summary>
    /// <param name="id">ID de la asignación</param>
    /// <param name="request">Datos de actualización</param>
    /// <returns>True si se actualizó exitosamente</returns>
    Task<bool> UpdateSiteStaff(int id, UpdateSiteStaffRequest request);

    /// <summary>
    /// Obtiene una asignación específica por su ID
    /// </summary>
    /// <param name="id">ID de la asignación</param>
    /// <returns>La asignación si existe, null en caso contrario</returns>
    Task<SiteStaffResponse?> GetSiteStaffById(int id);
}
