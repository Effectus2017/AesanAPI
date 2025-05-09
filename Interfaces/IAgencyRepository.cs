using Api.Models;

namespace Api.Interfaces;

public interface IAgencyRepository
{
    /// ------------------------------------------------------------------------------------------------
    /// Obtener
    /// ------------------------------------------------------------------------------------------------
    Task<dynamic> GetAgencyById(int id);
    Task<dynamic> GetAgencyByIdAndUserId(int agencyId, string userId);
    Task<dynamic> GetAllAgenciesFromDb(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, string? userId, bool alls);
    Task<dynamic> GetAllAgenciesList(int? id, string name, bool alls);
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
    Task<bool> UpdateAgencyStatus(int agencyId, int statusId, string rejectionJustification);
    Task<bool> UpdateAgencyProgram(int agencyId, int programId, string userId);
    Task<bool> UpdateAgencyInscription(int agencyId, int statusId, string comments, bool appointmentCoordinated, DateTime? appointmentDate, string? rejectionJustification);

    /// ------------------------------------------------------------------------------------------------    
    /// Eliminar
    /// ------------------------------------------------------------------------------------------------
    Task<bool> DeleteAgency(int agencyId);
}