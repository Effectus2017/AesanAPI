using Api.Models;

namespace Api.Interfaces;

public interface IProgramRepository
{
    Task<dynamic> GetProgramById(int id);
    Task<dynamic> GetAllProgramsFromDb(int take, int skip, string name, bool alls);
    Task<dynamic> GetAllProgramInscriptions(int take, int skip, int? agencyId = null, int? programId = null);
    Task<int> InsertProgram(ProgramRequest programRequest);
    Task<int> InsertProgramInscription(ProgramInscriptionRequest request);
    Task<List<DTOOptionSelection>> GetAllOptionSelections();
}