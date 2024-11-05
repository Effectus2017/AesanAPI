using Api.Models;

public interface IAgencyRepository
{
    Task<dynamic> GetAllAgenciesFromDb(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, bool alls);
    Task<dynamic> GetAgencyById(int id);
    Task<dynamic> GetAllAgencyStatus(int take, int skip, string name, bool alls);

    // Insertar Agencia
    Task<int> InsertAgency(AgencyRequest agencyRequest);
    // Programas
    Task<bool> InsertAgencyProgram(int agencyId, int programId);

    Task<bool> UpdateAgency(int agencyId, AgencyRequest agencyRequest);
    Task<bool> UpdateAgencyLogo(int agencyId, string imageUrl);
    Task<bool> UpdateAgencyStatus(int agencyId, int statusId, string rejectionJustification);
}