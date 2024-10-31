using Api.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IGeoRepository GeoRepository { get; }
    IProgramRepository ProgramRepository { get; }
    IAgencyRepository AgencyRepository { get; }
    void Save();
}