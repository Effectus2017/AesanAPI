namespace Api.Interfaces;

public interface IGeoRepository
{

    /// <summary>
    /// Obtiene una ciudad por su ID
    /// </summary>
    /// <param name="cityId">El ID de la ciudad</param>
    /// <returns>La ciudad encontrada</returns>
    Task<dynamic> GetCityById(int cityId);

    /// <summary>
    /// Obtiene una región por su ID
    /// </summary>
    /// <param name="regionId">El ID de la región</param>
    /// <returns>La región encontrada</returns>
    Task<dynamic> GetRegionById(int regionId);

    /// <summary>
    /// Obtiene todas las regiones por ID de ciudad
    /// </summary>
    /// <param name="cityId">El ID de la ciudad</param>
    /// <returns>Las regiones encontradas</returns>
    Task<dynamic> GetRegionsByCityId(int cityId);

    /// <summary>
    /// Obtiene todas las ciudades por ID de región
    /// </summary>
    /// <param name="regionId">El ID de la región</param>
    /// <returns>Las ciudades encontradas</returns>
    Task<dynamic> GetCitiesByRegionId(int regionId);

    /// <summary>
    /// Obtiene todas las ciudades de la base de datos
    /// </summary>
    /// <param name="take">El número de ciudades a tomar</param>
    /// <param name="skip">El número de ciudades a saltar</param>
    /// <param name="name">El nombre de la ciudad a buscar</param>
    /// <param name="alls">Si se deben obtener todas las ciudades</param>
    Task<dynamic> GetAllCitiesFromDb(int take, int skip, string name, bool alls);

    /// <summary>
    /// Obtiene todas las regiones de la base de datos
    /// </summary>
    /// <param name="take">El número de regiones a tomar</param>
    /// <param name="skip">El número de regiones a saltar</param>
    /// <param name="name">El nombre de la región a buscar</param>
    /// <param name="alls">Si se deben obtener todas las regiones</param>
    Task<dynamic> GetAllRegionsFromDb(int take, int skip, string name, bool alls);

}