using Api.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IGeoRepository GeoRepository { get; }
    IProgramRepository ProgramRepository { get; }
    IAgencyRepository AgencyRepository { get; }
    ISchoolRepository SchoolRepository { get; }
    void Save();
}