using Api.Models;
using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

public interface ISiteRepository
{
    /// <summary>
    /// Obtiene un sitio por su ID
    /// </summary>
    Task<SiteResponse> GetSiteById(int id);

    /// <summary>
    /// Obtiene todos los sitios
    /// </summary>
    Task<dynamic> GetAllSitesFromDB(int take, int skip, string name, int? cityId, int? regionId, int? agencyId, bool alls, bool isList);

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
    /// Verifica si existe un sitio principal en la base de datos
    /// </summary>
    /// <returns>True si existe un sitio principal, false en caso contrario</returns>
    Task<bool> HasMainSite();

    /// <summary>
    /// Actualiza el estado activo/inactivo de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="isActive">Estado activo (true) o inactivo (false)</param>
    /// <param name="inactiveJustification">Justificación cuando se inactiva (requerida si isActive es false)</param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    Task<bool> UpdateSiteActiveStatus(int siteId, bool isActive, string? inactiveJustification = null);
}
