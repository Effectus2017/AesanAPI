using Api.Models;

public interface IProgramRepository
{
    Task<dynamic> GetAllProgramsFromDb(int take, int skip, string name, bool alls);
    Task<dynamic> GetProgramById(int id);
    Task<int> InsertProgram(ProgramRequest programRequest);
    Task<int> InsertProgramInscription(ProgramInscriptionRequest request);
    Task<List<DTOFoodAuthority>> GetAllFoodAuthorities();
    Task<List<DTOOperatingPolicy>> GetAllOperatingPolicies();
    Task<List<DTOAlternativeCommunication>> GetAllAlternativeCommunications();
    Task<List<DTOOptionSelection>> GetAllOptionSelections();
    Task<List<DTOFederalFundingCertification>> GetAllFederalFundingCertifications();
    Task<dynamic> GetAllProgramInscriptions(int take, int skip, int? agencyId = null, int? programId = null);
}