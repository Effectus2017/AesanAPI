using Api.Models;

namespace Api.Interfaces;

public interface IFederalFundingCertificationRepository
{
    /// <summary>
    /// Obtiene una certificación de fondos federales por su ID.
    /// </summary>
    /// <param name="id">El ID de la certificación a obtener.</param>
    /// <returns>La certificación encontrada o null si no se encuentra.</returns>
    Task<dynamic> GetFederalFundingCertificationById(int id);

    /// <summary>
    /// Obtiene todas las certificaciones de fondos federales.
    /// </summary>
    /// <param name="take">El número de certificaciones a tomar.</param>
    /// <param name="skip">El número de certificaciones a saltar.</param>
    /// <param name="description">La descripción de la certificación a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las certificaciones.</param>
    /// <returns>Una lista de certificaciones de fondos federales.</returns>
    Task<dynamic> GetAllFederalFundingCertifications(int take, int skip, string description, bool alls);

    /// <summary>
    /// Inserta una nueva certificación de fondos federales.
    /// </summary>
    /// <param name="certification">La certificación a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertFederalFundingCertification(DTOFederalFundingCertification certification);

    /// <summary>
    /// Actualiza una certificación de fondos federales existente.
    /// </summary>
    /// <param name="certification">La certificación a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateFederalFundingCertification(DTOFederalFundingCertification certification);

    /// <summary>
    /// Elimina una certificación de fondos federales existente.
    /// </summary>
    /// <param name="id">El ID de la certificación a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteFederalFundingCertification(int id);
}