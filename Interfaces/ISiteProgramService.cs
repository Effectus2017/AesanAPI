using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el servicio de SiteProgram
/// </summary>
public interface ISiteProgramService
{
    /// <summary>
    /// Obtiene todos los programas de un sitio
    /// </summary>
    Task<IEnumerable<SiteProgramResponse>> GetSiteProgramsBySiteId(int siteId);

    /// <summary>
    /// Inserta una nueva relación sitio-programa
    /// </summary>
    Task<SiteProgramResponse> InsertSiteProgram(SiteProgramRequest request);

    /// <summary>
    /// Actualiza una relación sitio-programa existente
    /// </summary>
    Task<bool> UpdateSiteProgram(SiteProgramRequest request);
}

