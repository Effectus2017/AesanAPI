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
        string cacheKey = string.Format(_appSettings.Cache.Keys.School, id);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection dbConnection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@id", id, DbType.Int32);

                var result = await dbConnection.QueryMultipleAsync(
                    "100_GetSchoolById",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var schoolData = await result.ReadFirstOrDefaultAsync<dynamic>();
                if (schoolData == null)
                    return null;

                return new
                {
                    schoolData.Id,
                    schoolData.Name,
                    EducationLevel = new DTOEducationLevel
                    {
                        Id = schoolData.EducationLevelId,
                        Name = schoolData.EducationLevelName
                    },
                    OperatingPeriod = new DTOOperatingPeriod
                    {
                        Id = schoolData.OperatingPeriodId,
                        Name = schoolData.OperatingPeriodName
                    },
                    schoolData.Address,
                    City = new DTOCity { Id = schoolData.CityId, Name = schoolData.CityName },
                    Region = new DTORegion { Id = schoolData.RegionId, Name = schoolData.RegionName },
                    schoolData.ZipCode,
                    OrganizationType = new DTOOrganizationType
                    {
                        Id = schoolData.OrganizationTypeId,
                        Name = schoolData.OrganizationTypeName
                    }
                };
            },
            _logger,
            _appSettings
        );
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

                var result = await dbConnection.QueryMultipleAsync(
                    "100_GetSchools",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var schools = result.Read<dynamic>().Select(
                    item =>
                        new DTOSchool
                        {
                            Id = item.Id,
                            Name = item.Name,
                            EducationLevel = new DTOEducationLevel
                            {
                                Id = item.EducationLevelId,
                                Name = item.EducationLevelName
                            },
                            OperatingPeriod = new DTOOperatingPeriod
                            {
                                Id = item.OperatingPeriodId,
                                Name = item.OperatingPeriodName
                            },
                            Address = item.Address,
                            City = new DTOCity { Id = item.CityId, Name = item.CityName },
                            Region = new DTORegion { Id = item.RegionId, Name = item.RegionName },
                            ZipCode = item.ZipCode,
                            OrganizationType = new DTOOrganizationType
                            {
                                Id = item.OrganizationTypeId,
                                Name = item.OrganizationTypeName
                            }
                        }
                ).ToList();

                var count = result.Read<int>().Single();
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

            parameters.Add("@name", request.Name);
            parameters.Add("@educationLevelId", request.EducationLevelId);
            parameters.Add("@operatingPeriodId", request.OperatingPeriodId);
            parameters.Add("@address", request.Address);
            parameters.Add("@cityId", request.CityId);
            parameters.Add("@regionId", request.RegionId);
            parameters.Add("@zipCode", request.ZipCode);
            parameters.Add("@organizationTypeId", request.OrganizationTypeId);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync(
                "100_InsertSchool",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            int schoolId = parameters.Get<int>("@id");

            // Insertar facilidades
            if (request.FacilityIds != null && request.FacilityIds.Any())
            {
                foreach (var facilityId in request.FacilityIds)
                {
                    await dbConnection.ExecuteAsync(
                        "INSERT INTO SchoolFacility (SchoolId, FacilityId) VALUES (@SchoolId, @FacilityId)",
                        new { SchoolId = schoolId, FacilityId = facilityId }
                    );
                }
            }

            // Insertar tipos de comida
            if (request.MealTypeIds != null && request.MealTypeIds.Any())
            {
                foreach (var mealTypeId in request.MealTypeIds)
                {
                    await dbConnection.ExecuteAsync(
                        "INSERT INTO SchoolMeal (SchoolId, MealTypeId) VALUES (@SchoolId, @MealTypeId)",
                        new { SchoolId = schoolId, MealTypeId = mealTypeId }
                    );
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

            parameters.Add("@id", request.Id);
            parameters.Add("@name", request.Name);
            parameters.Add("@educationLevelId", request.EducationLevelId);
            parameters.Add("@operatingPeriodId", request.OperatingPeriodId);
            parameters.Add("@address", request.Address);
            parameters.Add("@cityId", request.CityId);
            parameters.Add("@regionId", request.RegionId);
            parameters.Add("@zipCode", request.ZipCode);
            parameters.Add("@organizationTypeId", request.OrganizationTypeId);

            await dbConnection.ExecuteAsync(
                "100_UpdateSchool",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Actualizar facilidades
            await dbConnection.ExecuteAsync(
                "DELETE FROM SchoolFacility WHERE SchoolId = @SchoolId",
                new { SchoolId = request.Id }
            );
            if (request.FacilityIds != null && request.FacilityIds.Any())
            {
                foreach (var facilityId in request.FacilityIds)
                {
                    await dbConnection.ExecuteAsync(
                        "INSERT INTO SchoolFacility (SchoolId, FacilityId) VALUES (@SchoolId, @FacilityId)",
                        new { SchoolId = request.Id, FacilityId = facilityId }
                    );
                }
            }

            // Actualizar tipos de comida
            await dbConnection.ExecuteAsync(
                "DELETE FROM SchoolMeal WHERE SchoolId = @SchoolId",
                new { SchoolId = request.Id }
            );
            if (request.MealTypeIds != null && request.MealTypeIds.Any())
            {
                foreach (var mealTypeId in request.MealTypeIds)
                {
                    await dbConnection.ExecuteAsync(
                        "INSERT INTO SchoolMeal (SchoolId, MealTypeId) VALUES (@SchoolId, @MealTypeId)",
                        new { SchoolId = request.Id, MealTypeId = mealTypeId }
                    );
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
            parameters.Add("@id", id);

            var rowsAffected = await dbConnection.ExecuteAsync(
                "100_DeleteSchool",
                parameters,
                commandType: CommandType.StoredProcedure
            );

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
}
