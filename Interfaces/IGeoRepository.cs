namespace Api.Interfaces;

public interface IGeoRepository
{
    Task<dynamic> GetAllCitiesFromDb(int take, int skip, string name, bool alls);
    Task<dynamic> GetAllRegionsFromDb(int take, int skip, string name, bool alls);
    Task<dynamic> GetCityById(int cityId);
    Task<dynamic> GetRegionsByCityId(int cityId);
    Task<dynamic> GetRegionById(int regionId);
    Task<dynamic> GetCitiesByRegionId(int regionId);
}