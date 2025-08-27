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
    IHouseholdMemberRepository householdMemberRepository,
    IEmployeeRepository employeeRepository,
    IStaffRepository staffRepository,
    IStaffTypeRepository staffTypeRepository,
    IStaffClassificationRepository staffClassificationRepository,
    IMessageRepository messageRepository,
    ISchoolStaffRepository schoolStaffRepository
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

    [Obsolete("Esta propiedad está deprecada. Use StaffRepository en su lugar. Será eliminada en una versión futura.")]
    public IEmployeeRepository EmployeeRepository { get; private set; } = employeeRepository;
    public IStaffRepository StaffRepository { get; private set; } = staffRepository;
    public IStaffTypeRepository StaffTypeRepository { get; private set; } = staffTypeRepository;
    public IStaffClassificationRepository StaffClassificationRepository { get; private set; } = staffClassificationRepository;
    public IMessageRepository MessageRepository { get; private set; } = messageRepository;
    public ISchoolStaffRepository SchoolStaffRepository { get; private set; } = schoolStaffRepository;
}