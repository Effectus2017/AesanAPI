using Api.Models;

namespace Api.Interfaces;

public interface IAgencyStatusRepository
{
    /// <summary>
    /// Obtiene un estado de agencia por su ID.
    /// </summary>
    /// <param name="id">El ID del estado a obtener.</param>
    /// <returns>El estado encontrado o null si no se encuentra.</returns>
    Task<dynamic> GetAgencyStatusById(int id);

    /// <summary>
    /// Obtiene todos los estados de agencia.
    /// </summary>
    /// <param name="take">El número de estados a tomar.</param>
    /// <param name="skip">El número de estados a saltar.</param>
    /// <param name="name">El nombre del estado a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los estados.</param>
    /// <returns>Una lista de estados de agencia.</returns>
    Task<dynamic> GetAllAgencyStatuses(int take, int skip, string name, bool alls);

    /// <summary>
    /// Inserta un nuevo estado de agencia.
    /// </summary>
    /// <param name="status">El estado a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    Task<bool> InsertAgencyStatus(DTOAgencyStatus status);

    /// <summary>
    /// Actualiza un estado de agencia existente.
    /// </summary>
    /// <param name="status">El estado a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    Task<bool> UpdateAgencyStatus(DTOAgencyStatus status);

    /// <summary>
    /// Elimina un estado de agencia existente.
    /// </summary>
    /// <param name="id">El ID del estado a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    Task<bool> DeleteAgencyStatus(int id);
}