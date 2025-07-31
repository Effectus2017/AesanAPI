using Api.Models;
using Api.Models.Request;

namespace Api.Interfaces;

public interface IStaffClassificationRepository
{
    /// <summary>
    /// Obtiene una clasificación de staff por su ID
    /// </summary>
    /// <param name="id">El ID de la clasificación de staff</param>
    /// <returns>La clasificación de staff</returns>
    Task<dynamic> GetStaffClassificationById(int id);

    /// <summary>
    /// Obtiene todas las clasificaciones de staff de la base de datos
    /// </summary>
    /// <param name="take">El número de clasificaciones a obtener</param>
    /// <param name="skip">El número de clasificaciones a saltar</param>
    /// <param name="name">El nombre de la clasificación a buscar</param>
    /// <param name="alls">Si se deben obtener todas las clasificaciones</param>
    /// <param name="isList">Si es para lista simple (dropdown)</param>
    /// <returns>Las clasificaciones de staff</returns>
    Task<dynamic> GetAllStaffClassificationsFromDb(int take, int skip, string name, bool alls, bool isList);

    /// <summary>
    /// Inserta una nueva clasificación de staff en la base de datos
    /// </summary>
    /// <param name="staffClassificationRequest">Datos de la clasificación de staff a insertar</param>
    /// <returns>True si se insertó correctamente</returns>
    Task<bool> InsertStaffClassification(StaffClassificationRequest staffClassificationRequest);

    /// <summary>
    /// Actualiza una clasificación de staff existente en la base de datos
    /// </summary>
    /// <param name="staffClassificationRequest">Datos de la clasificación de staff a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    Task<bool> UpdateStaffClassification(StaffClassificationRequest staffClassificationRequest);

    /// <summary>
    /// Elimina una clasificación de staff de la base de datos (baja lógica)
    /// </summary>
    /// <param name="id">El ID de la clasificación de staff a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    Task<bool> DeleteStaffClassification(int id);
}