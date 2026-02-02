using System.Data;
using System.Linq;
using System.Reflection;
using Api.Data;
using Api.Exceptions;
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

public class SiteRepository(DapperContext context, ILogger<SiteRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService, Lazy<ISchoolSiteRepository> schoolSiteRepository, Lazy<ICenterTypeRepository> centerTypeRepository, Lazy<ISiteOperatingDayServiceRepository> siteOperatingDayServiceRepository, Lazy<ISitePersonInChargeRepository> sitePersonInChargeRepository, Lazy<IAgencyRepository> agencyRepository, Lazy<ISiteCalendarRepository> siteCalendarRepository, Lazy<ISiteProgramRepository> siteProgramRepository, IServiceTypeRepository serviceTypeRepository) : ISiteRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SiteRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
    private readonly Lazy<ISchoolSiteRepository> _schoolSiteRepository = schoolSiteRepository ?? throw new ArgumentNullException(nameof(schoolSiteRepository));
    private readonly Lazy<ICenterTypeRepository> _centerTypeRepository = centerTypeRepository ?? throw new ArgumentNullException(nameof(centerTypeRepository));
    private readonly Lazy<ISiteOperatingDayServiceRepository> _siteOperatingDayServiceRepository = siteOperatingDayServiceRepository ?? throw new ArgumentNullException(nameof(siteOperatingDayServiceRepository));
    private readonly Lazy<ISitePersonInChargeRepository> _sitePersonInChargeRepository = sitePersonInChargeRepository ?? throw new ArgumentNullException(nameof(sitePersonInChargeRepository));
    private readonly Lazy<IAgencyRepository> _agencyRepository = agencyRepository ?? throw new ArgumentNullException(nameof(agencyRepository));
    private readonly Lazy<ISiteCalendarRepository> _siteCalendarRepository = siteCalendarRepository ?? throw new ArgumentNullException(nameof(siteCalendarRepository));
    private readonly Lazy<ISiteProgramRepository> _siteProgramRepository = siteProgramRepository ?? throw new ArgumentNullException(nameof(siteProgramRepository));
    private readonly IServiceTypeRepository _serviceTypeRepository = serviceTypeRepository ?? throw new ArgumentNullException(nameof(serviceTypeRepository));

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
            var operatingDaysOfWeek = result.Read<dynamic>().ToList();
            var services = result.Read<dynamic>().ToList();
            var dayCareHome = await result.ReadFirstOrDefaultAsync<dynamic>();
            var participants = result.Read<dynamic>().ToList();
            var childGroups = result.Read<dynamic>().ToList();

            if (site == null)
            {
                return null;
            }

            var data = _mappingService.MapSite(site);
            data.EducationLevels = educationLevels.Select(_mappingService.MapEducationLevel).ToList();

            // Mapear días de la semana de operación desde SiteOperatingDaysOfWeek
            data.OperatingDaysOfWeek = operatingDaysOfWeek
                .Select(_mappingService.MapOperatingDayOfWeek)
                .OfType<DayOfWeekResponse>()
                .ToList();

            // Mapear slots de servicios (SiteChildGroupService) y grupos
            var mappedServiceSlots = services
                .Select(_mappingService.MapSiteChildGroupServiceSlot)
                .Where(s => s != null)
                .Cast<SiteChildGroupServiceSlotResponse>()
                .ToList();
            data.DayCareHome = dayCareHome != null ? _mappingService.MapSiteDayCareHome(dayCareHome) : null;
            data.Participants = participants.Select(_mappingService.MapSiteParticipant).ToList();

            // Mapear grupos
            var mappedChildGroups = childGroups
                .Select(d => _mappingService.MapSiteChildGroup(d))
                .OfType<SiteChildGroupResponse>()
                .ToList();

            // Agrupar slots por ChildGroupId y asignarlos a cada grupo
            foreach (var group in mappedChildGroups)
            {
                group.ServiceSlots = mappedServiceSlots
                    .Where(s => s.ChildGroupId == group.Id)
                    .ToList();
            }

            data.ChildGroups = mappedChildGroups;

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
    /// <param name="isDayCareHomeId">ID de la opción IsDayCareHome para filtrar sitios (Sí, No, Ambos).</param>
    /// <returns>Los sitios encontrados como SiteTableResponse.</returns>
    public async Task<dynamic> GetAllSitesFromDB(int take, int skip, string name, int? cityId, int? regionId, int? agencyId, bool alls, bool isList, int? isDayCareHomeId = null)
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
            parameters.Add("@isDayCareHomeId", isDayCareHomeId == 0 ? null : isDayCareHomeId, DbType.Int32);

            if (isList)
            {
                using var result = await dbConnection.QueryMultipleAsync("104_GetSites", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return new List<object>();
                }

                var data = result.Read<dynamic>().Select(_mappingService.MapSiteTable).ToList();
                return data;
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
            _logger.LogError(ex, "Error getting sites with parameters: take={Take}, skip={Skip}, name={Name}, cityId={CityId}, regionId={RegionId}, agencyId={AgencyId}, alls={Alls}, isDayCareHomeId={IsDayCareHomeId}: {Message}",
                take, skip, name, cityId, regionId, agencyId, alls, isDayCareHomeId, ex.Message);
            throw new Exception($"Error al obtener los sitios: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Calcula el número de días operativos en un rango de fechas que coinciden con los días de la semana seleccionados.
    /// Sistema: 1=Lunes, 2=Martes, ..., 7=Domingo.
    /// </summary>
    private static int CalculateOperatingDaysCount(DateTime fromDate, DateTime toDate, List<int> selectedDayIds)
    {
        if (selectedDayIds == null || selectedDayIds.Count == 0)
            return 0;

        var start = fromDate.Date;
        var end = toDate.Date;
        if (start > end)
            (start, end) = (end, start);

        int count = 0;
        for (var d = start; d <= end; d = d.AddDays(1))
        {
            // .NET DayOfWeek: Sunday=0, Monday=1, ..., Saturday=6 → Sistema: 1=Mon, ..., 7=Sun
            int systemDay = d.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)d.DayOfWeek;
            if (selectedDayIds.Contains(systemDay))
                count++;
        }

        return count;
    }

    /// <summary>
    /// Inserta un nuevo sitio
    /// </summary>
    /// <param name="request">La solicitud del sitio a insertar.</param>
    /// <returns>El ID del sitio insertado.</returns>
    public async Task<bool> InsertSite(SiteRequest request)
    {
        IDbConnection? dbConnection = null;
        IDbTransaction? transaction = null;

        try
        {
            await ValidateStrongServicesForProgramsAsync(request);
            await ValidateTimeBetweenServicesAsync(request);

            // Recalcular OperatingDaysCalculated desde fechas y días de la semana (fuente de verdad en servidor)
            if (request.OperatingFromDate.HasValue && request.OperatingToDate.HasValue
                && request.OperatingDaysOfWeek != null && request.OperatingDaysOfWeek.Count > 0)
            {
                request.OperatingDaysCalculated = CalculateOperatingDaysCount(
                    request.OperatingFromDate.Value,
                    request.OperatingToDate.Value,
                    request.OperatingDaysOfWeek);
            }

            // Generar código único de sitio automáticamente si no se proporciona
            if (string.IsNullOrEmpty(request.SiteCode) && request.AgencyId.HasValue)
            {
                var existingSiteCodes = await GetExistingSiteCodes();
                var agencySequenceNumber = await GetAgencySequenceNumber(request.AgencyId.Value);
                request.SiteCode = Utilities.GenerateSiteCode(agencySequenceNumber, existingSiteCodes);
            }

            dbConnection = _context.CreateConnection();
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }

            transaction = dbConnection.BeginTransaction();
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
            parameters.Add("@operatingStartTime", request.OperatingStartTime, DbType.Time, ParameterDirection.Input);
            parameters.Add("@operatingEndTime", request.OperatingEndTime, DbType.Time, ParameterDirection.Input);
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
            parameters.Add("@diningRoomCapacity", request.DiningRoomCapacity, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@communityId", request.CommunityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@walkersId", request.WalkersId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteTypeId", request.SiteTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteLocationId", request.SiteLocationId, DbType.Int32, ParameterDirection.Input);
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
            parameters.Add("@publicAllianceContractId", request.PublicAllianceContractId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@isAffiliatedCenter", request.IsAffiliatedCenter, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@isDayCareHomeId", request.IsDayCareHomeId, DbType.Int32, ParameterDirection.Input);

            // Estado de actividad
            parameters.Add("@inactiveJustification", request.InactiveJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@inactiveDate", request.InactiveDate, DbType.DateTime, ParameterDirection.Input);

            // Obtener el siguiente número de sitio para la agencia
            int nextSiteNumber = await GetNextSiteNumber(request.AgencyId.Value);
            parameters.Add("@siteNumber", nextSiteNumber, DbType.Int32, ParameterDirection.Input);

            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("104_InsertSite", parameters, transaction, commandType: CommandType.StoredProcedure);

            int siteId = parameters.Get<int>("@id");

            // Insertar niveles educativos
            if (request.EducationLevels != null && request.EducationLevels.Count != 0)
            {
                var educationLevelIds = request.EducationLevels.Select(e => e.EducationLevelId).ToList();
                await InsertSiteEducationLevels(siteId, educationLevelIds, dbConnection, transaction);
            }

            // Insertar información de Day Care Home solo si el sitio es un Hogar (IsDayCareHomeId = "Sí")
            // Determinar si es un hogar basándose en IsDayCareHomeId
            bool isDayCareHome = false;
            if (request.IsDayCareHomeId.HasValue)
            {
                // Necesitamos verificar si el IsDayCareHomeId corresponde a "Sí"
                // Esto se puede hacer consultando OptionSelection o pasando un flag desde el frontend
                // Por ahora, asumimos que si IsDayCareHomeId tiene valor y DayCareHome no es null, es un hogar
                isDayCareHome = request.DayCareHome != null;
            }

            if (isDayCareHome)
            {
                await InsertSiteDayCareHome(siteId, request, dbConnection, transaction);
            }

            // Insertar grupos de niños PRIMERO (para todos los sitios de todos los programas)
            // Los servicios (ServiceSlots) se insertan dentro de cada grupo en InsertSiteChildGroups
            List<int> childGroupIds = [];
            if (request.ChildGroups != null && request.ChildGroups.Count != 0)
            {
                childGroupIds = await InsertSiteChildGroups(siteId, request.ChildGroups, dbConnection, transaction);
            }

            // Insertar tipos de participantes
            if (request.Participants != null && request.Participants.Count != 0)
            {
                var participantTypeIds = request.Participants.Select(p => p.ParticipantTypeId).ToList();
                await InsertSiteParticipants(siteId, participantTypeIds, dbConnection, transaction);
            }

            // Insertar relaciones sitio-programa si se proporcionan ProgramIds
            if (request.ProgramIds != null && request.ProgramIds.Count > 0)
            {
                foreach (var programId in request.ProgramIds)
                {
                    var startDate = request.OperatingFromDate ?? request.StartDate ?? DateTime.Now.Date;
                    var endDate = request.OperatingToDate ?? new DateTime(2099, 12, 31); // Fecha futura si no se especifica

                    var siteProgramRequest = new SiteProgramRequest
                    {
                        SiteId = siteId,
                        ProgramId = programId,
                        StartDate = startDate,
                        EndDate = endDate,
                        IsActive = true
                    };

                    try
                    {
                        await _siteProgramRepository.Value.InsertSiteProgram(siteProgramRequest, dbConnection, transaction);
                        _logger.LogInformation("Se creó relación sitio-programa: SiteId={SiteId}, ProgramId={ProgramId}", siteId, programId);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al crear relación sitio-programa para SiteId={SiteId}, ProgramId={ProgramId}", siteId, programId);
                        // No fallar la creación del sitio si falla la relación programa
                    }
                }
            }

            // Insertar días de funcionamiento si se proporcionan fechas
            if (request.OperatingFromDate.HasValue && request.OperatingToDate.HasValue)
            {
                // Validar que OperatingDaysOfWeek no esté vacío o nulo
                if (request.OperatingDaysOfWeek == null || request.OperatingDaysOfWeek.Count == 0)
                {
                    throw new ArgumentException("Los días de la semana de funcionamiento (OperatingDaysOfWeek) son requeridos y no pueden estar vacíos.");
                }

                // Insertar días de la semana seleccionados en la tabla de relación
                await InsertSiteOperatingDaysOfWeek(siteId, request.OperatingDaysOfWeek, dbConnection, transaction);

                // Convertir IsDayCareHomeId a bool para compatibilidad con InsertSiteOperatingDays
                bool? isDayCareHomeBool = null;

                if (request.IsDayCareHomeId.HasValue)
                {
                    // Si IsDayCareHomeId corresponde a "Sí" (booleanValue = true), entonces isDayCareHome = true
                    // Esto se puede determinar consultando OptionSelection, pero por ahora usamos DayCareHome como indicador
                    isDayCareHomeBool = request.DayCareHome != null;
                }
                await InsertSiteOperatingDays(
                    siteId,
                    request.OperatingFromDate.Value,
                    request.OperatingToDate.Value,
                    BuildServicesForOperatingDaysFromChildGroups(childGroupIds, request.ChildGroups),
                    request.ProgramIds,
                    request.CenterTypeId,
                    isDayCareHomeBool,
                    request.OperatingStartTime,
                    request.OperatingEndTime,
                    request.OperatingDaysOfWeek,
                    dbConnection,
                    transaction);
            }

            // Insertar información de Persona a Cargo
            if (request.PersonInCharge != null)
            {
                await _sitePersonInChargeRepository.Value.InsertSitePersonInCharge(siteId, request.PersonInCharge, dbConnection, transaction);
            }

            // Crear relación SchoolSite si se proporciona SchoolId
            if (request.SchoolId.HasValue && request.SchoolId.Value > 0)
            {
                var schoolSiteRequest = new SchoolSiteRequest
                {
                    SchoolId = request.SchoolId.Value,
                    SiteId = siteId,
                    IsActive = true,
                    Comment = $"Asignación automática al crear el sitio {request.Name}"
                };

                // Usar Lazy<ISchoolSiteRepository> para evitar dependencia circular
                var schoolSiteResult = await _schoolSiteRepository.Value.InsertSchoolSite(schoolSiteRequest, dbConnection, transaction);

                if (!schoolSiteResult)
                {
                    _logger.LogWarning("No se pudo crear la relación SchoolSite para el sitio {SiteId} y escuela {SchoolId}", siteId, request.SchoolId);
                }
            }

            transaction.Commit();

            // Invalidar caché
            //InvalidateCache(siteId);

            return siteId > 0;
        }
        catch (Exception ex)
        {
            transaction?.Rollback();
            _logger.LogError(ex, "Error al insertar el sitio: {Message}", ex.Message);
            throw new Exception($"Error al insertar el sitio: {ex.Message}", ex);
        }
        finally
        {
            transaction?.Dispose();
            dbConnection?.Dispose();
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
            await ValidateStrongServicesForProgramsAsync(request);
            await ValidateTimeBetweenServicesAsync(request);

            // Recalcular OperatingDaysCalculated desde fechas y días de la semana (fuente de verdad en servidor)
            if (request.OperatingFromDate.HasValue && request.OperatingToDate.HasValue
                && request.OperatingDaysOfWeek != null && request.OperatingDaysOfWeek.Count > 0)
            {
                request.OperatingDaysCalculated = CalculateOperatingDaysCount(
                    request.OperatingFromDate.Value,
                    request.OperatingToDate.Value,
                    request.OperatingDaysOfWeek);
            }

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
            parameters.Add("@operatingStartTime", request.OperatingStartTime, DbType.Time, ParameterDirection.Input);
            parameters.Add("@operatingEndTime", request.OperatingEndTime, DbType.Time, ParameterDirection.Input);
            parameters.Add("@kitchenTypeId", request.KitchenTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@groupTypeId", request.GroupTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@deliveryTypeId", request.DeliveryTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@sponsorTypeId", request.SponsorTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@applicantTypeId", request.ApplicantTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@residentialTypeId", request.ResidentialTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@operatingPolicyId", request.OperatingPolicyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@hasWarehouse", request.HasWarehouse, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@hasDiningRoom", request.HasDiningRoom, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@diningRoomCapacity", request.DiningRoomCapacity, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@communityId", request.CommunityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@walkersId", request.WalkersId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteTypeId", request.SiteTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@siteLocationId", request.SiteLocationId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@experienceId", request.ExperienceId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewResultId", request.ReviewResultId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewDate", request.ReviewDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@reviewJustification", request.ReviewJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@siteCode", request.SiteCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@siteNumber", request.SiteNumber, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@inactiveJustification", request.InactiveJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@inactiveDate", request.InactiveDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@generalEnrollment", request.GeneralEnrollment, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@serviceTime", request.ServiceTime, DbType.DateTime, ParameterDirection.Input);

            // ===== CAMPOS ESPECÍFICOS PARA PACNA =====
            parameters.Add("@organizedAthleticPrograms", request.OrganizedAthleticPrograms, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@atRiskService", request.AtRiskService, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@publicAllianceContractId", request.PublicAllianceContractId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@isAffiliatedCenter", request.IsAffiliatedCenter, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@isDayCareHomeId", request.IsDayCareHomeId, DbType.Int32, ParameterDirection.Input);

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
                    await UpdateSiteEducationLevels(request.Id.Value, educationLevelIds, dbConnection);
                }

                // Actualizar días de la semana de operación (el sync del calendario se hace después de child groups)
                if (request.OperatingDaysOfWeek != null && request.OperatingDaysOfWeek.Count > 0)
                {
                    await UpdateSiteOperatingDaysOfWeek(request.Id.Value, request.OperatingDaysOfWeek, dbConnection);
                }

                // Actualizar grupos de niños (los ServiceSlots se actualizan dentro de UpdateSiteChildGroups)
                List<int> childGroupIds = [];
                if (request.ChildGroups != null && request.ChildGroups.Count != 0)
                {
                    childGroupIds = await UpdateSiteChildGroups(request.Id.Value, request.ChildGroups, dbConnection);
                }

                // Sincronizar calendario después de child groups para que el sync use SiteChildGroupService actualizado
                if (request.OperatingDaysOfWeek != null && request.OperatingDaysOfWeek.Count > 0)
                {
                    await SyncSiteOperatingDaysWithWeekPattern(request.Id.Value, dbConnection);
                }

                // Reemplazar servicios del calendario en el rango: borrar existentes e insertar desde childGroups.serviceSlots
                if (request.ChildGroups != null && request.ChildGroups.Count != 0
                    && request.OperatingFromDate.HasValue && request.OperatingToDate.HasValue
                    && childGroupIds.Count == request.ChildGroups.Count)
                {
                    var services = BuildServicesForOperatingDaysFromChildGroups(childGroupIds, request.ChildGroups);
                    if (services.Count > 0)
                    {
                        var deleteParams = new DynamicParameters();
                        deleteParams.Add("@siteid", request.Id.Value, DbType.Int32);
                        deleteParams.Add("@operatingfromdate", request.OperatingFromDate.Value.Date, DbType.Date);
                        deleteParams.Add("@operatingtodate", request.OperatingToDate.Value.Date, DbType.Date);
                        await dbConnection.ExecuteAsync(
                            "100_DeleteSiteOperatingDayServicesBySiteAndDateRange",
                            deleteParams,
                            commandType: CommandType.StoredProcedure);
                        await InsertServicesForOperatingDays(
                            request.Id.Value,
                            request.OperatingFromDate.Value,
                            request.OperatingToDate.Value,
                            services,
                            dbConnection);
                    }
                }

                // Actualizar o insertar información de Persona a Cargo
                if (request.PersonInCharge != null)
                {
                    var existingPersonInCharge = await _sitePersonInChargeRepository.Value.GetSitePersonInChargeBySiteId(request.Id.Value);
                    
                    if (existingPersonInCharge != null)
                    {
                        await _sitePersonInChargeRepository.Value.UpdateSitePersonInCharge(request.Id.Value, request.PersonInCharge);
                    }
                    else
                    {
                        await _sitePersonInChargeRepository.Value.InsertSitePersonInCharge(request.Id.Value, request.PersonInCharge, dbConnection);
                    }
                }

                // Actualizar información de Day Care Home si se proporciona
                if (request.DayCareHome != null)
                {
                    await UpdateSiteDayCareHome(request.Id.Value, request.DayCareHome, dbConnection);
                }

                // Actualizar tipos de participantes
                if (request.Participants != null && request.Participants.Count != 0)
                {
                    var participantTypeIds = request.Participants.Select(p => p.ParticipantTypeId).ToList();
                    await UpdateSiteParticipants(request.Id.Value, participantTypeIds, dbConnection);
                }

                // Sincronizar relaciones sitio-programa
                if (request.ProgramIds != null)
                {
                    using var transaction = dbConnection.BeginTransaction();
                    try
                    {
                        // Eliminar programas existentes del sitio
                        await _siteProgramRepository.Value.DeleteSitePrograms(request.Id.Value, dbConnection, transaction);

                        // Insertar nuevos programas
                        foreach (var programId in request.ProgramIds)
                        {
                            var startDate = request.OperatingFromDate ?? request.StartDate ?? DateTime.Now.Date;
                            var endDate = request.OperatingToDate ?? new DateTime(2099, 12, 31); // Fecha futura si no se especifica

                            var siteProgramRequest = new SiteProgramRequest
                            {
                                SiteId = request.Id.Value,
                                ProgramId = programId,
                                StartDate = startDate,
                                EndDate = endDate,
                                IsActive = true
                            };

                            try
                            {
                                await _siteProgramRepository.Value.InsertSiteProgram(siteProgramRequest, dbConnection, transaction);
                                _logger.LogInformation("Se actualizó relación sitio-programa: SiteId={SiteId}, ProgramId={ProgramId}", request.Id.Value, programId);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error al actualizar relación sitio-programa para SiteId={SiteId}, ProgramId={ProgramId}", request.Id.Value, programId);
                                // Continuar con los demás programas
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex, "Error al sincronizar programas del sitio {SiteId}", request.Id.Value);
                        // No fallar la actualización del sitio si falla la sincronización de programas
                    }
                }

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
    private async Task<bool> InsertSiteEducationLevels(int siteId, List<int> educationLevelIds, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@educationLevelIds", string.Join(",", educationLevelIds), DbType.String);

            await dbConnection.ExecuteAsync("100_InsertSiteEducationLevels", parameters, transaction, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar niveles educativos para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Inserta múltiples días de la semana para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="dayOfWeekIds">Lista de IDs de días de la semana (1=Lunes, 2=Martes, ..., 7=Domingo)</param>
    /// <param name="connection">Conexión de base de datos (opcional)</param>
    /// <param name="transaction">Transacción de base de datos (opcional)</param>
    /// <returns>True si se insertaron correctamente</returns>
    private async Task<bool> InsertSiteOperatingDaysOfWeek(int siteId, List<int> dayOfWeekIds, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@dayOfWeekIds", string.Join(",", dayOfWeekIds), DbType.String);

            await dbConnection.ExecuteAsync("100_InsertSiteOperatingDaysOfWeek", parameters, transaction, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar días de la semana para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Actualiza múltiples niveles educativos para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="educationLevelIds">Lista de IDs de niveles educativos</param>
    /// <returns>True si se actualizaron correctamente</returns>
    private async Task<bool> UpdateSiteEducationLevels(int siteId, List<int> educationLevelIds, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@educationLevelIds", string.Join(",", educationLevelIds), DbType.String);

            await dbConnection.ExecuteAsync("100_UpdateSiteEducationLevels", parameters, transaction, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar niveles educativos para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Actualiza múltiples días de la semana para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="dayOfWeekIds">Lista de IDs de días de la semana (1=Lunes, 2=Martes, ..., 7=Domingo)</param>
    /// <param name="connection">Conexión de base de datos (opcional)</param>
    /// <param name="transaction">Transacción de base de datos (opcional)</param>
    /// <returns>True si se actualizaron correctamente</returns>
    private async Task<bool> UpdateSiteOperatingDaysOfWeek(int siteId, List<int> dayOfWeekIds, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@dayOfWeekIds", string.Join(",", dayOfWeekIds), DbType.String);

            await dbConnection.ExecuteAsync("100_UpdateSiteOperatingDaysOfWeek", parameters, transaction, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar días de la semana para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Sincroniza los días del calendario (SiteOperatingDays) con el patrón semanal definido en SiteOperatingDaysOfWeek.
    /// - Elimina días que ya no corresponden al patrón (excepto los agregados manualmente)
    /// - Agrega días nuevos según el nuevo patrón semanal
    /// - Genera servicios para los nuevos días agregados
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="connection">Conexión de base de datos (opcional)</param>
    /// <param name="transaction">Transacción de base de datos (opcional)</param>
    /// <returns>True si la sincronización fue exitosa</returns>
    private async Task<bool> SyncSiteOperatingDaysWithWeekPattern(int siteId, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);

            var result = await dbConnection.QueryFirstOrDefaultAsync<dynamic>(
                "100_SyncSiteOperatingDaysWithWeekPattern", 
                parameters, 
                transaction, 
                commandType: CommandType.StoredProcedure
            );

            if (result != null)
            {
                int daysDeleted = (int)(result.DaysDeleted ?? 0);
                int daysInserted = (int)(result.DaysInserted ?? 0);
                int servicesInserted = (int)(result.ServicesInserted ?? 0);
                
                _logger.LogInformation(
                    "Sincronización de calendario completada para sitio {SiteId}: {DaysDeleted} días eliminados, {DaysInserted} días insertados, {ServicesInserted} servicios insertados",
                    siteId, 
                    daysDeleted, 
                    daysInserted, 
                    servicesInserted
                );
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al sincronizar calendario con patrón semanal para el sitio {SiteId}", siteId);
            // No propagamos el error para no fallar la actualización del sitio
            // La sincronización fallida no debe impedir que el sitio se actualice
            return false;
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Actualiza el estado activo/inactivo de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="isActive">Estado activo (true) o inactivo (false)</param>
    /// <param name="inactiveJustification">Justificación cuando se inactiva (requerida si isActive es false)</param>
    /// <returns>True si se actualizó correctamente, false en caso contrario</returns>
    public async Task<bool> UpdateSiteActiveStatus(int siteId, bool isActive, string? inactiveJustification = null, DateTime? inactiveDate = null, bool? providedRationsService = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", siteId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@isActive", isActive, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@inactiveJustification", inactiveJustification, DbType.String, ParameterDirection.Input);
            parameters.Add("@inactiveDate", inactiveDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@providedRationsService", providedRationsService, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("103_UpdateSiteActiveStatus", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
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
            {
                return "001"; // Valor por defecto si no se encuentra la agencia
            }
            // Extraer el número de secuencia del código de agencia (última parte después del último guión)
            // El formato esperado es T-{año}-{secuencia}, por lo que necesitamos al menos 3 partes
            var parts = agencyCode.Split('-');
            return parts.Length >= 3 ? parts[^1] : "001";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener código de secuencia de agencia {AgencyId}", agencyId);
            return "001";
        }
    }

    /// <summary>
    /// Inserta información de Day Care Home para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="request">Request con datos de Day Care Home</param>
    /// <returns>True si se insertó correctamente</returns>
    private async Task<bool> InsertSiteDayCareHome(int siteId, SiteRequest request, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            if (request.DayCareHome == null)
            {
                // Si no hay información de Day Care Home, insertar un registro vacío
                return await InsertEmptySiteDayCareHome(siteId, dbConnection, transaction);
            }

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

            await dbConnection.ExecuteAsync("100_InsertSiteDayCareHome", parameters, transaction, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar información de Day Care Home para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Inserta un registro vacío de Day Care Home cuando no hay información específica
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <returns>True si se insertó correctamente</returns>
    private async Task<bool> InsertEmptySiteDayCareHome(int siteId, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
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

            await dbConnection.ExecuteAsync("100_InsertSiteDayCareHome", parameters, transaction, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar información vacía de Day Care Home para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Inserta tipos de participantes para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="participantTypeIds">Lista de IDs de tipos de participantes</param>
    /// <returns>True si se insertaron correctamente</returns>
    private async Task<bool> InsertSiteParticipants(int siteId, List<int> participantTypeIds, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@participantTypeIds", string.Join(",", participantTypeIds), DbType.String);

            await dbConnection.ExecuteAsync("100_InsertSiteParticipants", parameters, transaction, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar tipos de participantes para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Inserta los grupos de niños específicos para un sitio y sus servicios asociados
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="childGroups">Lista de grupos de niños con sus servicios</param>
    /// <param name="connection">Conexión a la base de datos</param>
    /// <param name="transaction">Transacción</param>
    /// <returns>Lista de IDs de los grupos insertados</returns>
    private async Task<List<int>> InsertSiteChildGroups(int siteId, List<SiteChildGroupRequest> childGroups, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;
        var childGroupIds = new List<int>();

        try
        {
            foreach (var childGroup in childGroups)
            {
                // Insertar el grupo
                var parameters = new DynamicParameters();
                parameters.Add("@siteId", siteId, DbType.Int32);
                parameters.Add("@groupName", childGroup.GroupName, DbType.String);
                parameters.Add("@numberOfChildren", childGroup.NumberOfChildren, DbType.Int32);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await dbConnection.ExecuteAsync("100_InsertSiteChildGroup", parameters, transaction, commandType: CommandType.StoredProcedure);
                
                int groupId = parameters.Get<int>("@id");
                childGroupIds.Add(groupId);

                // Insertar slots de servicios del grupo (SiteChildGroupService)
                if (childGroup.ServiceSlots != null && childGroup.ServiceSlots.Count > 0)
                {
                    foreach (var slot in childGroup.ServiceSlots)
                    {
                        ValidateServiceSlotTimes(slot);

                        var slotParameters = new DynamicParameters();
                        slotParameters.Add("@childgroupid", groupId, DbType.Int32);
                        slotParameters.Add("@servicetypeid", slot.ServiceTypeId, DbType.Int32);
                        slotParameters.Add("@isoffered", slot.IsOffered, DbType.Boolean);
                        slotParameters.Add("@fromtime", slot.FromTime, DbType.Time);
                        slotParameters.Add("@totime", slot.ToTime, DbType.Time);
                        slotParameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        await dbConnection.ExecuteAsync("100_InsertSiteChildGroupService", slotParameters, transaction, commandType: CommandType.StoredProcedure);
                    }
                }
            }

            return childGroupIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar grupos de niños para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Actualiza los grupos de niños específicos para un sitio y sus servicios asociados
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="childGroups">Lista de grupos de niños con sus servicios</param>
    /// <param name="connection">Conexión a la base de datos</param>
    /// <param name="transaction">Transacción</param>
    /// <returns>Lista de IDs de los grupos insertados</returns>
    private async Task<List<int>> UpdateSiteChildGroups(int siteId, List<SiteChildGroupRequest> childGroups, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;
        var childGroupIds = new List<int>();

        try
        {
            // Primero eliminar todos los grupos existentes para este sitio (esto también eliminará sus servicios por CASCADE)
            var deleteParameters = new DynamicParameters();
            deleteParameters.Add("@siteId", siteId, DbType.Int32);
            await dbConnection.ExecuteAsync("100_DeleteSiteChildGroupsBySiteId", deleteParameters, transaction, commandType: CommandType.StoredProcedure);

            // Luego insertar los nuevos grupos con sus servicios
            foreach (var childGroup in childGroups)
            {
                // Insertar el grupo
                var parameters = new DynamicParameters();
                parameters.Add("@siteId", siteId, DbType.Int32);
                parameters.Add("@groupName", childGroup.GroupName, DbType.String);
                parameters.Add("@numberOfChildren", childGroup.NumberOfChildren, DbType.Int32);
                parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await dbConnection.ExecuteAsync("100_InsertSiteChildGroup", parameters, transaction, commandType: CommandType.StoredProcedure);
                
                int groupId = parameters.Get<int>("@id");
                childGroupIds.Add(groupId);

                // Insertar slots de servicios del grupo (SiteChildGroupService)
                if (childGroup.ServiceSlots != null && childGroup.ServiceSlots.Count > 0)
                {
                    foreach (var slot in childGroup.ServiceSlots)
                    {
                        ValidateServiceSlotTimes(slot);

                        var slotParameters = new DynamicParameters();
                        slotParameters.Add("@childgroupid", groupId, DbType.Int32);
                        slotParameters.Add("@servicetypeid", slot.ServiceTypeId, DbType.Int32);
                        slotParameters.Add("@isoffered", slot.IsOffered, DbType.Boolean);
                        slotParameters.Add("@fromtime", slot.FromTime, DbType.Time);
                        slotParameters.Add("@totime", slot.ToTime, DbType.Time);
                        slotParameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

                        await dbConnection.ExecuteAsync("100_InsertSiteChildGroupService", slotParameters, transaction, commandType: CommandType.StoredProcedure);
                    }
                }
            }

            return childGroupIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar grupos de niños para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Inserta días de funcionamiento para un sitio basado en las fechas desde y hasta
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="operatingFromDate">Fecha desde</param>
    /// <param name="operatingToDate">Fecha hasta</param>
    /// <param name="services">Servicios del sitio</param>
    /// <param name="programIds">IDs de programas</param>
    /// <param name="centerTypeId">ID del tipo de centro</param>
    /// <param name="isDayCareHome">Si es Day Care Home</param>
    /// <param name="operatingStartTime">Hora de inicio</param>
    /// <param name="operatingEndTime">Hora de fin</param>
    /// <param name="operatingDaysOfWeek">Días de la semana seleccionados (1=Lunes, 2=Martes, ..., 7=Domingo)</param>
    /// <param name="connection">Conexión a la base de datos</param>
    /// <param name="transaction">Transacción</param>
    /// <returns>Número de días insertados</returns>
    private async Task<int> InsertSiteOperatingDays(
        int siteId,
        DateTime operatingFromDate,
        DateTime operatingToDate,
        List<SiteServiceRequest>? services,
        List<int>? programIds = null,
        int? centerTypeId = null,
        bool? isDayCareHome = null,
        TimeSpan? operatingStartTime = null,
        TimeSpan? operatingEndTime = null,
        List<int>? operatingDaysOfWeek = null,
        IDbConnection? connection = null,
        IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            // Validar que operatingDaysOfWeek no esté vacío
            if (operatingDaysOfWeek == null || operatingDaysOfWeek.Count == 0)
            {
                throw new ArgumentException("Los días de la semana de funcionamiento (OperatingDaysOfWeek) son requeridos y no pueden estar vacíos.");
            }

            // Usar las horas proporcionadas o valores por defecto
            var defaultStartTime = operatingStartTime ?? TimeSpan.FromHours(8); // 08:00:00
            var defaultEndTime = operatingEndTime ?? TimeSpan.FromHours(18);   // 18:00:00
            var defaultComment = "Día de funcionamiento generado automáticamente";

            // Establecer Lunes como primer día de la semana (DayOfWeek: 0=Domingo, 1=Lunes, ..., 6=Sábado)
            // Convertir a formato donde 1=Lunes, 2=Martes, ..., 7=Domingo
            int GetDayOfWeekNumber(DateTime date)
            {
                // DayOfWeek: 0=Domingo, 1=Lunes, 2=Martes, ..., 6=Sábado
                // Convertir a: 1=Lunes, 2=Martes, ..., 7=Domingo
                int dayOfWeek = (int)date.DayOfWeek;
                return dayOfWeek == 0 ? 7 : dayOfWeek; // Domingo = 7
            }

            int daysInserted = 0;
            DateTime currentDate = operatingFromDate.Date;

            // Iterar sobre todas las fechas en el rango
            while (currentDate <= operatingToDate.Date)
            {
                int dayOfWeekNumber = GetDayOfWeekNumber(currentDate);

                // Solo insertar si el día de la semana está en la lista de días seleccionados
                if (operatingDaysOfWeek.Contains(dayOfWeekNumber))
                {
                    var insertSql = @"
                        INSERT INTO SiteOperatingDays
                            (SiteId, OperatingDate, StartTime, EndTime, Comment, IsActive, CreatedAt)
                        VALUES
                            (@siteId, @operatingDate, @startTime, @endTime, @comment, 1, GETDATE())";

                    await dbConnection.ExecuteAsync(insertSql, new
                    {
                        siteId,
                        operatingDate = currentDate,
                        startTime = defaultStartTime,
                        endTime = defaultEndTime,
                        comment = defaultComment
                    }, transaction);

                    daysInserted++;
                }

                currentDate = currentDate.AddDays(1);
            }

            _logger.LogInformation("Se insertaron {DaysInserted} días de funcionamiento para el sitio {SiteId} desde {FromDate} hasta {ToDate}. Días de la semana seleccionados: {OperatingDaysOfWeek}",
                daysInserted, siteId, operatingFromDate.Date, operatingToDate.Date, string.Join(", ", operatingDaysOfWeek));

            // Después de crear los días, crear los servicios para cada día
            if (services != null && services.Count > 0)
            {
                await InsertServicesForOperatingDays(siteId, operatingFromDate, operatingToDate, services, dbConnection, transaction);
            }

            return daysInserted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar días de funcionamiento para el sitio {SiteId} desde {FromDate} hasta {ToDate}",
                siteId, operatingFromDate.Date, operatingToDate.Date);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Determina si se deben incluir fines de semana en los días de funcionamiento según el programa y tipo de sitio
    /// </summary>
    /// <param name="programIds">IDs de programas de la agencia</param>
    /// <param name="centerTypeId">ID del tipo de centro (para PACNA)</param>
    /// <param name="isDayCareHome">Si es Day Care Home (para PACNA)</param>
    /// <returns>True si se deben incluir fines de semana, false si solo Lunes a Viernes</returns>
    private async Task<bool> DetermineOperatingDaysConfiguration(List<int>? programIds, int? centerTypeId, bool? isDayCareHome)
    {
        try
        {
            // Constantes para IDs de programas
            const int PROGRAM_ID_PDAM = 1;
            const int PROGRAM_ID_PSAV = 2;
            const int PROGRAM_ID_PACNA = 3;

            // Nombres de tipos de centro para PACNA
            const string CENTER_TYPE_CUIDADO_DIURNO = "Centro de Cuidado Diurno";
            const string CENTER_TYPE_CUIDADO_ADULTO = "Centro de Cuidado Adulto";
            const string CENTER_TYPE_ALBERGUE = "Albergue";

            // Si no hay programas, usar comportamiento por defecto (todos los días)
            if (programIds == null || programIds.Count == 0)
            {
                _logger.LogInformation("No se proporcionaron programas - usando comportamiento por defecto (todos los días)");
                return true;
            }

            // Verificar si tiene PDAM
            if (programIds.Contains(PROGRAM_ID_PDAM))
            {
                _logger.LogInformation("Programa PDAM detectado - Solo Lunes a Viernes");
                return false;
            }

            // Verificar si tiene PSAV
            if (programIds.Contains(PROGRAM_ID_PSAV))
            {
                _logger.LogInformation("Programa PSAV detectado - Solo Lunes a Viernes (sábados y domingos se agregan manualmente)");
                return false;
            }

            // Verificar si tiene PACNA
            if (programIds.Contains(PROGRAM_ID_PACNA))
            {
                // Si es Day Care Home (Hogares), todos los días
                if (isDayCareHome == true)
                {
                    _logger.LogInformation("Programa PACNA - Hogares (isDayCareHome=true) - Todos los días");
                    return true;
                }

                // Si hay centerTypeId, consultar el nombre del tipo de centro
                if (centerTypeId.HasValue)
                {
                    try
                    {
                        var centerType = await _centerTypeRepository.Value.GetCenterTypeById(centerTypeId.Value);
                        if (centerType != null)
                        {
                            // Obtener el nombre del CenterType
                            string? centerTypeName = null;
                            if (centerType is DTOCenterType dtoCenterType)
                            {
                                centerTypeName = dtoCenterType.Name;
                            }
                            else
                            {
                                // Intentar obtener Name usando reflexión
                                var nameProperty = centerType.GetType().GetProperty("Name");
                                if (nameProperty != null)
                                {
                                    centerTypeName = nameProperty.GetValue(centerType)?.ToString();
                                }
                            }

                            if (!string.IsNullOrEmpty(centerTypeName))
                            {
                                // Centro Diurno: Solo Lunes a Viernes
                                if (centerTypeName.Equals(CENTER_TYPE_CUIDADO_DIURNO, StringComparison.OrdinalIgnoreCase))
                                {
                                    _logger.LogInformation("Programa PACNA - Centro Diurno ({CenterTypeName}) - Solo Lunes a Viernes (sábados y domingos se agregan manualmente)", centerTypeName);
                                    return false;
                                }

                                // Centro de Adultos o Albergue: Todos los días
                                if (centerTypeName.Equals(CENTER_TYPE_CUIDADO_ADULTO, StringComparison.OrdinalIgnoreCase) ||
                                    centerTypeName.Equals(CENTER_TYPE_ALBERGUE, StringComparison.OrdinalIgnoreCase))
                                {
                                    _logger.LogInformation("Programa PACNA - {CenterTypeName} - Todos los días", centerTypeName);
                                    return true;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error al obtener CenterType con ID {CenterTypeId}, usando comportamiento por defecto", centerTypeId.Value);
                    }
                }

                // Si no se pudo determinar el tipo de centro, usar comportamiento por defecto
                _logger.LogInformation("Programa PACNA - No se pudo determinar el tipo de centro - usando comportamiento por defecto (todos los días)");
                return true;
            }

            // Por defecto, incluir fines de semana (comportamiento actual)
            _logger.LogInformation("No aplica ninguna regla específica - usando comportamiento por defecto (todos los días)");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al determinar configuración de días de funcionamiento");
            // En caso de error, usar comportamiento por defecto
            return true;
        }
    }

    /// <summary>
    /// Inserta servicios para todos los días de funcionamiento creados usando stored procedure
    /// </summary>
    private async Task InsertServicesForOperatingDays(int siteId, DateTime operatingFromDate, DateTime operatingToDate, List<SiteServiceRequest> services, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            // Validar que hay servicios para procesar
            if (services == null || services.Count == 0)
            {
                _logger.LogInformation("No hay servicios para insertar para el sitio {SiteId}", siteId);
                return;
            }

            // Log detallado de los servicios recibidos
            _logger.LogInformation("Preparando {ServiceCount} servicios para insertar en días de funcionamiento del sitio {SiteId}",
                services.Count, siteId);

            // Helper para verificar si un servicio tiene al menos un servicio activo con horarios válidos
            bool HasValidService(SiteServiceRequest service)
            {
                return (service.Breakfast == true && service.BreakfastFrom.HasValue && service.BreakfastTo.HasValue) ||
                       (service.Lunch == true && service.LunchFrom.HasValue && service.LunchTo.HasValue) ||
                       (service.SnackAM == true && service.SnackAMFrom.HasValue && service.SnackAMTo.HasValue) ||
                       (service.Dinner == true && service.DinnerFrom.HasValue && service.DinnerTo.HasValue) ||
                       (service.SnackPM == true && service.SnackPMFrom.HasValue && service.SnackPMTo.HasValue) ||
                       (service.SnackNight == true && service.SnackNightFrom.HasValue && service.SnackNightTo.HasValue) ||
                       (service.DinnerExtended == true && service.DinnerExtendedFrom.HasValue && service.DinnerExtendedTo.HasValue) ||
                       (service.DinnerAtRisk == true && service.DinnerAtRiskFrom.HasValue && service.DinnerAtRiskTo.HasValue) ||
                       (service.SnackExtended == true && service.SnackExtendedFrom.HasValue && service.SnackExtendedTo.HasValue) ||
                       (service.SnackAtRisk == true && service.SnackAtRiskFrom.HasValue && service.SnackAtRiskTo.HasValue);
            }

            // Filtrar servicios que tengan al menos un servicio activo con horarios válidos
            var validServices = services.Where(HasValidService).ToList();

            if (validServices.Count == 0)
            {
                _logger.LogWarning("No hay servicios válidos para insertar para el sitio {SiteId}. Todos los servicios están desactivados o no tienen horarios válidos. " +
                    "Total de servicios recibidos: {TotalCount}",
                    siteId, services.Count);

                // Log detallado de cada servicio para diagnóstico
                foreach (var service in services)
                {
                    _logger.LogDebug("Servicio inválido - ChildGroupId: {ChildGroupId}, " +
                        "Breakfast: {Breakfast} (From: {BreakfastFrom}, To: {BreakfastTo}), " +
                        "Lunch: {Lunch} (From: {LunchFrom}, To: {LunchTo}), " +
                        "Dinner: {Dinner} (From: {DinnerFrom}, To: {DinnerTo}), " +
                        "SnackAM: {SnackAM} (From: {SnackAMFrom}, To: {SnackAMTo}), " +
                        "SnackPM: {SnackPM} (From: {SnackPMFrom}, To: {SnackPMTo}), " +
                        "SnackNight: {SnackNight} (From: {SnackNightFrom}, To: {SnackNightTo})",
                        service.ChildGroupId,
                        service.Breakfast, service.BreakfastFrom, service.BreakfastTo,
                        service.Lunch, service.LunchFrom, service.LunchTo,
                        service.Dinner, service.DinnerFrom, service.DinnerTo,
                        service.SnackAM, service.SnackAMFrom, service.SnackAMTo,
                        service.SnackPM, service.SnackPMFrom, service.SnackPMTo,
                        service.SnackNight, service.SnackNightFrom, service.SnackNightTo);
                }
                return;
            }

            _logger.LogInformation("Se encontraron {ValidCount} servicios válidos de {TotalCount} servicios recibidos para el sitio {SiteId}",
                validServices.Count, services.Count, siteId);

            foreach (var service in validServices)
            {
                // Contar servicios activos en este servicio
                var activeServicesCount = new[]
                {
                    (service.Breakfast == true && service.BreakfastFrom.HasValue && service.BreakfastTo.HasValue) ? 1 : 0,
                    (service.Lunch == true && service.LunchFrom.HasValue && service.LunchTo.HasValue) ? 1 : 0,
                    (service.SnackAM == true && service.SnackAMFrom.HasValue && service.SnackAMTo.HasValue) ? 1 : 0,
                    (service.Dinner == true && service.DinnerFrom.HasValue && service.DinnerTo.HasValue) ? 1 : 0,
                    (service.SnackPM == true && service.SnackPMFrom.HasValue && service.SnackPMTo.HasValue) ? 1 : 0,
                    (service.SnackNight == true && service.SnackNightFrom.HasValue && service.SnackNightTo.HasValue) ? 1 : 0,
                    (service.DinnerExtended == true && service.DinnerExtendedFrom.HasValue && service.DinnerExtendedTo.HasValue) ? 1 : 0,
                    (service.DinnerAtRisk == true && service.DinnerAtRiskFrom.HasValue && service.DinnerAtRiskTo.HasValue) ? 1 : 0,
                    (service.SnackExtended == true && service.SnackExtendedFrom.HasValue && service.SnackExtendedTo.HasValue) ? 1 : 0,
                    (service.SnackAtRisk == true && service.SnackAtRiskFrom.HasValue && service.SnackAtRiskTo.HasValue) ? 1 : 0
                }.Sum();

                _logger.LogDebug("Servicio válido - ChildGroupId: {ChildGroupId}, Servicios activos: {ActiveCount}, " +
                    "Breakfast: {Breakfast} (From: {BreakfastFrom}, To: {BreakfastTo}), " +
                    "Lunch: {Lunch} (From: {LunchFrom}, To: {LunchTo}), " +
                    "Dinner: {Dinner} (From: {DinnerFrom}, To: {DinnerTo})",
                    service.ChildGroupId, activeServicesCount,
                    service.Breakfast, service.BreakfastFrom, service.BreakfastTo,
                    service.Lunch, service.LunchFrom, service.LunchTo,
                    service.Dinner, service.DinnerFrom, service.DinnerTo);
            }

            // Crear DataTable para Table-Valued Parameter (SiteServiceForOperatingDaysType)
            var dataTable = new DataTable();
            dataTable.Columns.Add("ChildGroupId", typeof(int));
            // Breakfast
            dataTable.Columns.Add("Breakfast", typeof(bool));
            dataTable.Columns.Add("BreakfastFrom", typeof(TimeSpan));
            dataTable.Columns.Add("BreakfastTo", typeof(TimeSpan));
            // Lunch
            dataTable.Columns.Add("Lunch", typeof(bool));
            dataTable.Columns.Add("LunchFrom", typeof(TimeSpan));
            dataTable.Columns.Add("LunchTo", typeof(TimeSpan));
            // SnackAM
            dataTable.Columns.Add("SnackAM", typeof(bool));
            dataTable.Columns.Add("SnackAMFrom", typeof(TimeSpan));
            dataTable.Columns.Add("SnackAMTo", typeof(TimeSpan));
            // Dinner
            dataTable.Columns.Add("Dinner", typeof(bool));
            dataTable.Columns.Add("DinnerFrom", typeof(TimeSpan));
            dataTable.Columns.Add("DinnerTo", typeof(TimeSpan));
            // SnackPM
            dataTable.Columns.Add("SnackPM", typeof(bool));
            dataTable.Columns.Add("SnackPMFrom", typeof(TimeSpan));
            dataTable.Columns.Add("SnackPMTo", typeof(TimeSpan));
            // SnackNight
            dataTable.Columns.Add("SnackNight", typeof(bool));
            dataTable.Columns.Add("SnackNightFrom", typeof(TimeSpan));
            dataTable.Columns.Add("SnackNightTo", typeof(TimeSpan));
            // DinnerExtended (PACNA)
            dataTable.Columns.Add("DinnerExtended", typeof(bool));
            dataTable.Columns.Add("DinnerExtendedFrom", typeof(TimeSpan));
            dataTable.Columns.Add("DinnerExtendedTo", typeof(TimeSpan));
            // DinnerAtRisk (PACNA)
            dataTable.Columns.Add("DinnerAtRisk", typeof(bool));
            dataTable.Columns.Add("DinnerAtRiskFrom", typeof(TimeSpan));
            dataTable.Columns.Add("DinnerAtRiskTo", typeof(TimeSpan));
            // SnackExtended (PACNA)
            dataTable.Columns.Add("SnackExtended", typeof(bool));
            dataTable.Columns.Add("SnackExtendedFrom", typeof(TimeSpan));
            dataTable.Columns.Add("SnackExtendedTo", typeof(TimeSpan));
            // SnackAtRisk (PACNA)
            dataTable.Columns.Add("SnackAtRisk", typeof(bool));
            dataTable.Columns.Add("SnackAtRiskFrom", typeof(TimeSpan));
            dataTable.Columns.Add("SnackAtRiskTo", typeof(TimeSpan));

            // Llenar DataTable solo con servicios válidos
            foreach (var service in validServices)
            {
                // Helper para convertir bool? a object: solo enviar true (1) si es true, DBNull.Value si es false o null
                // Esto asegura que el stored procedure solo procese servicios con valor true
                object ConvertBoolToDbValue(bool? value) => value == true ? (object)true : DBNull.Value;

                // Log de cada servicio antes de agregarlo al DataTable
                _logger.LogDebug("Agregando servicio al DataTable - ChildGroupId: {ChildGroupId}, Breakfast: {Breakfast}, Lunch: {Lunch}, Dinner: {Dinner}",
                    service.ChildGroupId, service.Breakfast, service.Lunch, service.Dinner);

                dataTable.Rows.Add(
                    service.ChildGroupId,
                    // Breakfast - Solo enviar true (1) si es true, DBNull.Value si es false o null
                    ConvertBoolToDbValue(service.Breakfast),
                    service.BreakfastFrom ?? (object)DBNull.Value,
                    service.BreakfastTo ?? (object)DBNull.Value,
                    // Lunch
                    ConvertBoolToDbValue(service.Lunch),
                    service.LunchFrom ?? (object)DBNull.Value,
                    service.LunchTo ?? (object)DBNull.Value,
                    // SnackAM
                    ConvertBoolToDbValue(service.SnackAM),
                    service.SnackAMFrom ?? (object)DBNull.Value,
                    service.SnackAMTo ?? (object)DBNull.Value,
                    // Dinner
                    ConvertBoolToDbValue(service.Dinner),
                    service.DinnerFrom ?? (object)DBNull.Value,
                    service.DinnerTo ?? (object)DBNull.Value,
                    // SnackPM
                    ConvertBoolToDbValue(service.SnackPM),
                    service.SnackPMFrom ?? (object)DBNull.Value,
                    service.SnackPMTo ?? (object)DBNull.Value,
                    // SnackNight
                    ConvertBoolToDbValue(service.SnackNight),
                    service.SnackNightFrom ?? (object)DBNull.Value,
                    service.SnackNightTo ?? (object)DBNull.Value,
                    // DinnerExtended
                    ConvertBoolToDbValue(service.DinnerExtended),
                    service.DinnerExtendedFrom ?? (object)DBNull.Value,
                    service.DinnerExtendedTo ?? (object)DBNull.Value,
                    // DinnerAtRisk
                    ConvertBoolToDbValue(service.DinnerAtRisk),
                    service.DinnerAtRiskFrom ?? (object)DBNull.Value,
                    service.DinnerAtRiskTo ?? (object)DBNull.Value,
                    // SnackExtended
                    ConvertBoolToDbValue(service.SnackExtended),
                    service.SnackExtendedFrom ?? (object)DBNull.Value,
                    service.SnackExtendedTo ?? (object)DBNull.Value,
                    // SnackAtRisk
                    ConvertBoolToDbValue(service.SnackAtRisk),
                    service.SnackAtRiskFrom ?? (object)DBNull.Value,
                    service.SnackAtRiskTo ?? (object)DBNull.Value
                );
            }

            // Log del DataTable antes de enviarlo
            _logger.LogInformation("DataTable creado con {RowCount} filas para el sitio {SiteId}",
                dataTable.Rows.Count, siteId);

            // Preparar parámetros para el stored procedure
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@operatingFromDate", operatingFromDate.Date, DbType.Date);
            parameters.Add("@operatingToDate", operatingToDate.Date, DbType.Date);
            parameters.Add("@services", dataTable.AsTableValuedParameter("SiteServiceForOperatingDaysType"));
            parameters.Add("@rowsInserted", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // Llamar al stored procedure
            _logger.LogInformation("Llamando al stored procedure 100_InsertServicesForOperatingDays para el sitio {SiteId}", siteId);
            await dbConnection.ExecuteAsync("100_InsertServicesForOperatingDays", parameters, transaction, commandType: CommandType.StoredProcedure);

            var rowsInserted = parameters.Get<int>("@rowsInserted");

            _logger.LogInformation("Se crearon {RowsInserted} servicios para los días de funcionamiento del sitio {SiteId} desde {FromDate} hasta {ToDate}",
                rowsInserted, siteId, operatingFromDate.Date, operatingToDate.Date);

            // Si no se insertaron servicios, log de advertencia con más detalles
            if (rowsInserted == 0)
            {
                _logger.LogWarning("No se insertaron servicios para el sitio {SiteId}. " +
                    "DataTable tenía {RowCount} filas con servicios válidos. " +
                    "Verificar que: " +
                    "1) Los días de funcionamiento existan en el rango {FromDate} a {ToDate}, " +
                    "2) Los días de funcionamiento estén activos (IsActive=1), " +
                    "3) Los servicios tengan valores true (1) y horarios no nulos en el stored procedure.",
                    siteId, dataTable.Rows.Count, operatingFromDate.Date, operatingToDate.Date);

                // Log adicional: verificar si existen días de funcionamiento
                var daysCount = await dbConnection.QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM SiteOperatingDays WHERE SiteId = @siteId AND OperatingDate >= @fromDate AND OperatingDate <= @toDate AND IsActive = 1",
                    new { siteId, fromDate = operatingFromDate.Date, toDate = operatingToDate.Date },
                    transaction);

                _logger.LogWarning("Días de funcionamiento encontrados para el sitio {SiteId} en el rango {FromDate} a {ToDate}: {DaysCount}",
                    siteId, operatingFromDate.Date, operatingToDate.Date, daysCount);
            }
            else
            {
                _logger.LogInformation("✓ Éxito: Se insertaron {RowsInserted} servicios en SiteOperatingDayService para el sitio {SiteId}",
                    rowsInserted, siteId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar servicios para los días de funcionamiento del sitio {SiteId} desde {FromDate} hasta {ToDate}",
                siteId, operatingFromDate.Date, operatingToDate.Date);
            throw;
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Helper method para agregar un servicio a la lista si es válido
    /// </summary>
    private void AddServiceIfValid(
        List<SiteOperatingDayServiceRequest> servicesList,
        int operatingDayId,
        int serviceTypeId,
        bool? isEnabled,
        TimeSpan? startTime,
        TimeSpan? endTime,
        int childGroupId,
        TimeSpan dayStartTime,
        TimeSpan dayEndTime)
    {
        if (isEnabled == true && startTime.HasValue && endTime.HasValue)
        {
            if (IsTimeWithinRange(startTime.Value, endTime.Value, dayStartTime, dayEndTime))
            {
                servicesList.Add(new SiteOperatingDayServiceRequest
                {
                    OperatingDayId = operatingDayId,
                    ServiceTypeId = serviceTypeId,
                    ChildGroupId = childGroupId,
                    StartTime = startTime.Value,
                    EndTime = endTime.Value,
                    IsEnabled = true,
                    Comment = null
                });
            }
        }
    }

    // ===== MIGRACIÓN COMPLETADA =====
    // Este método ya no se usa. Se reemplazó por batch insert para optimizar performance.
    // Se mantiene comentado por referencia durante la transición.
    //
    // /// <summary>
    // /// Inserta un servicio para un día específico usando el repositorio
    // /// </summary>
    // private async Task InsertServiceForDay(int operatingDayId, int serviceTypeId, int? childGroupId, TimeSpan startTime, TimeSpan endTime)
    // {
    //     var request = new SiteOperatingDayServiceRequest
    //     {
    //         OperatingDayId = operatingDayId,
    //         ServiceTypeId = serviceTypeId,
    //         ChildGroupId = childGroupId,
    //         StartTime = startTime,
    //         EndTime = endTime,
    //         IsEnabled = true,
    //         Comment = null
    //     };
    //
    //     await _siteOperatingDayServiceRepository.Value.CreateService(request);
    // }

    /// <summary>
    /// Valida que los horarios del servicio estén dentro del rango del día
    /// </summary>
    private bool IsTimeWithinRange(TimeSpan serviceStart, TimeSpan serviceEnd, TimeSpan dayStart, TimeSpan dayEnd)
    {
        return serviceStart >= dayStart && serviceEnd <= dayEnd && serviceStart < serviceEnd;
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
    /// Obtiene todos los "filas de servicio" del request: una lista por grupo (ChildGroups[].ServiceSlots). AESAN-257.
    /// </summary>
    private static List<List<SiteChildGroupServiceSlotRequest>> GetAllServicesFromRequest(SiteRequest request)
    {
        if (request.ChildGroups == null)
        {
            return [];
        }

        return request.ChildGroups
            .Select(cg => (cg.ServiceSlots ?? []).ToList())
            .ToList();
    }

    /// <summary>
    /// Indica si el servicio tipo serviceTypeId está seleccionado en la lista de slots del grupo. AESAN-257.
    /// </summary>
    private static bool IsServiceTypeSelected(List<SiteChildGroupServiceSlotRequest> slots, int serviceTypeId)
    {
        return slots.Any(s => s.ServiceTypeId == serviceTypeId && s.IsOffered);
    }

    /// <summary>
    /// Obtiene (ServiceTypeId, From, To) para los slots activos (IsOffered y con horarios) del grupo. AESAN-257.
    /// </summary>
    private static List<(int ServiceTypeId, TimeSpan From, TimeSpan To)> GetActiveServiceSlots(List<SiteChildGroupServiceSlotRequest> slots)
    {
        return slots
            .Where(s => s.IsOffered && s.FromTime.HasValue && s.ToTime.HasValue)
            .Select(s => (s.ServiceTypeId, s.FromTime!.Value, s.ToTime!.Value))
            .ToList();
    }

    /// <summary>
    /// Valida que para cada programa PDAM(1), PSAV(2), PACNA(3) del sitio haya al menos un servicio fuerte seleccionado. AESAN-257.
    /// </summary>
    private async Task ValidateStrongServicesForProgramsAsync(SiteRequest request)
    {
        var programIdsToCheck = (request.ProgramIds ?? [])
            .Where(id => id == 1 || id == 2 || id == 3)
            .Distinct()
            .ToList();

        if (programIdsToCheck.Count == 0)
        {
            return;
        }

        var allServices = GetAllServicesFromRequest(request);
        if (allServices.Count == 0)
        {
            var programNames = string.Join(", ", programIdsToCheck.Select(p => p == 1 ? "PDAM" : p == 2 ? "PSAV" : "PACNA"));
            throw new SiteValidationException(
                "MissingStrongService",
                $"Para {programNames} debe incluir al menos uno de los siguientes servicios: Almuerzo o Cena (según programa). Incluya al menos uno en su selección.");
        }

        foreach (var programId in programIdsToCheck)
        {
            var typesByProgram = (await _serviceTypeRepository.GetServiceTypesByProgram(programId)).ToList();
            var strongTypes = typesByProgram.Where(t => t.IsStrongService).Select(t => t.Id).ToList();

            if (strongTypes.Count == 0)
            {
                continue;
            }

            var hasStrong = strongTypes.Any(st => allServices.Any(svc => IsServiceTypeSelected(svc, st)));
            if (!hasStrong)
            {
                var names = string.Join(", ", typesByProgram.Where(t => t.IsStrongService).Select(t => t.Name));
                var programName = programId == 1 ? "PDAM" : programId == 2 ? "PSAV" : "PACNA";
                throw new SiteValidationException(
                    "MissingStrongService",
                    $"Para el programa {programName} debe incluir al menos uno de los siguientes servicios: {names}. Incluya al menos uno en su selección.");
            }
        }
    }

    /// <summary>
    /// Valida que entre cada par de servicios consecutivos (por hora) se respete el mínimo de minutos. AESAN-257.
    /// </summary>
    private async Task ValidateTimeBetweenServicesAsync(SiteRequest request)
    {
        var programIdsToCheck = (request.ProgramIds ?? [])
            .Where(id => id == 1 || id == 2 || id == 3)
            .Distinct()
            .ToList();

        if (programIdsToCheck.Count == 0)
        {
            return;
        }

        var allServices = GetAllServicesFromRequest(request);
        if (allServices.Count == 0)
        {
            return;
        }

        foreach (var programId in programIdsToCheck)
        {
            var typesByProgram = (await _serviceTypeRepository.GetServiceTypesByProgram(programId)).ToList();
            var minMinutesByType = typesByProgram
                .Where(t => t.MinimumMinutesToNextService.HasValue && t.MinimumMinutesToNextService.Value > 0)
                .ToDictionary(t => t.Id, t => t.MinimumMinutesToNextService!.Value);
            var nameByType = typesByProgram.ToDictionary(t => t.Id, t => t.Name);

            foreach (var serviceRow in allServices)
            {
                var slots = GetActiveServiceSlots(serviceRow)
                    .OrderBy(x => x.From)
                    .ToList();

                for (var i = 0; i < slots.Count - 1; i++)
                {
                    var (typeA, _, toA) = slots[i];
                    var (typeB, fromB, _) = slots[i + 1];

                    if (!minMinutesByType.TryGetValue(typeA, out var minMinutes))
                    {
                        continue;
                    }

                    var gapMinutes = (fromB - toA).TotalMinutes;
                    if (gapMinutes < minMinutes)
                    {
                        var nameA = nameByType.TryGetValue(typeA, out var nA) ? nA : $"Servicio {typeA}";
                        var nameB = nameByType.TryGetValue(typeB, out var nB) ? nB : $"Servicio {typeB}";
                        throw new SiteValidationException(
                            "InsufficientTimeBetweenServices",
                            $"Entre {nameA} y {nameB} debe haber al menos {minMinutes} minutos. Ajuste los horarios.");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Valida que si un slot está ofrecido (IsOffered), tenga horarios de inicio y fin.
    /// </summary>
    private static void ValidateServiceSlotTimes(SiteChildGroupServiceSlotRequest slot)
    {
        if (slot.IsOffered && (!slot.FromTime.HasValue || !slot.ToTime.HasValue))
        {
            throw new ArgumentException(
                $"Los horarios (desde y hasta) son requeridos cuando el servicio tipo {slot.ServiceTypeId} está habilitado.");
        }
    }

    /// <summary>
    /// Construye una lista de SiteServiceRequest (formato ancho) desde ChildGroups[].ServiceSlots para InsertServicesForOperatingDays.
    /// Mapeo ServiceTypeId: 1=Breakfast, 2=Lunch, 3=SnackAM, 4=Dinner, 5=SnackPM, 6=SnackNight, 7=DinnerExtended, 8=DinnerAtRisk, 9=SnackExtended, 10=SnackAtRisk.
    /// </summary>
    private static List<SiteServiceRequest> BuildServicesForOperatingDaysFromChildGroups(List<int> childGroupIds, List<SiteChildGroupRequest> childGroups)
    {
        var result = new List<SiteServiceRequest>();
        if (childGroupIds == null || childGroups == null || childGroupIds.Count != childGroups.Count)
        {
            return result;
        }

        for (var i = 0; i < childGroups.Count; i++)
        {
            var group = childGroups[i];
            var childGroupId = childGroupIds[i];
            var wide = new SiteServiceRequest { ChildGroupId = childGroupId };
            foreach (var slot in group.ServiceSlots ?? [])
            {
                if (!slot.IsOffered || !slot.FromTime.HasValue || !slot.ToTime.HasValue)
                {
                    continue;
                }

                switch (slot.ServiceTypeId)
                {
                    case 1: wide.Breakfast = true; wide.BreakfastFrom = slot.FromTime; wide.BreakfastTo = slot.ToTime; break;
                    case 2: wide.Lunch = true; wide.LunchFrom = slot.FromTime; wide.LunchTo = slot.ToTime; break;
                    case 3: wide.SnackAM = true; wide.SnackAMFrom = slot.FromTime; wide.SnackAMTo = slot.ToTime; break;
                    case 4: wide.Dinner = true; wide.DinnerFrom = slot.FromTime; wide.DinnerTo = slot.ToTime; break;
                    case 5: wide.SnackPM = true; wide.SnackPMFrom = slot.FromTime; wide.SnackPMTo = slot.ToTime; break;
                    case 6: wide.SnackNight = true; wide.SnackNightFrom = slot.FromTime; wide.SnackNightTo = slot.ToTime; break;
                    case 7: wide.DinnerExtended = true; wide.DinnerExtendedFrom = slot.FromTime; wide.DinnerExtendedTo = slot.ToTime; break;
                    case 8: wide.DinnerAtRisk = true; wide.DinnerAtRiskFrom = slot.FromTime; wide.DinnerAtRiskTo = slot.ToTime; break;
                    case 9: wide.SnackExtended = true; wide.SnackExtendedFrom = slot.FromTime; wide.SnackExtendedTo = slot.ToTime; break;
                    case 10: wide.SnackAtRisk = true; wide.SnackAtRiskFrom = slot.FromTime; wide.SnackAtRiskTo = slot.ToTime; break;
                }
            }

            result.Add(wide);
        }

        return result;
    }

    /// <summary>
    /// Valida que si un servicio está habilitado, tenga horarios de inicio y fin (formato ancho, legado).
    /// </summary>
    /// <param name="service">Servicio a validar</param>
    /// <exception cref="ArgumentException">Lanza excepción si un servicio habilitado no tiene horarios</exception>
    private void ValidateServiceTimes(SiteServiceRequest service)
    {
        if (service.Breakfast == true && (!service.BreakfastFrom.HasValue || !service.BreakfastTo.HasValue))
        {
            throw new ArgumentException("Los horarios de desayuno (desde y hasta) son requeridos cuando el servicio está habilitado");
        }

        if (service.Lunch == true && (!service.LunchFrom.HasValue || !service.LunchTo.HasValue))
        {
            throw new ArgumentException("Los horarios de almuerzo (desde y hasta) son requeridos cuando el servicio está habilitado");
        }

        if (service.SnackAM == true && (!service.SnackAMFrom.HasValue || !service.SnackAMTo.HasValue))
        {
            throw new ArgumentException("Los horarios de merienda AM (desde y hasta) son requeridos cuando el servicio está habilitado");
        }

        if (service.Dinner == true && (!service.DinnerFrom.HasValue || !service.DinnerTo.HasValue))
        {
            throw new ArgumentException("Los horarios de cena (desde y hasta) son requeridos cuando el servicio está habilitado");
        }

        if (service.SnackPM == true && (!service.SnackPMFrom.HasValue || !service.SnackPMTo.HasValue))
        {
            throw new ArgumentException("Los horarios de merienda PM (desde y hasta) son requeridos cuando el servicio está habilitado");
        }

        if (service.SnackNight == true && (!service.SnackNightFrom.HasValue || !service.SnackNightTo.HasValue))
        {
            throw new ArgumentException("Los horarios de merienda nocturna (desde y hasta) son requeridos cuando el servicio está habilitado");
        }

        if (service.DinnerExtended == true && (!service.DinnerExtendedFrom.HasValue || !service.DinnerExtendedTo.HasValue))
        {
            throw new ArgumentException("Los horarios de cena extendida (desde y hasta) son requeridos cuando el servicio está habilitado");
        }

        if (service.DinnerAtRisk == true && (!service.DinnerAtRiskFrom.HasValue || !service.DinnerAtRiskTo.HasValue))
        {
            throw new ArgumentException("Los horarios de cena en riesgo (desde y hasta) son requeridos cuando el servicio está habilitado");
        }

        if (service.SnackExtended == true && (!service.SnackExtendedFrom.HasValue || !service.SnackExtendedTo.HasValue))
        {
            throw new ArgumentException("Los horarios de merienda extendida (desde y hasta) son requeridos cuando el servicio está habilitado");
        }

        if (service.SnackAtRisk == true && (!service.SnackAtRiskFrom.HasValue || !service.SnackAtRiskTo.HasValue))
        {
            throw new ArgumentException("Los horarios de merienda en riesgo (desde y hasta) son requeridos cuando el servicio está habilitado");
        }
    }

    /// <summary>
    /// Actualiza información de Day Care Home para un sitio existente
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="dayCareHome">Datos de Day Care Home</param>
    /// <returns>True si se actualizó correctamente</returns>
    private async Task<bool> UpdateSiteDayCareHome(int siteId, SiteDayCareHomeRequest dayCareHome, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {

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
            parameters.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("100_UpdateSiteDayCareHome", parameters, transaction, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@returnValue");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar información de Day Care Home para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }


    /// <summary>
    /// Actualiza tipos de participantes para un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="participantTypeIds">Lista de IDs de tipos de participantes</param>
    /// <returns>True si se actualizaron correctamente</returns>
    private async Task<bool> UpdateSiteParticipants(int siteId, List<int> participantTypeIds, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@participantTypeIds", string.Join(",", participantTypeIds), DbType.String);

            await dbConnection.ExecuteAsync("100_UpdateSiteParticipants", parameters, transaction, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar tipos de participantes para el sitio {SiteId}", siteId);
            throw new Exception(ex.Message);
        }
        finally
        {
            if (shouldDisposeConnection)
            {
                dbConnection.Dispose();
            }
        }
    }

}
