using Api.Interfaces;
using Api.Models;


public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IGeoRepository GeoRepository { get; }
    IProgramRepository ProgramRepository { get; }
    IAgencyRepository AgencyRepository { get; }
    ISchoolRepository SchoolRepository { get; }
    IAgencyUsersRepository AgencyUsersRepository { get; }
    IAgencyFilesRepository AgencyFilesRepository { get; }
    ICenterTypeRepository CenterTypeRepository { get; }
    IHouseholdRepository HouseholdRepository { get; }
    IHouseholdMemberRepository HouseholdMemberRepository { get; }
    [Obsolete("Esta propiedad está deprecada. Use StaffRepository en su lugar. Será eliminada en una versión futura.")]
    IEmployeeRepository EmployeeRepository { get; }
    IStaffRepository StaffRepository { get; }
    IStaffTypeRepository StaffTypeRepository { get; }
    IStaffClassificationRepository StaffClassificationRepository { get; }
    IMessageRepository MessageRepository { get; }
}