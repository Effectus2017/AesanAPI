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
}