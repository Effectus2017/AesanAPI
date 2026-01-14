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
    /// <param name="alls">Si se deben obtener todas las agencias</param>
    /// <returns>Lista de agencias asignadas al usuario</returns>
    Task<dynamic> GetUserAssignedAgencies(string userId, int take, int skip, bool alls, bool isList);

    /// <summary>
    /// Obtiene la agencia asignada a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>La agencia asignada al usuario</returns>
    Task<dynamic> GetUserAssignedAgency(string userId);

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

    /// <summary>
    /// Calcula el AgencyAssignmentType apropiado basado en el rol del usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>El AgencyAssignmentType calculado</returns>
    Task<string> CalculateAgencyAssignmentTypeFromRole(string userId);

    /// <summary>
    /// Asigna una agencia a un usuario usando AgencyAssignmentType
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="agencyId">ID de la agencia</param>
    /// <param name="assignedBy">ID del usuario que asigna</param>
    /// <param name="agencyAssignmentType">Tipo de asignación (AGENCY_OWNER, AGENCY_STAFF, NUTRE_COORDINATOR, etc.)</param>
    /// <returns>True si la asignación fue exitosa</returns>
    Task<bool> AssignAgencyToUser(string userId, int agencyId, string assignedBy, string agencyAssignmentType);
}