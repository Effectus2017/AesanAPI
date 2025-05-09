using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
namespace Api.Repositories;

public class SchoolRepository(DapperContext context, ILogger<SchoolRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : ISchoolRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SchoolRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene una escuela por su ID
    /// </summary>
    /// <param name="id">El ID de la escuela a obtener.</param>
    /// <returns>La escuela encontrada.</returns>
    public async Task<dynamic> GetSchoolById(int id)
    {
        using IDbConnection dbConnection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);

        var result = await dbConnection.QueryMultipleAsync("101_GetSchoolById", parameters, commandType: CommandType.StoredProcedure);

        var schoolData = await result.ReadFirstOrDefaultAsync<dynamic>();
        var facilities = (await result.ReadAsync<dynamic>()).ToList();
        var satellites = (await result.ReadAsync<dynamic>()).ToList();

        if (schoolData == null)
        {
            return null;
        }

        return new
        {
            schoolData,
            Facilities = facilities,
            Satellites = satellites
        };
    }

    /// <summary>
    /// Obtiene todas las escuelas
    /// </summary>
    /// <param name="take">El número de escuelas a obtener.</param>
    /// <param name="skip">El número de escuelas a saltar.</param>
    /// <param name="name">El nombre de la escuela a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las escuelas.</param>
    /// <returns>Las escuelas encontradas.</returns>
    public async Task<dynamic> GetAllSchools(int take, int skip, string name, bool alls)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.Schools, take, skip, name, alls);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@take", take, DbType.Int32);
                parameters.Add("@skip", skip, DbType.Int32);
                parameters.Add("@name", name, DbType.String);
                parameters.Add("@alls", alls, DbType.Boolean);

                var result = await dbConnection.QueryMultipleAsync("101_GetSchools", parameters, commandType: CommandType.StoredProcedure);

                var schools = (await result.ReadAsync<dynamic>()).ToList();
                var count = (await result.ReadAsync<int>()).SingleOrDefault();
                return new { data = schools, count };
            },
            _logger,
            _appSettings
        );
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
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@Name", request.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@StartDate", request.StartDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Address", request.Address, DbType.String, ParameterDirection.Input);
            parameters.Add("@PostalAddress", request.PostalAddress, DbType.String, ParameterDirection.Input);
            parameters.Add("@ZipCode", request.ZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@CityId", request.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@RegionId", request.RegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@AreaCode", request.AreaCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@AdminFullName", request.AdminFullName, DbType.String, ParameterDirection.Input);
            parameters.Add("@Phone", request.Phone, DbType.String, ParameterDirection.Input);
            parameters.Add("@PhoneExtension", request.PhoneExtension, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mobile", request.Mobile, DbType.String, ParameterDirection.Input);
            parameters.Add("@BaseYear", request.BaseYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@NextRenewalYear", request.NextRenewalYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@OrganizationTypeId", request.OrganizationTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@EducationLevelId", request.EducationLevelId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@OperatingPeriodId", request.OperatingPeriodId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@KitchenTypeId", request.KitchenTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@GroupTypeId", request.GroupTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@DeliveryTypeId", request.DeliveryTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SponsorTypeId", request.SponsorTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ApplicantTypeId", request.ApplicantTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@OperatingPolicyId", request.OperatingPolicyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("101_InsertSchool", parameters, commandType: CommandType.StoredProcedure);

            int schoolId = parameters.Get<int>("@Id");

            // Insertar facilidades
            if (request.FacilityIds != null && request.FacilityIds.Any())
            {
                foreach (var facilityId in request.FacilityIds)
                {
                    var ok = await InsertSchoolFacilityAsync(dbConnection, schoolId, facilityId);

                    if (!ok)
                    {
                        _logger.LogWarning($"No se pudo insertar SchoolFacility para SchoolId={schoolId}, FacilityId={facilityId}");
                    }
                }
            }

            // Insertar satélites
            if (request.SatelliteSchoolIds != null && request.SatelliteSchoolIds.Any())
            {
                foreach (var satelliteId in request.SatelliteSchoolIds)
                {
                    var ok = await InsertSatelliteSchoolAsync(dbConnection, schoolId, satelliteId);

                    if (!ok)
                    {
                        _logger.LogWarning($"No se pudo insertar SatelliteSchool para MainSchoolId={schoolId}, SatelliteSchoolId={satelliteId}");
                    }
                }
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
    public async Task<bool> UpdateSchool(SchoolRequest request)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@Id", request.Id, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@Name", request.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@StartDate", request.StartDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@Address", request.Address, DbType.String, ParameterDirection.Input);
            parameters.Add("@PostalAddress", request.PostalAddress, DbType.String, ParameterDirection.Input);
            parameters.Add("@ZipCode", request.ZipCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@CityId", request.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@RegionId", request.RegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@AreaCode", request.AreaCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@AdminFullName", request.AdminFullName, DbType.String, ParameterDirection.Input);
            parameters.Add("@Phone", request.Phone, DbType.String, ParameterDirection.Input);
            parameters.Add("@PhoneExtension", request.PhoneExtension, DbType.String, ParameterDirection.Input);
            parameters.Add("@Mobile", request.Mobile, DbType.String, ParameterDirection.Input);
            parameters.Add("@BaseYear", request.BaseYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@NextRenewalYear", request.NextRenewalYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@OrganizationTypeId", request.OrganizationTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@EducationLevelId", request.EducationLevelId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@OperatingPeriodId", request.OperatingPeriodId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@KitchenTypeId", request.KitchenTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@GroupTypeId", request.GroupTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@DeliveryTypeId", request.DeliveryTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@SponsorTypeId", request.SponsorTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@ApplicantTypeId", request.ApplicantTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@OperatingPolicyId", request.OperatingPolicyId, DbType.Int32, ParameterDirection.Input);

            await dbConnection.ExecuteAsync("101_UpdateSchool", parameters, commandType: CommandType.StoredProcedure);

            // Actualizar facilidades
            var facilityParams = new DynamicParameters();
            facilityParams.Add("@school_id", request.Id, DbType.Int32, ParameterDirection.Input);
            facilityParams.Add("@is_active", false, DbType.Boolean, ParameterDirection.Input);
            await dbConnection.ExecuteAsync("102_UpdateSchoolFacilityIsActive", facilityParams, commandType: CommandType.StoredProcedure);

            if (request.FacilityIds != null && request.FacilityIds.Count != 0)
            {
                foreach (var facilityId in request.FacilityIds)
                {
                    var ok = await InsertSchoolFacilityAsync(dbConnection, request.Id, facilityId);
                    if (!ok)
                    {
                        _logger.LogWarning($"No se pudo insertar SchoolFacility para SchoolId={request.Id}, FacilityId={facilityId}");
                    }
                }
            }

            // Actualizar satélites
            var satelliteParams = new DynamicParameters();
            satelliteParams.Add("@main_school_id", request.Id, DbType.Int32, ParameterDirection.Input);
            satelliteParams.Add("@is_active", false, DbType.Boolean, ParameterDirection.Input);
            await dbConnection.ExecuteAsync("102_UpdateSatelliteSchoolIsActive", satelliteParams, commandType: CommandType.StoredProcedure);

            if (request.SatelliteSchoolIds != null && request.SatelliteSchoolIds.Count != 0)
            {
                foreach (var satelliteId in request.SatelliteSchoolIds)
                {
                    var ok = await InsertSatelliteSchoolAsync(dbConnection, request.Id, satelliteId);
                    if (!ok)
                    {
                        _logger.LogWarning($"No se pudo insertar SatelliteSchool para MainSchoolId={request.Id}, SatelliteSchoolId={satelliteId}");
                    }
                }
            }

            // Invalidar caché
            InvalidateCache(request.Id);

            return true;
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

            var rowsAffected = await dbConnection.ExecuteAsync("101_DeleteSchool", parameters, commandType: CommandType.StoredProcedure);

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

    private void InvalidateCache(int? schoolId = null)
    {
        if (schoolId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.School, schoolId));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.Schools);
        _logger.LogInformation("Cache invalidado para School Repository");
    }

    public async Task<bool> InsertSchoolFacilityAsync(IDbConnection dbConnection, int schoolId, int facilityId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@school_id", schoolId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@facility_id", facilityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@is_active", true, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@created_at", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);
            await dbConnection.ExecuteAsync("101_InsertSchoolFacility", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al insertar SchoolFacility para SchoolId={schoolId}, FacilityId={facilityId}");
            return false;
        }
    }

    public async Task<bool> InsertSatelliteSchoolAsync(IDbConnection dbConnection, int mainSchoolId, int satelliteSchoolId)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@main_school_id", mainSchoolId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@satellite_school_id", satelliteSchoolId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@is_active", true, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@created_at", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);
            await dbConnection.ExecuteAsync("101_InsertSatelliteSchool", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al insertar SatelliteSchool para MainSchoolId={mainSchoolId}, SatelliteSchoolId={satelliteSchoolId}");
            return false;
        }
    }
}
