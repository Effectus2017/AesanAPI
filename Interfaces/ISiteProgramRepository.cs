using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de SiteProgram
/// </summary>
public interface ISiteProgramRepository
{
    /// <summary>
    /// Obtiene todos los programas de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <returns>Lista de programas del sitio</returns>
    Task<IEnumerable<SiteProgramResponse>> GetSiteProgramsBySiteId(int siteId);

    /// <summary>
    /// Inserta una nueva relación sitio-programa
    /// </summary>
    /// <param name="request">Datos de la relación a insertar</param>
    /// <returns>ID de la relación insertada</returns>
    Task<int> InsertSiteProgram(SiteProgramRequest request);

    /// <summary>
    /// Actualiza una relación sitio-programa existente
    /// </summary>
    /// <param name="request">Datos de la relación a actualizar</param>
    /// <returns>True si la actualización fue exitosa</returns>
    Task<bool> UpdateSiteProgram(SiteProgramRequest request);
}

