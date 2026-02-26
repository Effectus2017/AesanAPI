using Api.Models;

namespace Api.Interfaces;

public interface IAgencyRepository
{
    /// ------------------------------------------------------------------------------------------------
    /// Obtener
    /// ------------------------------------------------------------------------------------------------
    Task<dynamic> GetAgencyById(int id);
    Task<dynamic> GetAgencyByIdAndUserId(int agencyId, string userId);
    /// <summary>
    /// Obtiene los usuarios asignados a una agencia (excluye agency_administrator). Solo para listado del modal.
    /// </summary>
    Task<dynamic> GetAgencyAssignedUsers(int agencyId, string userId);
    Task<dynamic> GetAllAgenciesFromDb(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, string? userId, bool alls, bool isList, bool? isPropietary, string? userFirstName, string? statusName, DateTime? createdAtFrom, DateTime? createdAtTo, long? uieNumber, int? einNumber, long? sdrNumber);
    Task<dynamic> GetAgencyProgramsByUserId(string userId);

    /// ------------------------------------------------------------------------------------------------
    /// Insertar
    /// ------------------------------------------------------------------------------------------------
    Task<int> InsertAgency(AgencyRequest agencyRequest);
    Task<bool> InsertAgencyProgram(int agencyId, int programId);

    /// ------------------------------------------------------------------------------------------------
    /// Actualizar 
    /// ------------------------------------------------------------------------------------------------

    Task<bool> UpdateAgency(int agencyId, AgencyRequest agencyRequest);
    Task<bool> UpdateAgencyLogo(int agencyId, string imageUrl);
    Task<bool> UpdateAgencyStatus(int agencyId, int statusId, string rejectionJustification, string userId);
    Task<bool> UpdateAgencyProgram(int agencyId, int programId, string userId);
    Task<bool> UpdateAgencyInscription(int agencyId, int statusId, string comments, bool appointmentCoordinated, DateTime? appointmentDate, string? rejectionJustification);
    Task<bool> UpdateCompletedRegistrationDate(int agencyId, DateTime completedRegistrationDate);

    /// ------------------------------------------------------------------------------------------------    
    /// Eliminar
    /// ------------------------------------------------------------------------------------------------
    Task<bool> DeleteAgency(int agencyId);

    /// ------------------------------------------------------------------------------------------------
    /// Helpers
    /// ------------------------------------------------------------------------------------------------
    /// <summary>
    /// Obtiene el UserId del evaluador (monitor) asignado a una agencia
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>UserId del evaluador o null si no se encuentra</returns>
    Task<string?> GetEvaluatorUserIdByAgencyId(int agencyId);

    /// <summary>
    /// Obtiene todos los UserIds de evaluadores relacionados a una agencia
    /// Incluye evaluadores asignados directamente a la agencia y evaluadores asignados a los programas de la agencia
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>Lista de UserIds únicos de evaluadores</returns>
    Task<List<string>> GetAllEvaluatorUserIdsByAgencyId(int agencyId);

    /// <summary>
    /// Verifica si un IUE (Identificador Único de Entidad) ya existe en la tabla Agency
    /// </summary>
    /// <param name="uieNumber">El número IUE a verificar</param>
    /// <returns>True si el IUE existe, False si no existe</returns>
    Task<bool> UieNumberExists(long uieNumber);

    /// <summary>
    /// Verifica si un SDR (Número de Registro del Departamento de Estado) ya existe en la tabla Agency
    /// </summary>
    /// <param name="sdrNumber">El número SDR a verificar</param>
    /// <returns>True si el SDR existe, False si no existe</returns>
    Task<bool> SdrNumberExists(long sdrNumber);

    /// <summary>
    /// Verifica si un EIN (Número de Seguro Social Patronal) ya existe en la tabla Agency
    /// </summary>
    /// <param name="einNumber">El número EIN a verificar</param>
    /// <returns>True si el EIN existe, False si no existe</returns>
    Task<bool> EinNumberExists(int einNumber);
}