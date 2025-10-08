using System.Data;
using System.Linq;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Models.Response;
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
    /// <returns>La escuela encontrada como SchoolResponse.</returns>
    public async Task<SchoolResponse> GetSchoolById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QueryMultipleAsync("105_GetSchoolById", parameters, commandType: CommandType.StoredProcedure);

            var school = await result.ReadFirstOrDefaultAsync<dynamic>();
            var satellites = result.Read<dynamic>().ToList();
            var educationLevels = result.Read<dynamic>().ToList();
            var services = result.Read<dynamic>().ToList();
            var dayCareHome = await result.ReadFirstOrDefaultAsync<dynamic>();
            var participants = result.Read<dynamic>().ToList();

            if (school == null)
            {
                return null;
            }

            var data = _mappingService.MapSchool(school);
            data.Satellites = satellites.Select(_mappingService.MapSatelliteSchool).ToList();
            data.EducationLevels = educationLevels.Select(_mappingService.MapEducationLevel).ToList();
            data.Services = services.Select(_mappingService.MapSchoolService).ToList();
            data.DayCareHome = dayCareHome != null ? _mappingService.MapSchoolDayCareHome(dayCareHome) : null;
            data.Participants = participants.Select(_mappingService.MapSchoolParticipant).ToList();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting school by id {Id}: {Message}", id, ex.Message);
            throw new Exception($"Error al obtener la escuela con ID {id}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todas las escuelas
    /// </summary>
    /// <param name="take">El número de escuelas a obtener.</param>
    /// <param name="skip">El número de escuelas a saltar.</param>
    /// <param name="name">El nombre de la escuela a buscar.</param>
    /// <param name="cityId">ID de la ciudad para filtrar.</param>
    /// <param name="regionId">ID de la región para filtrar.</param>
    /// <param name="agencyId">ID de la agencia para filtrar.</param>
    /// <param name="alls">Si se deben obtener todas las escuelas.</param>
    /// <param name="isList">Si es para lista o paginación.</param>
    /// <returns>Las escuelas encontradas como SchoolTableResponse.</returns>
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
                        using var result = await dbConnection.QueryMultipleAsync("104_GetSchools", parameters, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(_mappingService.MapSchoolTable).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(1) // Cache for 10 minutes
                );
            }
            else
            {
                using var result = await dbConnection.QueryMultipleAsync("104_GetSchools", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var schools = result.Read<dynamic>().ToList();

                var data = schools.Select(_mappingService.MapSchoolTable).ToList();
                var count = result.ReadFirstOrDefault<int>();

                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting schools with parameters: take={Take}, skip={Skip}, name={Name}, cityId={CityId}, regionId={RegionId}, agencyId={AgencyId}, alls={Alls}: {Message}",
                take, skip, name, cityId, regionId, agencyId, alls, ex.Message);
            throw new Exception($"Error al obtener las escuelas: {ex.Message}", ex);
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
            parameters.Add("@locationTypeId", request.LocationTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@hasWarehouse", request.HasWarehouse, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@hasDiningRoom", request.HasDiningRoom, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@sitePhone", request.SitePhone, DbType.String, ParameterDirection.Input);
            parameters.Add("@extension", request.Extension, DbType.String, ParameterDirection.Input);
            parameters.Add("@mobilePhone", request.MobilePhone, DbType.String, ParameterDirection.Input);
            parameters.Add("@communityId", request.CommunityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@walkersId", request.WalkersId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteTypeId", request.SiteTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@experienceId", request.ExperienceId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewResultId", request.ReviewResultId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewDate", request.ReviewDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@reviewJustification", request.ReviewJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@siteCode", request.SiteCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@generalEnrollment", request.GeneralEnrollment, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@serviceTime", request.ServiceTime, DbType.DateTime, ParameterDirection.Input);

            // ===== CAMPOS ESPECÍFICOS PARA PACNA =====
            parameters.Add("@organizedAthleticPrograms", request.OrganizedAthleticPrograms, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@atRiskService", request.AtRiskService, DbType.Boolean, ParameterDirection.Input);

            // Obtener el siguiente número de sitio para la agencia
            int nextSiteNumber = await GetNextSiteNumber(request.AgencyId.Value);
            parameters.Add("@siteNumber", nextSiteNumber, DbType.Int32, ParameterDirection.Input);

            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("104_InsertSchool", parameters, commandType: CommandType.StoredProcedure);

            int schoolId = parameters.Get<int>("@id");

            // Si tenemos un mainSchoolId, insertamos la escuela principal
            if (request.MainSchoolId.HasValue)
            {
                await InsertSatelliteSchool(request.MainSchoolId.Value, schoolId);
            }

            // Insertar niveles educativos
            if (request.EducationLevels != null && request.EducationLevels.Count != 0)
            {
                var educationLevelIds = request.EducationLevels.Select(e => e.EducationLevelId).ToList();
                await InsertSchoolEducationLevels(schoolId, educationLevelIds);
            }

            // Insertar servicios de alimentación
            await InsertSchoolService(schoolId, request);

            // Insertar información de Day Care Home
            await InsertSchoolDayCareHome(schoolId, request);

            // Insertar grupos de niños específicos (solo si OffersServiceToDifferentGroups = true)
            if (request.DayCareHome?.OffersServiceToDifferentGroups == true &&
                request.ChildGroups != null && request.ChildGroups.Count != 0)
            {
                await InsertSchoolChildGroups(schoolId, request.ChildGroups);
            }

            // Insertar tipos de participantes
            if (request.Participants != null && request.Participants.Count != 0)
            {
                var participantTypeIds = request.Participants.Select(p => p.ParticipantTypeId).ToList();
                await InsertSchoolParticipants(schoolId, participantTypeIds);
            }

            // Insertar días de funcionamiento si se proporcionan fechas
            if (request.OperatingFromDate.HasValue && request.OperatingToDate.HasValue)
            {
                await InsertSiteOperatingDays(schoolId, request.OperatingFromDate.Value, request.OperatingToDate.Value);
            }

            // Invalidar caché
            InvalidateCache(schoolId);

            return schoolId > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la escuela: {Message}", ex.Message);
            throw new Exception($"Error al insertar la escuela: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza una escuela existente
    /// </summary>
    /// <param name="request">La solicitud de la escuela a actualizar.</param>
    /// <returns>True si la escuela se actualizó correctamente, false en caso contrario.</returns>
    public async Task<bool> UpdateSchool(SchoolRequest request)
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
            parameters.Add("@locationTypeId", request.LocationTypeId, DbType.Int32, ParameterDirection.Input);
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
            parameters.Add("@sitePhone", request.SitePhone, DbType.String, ParameterDirection.Input);
            parameters.Add("@extension", request.Extension, DbType.String, ParameterDirection.Input);
            parameters.Add("@mobilePhone", request.MobilePhone, DbType.String, ParameterDirection.Input);


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
            parameters.Add("@generalEnrollment", request.GeneralEnrollment, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteNumber", request.SiteNumber, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@serviceTime", request.ServiceTime, DbType.DateTime, ParameterDirection.Input);

            // ===== CAMPOS ESPECÍFICOS PARA PACNA =====
            parameters.Add("@organizedAthleticPrograms", request.OrganizedAthleticPrograms, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@atRiskService", request.AtRiskService, DbType.Boolean, ParameterDirection.Input);

            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("104_UpdateSchool", parameters, commandType: CommandType.StoredProcedure);

            var rowsAffected = parameters.Get<int>("@rowsAffected");

            // Si tenemos un mainSchoolId, actualizamos la escuela principal
            if (request.MainSchoolId.HasValue && request.Id.HasValue)
            {
                await UpdateSatelliteSchool(request.MainSchoolId.Value, request.Id.Value);
            }

            // Actualizar niveles educativos
            if (request.EducationLevels != null && request.EducationLevels.Count != 0 && request.Id.HasValue)
            {
                var educationLevelIds = request.EducationLevels.Select(e => e.EducationLevelId).ToList();
                await UpdateSchoolEducationLevels(request.Id.Value, educationLevelIds);
            }

            // Actualizar grupos de niños específicos (solo si OffersServiceToDifferentGroups = true)
            if (request.DayCareHome?.OffersServiceToDifferentGroups == true &&
                request.ChildGroups != null && request.ChildGroups.Count != 0 && request.Id.HasValue)
            {
                await UpdateSchoolChildGroups(request.Id.Value, request.ChildGroups);
            }

            // Actualizar servicios de alimentación
            if (request.Services != null && request.Services.Count != 0 && request.Id.HasValue)
            {
                await UpdateSchoolService(request.Id.Value, request.Services);
            }

            // Actualizar información de Day Care Home
            if (request.DayCareHome != null && request.Id.HasValue)
            {
                await UpdateSchoolDayCareHome(request.Id.Value, request.DayCareHome);
            }

            // Actualizar tipos de participantes
            if (request.Participants != null && request.Participants.Count != 0 && request.Id.HasValue)
            {
                var participantTypeIds = request.Participants.Select(p => p.ParticipantTypeId).ToList();
                await UpdateSchoolParticipants(request.Id.Value, participantTypeIds);
            }

            if (rowsAffected > 0 && request.Id.HasValue)
            {
                // Invalidar caché
                InvalidateCache(request.Id.Value);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la escuela: {Message}", ex.Message);
            throw new Exception($"Error al actualizar la escuela: {ex.Message}", ex);
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
            _logger.LogError(ex, "Error al eliminar la escuela: {Message}", ex.Message);
            throw new Exception($"Error al eliminar la escuela: {ex.Message}", ex);
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
            _logger.LogError(ex, "Error al actualizar el estado activo de la escuela {SchoolId}: {Message}", schoolId, ex.Message);
            throw new Exception($"Error al actualizar el estado activo de la escuela {schoolId}: {ex.Message}", ex);
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

    // DESHABILITADO: Funciones de validación comentadas por el momento
    // /// <summary>
    // /// Verifica si el request tiene datos de servicios
    // /// </summary>
    // /// <param name="request">Request de la escuela</param>
    // /// <returns>True si tiene datos de servicios</returns>
    // private static bool HasServiceData(SchoolRequest request)
    // {
    //     return request.Breakfast.HasValue || request.Lunch.HasValue || request.SnackAM.HasValue ||
    //            request.Dinner.HasValue || request.SnackPM.HasValue || request.SnackNight.HasValue;
    // }

    // /// <summary>
    // /// Verifica si el request tiene datos de Day Care Home
    // /// </summary>
    // /// <param name="request">Request de la escuela</param>
    // /// <returns>True si tiene datos de Day Care Home</returns>
    // private static bool HasDayCareHomeData(SchoolRequest request)
    // {
    //     return request.IsAuthorizedToOperate.HasValue || request.HasFamilyDepartmentLicense.HasValue ||
    //            request.NumberOfEnrolledChildren.HasValue || request.NumberOfProviderChildren.HasValue ||
    //            request.NumberOfParticipantsWithBloodTies.HasValue || request.NumberOfParticipantsWithoutBloodTies.HasValue ||
    //            request.MinorsLiveWithProvider.HasValue || request.RelationshipTypeId.HasValue ||
    //            request.OffersServiceToImmigrantChildren.HasValue || request.HomeTypeId.HasValue ||
    //            !string.IsNullOrEmpty(request.AdministratorAuthorizedName) || request.AdministratorBirthDate.HasValue ||
    //            request.OffersServiceToDifferentGroups.HasValue;
    // }

    /// <summary>
    /// Inserta servicios de alimentación para una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="request">Request con datos de servicios</param>
    /// <returns>True si se insertó correctamente</returns>
    private async Task<bool> InsertSchoolService(int schoolId, SchoolRequest request)
    {
        try
        {
            if (request.Services == null || request.Services.Count == 0)
            {
                // Si no hay servicios, insertar un registro vacío
                return await InsertEmptySchoolService(schoolId);
            }

            using IDbConnection dbConnection = _context.CreateConnection();

            foreach (var service in request.Services)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@schoolId", schoolId, DbType.Int32);
                parameters.Add("@childGroupId", service.ChildGroupId, DbType.Int32);
                parameters.Add("@breakfast", service.Breakfast, DbType.Boolean);
                parameters.Add("@breakfastFrom", service.BreakfastFrom, DbType.Time);
                parameters.Add("@breakfastTo", service.BreakfastTo, DbType.Time);
                parameters.Add("@lunch", service.Lunch, DbType.Boolean);
                parameters.Add("@lunchFrom", service.LunchFrom, DbType.Time);
                parameters.Add("@lunchTo", service.LunchTo, DbType.Time);
                parameters.Add("@snackAM", service.SnackAM, DbType.Boolean);
                parameters.Add("@snackAMFrom", service.SnackAMFrom, DbType.Time);
                parameters.Add("@snackAMTo", service.SnackAMTo, DbType.Time);
                parameters.Add("@dinner", service.Dinner, DbType.Boolean);
                parameters.Add("@dinnerFrom", service.DinnerFrom, DbType.Time);
                parameters.Add("@dinnerTo", service.DinnerTo, DbType.Time);
                parameters.Add("@snackPM", service.SnackPM, DbType.Boolean);
                parameters.Add("@snackPMFrom", service.SnackPMFrom, DbType.Time);
                parameters.Add("@snackPMTo", service.SnackPMTo, DbType.Time);
                parameters.Add("@snackNight", service.SnackNight, DbType.Boolean);
                parameters.Add("@snackNightFrom", service.SnackNightFrom, DbType.Time);
                parameters.Add("@snackNightTo", service.SnackNightTo, DbType.Time);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await dbConnection.ExecuteAsync("100_InsertSchoolService", parameters, commandType: CommandType.StoredProcedure);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar servicios para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Inserta un registro vacío de servicios cuando no hay servicios definidos
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <returns>True si se insertó correctamente</returns>
    private async Task<bool> InsertEmptySchoolService(int schoolId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@schoolId", schoolId, DbType.Int32);
            parameters.Add("@childGroupId", null, DbType.Int32);
            parameters.Add("@breakfast", null, DbType.Boolean);
            parameters.Add("@breakfastFrom", null, DbType.Time);
            parameters.Add("@breakfastTo", null, DbType.Time);
            parameters.Add("@lunch", null, DbType.Boolean);
            parameters.Add("@lunchFrom", null, DbType.Time);
            parameters.Add("@lunchTo", null, DbType.Time);
            parameters.Add("@snackAM", null, DbType.Boolean);
            parameters.Add("@snackAMFrom", null, DbType.Time);
            parameters.Add("@snackAMTo", null, DbType.Time);
            parameters.Add("@dinner", null, DbType.Boolean);
            parameters.Add("@dinnerFrom", null, DbType.Time);
            parameters.Add("@dinnerTo", null, DbType.Time);
            parameters.Add("@snackPM", null, DbType.Boolean);
            parameters.Add("@snackPMFrom", null, DbType.Time);
            parameters.Add("@snackPMTo", null, DbType.Time);
            parameters.Add("@snackNight", null, DbType.Boolean);
            parameters.Add("@snackNightFrom", null, DbType.Time);
            parameters.Add("@snackNightTo", null, DbType.Time);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSchoolService", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar servicios vacíos para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Inserta información de Day Care Home para una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="request">Request con datos de Day Care Home</param>
    /// <returns>True si se insertó correctamente</returns>
    private async Task<bool> InsertSchoolDayCareHome(int schoolId, SchoolRequest request)
    {
        try
        {
            if (request.DayCareHome == null)
            {
                // Si no hay información de Day Care Home, insertar un registro vacío
                return await InsertEmptySchoolDayCareHome(schoolId);
            }

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@schoolId", schoolId, DbType.Int32);
            parameters.Add("@isAuthorizedToOperate", request.DayCareHome.IsAuthorizedToOperate, DbType.Boolean);
            parameters.Add("@hasFamilyDepartmentLicense", request.DayCareHome.HasFamilyDepartmentLicense, DbType.Boolean);
            parameters.Add("@numberOfEnrolledChildren", request.DayCareHome.NumberOfEnrolledChildren, DbType.Int32);
            parameters.Add("@numberOfProviderChildren", request.DayCareHome.NumberOfProviderChildren, DbType.Int32);
            parameters.Add("@numberOfParticipantsWithBloodTies", request.DayCareHome.NumberOfParticipantsWithBloodTies, DbType.Int32);
            parameters.Add("@numberOfParticipantsWithoutBloodTies", request.DayCareHome.NumberOfParticipantsWithoutBloodTies, DbType.Int32);
            parameters.Add("@minorsLiveWithProvider", request.DayCareHome.MinorsLiveWithProvider, DbType.Boolean);
            parameters.Add("@relationshipTypeId", request.DayCareHome.RelationshipTypeId, DbType.Int32);
            parameters.Add("@offersServiceToImmigrantChildren", request.DayCareHome.OffersServiceToImmigrantChildren, DbType.Boolean);
            parameters.Add("@homeTypeId", request.DayCareHome.HomeTypeId, DbType.Int32);
            parameters.Add("@administratorAuthorizedName", request.DayCareHome.AdministratorAuthorizedName, DbType.String);
            parameters.Add("@administratorBirthDate", request.DayCareHome.AdministratorBirthDate, DbType.Date);
            parameters.Add("@offersServiceToDifferentGroups", request.DayCareHome.OffersServiceToDifferentGroups, DbType.Boolean);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSchoolDayCareHome", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar información de Day Care Home para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Inserta un registro vacío de Day Care Home cuando no hay información específica
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <returns>True si se insertó correctamente</returns>
    private async Task<bool> InsertEmptySchoolDayCareHome(int schoolId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@schoolId", schoolId, DbType.Int32);
            parameters.Add("@isAuthorizedToOperate", null, DbType.Boolean);
            parameters.Add("@hasFamilyDepartmentLicense", null, DbType.Boolean);
            parameters.Add("@numberOfEnrolledChildren", null, DbType.Int32);
            parameters.Add("@numberOfProviderChildren", null, DbType.Int32);
            parameters.Add("@numberOfParticipantsWithBloodTies", null, DbType.Int32);
            parameters.Add("@numberOfParticipantsWithoutBloodTies", null, DbType.Int32);
            parameters.Add("@minorsLiveWithProvider", null, DbType.Boolean);
            parameters.Add("@relationshipTypeId", null, DbType.Int32);
            parameters.Add("@offersServiceToImmigrantChildren", null, DbType.Boolean);
            parameters.Add("@homeTypeId", null, DbType.Int32);
            parameters.Add("@administratorAuthorizedName", null, DbType.String);
            parameters.Add("@administratorBirthDate", null, DbType.Date);
            parameters.Add("@offersServiceToDifferentGroups", null, DbType.Boolean);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSchoolDayCareHome", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar información vacía de Day Care Home para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Inserta tipos de participantes para una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="participantTypeIds">Lista de IDs de tipos de participantes</param>
    /// <returns>True si se insertaron correctamente</returns>
    private async Task<bool> InsertSchoolParticipants(int schoolId, List<int> participantTypeIds)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32);
            parameters.Add("@participantTypeIds", string.Join(",", participantTypeIds), DbType.String);

            await dbConnection.ExecuteAsync("100_InsertSchoolParticipants", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar tipos de participantes para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Inserta los grupos de niños específicos para una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="childGroups">Lista de grupos de niños</param>
    /// <returns>True si se insertaron correctamente</returns>
    private async Task<bool> InsertSchoolChildGroups(int schoolId, List<SchoolChildGroupRequest> childGroups)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            foreach (var childGroup in childGroups)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@schoolId", schoolId, DbType.Int32);
                parameters.Add("@groupName", childGroup.GroupName, DbType.String);
                parameters.Add("@numberOfChildren", childGroup.NumberOfChildren, DbType.Int32);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await dbConnection.ExecuteAsync("100_InsertSchoolChildGroup", parameters, commandType: CommandType.StoredProcedure);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar grupos de niños para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza los grupos de niños específicos para una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="childGroups">Lista de grupos de niños</param>
    /// <returns>True si se actualizaron correctamente</returns>
    private async Task<bool> UpdateSchoolChildGroups(int schoolId, List<SchoolChildGroupRequest> childGroups)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            // Primero eliminar todos los grupos existentes para esta escuela
            var deleteParameters = new DynamicParameters();
            deleteParameters.Add("@schoolId", schoolId, DbType.Int32);
            await dbConnection.ExecuteAsync("100_DeleteSchoolChildGroupsBySchoolId", deleteParameters, commandType: CommandType.StoredProcedure);

            // Luego insertar los nuevos grupos
            foreach (var childGroup in childGroups)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@schoolId", schoolId, DbType.Int32);
                parameters.Add("@groupName", childGroup.GroupName, DbType.String);
                parameters.Add("@numberOfChildren", childGroup.NumberOfChildren, DbType.Int32);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await dbConnection.ExecuteAsync("100_InsertSchoolChildGroup", parameters, commandType: CommandType.StoredProcedure);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar grupos de niños para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Inserta días de funcionamiento para un sitio basado en las fechas desde y hasta
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="operatingFromDate">Fecha desde</param>
    /// <param name="operatingToDate">Fecha hasta</param>
    /// <returns>Número de días insertados</returns>
    private async Task<int> InsertSiteOperatingDays(int schoolId, DateTime operatingFromDate, DateTime operatingToDate)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@SchoolId", schoolId, DbType.Int32);
            parameters.Add("@OperatingFromDate", operatingFromDate.Date, DbType.Date);
            parameters.Add("@OperatingToDate", operatingToDate.Date, DbType.Date);
            parameters.Add("@DefaultStartTime", TimeSpan.FromHours(8), DbType.Time); // 08:00:00
            parameters.Add("@DefaultEndTime", TimeSpan.FromHours(16), DbType.Time);  // 16:00:00
            parameters.Add("@DefaultComment", "Día de funcionamiento generado automáticamente", DbType.String);

            var result = await dbConnection.QuerySingleAsync<int>("100_InsertSiteOperatingDays", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation("Se insertaron {DaysInserted} días de funcionamiento para la escuela {SchoolId} desde {FromDate} hasta {ToDate}",
                result, schoolId, operatingFromDate.Date, operatingToDate.Date);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar días de funcionamiento para la escuela {SchoolId} desde {FromDate} hasta {ToDate}",
                schoolId, operatingFromDate.Date, operatingToDate.Date);
            throw;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>El siguiente número de sitio disponible</returns>
    private async Task<int> GetNextSiteNumber(int agencyId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@AgencyId", agencyId, DbType.Int32, ParameterDirection.Input);

            var result = await dbConnection.QuerySingleAsync<int>("100_GetNextSiteNumber", parameters, commandType: CommandType.StoredProcedure);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el siguiente número de sitio para la agencia {AgencyId}", agencyId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza servicios de alimentación para una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="services">Lista de servicios</param>
    /// <returns>True si se actualizaron correctamente</returns>
    private async Task<bool> UpdateSchoolService(int schoolId, List<SchoolServiceRequest> services)
    {
        try
        {
            // Primero obtener los servicios existentes
            using IDbConnection dbConnection = _context.CreateConnection();

            // Usar estrategia "eliminar y recrear" para mantener consistencia
            var deleteParameters = new DynamicParameters();
            deleteParameters.Add("@schoolId", schoolId, DbType.Int32);
            await dbConnection.ExecuteAsync("DELETE FROM SchoolService WHERE SchoolId = @schoolId", deleteParameters);

            // Ahora insertar los nuevos servicios
            foreach (var service in services)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@schoolId", schoolId, DbType.Int32);
                parameters.Add("@childGroupId", service.ChildGroupId, DbType.Int32);
                parameters.Add("@breakfast", service.Breakfast, DbType.Boolean);
                parameters.Add("@breakfastFrom", service.BreakfastFrom, DbType.Time);
                parameters.Add("@breakfastTo", service.BreakfastTo, DbType.Time);
                parameters.Add("@lunch", service.Lunch, DbType.Boolean);
                parameters.Add("@lunchFrom", service.LunchFrom, DbType.Time);
                parameters.Add("@lunchTo", service.LunchTo, DbType.Time);
                parameters.Add("@snackAM", service.SnackAM, DbType.Boolean);
                parameters.Add("@snackAMFrom", service.SnackAMFrom, DbType.Time);
                parameters.Add("@snackAMTo", service.SnackAMTo, DbType.Time);
                parameters.Add("@dinner", service.Dinner, DbType.Boolean);
                parameters.Add("@dinnerFrom", service.DinnerFrom, DbType.Time);
                parameters.Add("@dinnerTo", service.DinnerTo, DbType.Time);
                parameters.Add("@snackPM", service.SnackPM, DbType.Boolean);
                parameters.Add("@snackPMFrom", service.SnackPMFrom, DbType.Time);
                parameters.Add("@snackPMTo", service.SnackPMTo, DbType.Time);
                parameters.Add("@snackNight", service.SnackNight, DbType.Boolean);
                parameters.Add("@snackNightFrom", service.SnackNightFrom, DbType.Time);
                parameters.Add("@snackNightTo", service.SnackNightTo, DbType.Time);
                parameters.Add("@dinnerExtended", service.DinnerExtended, DbType.Boolean);
                parameters.Add("@dinnerExtendedFrom", service.DinnerExtendedFrom, DbType.Time);
                parameters.Add("@dinnerExtendedTo", service.DinnerExtendedTo, DbType.Time);
                parameters.Add("@dinnerAtRisk", service.DinnerAtRisk, DbType.Boolean);
                parameters.Add("@dinnerAtRiskFrom", service.DinnerAtRiskFrom, DbType.Time);
                parameters.Add("@dinnerAtRiskTo", service.DinnerAtRiskTo, DbType.Time);
                parameters.Add("@snackExtended", service.SnackExtended, DbType.Boolean);
                parameters.Add("@snackExtendedFrom", service.SnackExtendedFrom, DbType.Time);
                parameters.Add("@snackExtendedTo", service.SnackExtendedTo, DbType.Time);
                parameters.Add("@snackAtRisk", service.SnackAtRisk, DbType.Boolean);
                parameters.Add("@snackAtRiskFrom", service.SnackAtRiskFrom, DbType.Time);
                parameters.Add("@snackAtRiskTo", service.SnackAtRiskTo, DbType.Time);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await dbConnection.ExecuteAsync("100_InsertSchoolService", parameters, commandType: CommandType.StoredProcedure);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar servicios para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza información de Day Care Home para una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="dayCareHome">Datos de Day Care Home</param>
    /// <returns>True si se actualizó correctamente</returns>
    private async Task<bool> UpdateSchoolDayCareHome(int schoolId, SchoolDayCareHomeRequest dayCareHome)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            // Buscar el registro existente
            var existingId = await dbConnection.QuerySingleOrDefaultAsync<int?>(
                "SELECT Id FROM SchoolDayCareHome WHERE SchoolId = @schoolId",
                new { schoolId });

            if (existingId.HasValue)
            {
                // Actualizar registro existente
                var parameters = new DynamicParameters();
                parameters.Add("@id", existingId.Value, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@schoolId", schoolId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@isAuthorizedToOperate", dayCareHome.IsAuthorizedToOperate, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@hasFamilyDepartmentLicense", dayCareHome.HasFamilyDepartmentLicense, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@numberOfEnrolledChildren", dayCareHome.NumberOfEnrolledChildren, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@numberOfProviderChildren", dayCareHome.NumberOfProviderChildren, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@numberOfParticipantsWithBloodTies", dayCareHome.NumberOfParticipantsWithBloodTies, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@numberOfParticipantsWithoutBloodTies", dayCareHome.NumberOfParticipantsWithoutBloodTies, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@minorsLiveWithProvider", dayCareHome.MinorsLiveWithProvider, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@relationshipTypeId", dayCareHome.RelationshipTypeId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@offersServiceToImmigrantChildren", dayCareHome.OffersServiceToImmigrantChildren, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@homeTypeId", dayCareHome.HomeTypeId, DbType.Int32, ParameterDirection.Input);
                parameters.Add("@administratorAuthorizedName", dayCareHome.AdministratorAuthorizedName, DbType.String, ParameterDirection.Input);
                parameters.Add("@administratorBirthDate", dayCareHome.AdministratorBirthDate, DbType.Date, ParameterDirection.Input);
                parameters.Add("@offersServiceToDifferentGroups", dayCareHome.OffersServiceToDifferentGroups, DbType.Boolean, ParameterDirection.Input);
                parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await dbConnection.ExecuteAsync("100_UpdateSchoolDayCareHome", parameters, commandType: CommandType.StoredProcedure);
                var rowsAffected = parameters.Get<int>("@rowsAffected");
                return rowsAffected > 0;
            }
            else
            {
                // Insertar nuevo registro si no existe
                return await InsertSchoolDayCareHome(schoolId, new SchoolRequest { DayCareHome = dayCareHome });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar información de Day Care Home para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza tipos de participantes para una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="participantTypeIds">Lista de IDs de tipos de participantes</param>
    /// <returns>True si se actualizaron correctamente</returns>
    private async Task<bool> UpdateSchoolParticipants(int schoolId, List<int> participantTypeIds)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32);
            parameters.Add("@participantTypeIds", string.Join(",", participantTypeIds), DbType.String);

            await dbConnection.ExecuteAsync("100_UpdateSchoolParticipants", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tipos de participantos para la escuela {SchoolId}", schoolId);
            throw;
        }
    }
}

