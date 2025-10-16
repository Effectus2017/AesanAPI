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

public class SiteRepository(DapperContext context, ILogger<SiteRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : ISiteRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SiteRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Obtiene un sitio por su ID
    /// </summary>
    /// <param name="id">El ID del sitio a obtener.</param>
    /// <returns>El sitio encontrado como SiteResponse.</returns>
    public async Task<SiteResponse> GetSiteById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QueryMultipleAsync("105_GetSiteById", parameters, commandType: CommandType.StoredProcedure);

            var site = await result.ReadFirstOrDefaultAsync<dynamic>();
            var educationLevels = result.Read<dynamic>().ToList();
            var services = result.Read<dynamic>().ToList();
            var dayCareHome = await result.ReadFirstOrDefaultAsync<dynamic>();
            var participants = result.Read<dynamic>().ToList();

            if (site == null)
            {
                return null;
            }

            var data = _mappingService.MapSite(site);
            data.EducationLevels = educationLevels.Select(_mappingService.MapEducationLevel).ToList();
            data.Services = services.Select(_mappingService.MapSiteService).ToList();
            data.DayCareHome = dayCareHome != null ? _mappingService.MapSiteDayCareHome(dayCareHome) : null;
            data.Participants = participants.Select(_mappingService.MapSiteParticipant).ToList();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting site by id {Id}: {Message}", id, ex.Message);
            throw new Exception($"Error al obtener el sitio con ID {id}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los sitios
    /// </summary>
    /// <param name="take">El número de sitios a obtener.</param>
    /// <param name="skip">El número de sitios a saltar.</param>
    /// <param name="name">El nombre del sitio a buscar.</param>
    /// <param name="cityId">ID de la ciudad para filtrar.</param>
    /// <param name="regionId">ID de la región para filtrar.</param>
    /// <param name="agencyId">ID de la agencia para filtrar.</param>
    /// <param name="alls">Si se deben obtener todos los sitios.</param>
    /// <param name="isList">Si es para lista o paginación.</param>
    /// <returns>Los sitios encontrados como SiteTableResponse.</returns>
    public async Task<dynamic> GetAllSitesFromDB(int take, int skip, string name, int? cityId, int? regionId, int? agencyId, bool alls, bool isList)
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
                string cacheKey = string.Format(_appSettings.Cache.Keys.Sites, take, skip, name, cityId, regionId, agencyId, alls);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        using var result = await dbConnection.QueryMultipleAsync("104_GetSites", parameters, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(_mappingService.MapSiteTable).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(1) // Cache for 10 minutes
                );
            }
            else
            {
                using var result = await dbConnection.QueryMultipleAsync("104_GetSites", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var sites = result.Read<dynamic>().ToList();

                var data = sites.Select(_mappingService.MapSiteTable).ToList();
                var count = result.ReadFirstOrDefault<int>();

                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sites with parameters: take={Take}, skip={Skip}, name={Name}, cityId={CityId}, regionId={RegionId}, agencyId={AgencyId}, alls={Alls}: {Message}",
                take, skip, name, cityId, regionId, agencyId, alls, ex.Message);
            throw new Exception($"Error al obtener los sitios: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Inserta un nuevo sitio
    /// </summary>
    /// <param name="request">La solicitud del sitio a insertar.</param>
    /// <returns>El ID del sitio insertado.</returns>
    public async Task<bool> InsertSite(SiteRequest request)
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
            parameters.Add("@startDate", request.StartDate, DbType.Date, ParameterDirection.Input);
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
            parameters.Add("@operatingFromDate", request.OperatingFromDate, DbType.Date, ParameterDirection.Input);
            parameters.Add("@operatingToDate", request.OperatingToDate, DbType.Date, ParameterDirection.Input);
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
            parameters.Add("@siteLocationId", request.SiteLocationId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@experienceId", request.ExperienceId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewResultId", request.ReviewResultId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewDate", request.ReviewDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@reviewJustification", request.ReviewJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@generalEnrollment", request.GeneralEnrollment, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@serviceTime", request.ServiceTime, DbType.DateTime, ParameterDirection.Input);

            // ===== CAMPOS ESPECÍFICOS PARA PACNA =====
            parameters.Add("@organizedAthleticPrograms", request.OrganizedAthleticPrograms, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@atRiskService", request.AtRiskService, DbType.Boolean, ParameterDirection.Input);

            // Obtener el siguiente número de sitio para la agencia
            int nextSiteNumber = await GetNextSiteNumber(request.AgencyId.Value);
            parameters.Add("@siteNumber", nextSiteNumber, DbType.Int32, ParameterDirection.Input);

            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("104_InsertSite", parameters, commandType: CommandType.StoredProcedure);

            int siteId = parameters.Get<int>("@id");

            // Insertar niveles educativos
            if (request.EducationLevels != null && request.EducationLevels.Count != 0)
            {
                var educationLevelIds = request.EducationLevels.Select(e => e.EducationLevelId).ToList();
                await InsertSiteEducationLevels(siteId, educationLevelIds);
            }

            // Insertar servicios de alimentación
            await InsertSiteService(siteId, request);

            // Insertar información de Day Care Home solo si la agencia es Day Care Home
            if (request.IsDayCareHome == true)
            {
                await InsertSiteDayCareHome(siteId, request);
            }

            // Insertar grupos de niños específicos (solo si OffersServiceToDifferentGroups = true)
            if (request.DayCareHome?.OffersServiceToDifferentGroups == true &&
                request.ChildGroups != null && request.ChildGroups.Count != 0)
            {
                await InsertSiteChildGroups(siteId, request.ChildGroups);
            }

            // Insertar tipos de participantes
            if (request.Participants != null && request.Participants.Count != 0)
            {
                var participantTypeIds = request.Participants.Select(p => p.ParticipantTypeId).ToList();
                await InsertSiteParticipants(siteId, participantTypeIds);
            }

            // Insertar días de funcionamiento si se proporcionan fechas
            if (request.OperatingFromDate.HasValue && request.OperatingToDate.HasValue)
            {
                await InsertSiteOperatingDays(siteId, request.OperatingFromDate.Value, request.OperatingToDate.Value);
            }

            // Invalidar caché
            InvalidateCache(siteId);

            return siteId > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el sitio: {Message}", ex.Message);
            throw new Exception($"Error al insertar el sitio: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza un sitio existente
    /// </summary>
    /// <param name="request">La solicitud del sitio a actualizar.</param>
    /// <returns>True si el sitio se actualizó correctamente, false en caso contrario.</returns>
    public async Task<bool> UpdateSite(SiteRequest request)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@id", request.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@agencyId", request.AgencyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@name", request.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@startDate", request.StartDate, DbType.Date, ParameterDirection.Input);
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
            parameters.Add("@operatingFromDate", request.OperatingFromDate, DbType.Date, ParameterDirection.Input);
            parameters.Add("@operatingToDate", request.OperatingToDate, DbType.Date, ParameterDirection.Input);
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
            parameters.Add("@administratorAuthorizedName", request.AdministratorAuthorizedName, DbType.String, ParameterDirection.Input);
            parameters.Add("@communityId", request.CommunityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@walkersId", request.WalkersId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteTypeId", request.SiteTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteLocationId", request.SiteLocationId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@experienceId", request.ExperienceId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewResultId", request.ReviewResultId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewDate", request.ReviewDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@reviewJustification", request.ReviewJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@isMainSite", request.IsMainSite, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@inactiveJustification", request.InactiveJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@inactiveDate", request.InactiveDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@generalEnrollment", request.GeneralEnrollment, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@serviceTime", request.ServiceTime, DbType.DateTime, ParameterDirection.Input);

            // ===== CAMPOS ESPECÍFICOS PARA PACNA =====
            parameters.Add("@organizedAthleticPrograms", request.OrganizedAthleticPrograms, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@atRiskService", request.AtRiskService, DbType.Boolean, ParameterDirection.Input);

            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("104_UpdateSite", parameters, commandType: CommandType.StoredProcedure);

            int rowsAffected = parameters.Get<int>("@rowsAffected");

            // Solo continuar con las actualizaciones relacionadas si el sitio principal se actualizó correctamente
            if (rowsAffected > 0)
            {
                // Actualizar niveles educativos
                if (request.EducationLevels != null && request.EducationLevels.Count != 0)
                {
                    var educationLevelIds = request.EducationLevels.Select(e => e.EducationLevelId).ToList();
                    await UpdateSiteEducationLevels(request.Id.Value, educationLevelIds);
                }

                // Actualizar grupos de niños específicos (solo si OffersServiceToDifferentGroups = true)
                // if (request.DayCareHome?.OffersServiceToDifferentGroups == true && request.ChildGroups != null && request.ChildGroups.Count != 0)
                // {
                //     await UpdateSiteChildGroups(request.Id.Value, request.ChildGroups);
                // }

                // // Actualizar servicios de alimentación
                // if (request.Services != null && request.Services.Count != 0)
                // {
                //     await UpdateSiteService(request.Id.Value, request.Services);
                // }

                // // Actualizar información de Day Care Home solo si la agencia es Day Care Home
                // if (request.DayCareHome != null && request.IsDayCareHome == true)
                // {
                //     await UpdateSiteDayCareHome(request.Id.Value, request.DayCareHome);
                // }

                // // Actualizar tipos de participantes
                // if (request.Participants != null && request.Participants.Count != 0)
                // {
                //     var participantTypeIds = request.Participants.Select(p => p.ParticipantTypeId).ToList();
                //     await UpdateSiteParticipants(request.Id.Value, participantTypeIds);
                // }

                // Invalidar caché
                //InvalidateCache(request.Id.Value);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el sitio: {Message}", ex.Message);
            throw new Exception($"Error al actualizar el sitio: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Elimina un sitio existente
    /// </summary>
    /// <param name="id">El ID del sitio a eliminar.</param>
    /// <returns>True si el sitio se eliminó correctamente, false en caso contrario.</returns>
    public async Task<bool> DeleteSite(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);

            var rowsAffected = await dbConnection.ExecuteAsync("102_DeleteSite", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el sitio: {Message}", ex.Message);
            throw new Exception($"Error al eliminar el sitio: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Invalida la caché para el sitio
    /// </summary>
    /// <param name="siteId">El ID del sitio.</param>
    private void InvalidateCache(int? siteId = null)
    {
        if (siteId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.Sites, 0, 0, "", false));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.Sites);
        _logger.LogInformation("Cache invalidado para Site Repository");
    }

    /// <summary>
    /// Inserta múltiples niveles educativos para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="educationLevelIds">Lista de IDs de niveles educativos</param>
    /// <returns>True si se insertaron correctamente</returns>
    private async Task<bool> InsertSiteEducationLevels(int siteId, List<int> educationLevelIds)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@educationLevelIds", string.Join(",", educationLevelIds), DbType.String);

            await dbConnection.ExecuteAsync("100_InsertSiteEducationLevels", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar niveles educativos para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza múltiples niveles educativos para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="educationLevelIds">Lista de IDs de niveles educativos</param>
    /// <returns>True si se actualizaron correctamente</returns>
    private async Task<bool> UpdateSiteEducationLevels(int siteId, List<int> educationLevelIds)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@educationLevelIds", string.Join(",", educationLevelIds), DbType.String);

            await dbConnection.ExecuteAsync("100_UpdateSiteEducationLevels", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar niveles educativos para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Verifica si existe un sitio principal en la base de datos
    /// </summary>
    /// <returns>True si existe un sitio principal, false en caso contrario</returns>
    public async Task<bool> HasMainSite()
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var result = await dbConnection.ExecuteScalarAsync<int>("102_HasMainSite", commandType: CommandType.StoredProcedure);
            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si existe un sitio principal");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza el estado activo/inactivo de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="isActive">Estado activo (true) o inactivo (false)</param>
    /// <param name="inactiveJustification">Justificación cuando se inactiva (requerida si isActive es false)</param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    public async Task<bool> UpdateSiteActiveStatus(int siteId, bool isActive, string? inactiveJustification = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", siteId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@isActive", isActive, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@inactiveJustification", inactiveJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("103_UpdateSiteActiveStatus", parameters, commandType: CommandType.StoredProcedure);

            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(siteId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado activo del sitio {SiteId}: {Message}", siteId, ex.Message);
            throw new Exception(ex.Message);
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

    /// <summary>
    /// Inserta servicios de alimentación para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="request">Request con datos de servicios</param>
    /// <returns>True si se insertó correctamente</returns>
    private async Task<bool> InsertSiteService(int siteId, SiteRequest request)
    {
        try
        {
            if (request.Services == null || request.Services.Count == 0)
            {
                // Si no hay servicios, insertar un registro vacío
                return await InsertEmptySiteService(siteId);
            }

            using IDbConnection dbConnection = _context.CreateConnection();

            foreach (var service in request.Services)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@siteId", siteId, DbType.Int32);
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

                await dbConnection.ExecuteAsync("100_InsertSiteService", parameters, commandType: CommandType.StoredProcedure);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar servicios para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Inserta un registro vacío de servicios cuando no hay servicios definidos
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <returns>True si se insertó correctamente</returns>
    private async Task<bool> InsertEmptySiteService(int siteId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@siteId", siteId, DbType.Int32);
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

            await dbConnection.ExecuteAsync("100_InsertSiteService", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar servicios vacíos para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Inserta información de Day Care Home para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="request">Request con datos de Day Care Home</param>
    /// <returns>True si se insertó correctamente</returns>
    private async Task<bool> InsertSiteDayCareHome(int siteId, SiteRequest request)
    {
        try
        {
            if (request.DayCareHome == null)
            {
                // Si no hay información de Day Care Home, insertar un registro vacío
                return await InsertEmptySiteDayCareHome(siteId);
            }

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@siteId", siteId, DbType.Int32);
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
            parameters.Add("@administratorBirthDate", request.DayCareHome.AdministratorBirthDate, DbType.Date);
            parameters.Add("@offersServiceToDifferentGroups", request.DayCareHome.OffersServiceToDifferentGroups, DbType.Boolean);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSiteDayCareHome", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar información de Day Care Home para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Inserta un registro vacío de Day Care Home cuando no hay información específica
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <returns>True si se insertó correctamente</returns>
    private async Task<bool> InsertEmptySiteDayCareHome(int siteId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@siteId", siteId, DbType.Int32);
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

            await dbConnection.ExecuteAsync("100_InsertSiteDayCareHome", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar información vacía de Day Care Home para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Inserta tipos de participantes para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="participantTypeIds">Lista de IDs de tipos de participantes</param>
    /// <returns>True si se insertaron correctamente</returns>
    private async Task<bool> InsertSiteParticipants(int siteId, List<int> participantTypeIds)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@participantTypeIds", string.Join(",", participantTypeIds), DbType.String);

            await dbConnection.ExecuteAsync("100_InsertSiteParticipants", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar tipos de participantes para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Inserta los grupos de niños específicos para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="childGroups">Lista de grupos de niños</param>
    /// <returns>True si se insertaron correctamente</returns>
    private async Task<bool> InsertSiteChildGroups(int siteId, List<SiteChildGroupRequest> childGroups)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            foreach (var childGroup in childGroups)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@siteId", siteId, DbType.Int32);
                parameters.Add("@groupName", childGroup.GroupName, DbType.String);
                parameters.Add("@numberOfChildren", childGroup.NumberOfChildren, DbType.Int32);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await dbConnection.ExecuteAsync("100_InsertSiteChildGroup", parameters, commandType: CommandType.StoredProcedure);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar grupos de niños para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza los grupos de niños específicos para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="childGroups">Lista de grupos de niños</param>
    /// <returns>True si se actualizaron correctamente</returns>
    private async Task<bool> UpdateSiteChildGroups(int siteId, List<SiteChildGroupRequest> childGroups)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            // Primero eliminar todos los grupos existentes para este sitio
            var deleteParameters = new DynamicParameters();
            deleteParameters.Add("@siteId", siteId, DbType.Int32);
            await dbConnection.ExecuteAsync("100_DeleteSiteChildGroupsBySiteId", deleteParameters, commandType: CommandType.StoredProcedure);

            // Luego insertar los nuevos grupos
            foreach (var childGroup in childGroups)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@siteId", siteId, DbType.Int32);
                parameters.Add("@groupName", childGroup.GroupName, DbType.String);
                parameters.Add("@numberOfChildren", childGroup.NumberOfChildren, DbType.Int32);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await dbConnection.ExecuteAsync("100_InsertSiteChildGroup", parameters, commandType: CommandType.StoredProcedure);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar grupos de niños para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Inserta días de funcionamiento para un sitio basado en las fechas desde y hasta
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="operatingFromDate">Fecha desde</param>
    /// <param name="operatingToDate">Fecha hasta</param>
    /// <returns>Número de días insertados</returns>
    private async Task<int> InsertSiteOperatingDays(int siteId, DateTime operatingFromDate, DateTime operatingToDate)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@SiteId", siteId, DbType.Int32);
            parameters.Add("@OperatingFromDate", operatingFromDate.Date, DbType.Date);
            parameters.Add("@OperatingToDate", operatingToDate.Date, DbType.Date);
            parameters.Add("@DefaultStartTime", TimeSpan.FromHours(8), DbType.Time); // 08:00:00
            parameters.Add("@DefaultEndTime", TimeSpan.FromHours(16), DbType.Time);  // 16:00:00
            parameters.Add("@DefaultComment", "Día de funcionamiento generado automáticamente", DbType.String);

            var result = await dbConnection.QuerySingleAsync<int>("100_InsertSiteOperatingDays", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation("Se insertaron {DaysInserted} días de funcionamiento para el sitio {SiteId} desde {FromDate} hasta {ToDate}",
                result, siteId, operatingFromDate.Date, operatingToDate.Date);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar días de funcionamiento para el sitio {SiteId} desde {FromDate} hasta {ToDate}",
                siteId, operatingFromDate.Date, operatingToDate.Date);
            throw new Exception(ex.Message);
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
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza servicios de alimentación para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="services">Lista de servicios</param>
    /// <returns>True si se actualizaron correctamente</returns>
    private async Task<bool> UpdateSiteService(int siteId, List<SiteServiceRequest> services)
    {
        try
        {
            // Primero obtener los servicios existentes
            using IDbConnection dbConnection = _context.CreateConnection();

            // Usar estrategia "eliminar y recrear" para mantener consistencia
            var deleteParameters = new DynamicParameters();
            deleteParameters.Add("@siteId", siteId, DbType.Int32);
            await dbConnection.ExecuteAsync("DELETE FROM SiteService WHERE SiteId = @siteId", deleteParameters);

            // Ahora insertar los nuevos servicios
            foreach (var service in services)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@siteId", siteId, DbType.Int32);
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

                await dbConnection.ExecuteAsync("100_InsertSiteService", parameters, commandType: CommandType.StoredProcedure);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar servicios para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza información de Day Care Home para un sitio existente
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="dayCareHome">Datos de Day Care Home</param>
    /// <returns>True si se actualizó correctamente</returns>
    private async Task<bool> UpdateSiteDayCareHome(int siteId, SiteDayCareHomeRequest dayCareHome)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            // Actualizar registro existente usando SiteId directamente
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32, ParameterDirection.Input);
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
            parameters.Add("@administratorBirthDate", dayCareHome.AdministratorBirthDate, DbType.Date, ParameterDirection.Input);
            parameters.Add("@offersServiceToDifferentGroups", dayCareHome.OffersServiceToDifferentGroups, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_UpdateSiteDayCareHome", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar información de Day Care Home para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }


    /// <summary>
    /// Actualiza tipos de participantes para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="participantTypeIds">Lista de IDs de tipos de participantes</param>
    /// <returns>True si se actualizaron correctamente</returns>
    private async Task<bool> UpdateSiteParticipants(int siteId, List<int> participantTypeIds)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@participantTypeIds", string.Join(",", participantTypeIds), DbType.String);

            await dbConnection.ExecuteAsync("100_UpdateSiteParticipants", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tipos de participantes para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Verifica si una agencia tiene un sitio principal
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>True si tiene sitio principal, false en caso contrario</returns>
    public async Task<bool> HasMainSite(int agencyId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId, DbType.Int32);

            var count = await dbConnection.QuerySingleAsync<int>("SELECT COUNT(*) FROM Site WHERE AgencyId = @agencyId AND IsMainSite = 1", parameters);
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si la agencia {AgencyId} tiene sitio principal", agencyId);
            throw new Exception(ex.Message);
        }
    }

}
