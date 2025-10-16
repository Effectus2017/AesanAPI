using Api.Interfaces;
using Api.Models;


public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IGeoRepository GeoRepository { get; }
    IProgramRepository ProgramRepository { get; }
    IAgencyRepository AgencyRepository { get; }
    ISiteRepository SiteRepository { get; }
    ISchoolRepository SchoolRepository { get; }
    ISchoolSiteRepository SchoolSiteRepository { get; }
    IAgencyUsersRepository AgencyUsersRepository { get; }
    IAgencyFilesRepository AgencyFilesRepository { get; }
    ICenterTypeRepository CenterTypeRepository { get; }
    IHouseholdRepository HouseholdRepository { get; }
    IHouseholdMemberRepository HouseholdMemberRepository { get; }
    IStaffRepository StaffRepository { get; }
    IStaffTypeRepository StaffTypeRepository { get; }
    IStaffClassificationRepository StaffClassificationRepository { get; }
    IMessageRepository MessageRepository { get; }
    ISiteStaffRepository SiteStaffRepository { get; }
    ISiteCalendarRepository SiteCalendarRepository { get; }
}