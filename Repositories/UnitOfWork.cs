using Api.Interfaces;
using Api.Data;
namespace Api.Repositories;

public class UnitOfWork(ApplicationDbContext context,
    IUserRepository userRepository,
    IGeoRepository geoRepository,
    IProgramRepository programRepository,
    IAgencyRepository agencyRepository,
    ISiteRepository siteRepository,
    ISchoolRepository schoolRepository,
    ISchoolSiteRepository schoolSiteRepository,
    IAgencyUsersRepository agencyUsersRepository,
    IAgencyFilesRepository agencyFilesRepository,
    ICenterTypeRepository centerTypeRepository,
    IHouseholdRepository householdRepository,
    IHouseholdMemberRepository householdMemberRepository,
    IStaffRepository staffRepository,
    IStaffTypeRepository staffTypeRepository,
    IStaffClassificationRepository staffClassificationRepository,
    IMessageRepository messageRepository,
    ISiteStaffRepository siteStaffRepository,
    ISiteCalendarRepository siteCalendarRepository,
    ISiteExcursionRepository siteExcursionRepository
    ) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    public IUserRepository UserRepository { get; private set; } = userRepository;
    public IGeoRepository GeoRepository { get; private set; } = geoRepository;
    public IProgramRepository ProgramRepository { get; private set; } = programRepository;
    public IAgencyRepository AgencyRepository { get; private set; } = agencyRepository;
    public ISiteRepository SiteRepository { get; private set; } = siteRepository;
    public ISchoolRepository SchoolRepository { get; private set; } = schoolRepository;
    public ISchoolSiteRepository SchoolSiteRepository { get; private set; } = schoolSiteRepository;
    public IAgencyUsersRepository AgencyUsersRepository { get; private set; } = agencyUsersRepository;
    public IAgencyFilesRepository AgencyFilesRepository { get; private set; } = agencyFilesRepository;
    public ICenterTypeRepository CenterTypeRepository { get; private set; } = centerTypeRepository;
    public IHouseholdRepository HouseholdRepository { get; private set; } = householdRepository;
    public IHouseholdMemberRepository HouseholdMemberRepository { get; private set; } = householdMemberRepository;

    public IStaffRepository StaffRepository { get; private set; } = staffRepository;
    public IStaffTypeRepository StaffTypeRepository { get; private set; } = staffTypeRepository;
    public IStaffClassificationRepository StaffClassificationRepository { get; private set; } = staffClassificationRepository;
    public IMessageRepository MessageRepository { get; private set; } = messageRepository;
    public ISiteStaffRepository SiteStaffRepository { get; private set; } = siteStaffRepository;
    public ISiteCalendarRepository SiteCalendarRepository { get; private set; } = siteCalendarRepository;
    public ISiteExcursionRepository SiteExcursionRepository { get; private set; } = siteExcursionRepository;
}