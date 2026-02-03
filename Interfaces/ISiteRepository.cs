using Api.Models;
using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

public interface ISiteRepository
{
    /// <summary>
    /// Obtiene un sitio por su ID
    /// </summary>
    /// <param name="id">ID del sitio</param>
    Task<SiteResponse> GetSiteById(int id);

    /// <summary>
    /// Obtiene todos los sitios
    /// </summary>
    Task<dynamic> GetAllSitesFromDB(int take, int skip, string name, int? cityId, int? regionId, int? agencyId, bool alls, bool isList, int? isDayCareHomeId = null);

    /// <summary>
    /// Inserta un nuevo sitio
    /// </summary>
    Task<bool> InsertSite(SiteRequest request);

    /// <summary>
    /// Actualiza un sitio existente
    /// </summary>
    Task<bool> UpdateSite(SiteRequest request);

    /// <summary>
    /// Elimina un sitio
    /// </summary>
    Task<bool> DeleteSite(int id);

    /// <summary>
    /// Actualiza el estado activo/inactivo de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="isActive">Estado activo (true) o inactivo (false)</param>
    /// <param name="inactiveJustification">Justificación cuando se inactiva (requerida si isActive es false)</param>
    /// <param name="inactiveDate">Fecha de inactivación (opcional, si no se proporciona se usa GETDATE())</param>
    /// <param name="providedRationsService">¿Brindó servicio de raciones durante su periodo de funcionamiento?</param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    Task<bool> UpdateSiteActiveStatus(int siteId, bool isActive, string? inactiveJustification = null, DateTime? inactiveDate = null, bool? providedRationsService = null);

    /// <summary>
    /// Actualiza solo los grupos de niños y sus servicios de un sitio (persistencia inmediata desde el modal).
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="childGroups">Lista de grupos de niños con sus servicios</param>
    /// <returns>True si se actualizó correctamente</returns>
    Task<bool> UpdateSiteChildGroupsOnly(int siteId, List<SiteChildGroupRequest> childGroups);

}
