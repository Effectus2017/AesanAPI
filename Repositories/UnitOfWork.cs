using Api.Interfaces;
using Api.Data;
namespace Api.Repositories;

public class UnitOfWork(ApplicationDbContext context,
    IUserRepository userRepository,
    IGeoRepository geoRepository,
    IProgramRepository programRepository,
    IAgencyRepository agencyRepository,
    ISchoolRepository schoolRepository,
    IAgencyUsersRepository agencyUsersRepository,
    IAgencyFilesRepository agencyFilesRepository,
    ICenterTypeRepository centerTypeRepository,
    IHouseholdRepository householdRepository,
    IHouseholdMemberRepository householdMemberRepository
    ) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    public IUserRepository UserRepository { get; private set; } = userRepository;
    public IGeoRepository GeoRepository { get; private set; } = geoRepository;
    public IProgramRepository ProgramRepository { get; private set; } = programRepository;
    public IAgencyRepository AgencyRepository { get; private set; } = agencyRepository;
    public ISchoolRepository SchoolRepository { get; private set; } = schoolRepository;
    public IAgencyUsersRepository AgencyUsersRepository { get; private set; } = agencyUsersRepository;
    public IAgencyFilesRepository AgencyFilesRepository { get; private set; } = agencyFilesRepository;
    public ICenterTypeRepository CenterTypeRepository { get; private set; } = centerTypeRepository;
    public IHouseholdRepository HouseholdRepository { get; private set; } = householdRepository;
    public IHouseholdMemberRepository HouseholdMemberRepository { get; private set; } = householdMemberRepository;
}