using Api.Models;

namespace Api.Interfaces;

public interface IFacilityRepository
{
    /// <summary>
    /// Obtiene una instalación por su ID.
    /// </summary>
    /// <param name="id">El ID de la instalación a obtener.</param>
    /// <returns>La instalación encontrada o null si no se encuentra.</returns>
    Task<dynamic> GetFacilityById(int id);

    /// <summary>
    /// Obtiene todas las instalaciones.
    /// </summary>
    /// <param name="take">El número de instalaciones a tomar.</param>
    /// <param name="skip">El número de instalaciones a saltar.</param>
    /// <param name="name">El nombre de la instalación a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las instalaciones.</param>
    /// <returns>Una lista de instalaciones.</returns>
    Task<dynamic> GetAllFacilities(int take, int skip, string name, bool alls);

    /// <summary>
    /// Inserta una nueva instalación.
    /// </summary>
    /// <param name="facility">La instalación a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertFacility(DTOFacility facility);

    /// <summary>
    /// Actualiza una instalación existente.
    /// </summary>
    /// <param name="facility">La instalación a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateFacility(DTOFacility facility);

    /// <summary>
    /// Elimina una instalación existente.
    /// </summary>
    /// <param name="id">El ID de la instalación a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteFacility(int id);
}