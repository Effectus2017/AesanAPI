using Api.Models;

public interface IAgencyRepository
{
    /// ------------------------------------------------------------------------------------------------
    /// Obtener
    /// ------------------------------------------------------------------------------------------------
    Task<dynamic> GetAgencyById(int id);
    Task<dynamic> GetAgencyByIdAndUserId(int agencyId, string userId);
    Task<dynamic> GetAllAgenciesFromDb(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, string? userId, bool alls);
    Task<dynamic> GetAllAgencyStatus(int take, int skip, string name, bool alls);
    Task<List<DTOProgram>> GetAgencyProgramsByUserId(string userId);

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
    Task<bool> UpdateAgencyProgram(int agencyId, int programId, int statusId, string userId, string comment, bool appointmentCoordinated, DateTime? appointmentDate);

    /// ------------------------------------------------------------------------------------------------    
    /// Eliminar
    /// ------------------------------------------------------------------------------------------------
    Task<bool> DeleteAgency(int agencyId);
}