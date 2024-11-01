public interface IAgencyRepository
{
    Task<dynamic> GetAllAgenciesFromDb(int take, int skip, string name, int? regionId, int? cityId, int? programId, int? statusId, bool alls);
    Task<dynamic> GetAgencyById(int id);
    Task<dynamic> GetAllAgencyStatus(int take, int skip, string name, bool alls);
    Task<bool> UpdateAgencyStatus(int agencyId, int statusId);
}