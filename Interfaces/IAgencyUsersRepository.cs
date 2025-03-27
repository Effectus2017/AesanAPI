using Api.Models;

namespace Api.Interfaces;

public interface IAgencyUsersRepository
{
    /// <summary>
    /// Obtiene las agencias asignadas a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="take">Número de registros a tomar</param>
    /// <param name="skip">Número de registros a saltar</param>
    /// <returns>Lista de agencias asignadas al usuario</returns>
    Task<dynamic> GetUserAssignedAgencies(string userId, int take, int skip);

    /// <summary>
    /// Obtiene la agencia asignada a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>La agencia asignada al usuario</returns>
    Task<dynamic> GetUserAssignedAgency(string userId);

    /// <summary>
    /// Asigna una agencia a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="agencyId">ID de la agencia</param>
    /// <param name="assignedBy">ID del usuario que asigna</param>
    /// <returns>True si la asignación fue exitosa</returns>
    Task<bool> AssignAgencyToUser(string userId, int agencyId, string assignedBy, bool isOwner = false, bool isMonitor = false);

    /// <summary>
    /// Desasigna una agencia de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>True si la desasignación fue exitosa</returns>
    Task<bool> UnassignAgencyFromUser(string userId, int agencyId);

    /// <summary>
    /// Actualiza la agencia principal a la que pertenece un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="agencyId">ID de la nueva agencia</param>
    /// <param name="assignedBy">ID del usuario que realiza el cambio</param>
    /// <returns>True si la actualización fue exitosa</returns>
    Task<bool> UpdateUserMainAgency(string userId, int agencyId, string assignedBy);
}