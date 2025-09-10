using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
namespace Api.Repositories;

public class SchoolRepository(DapperContext context, ILogger<SchoolRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : ISchoolRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SchoolRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Obtiene una escuela por su ID
    /// </summary>
    /// <param name="id">El ID de la escuela a obtener.</param>
    /// <returns>La escuela encontrada.</returns>
    public async Task<DTOSchool> GetSchoolById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QueryMultipleAsync("102_GetSchoolById", parameters, commandType: CommandType.StoredProcedure);

            var school = await result.ReadFirstOrDefaultAsync<dynamic>();
            //var facilities = result.Read<dynamic>().ToList();
            var satellites = result.Read<dynamic>().ToList();
            var educationLevels = result.Read<dynamic>().ToList();

            if (school == null)
            {
                return null;
            }

            var data = _mappingService.MapSchool(school);
            //data.Facilities = facilities.Select(_mappingService.MapFacility).ToList();
            data.Satellites = satellites.Select(_mappingService.MapSatelliteSchool).ToList();
            data.EducationLevels = educationLevels.Select(_mappingService.MapEducationLevel).ToList();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting school by id {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las escuelas
    /// </summary>
    /// <param name="take">El número de escuelas a obtener.</param>
    /// <param name="skip">El número de escuelas a saltar.</param>
    /// <param name="name">El nombre de la escuela a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las escuelas.</param>
    /// <returns>Las escuelas encontradas.</returns>
    public async Task<dynamic> GetAllSchoolsFromDB(int take, int skip, string name, int? cityId, int? regionId, int? agencyId, bool alls, bool isList)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@cityId", cityId == 0 ? null : cityId, DbType.Int32);
            parameters.Add("@regionId", regionId == 0 ? null : regionId, DbType.Int32);
            parameters.Add("@agencyId", agencyId == 0 ? null : agencyId, DbType.Int32);
            parameters.Add("@alls", alls, DbType.Boolean);

            if (isList)
            {
                string cacheKey = string.Format(_appSettings.Cache.Keys.Schools, take, skip, name, cityId, regionId, agencyId, alls);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        using var result = await dbConnection.QueryMultipleAsync("102_GetSchools", parameters, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(_mappingService.MapSchoolList).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(1) // Cache for 10 minutes
                );
            }
            else
            {
                using var result = await dbConnection.QueryMultipleAsync("102_GetSchools", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var schoolsDynamic = result.Read<dynamic>().ToList();
                var count = result.ReadFirstOrDefault<int>();
                var data = schoolsDynamic.Select(_mappingService.MapSchool).ToList();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting schools with parameters: take={Take}, skip={Skip}, name={Name}, cityId={CityId}, regionId={RegionId}, agencyId={AgencyId}, alls={Alls}",
                take, skip, name, cityId, regionId, agencyId, alls);
            throw;
        }
    }

    /// <summary>
    /// Inserta una nueva escuela
    /// </summary>
    /// <param name="request">La solicitud de la escuela a insertar.</param>
    /// <returns>El ID de la escuela insertada.</returns>
    public async Task<bool> InsertSchool(SchoolRequest request)
    {
        try
        {
            // Generar código único de sitio automáticamente si no se proporciona
            if (string.IsNullOrEmpty(request.SiteCode) && request.AgencyId.HasValue)
            {
                var existingSiteCodes = await GetExistingSiteCodes();
                var agencySequenceNumber = await GetAgencySequenceNumber(request.AgencyId.Value);
                request.SiteCode = Utilities.GenerateSiteCode(agencySequenceNumber, existingSiteCodes);
            }

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@agencyId", request.AgencyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@name", request.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@startDate", request.StartDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@address", request.Address, DbType.String, ParameterDirection.Input);
            parameters.Add("@cityId", request.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@regionId", request.RegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@zipCode", request.ZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@latitude", request.Latitude, DbType.Double, ParameterDirection.Input);
            parameters.Add("@longitude", request.Longitude, DbType.Double, ParameterDirection.Input);
            parameters.Add("@postalAddress", request.PostalAddress, DbType.String, ParameterDirection.Input);
            parameters.Add("@postalCityId", request.PostalCityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@postalRegionId", request.PostalRegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@postalZipCode", request.PostalZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@sameAsPhysicalAddress", request.SameAsPhysicalAddress, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@organizationTypeId", request.OrganizationTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@centerTypeId", request.CenterTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@nonProfit", request.NonProfit, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@baseYear", request.BaseYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@renewalYear", request.RenewalYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@operatingFromDate", request.OperatingFromDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@operatingToDate", request.OperatingToDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@operatingDaysCalculated", request.OperatingDaysCalculated, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@kitchenTypeId", request.KitchenTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@groupTypeId", request.GroupTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@deliveryTypeId", request.DeliveryTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@sponsorTypeId", request.SponsorTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@applicantTypeId", request.ApplicantTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@residentialTypeId", request.ResidentialTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@operatingPolicyId", request.OperatingPolicyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@areaTypeId", request.AreaTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@hasWarehouse", request.HasWarehouse, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@hasDiningRoom", request.HasDiningRoom, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@administratorAuthorizedName", request.AdministratorAuthorizedName, DbType.String, ParameterDirection.Input);
            parameters.Add("@sitePhone", request.SitePhone, DbType.String, ParameterDirection.Input);
            parameters.Add("@extension", request.Extension, DbType.String, ParameterDirection.Input);
            parameters.Add("@mobilePhone", request.MobilePhone, DbType.String, ParameterDirection.Input);
            parameters.Add("@breakfast", request.Breakfast, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@breakfastFrom", request.BreakfastFrom, DbType.Time, ParameterDirection.Input);
            parameters.Add("@breakfastTo", request.BreakfastTo, DbType.Time, ParameterDirection.Input);
            parameters.Add("@lunch", request.Lunch, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@lunchFrom", request.LunchFrom, DbType.Time, ParameterDirection.Input);
            parameters.Add("@lunchTo", request.LunchTo, DbType.Time, ParameterDirection.Input);
            parameters.Add("@snack", request.Snack, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@snackFrom", request.SnackFrom, DbType.Time, ParameterDirection.Input);
            parameters.Add("@snackTo", request.SnackTo, DbType.Time, ParameterDirection.Input);
            parameters.Add("@dinner", request.Dinner, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@dinnerFrom", request.DinnerFrom, DbType.Time, ParameterDirection.Input);
            parameters.Add("@dinnerTo", request.DinnerTo, DbType.Time, ParameterDirection.Input);
            parameters.Add("@snackNight", request.SnackNight, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@snackNightFrom", request.SnackNightFrom, DbType.Time, ParameterDirection.Input);
            parameters.Add("@snackNightTo", request.SnackNightTo, DbType.Time, ParameterDirection.Input);
            parameters.Add("@communityId", request.CommunityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@walkersId", request.WalkersId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteTypeId", request.SiteTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@experienceId", request.ExperienceId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewResultId", request.ReviewResultId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewDate", request.ReviewDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@reviewJustification", request.ReviewJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@siteCode", request.SiteCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("102_InsertSchool", parameters, commandType: CommandType.StoredProcedure);

            int schoolId = parameters.Get<int>("@id");

            // Si tenemos un mainSchoolId, insertamos la escuela principal
            if (request.MainSchoolId.HasValue)
            {
                await InsertSatelliteSchool(request.MainSchoolId.Value, schoolId);
            }

            // Insertar niveles educativos
            if (request.EducationLevelIds != null && request.EducationLevelIds.Count != 0)
            {
                await InsertSchoolEducationLevels(schoolId, request.EducationLevelIds);
            }

            // Invalidar caché
            InvalidateCache(schoolId);

            return schoolId > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la escuela");
            throw;
        }
    }

    /// <summary>
    /// Actualiza una escuela existente
    /// </summary>
    /// <param name="request">La solicitud de la escuela a actualizar.</param>
    /// <returns>True si la escuela se actualizó correctamente, false en caso contrario.</returns>
    public async Task<bool> UpdateSchool(DTOSchool request)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@id", request.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@agencyId", request.AgencyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@name", request.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@startDate", request.StartDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@address", request.Address, DbType.String, ParameterDirection.Input);
            parameters.Add("@cityId", request.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@regionId", request.RegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@zipCode", request.ZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@latitude", request.Latitude, DbType.Double, ParameterDirection.Input);
            parameters.Add("@longitude", request.Longitude, DbType.Double, ParameterDirection.Input);
            parameters.Add("@postalAddress", request.PostalAddress, DbType.String, ParameterDirection.Input);
            parameters.Add("@postalCityId", request.PostalCityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@postalRegionId", request.PostalRegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@postalZipCode", request.PostalZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@sameAsPhysicalAddress", request.SameAsPhysicalAddress, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@organizationTypeId", request.OrganizationTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@centerTypeId", request.CenterTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@areaTypeId", request.AreaTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@nonProfit", request.NonProfit, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@baseYear", request.BaseYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@renewalYear", request.RenewalYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@operatingFromDate", request.OperatingFromDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@operatingToDate", request.OperatingToDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@operatingDaysCalculated", request.OperatingDaysCalculated, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@kitchenTypeId", request.KitchenTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@groupTypeId", request.GroupTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@deliveryTypeId", request.DeliveryTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@sponsorTypeId", request.SponsorTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@applicantTypeId", request.ApplicantTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@residentialTypeId", request.ResidentialTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@operatingPolicyId", request.OperatingPolicyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@hasWarehouse", request.HasWarehouse, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@hasDiningRoom", request.HasDiningRoom, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@administratorAuthorizedName", request.AdministratorAuthorizedName, DbType.String, ParameterDirection.Input);
            parameters.Add("@sitePhone", request.SitePhone, DbType.String, ParameterDirection.Input);
            parameters.Add("@extension", request.Extension, DbType.String, ParameterDirection.Input);
            parameters.Add("@mobilePhone", request.MobilePhone, DbType.String, ParameterDirection.Input);
            parameters.Add("@breakfast", request.Breakfast, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@breakfastFrom", request.BreakfastFrom, DbType.Time, ParameterDirection.Input);
            parameters.Add("@breakfastTo", request.BreakfastTo, DbType.Time, ParameterDirection.Input);
            parameters.Add("@lunch", request.Lunch, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@lunchFrom", request.LunchFrom, DbType.Time, ParameterDirection.Input);
            parameters.Add("@lunchTo", request.LunchTo, DbType.Time, ParameterDirection.Input);
            parameters.Add("@snack", request.Snack, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@snackFrom", request.SnackFrom, DbType.Time, ParameterDirection.Input);
            parameters.Add("@snackTo", request.SnackTo, DbType.Time, ParameterDirection.Input);
            parameters.Add("@dinner", request.Dinner, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@dinnerFrom", request.DinnerFrom, DbType.Time, ParameterDirection.Input);
            parameters.Add("@dinnerTo", request.DinnerTo, DbType.Time, ParameterDirection.Input);
            parameters.Add("@snackNight", request.SnackNight, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@snackNightFrom", request.SnackNightFrom, DbType.Time, ParameterDirection.Input);
            parameters.Add("@snackNightTo", request.SnackNightTo, DbType.Time, ParameterDirection.Input);
            parameters.Add("@communityId", request.CommunityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@walkersId", request.WalkersId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteTypeId", request.SiteTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@experienceId", request.ExperienceId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewResultId", request.ReviewResultId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewDate", request.ReviewDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@reviewJustification", request.ReviewJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@inactiveJustification", request.InactiveJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@inactiveDate", request.InactiveDate, DbType.DateTime, ParameterDirection.Input);

            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("102_UpdateSchool", parameters, commandType: CommandType.StoredProcedure);

            var rowsAffected = parameters.Get<int>("@rowsAffected");

            // Si tenemos un mainSchoolId, actualizamos la escuela principal
            if (request.MainSchoolId.HasValue)
            {
                await UpdateSatelliteSchool(request.MainSchoolId.Value, request.Id);
            }

            // Actualizar niveles educativos
            if (request.EducationLevelIds != null && request.EducationLevelIds.Count != 0)
            {
                await UpdateSchoolEducationLevels(request.Id, request.EducationLevelIds);
            }

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(request.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la escuela");
            throw;
        }
    }

    /// <summary>
    /// Elimina una escuela existente
    /// </summary>
    /// <param name="id">El ID de la escuela a eliminar.</param>
    /// <returns>True si la escuela se eliminó correctamente, false en caso contrario.</returns>
    public async Task<bool> DeleteSchool(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);

            var rowsAffected = await dbConnection.ExecuteAsync("102_DeleteSchool", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la escuela");
            throw;
        }
    }

    /// <summary>
    /// Invalida la caché para la escuela
    /// </summary>
    /// <param name="schoolId">El ID de la escuela.</param>
    private void InvalidateCache(int? schoolId = null)
    {
        if (schoolId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.Schools, 0, 0, "", false));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.Schools);
        _logger.LogInformation("Cache invalidado para School Repository");
    }

    /// <summary>
    /// Inserta una escuela satélite
    /// </summary>
    /// <param name="mainSchoolId">El ID de la escuela principal.</param>
    /// <param name="satelliteSchoolId">El ID de la escuela satélite.</param>
    /// <returns>True si la escuela satélite se insertó correctamente, false en caso contrario.</returns>
    public async Task<bool> InsertSatelliteSchool(int mainSchoolId, int satelliteSchoolId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@mainSchoolId", mainSchoolId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@satelliteSchoolId", satelliteSchoolId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@comment", "Escuela actualizada", DbType.String, ParameterDirection.Input);
            parameters.Add("@isActive", true, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("102_InsertSatelliteSchool", parameters, commandType: CommandType.StoredProcedure);

            int id = parameters.Get<int>("@id");

            if (id == 0)
            {
                _logger.LogError("Error al insertar la escuela satélite {SatelliteSchoolId}", satelliteSchoolId);
                return false;
            }

            InvalidateCache(satelliteSchoolId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al insertar SatelliteSchool para MainSchoolId={mainSchoolId}, SatelliteSchoolId={satelliteSchoolId}");
            return false;
        }
    }

    /// <summary>
    /// Actualiza o inserta una escuela satélite
    /// </summary>
    /// <param name="mainSchoolId">El ID de la escuela principal.</param>
    /// <param name="satelliteSchoolId">El ID de la escuela satélite.</param>
    /// <returns>True si la escuela satélite se actualizó o insertó correctamente, false en caso contrario.</returns>
    public async Task<bool> UpdateSatelliteSchool(int mainSchoolId, int satelliteSchoolId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@mainSchoolId", mainSchoolId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@satelliteSchoolId", satelliteSchoolId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@comment", "Escuela actualizada/creada", DbType.String, ParameterDirection.Input);
            parameters.Add("@isActive", true, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("103_UpdateSchoolSatellite", parameters, commandType: CommandType.StoredProcedure);

            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected == 0)
            {
                _logger.LogError("Error al actualizar/insertar la escuela principal {MainSchoolId} con la escuela satélite {SatelliteSchoolId}", mainSchoolId, satelliteSchoolId);
                return false;
            }

            return true;
        }

        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar/insertar SatelliteSchool para MainSchoolId={mainSchoolId}, SatelliteSchoolId={satelliteSchoolId}");
            return false;
        }
    }


    /// <summary>
    /// Inserta múltiples niveles educativos para una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="educationLevelIds">Lista de IDs de niveles educativos</param>
    /// <returns>True si se insertaron correctamente</returns>
    private async Task<bool> InsertSchoolEducationLevels(int schoolId, List<int> educationLevelIds)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32);
            parameters.Add("@educationLevelIds", string.Join(",", educationLevelIds), DbType.String);

            await dbConnection.ExecuteAsync("100_InsertSchoolEducationLevels", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar niveles educativos para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza múltiples niveles educativos para una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="educationLevelIds">Lista de IDs de niveles educativos</param>
    /// <returns>True si se actualizaron correctamente</returns>
    private async Task<bool> UpdateSchoolEducationLevels(int schoolId, List<int> educationLevelIds)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32);
            parameters.Add("@educationLevelIds", string.Join(",", educationLevelIds), DbType.String);

            await dbConnection.ExecuteAsync("100_UpdateSchoolEducationLevels", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar niveles educativos para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Verifica si existe una escuela principal en la base de datos
    /// </summary>
    /// <returns>True si existe una escuela principal, false en caso contrario</returns>
    public async Task<bool> HasMainSchool()
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var result = await dbConnection.ExecuteScalarAsync<int>("102_HasMainSchool", commandType: CommandType.StoredProcedure);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si existe una escuela principal");
            throw;
        }
    }

    /// <summary>
    /// Actualiza el estado activo/inactivo de una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="isActive">Estado activo (true) o inactivo (false)</param>
    /// <param name="inactiveJustification">Justificación cuando se inactiva (requerida si isActive es false)</param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    public async Task<bool> UpdateSchoolActiveStatus(int schoolId, bool isActive, string inactiveJustification = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", schoolId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@isActive", isActive, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@inactiveJustification", inactiveJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("103_UpdateSchoolActiveStatus", parameters, commandType: CommandType.StoredProcedure);

            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(schoolId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado activo de la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los códigos de sitios existentes
    /// </summary>
    /// <returns>Lista de códigos de sitios existentes</returns>
    public async Task<List<string>> GetExistingSiteCodes()
    {
        try
        {
            using IDbConnection connection = _context.CreateConnection();
            var codes = await connection.QueryAsync<string>("112_GetExistingSiteCodes", commandType: CommandType.StoredProcedure);
            return codes?.ToList() ?? [];
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener códigos de sitios existentes");
            return [];
        }
    }

    /// <summary>
    /// Obtiene el código de secuencia de la agencia para generar códigos de sitios
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>Código de secuencia de la agencia</returns>
    public async Task<string> GetAgencySequenceNumber(int agencyId)
    {
        try
        {
            using IDbConnection connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId, DbType.Int32);

            var agencyCode = await connection.QueryFirstOrDefaultAsync<string>("112_GetAgencyCodeById", parameters, commandType: CommandType.StoredProcedure);

            if (string.IsNullOrEmpty(agencyCode))
                return "001"; // Valor por defecto si no se encuentra la agencia

            // Extraer el número de secuencia del código de agencia (última parte después del último guión)
            var parts = agencyCode.Split('-');
            return parts.Length > 0 ? parts[^1] : "001";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener código de secuencia de agencia {AgencyId}", agencyId);
            return "001";
        }
    }
}

