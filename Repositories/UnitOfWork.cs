using Api.Interfaces;
using Api.Data;
namespace Api.Repositories;

public class UnitOfWork(ApplicationDbContext context,
    IUserRepository userRepository,
    IGeoRepository geoRepository,
    IProgramRepository programRepository,
    IAgencyRepository agencyRepository,
    ISchoolRepository schoolRepository,
    IAgencyUserAssignmentRepository agencyUserAssignmentRepository) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    public IUserRepository UserRepository { get; private set; } = userRepository; // Asignar repositorio
    public IGeoRepository GeoRepository { get; private set; } = geoRepository; // Asignar repositorio
    public IProgramRepository ProgramRepository { get; private set; } = programRepository; // Asignar repositorio
    public IAgencyRepository AgencyRepository { get; private set; } = agencyRepository; // Asignar repositorio
    public ISchoolRepository SchoolRepository { get; private set; } = schoolRepository;
    public IAgencyUserAssignmentRepository AgencyUserAssignmentRepository { get; private set; } = agencyUserAssignmentRepository;
    public void Save()
    {
        try
        {
            _context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}