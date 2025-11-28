using Api.Models.Request;
using Api.Models.Response;
using System.Data;

namespace Api.Interfaces;

/// <summary>
/// Interfaz para el repositorio de SitePersonInCharge
/// Gestiona las operaciones de Persona a Cargo de los sitios
/// </summary>
public interface ISitePersonInChargeRepository
{
    /// <summary>
    /// Inserta información de Persona a Cargo para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="request">Datos de la Persona a Cargo</param>
    /// <param name="connection">Conexión opcional a la base de datos</param>
    /// <param name="transaction">Transacción opcional</param>
    /// <returns>ID del registro insertado</returns>
    Task<int> InsertSitePersonInCharge(int siteId, SitePersonInChargeRequest request, IDbConnection? connection = null, IDbTransaction? transaction = null);

    /// <summary>
    /// Actualiza información de Persona a Cargo para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="request">Datos de la Persona a Cargo</param>
    /// <returns>True si se actualizó exitosamente</returns>
    Task<bool> UpdateSitePersonInCharge(int siteId, SitePersonInChargeRequest request);

    /// <summary>
    /// Obtiene información de Persona a Cargo por SiteId
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <returns>La información de Persona a Cargo si existe, null en caso contrario</returns>
    Task<SitePersonInChargeResponse?> GetSitePersonInChargeBySiteId(int siteId);
}

