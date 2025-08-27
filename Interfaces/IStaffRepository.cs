using Api.Models;
using Api.Models.Request;

namespace Api.Interfaces;

public interface IStaffRepository
{
    /// <summary>
    /// Obtiene un miembro del staff por su ID
    /// </summary>
    /// <param name="id">El ID del miembro del staff</param>
    /// <returns>El miembro del staff</returns>
    Task<dynamic> GetStaffById(int id);

    /// <summary>
    /// Obtiene todos los miembros del staff de la base de datos
    /// </summary>
    /// <param name="take">El número de miembros del staff a obtener</param>
    /// <param name="skip">El número de miembros del staff a saltar</param>
    /// <param name="name">El nombre del miembro del staff a buscar</param>
    /// <param name="alls">Si se deben obtener todos los miembros del staff</param>
    /// <param name="isList">Si es para lista simple (dropdown)</param>
    /// <param name="staffTypeId">ID del tipo de staff para filtrar</param>
    /// <param name="agencyId">ID de la agencia para filtrar</param>
    /// <returns>Los miembros del staff</returns>
    Task<dynamic> GetAllStaffFromDb(int take, int skip, string name, bool alls, bool isList, int? staffTypeId = null, int? agencyId = null);

    /// <summary>
    /// Inserta un nuevo miembro del staff en la base de datos
    /// </summary>
    /// <param name="staffRequest">Datos del miembro del staff a insertar</param>
    /// <returns>True si se insertó correctamente</returns>
    Task<bool> InsertStaff(StaffRequest staffRequest);

    /// <summary>
    /// Actualiza un miembro del staff existente en la base de datos
    /// </summary>
    /// <param name="staffRequest">Datos del miembro del staff a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    Task<bool> UpdateStaff(StaffRequest staffRequest);

    /// <summary>
    /// Elimina un miembro del staff de la base de datos (baja lógica)
    /// </summary>
    /// <param name="id">El ID del miembro del staff a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    Task<bool> DeleteStaff(int id);

    /// <summary>
    /// Convierte un miembro del staff en usuario del sistema
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>True si se convirtió correctamente</returns>
    Task<bool> ConvertStaffToUser(int staffId, string userId);

    /// <summary>
    /// Actualiza el estado activo de un miembro del staff
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    /// <param name="isActive">Nuevo estado activo</param>
    /// <returns>True si se actualizó correctamente</returns>
    Task<bool> UpdateStaffActiveStatus(int staffId, bool isActive);

    /// <summary>
    /// Obtiene todos los miembros del staff de una agencia específica
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <param name="take">El número de miembros del staff a obtener</param>
    /// <param name="skip">El número de miembros del staff a saltar</param>
    /// <param name="name">El nombre del miembro del staff a buscar</param>
    /// <param name="staffTypeId">ID del tipo de staff para filtrar</param>
    /// <returns>Los miembros del staff de la agencia</returns>
    Task<dynamic> GetStaffByAgency(int agencyId, int take, int skip, string name, int? staffTypeId = null);
}