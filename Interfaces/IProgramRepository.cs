public interface IProgramRepository
{
    Task<dynamic> GetAllProgramsFromDb(int take, int skip, string name, bool alls);
    Task<dynamic> GetProgramById(int id);
}