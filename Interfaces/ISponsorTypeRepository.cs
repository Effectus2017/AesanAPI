using Api.Models;

namespace Api.Interfaces;

public interface ISponsorTypeRepository
{
    /// <summary>
    /// Obtiene un tipo de auspiciador por su ID
    /// </summary>
    /// <param name="id">ID del tipo de auspiciador</param>
    /// <returns>Tipo de auspiciador</returns>
    Task<dynamic> GetSponsorTypeById(int id);

    /// <summary>
    /// Obtiene todos los tipos de auspiciador
    /// </summary>
    /// <param name="take">Número de tipos de auspiciador a obtener</param>
    /// <param name="skip">Número de tipos de auspiciador a saltar</param>
    /// <param name="name">Nombre del tipo de auspiciador</param>
    /// <param name="alls">Indica si se deben obtener todos los tipos de auspiciador</param>
    Task<dynamic> GetAllSponsorTypes(int take, int skip, string name, bool alls, bool isList);

    /// <summary>
    /// Inserta un nuevo tipo de auspiciador
    /// </summary>
    /// <param name="sponsorType">Tipo de auspiciador a insertar</param>
    /// <returns>True si se insertó correctamente, false en caso contrario</returns>
    Task<bool> InsertSponsorType(DTOSponsorType sponsorType);

    /// <summary>
    /// Actualiza un tipo de auspiciador
    /// </summary>
    /// <param name="sponsorType">Tipo de auspiciador a actualizar</param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    Task<bool> UpdateSponsorType(DTOSponsorType sponsorType);

    /// <summary>
    /// Elimina un tipo de auspiciador
    /// </summary>
    /// <param name="id">ID del tipo de auspiciador a eliminar</param>
    /// <returns>True si se eliminó correctamente, false en caso contrario</returns>
    Task<bool> DeleteSponsorType(int id);
}