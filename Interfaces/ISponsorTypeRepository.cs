using Api.Models;

namespace Api.Interfaces;

public interface ISponsorTypeRepository
{
    Task<dynamic> GetSponsorTypeById(int id);
    Task<dynamic> GetAllSponsorTypes(int take, int skip, string name, bool alls);
    Task<bool> InsertSponsorType(DTOSponsorType sponsorType);
    Task<bool> UpdateSponsorType(DTOSponsorType sponsorType);
    Task<bool> DeleteSponsorType(int id);
}