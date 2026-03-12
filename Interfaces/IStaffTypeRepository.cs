using Api.Models;
using Api.Models.Request;

namespace Api.Interfaces;

public interface IStaffTypeRepository
{
    /// <summary>
    /// Obtiene un tipo de staff por su ID
    /// </summary>
    /// <param name="id">El ID del tipo de staff</param>
    /// <returns>El tipo de staff</returns>
    Task<dynamic> GetStaffTypeById(int id);

    /// <summary>
    /// Obtiene todos los tipos de staff de la base de datos
    /// </summary>
    /// <param name="take">El número de tipos de staff a obtener</param>
    /// <param name="skip">El número de tipos de staff a saltar</param>
    /// <param name="name">El nombre del tipo de staff a buscar</param>
    /// <param name="alls">Si se deben obtener todos los tipos de staff</param>
    /// <param name="forDropdown">Si es para lista simple (dropdown)</param>
    /// <returns>Los tipos de staff</returns>
    Task<dynamic> GetAllStaffTypesFromDb(int take, int skip, string name, bool alls, bool forDropdown);

    /// <summary>
    /// Inserta un nuevo tipo de staff en la base de datos
    /// </summary>
    /// <param name="staffTypeRequest">Datos del tipo de staff a insertar</param>
    /// <returns>True si se insertó correctamente</returns>
    Task<bool> InsertStaffType(StaffTypeRequest staffTypeRequest);

    /// <summary>
    /// Actualiza un tipo de staff existente en la base de datos
    /// </summary>
    /// <param name="staffTypeRequest">Datos del tipo de staff a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    Task<bool> UpdateStaffType(StaffTypeRequest staffTypeRequest);

    /// <summary>
    /// Elimina un tipo de staff de la base de datos (baja lógica)
    /// </summary>
    /// <param name="id">El ID del tipo de staff a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    Task<bool> DeleteStaffType(int id);
}