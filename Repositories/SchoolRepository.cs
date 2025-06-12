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
    public async Task<DTOSchool> GetSchoolById(int id)
    {
        using IDbConnection dbConnection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@Id", id, DbType.Int32);

        var result = await dbConnection.QueryMultipleAsync("102_GetSchoolById", parameters, commandType: CommandType.StoredProcedure);

        var schoolData = await result.ReadFirstOrDefaultAsync<dynamic>();
        var facilities = result.Read<dynamic>().ToList();
        var satellites = result.Read<dynamic>().ToList();

        if (schoolData == null)
        {
            return null;
        }

        var dtoSchool = MapSchoolFromResult(schoolData);
        dtoSchool.Facilities = facilities.Select(MapFacilityFromResult).ToList();
        dtoSchool.Satellites = satellites.Select(MapSatelliteFromResult).ToList();
        return dtoSchool;
    }

    /// <summary>
    /// Obtiene todas las escuelas
    /// </summary>
    /// <param name="take">El número de escuelas a obtener.</param>
    /// <param name="skip">El número de escuelas a saltar.</param>
    /// <param name="name">El nombre de la escuela a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las escuelas.</param>
    /// <returns>Las escuelas encontradas.</returns>
    public async Task<dynamic> GetAllSchoolsFromDB(int take, int skip, string name, int? cityId, int? regionId, bool alls)
    {
        using IDbConnection dbConnection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@take", take, DbType.Int32);
        parameters.Add("@skip", skip, DbType.Int32);
        parameters.Add("@name", name, DbType.String);
        parameters.Add("@cityId", cityId, DbType.Int32);
        parameters.Add("@regionId", regionId, DbType.Int32);
        parameters.Add("@alls", alls, DbType.Boolean);

        var schoolsDynamic = new List<dynamic>();
        var count = 0;

        using var result = await dbConnection.QueryMultipleAsync("102_GetSchools", parameters, commandType: CommandType.StoredProcedure);

        if (result == null)
        {
            return null;
        }

        if (!result.IsConsumed)
        {
            schoolsDynamic = result.Read<dynamic>().ToList();
        }

        if (!result.IsConsumed)
        {
            count = result.ReadFirstOrDefault<int>();
        }

        var data = schoolsDynamic.Select(MapSchoolFromResult).ToList();

        return new { data, count };
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

            parameters.Add("@ame", request.Name, DbType.String, ParameterDirection.Input);
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
            parameters.Add("@centerId", request.CenterId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@nonProfit", request.NonProfit, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@baseYear", request.BaseYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@renewalYear", request.RenewalYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@educationLevelId", request.EducationLevelId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@operatingDays", request.OperatingDays, DbType.Int32, ParameterDirection.Input);
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
            parameters.Add("@isMainSchool", request.IsMainSchool, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("102_InsertSchool", parameters, commandType: CommandType.StoredProcedure);

            int schoolId = parameters.Get<int>("@Id");

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

            parameters.Add("@id", request.Id, DbType.Int32, ParameterDirection.Input);
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
            parameters.Add("@centerId", request.CenterId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@nonProfit", request.NonProfit, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@baseYear", request.BaseYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@renewalYear", request.RenewalYear, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@educationLevelId", request.EducationLevelId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@operatingDays", request.OperatingDays, DbType.Int32, ParameterDirection.Input);
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
            parameters.Add("@isMainSchool", request.IsMainSchool, DbType.Boolean, ParameterDirection.Input);

            await dbConnection.ExecuteAsync("102_UpdateSchool", parameters, commandType: CommandType.StoredProcedure);

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

    public async Task<bool> InsertSchoolFacilityAsync(int schoolId, int facilityId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@facilityId", facilityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@isActive", true, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@createdAt", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);
            await dbConnection.ExecuteAsync("102_InsertSchoolFacility", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al insertar SchoolFacility para SchoolId={schoolId}, FacilityId={facilityId}");
            return false;
        }
    }

    public async Task<bool> InsertSatelliteSchoolAsync(int mainSchoolId, int satelliteSchoolId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@mainSchoolId", mainSchoolId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@satelliteSchoolId", satelliteSchoolId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@isActive", true, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("@createdAt", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Input);
            await dbConnection.ExecuteAsync("102_InsertSatelliteSchool", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al insertar SatelliteSchool para MainSchoolId={mainSchoolId}, SatelliteSchoolId={satelliteSchoolId}");
            return false;
        }
    }

    /// <summary>
    /// Maps a dynamic result to a DTOSchool object
    /// </summary>
    /// <param name="item">Resultado dinámico</param>
    /// <returns>DTOSchool</returns>
    private static DTOSchool MapSchoolFromResult(dynamic item)
    {
        try
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item), "El objeto item no puede ser nulo");
            }

            return new DTOSchool
            {
                Id = item.Id ?? 0,
                Name = item.Name ?? string.Empty,
                StartDate = item.StartDate,
                Address = item.Address ?? string.Empty,
                CityId = item.CityId ?? 0,
                RegionId = item.RegionId ?? 0,
                ZipCode = item.ZipCode ?? string.Empty,
                Latitude = item.Latitude,
                Longitude = item.Longitude,
                PostalAddress = item.PostalAddress ?? string.Empty,
                PostalCityId = item.PostalCityId,
                PostalRegionId = item.PostalRegionId,
                PostalZipCode = item.PostalZipCode ?? string.Empty,
                SameAsPhysicalAddress = item.SameAsPhysicalAddress,
                OrganizationTypeId = item.OrganizationTypeId ?? 0,
                CenterId = item.CenterId,
                NonProfit = item.NonProfit,
                BaseYear = item.BaseYear,
                RenewalYear = item.RenewalYear,
                EducationLevelId = item.EducationLevelId ?? 0,
                OperatingDays = item.OperatingDays,
                KitchenTypeId = item.KitchenTypeId,
                GroupTypeId = item.GroupTypeId,
                DeliveryTypeId = item.DeliveryTypeId,
                SponsorTypeId = item.SponsorTypeId,
                ApplicantTypeId = item.ApplicantTypeId,
                ResidentialTypeId = item.ResidentialTypeId,
                OperatingPolicyId = item.OperatingPolicyId,
                HasWarehouse = item.HasWarehouse,
                HasDiningRoom = item.HasDiningRoom,
                AdministratorAuthorizedName = item.AdministratorAuthorizedName ?? string.Empty,
                SitePhone = item.SitePhone ?? string.Empty,
                Extension = item.Extension ?? string.Empty,
                MobilePhone = item.MobilePhone ?? string.Empty,
                Breakfast = item.Breakfast,
                BreakfastFrom = item.BreakfastFrom,
                BreakfastTo = item.BreakfastTo,
                Lunch = item.Lunch,
                LunchFrom = item.LunchFrom,
                LunchTo = item.LunchTo,
                Snack = item.Snack,
                SnackFrom = item.SnackFrom,
                SnackTo = item.SnackTo,
                IsActive = item.IsActive ?? true,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                // Nested catalogs (if needed, can be mapped here)
                City = item.CityId != null ? new DTOCity
                {
                    Id = item.CityId,
                    Name = item.CityName ?? string.Empty
                } : null,
                Region = item.RegionId != null ? new DTORegion
                {
                    Id = item.RegionId,
                    Name = item.RegionName ?? string.Empty
                } : null,
                PostalCity = item.PostalCityId != null ? new DTOCity
                {
                    Id = item.PostalCityId,
                    Name = item.PostalCityName ?? string.Empty
                } : null,
                PostalRegion = item.PostalRegionId != null ? new DTORegion
                {
                    Id = item.PostalRegionId,
                    Name = item.PostalRegionName ?? string.Empty
                } : null,
                EducationLevel = item.EducationLevelId != null ? new DTOEducationLevel
                {
                    Id = item.EducationLevelId,
                    Name = item.EducationLevelName ?? string.Empty,
                    NameEN = item.EducationLevelNameEN ?? string.Empty
                } : null,
                OrganizationType = item.OrganizationTypeId != null
                    ? new DTOOrganizationType
                    {
                        Id = item.OrganizationTypeId,
                        Name = item.OrganizationTypeName ?? string.Empty,
                        NameEN = item.OrganizationTypeNameEN ?? string.Empty
                    }
                    : null,
                KitchenType = item.KitchenTypeId != null
                    ? new DTOKitchenType
                    {
                        Id = item.KitchenTypeId,
                        Name = item.KitchenTypeName ?? string.Empty,
                        NameEN = item.KitchenTypeNameEN ?? string.Empty
                    }
                    : null,
                GroupType = item.GroupTypeId != null
                    ? new DTOGroupType
                    {
                        Id = item.GroupTypeId,
                        Name = item.GroupTypeName ?? string.Empty,
                        NameEN = item.GroupTypeNameEN ?? string.Empty
                    }
                    : null,
                DeliveryType = item.DeliveryTypeId != null
                    ? new DTODeliveryType
                    {
                        Id = item.DeliveryTypeId,
                        Name = item.DeliveryTypeName ?? string.Empty,
                        NameEN = item.DeliveryTypeNameEN ?? string.Empty
                    }
                    : null,
                SponsorType = item.SponsorTypeId != null
                    ? new DTOSponsorType
                    {
                        Id = item.SponsorTypeId,
                        Name = item.SponsorTypeName ?? string.Empty,
                        NameEN = item.SponsorTypeNameEN ?? string.Empty
                    }
                    : null,
                ApplicantType = item.ApplicantTypeId != null
                    ? new DTOApplicantType
                    {
                        Id = item.ApplicantTypeId,
                        Name = item.ApplicantTypeName ?? string.Empty,
                        NameEN = item.ApplicantTypeNameEN ?? string.Empty
                    }
                    : null,
                ResidentialType = item.ResidentialTypeId != null
                    ? new DTOResidentialType
                    {
                        Id = item.ResidentialTypeId,
                        Name = item.ResidentialTypeName ?? string.Empty,
                        NameEN = item.ResidentialTypeNameEN ?? string.Empty
                    }
                    : null,
                OperatingPolicy = item.OperatingPolicyId != null
                    ? new DTOOperatingPolicy
                    {
                        Id = item.OperatingPolicyId,
                        Name = item.OperatingPolicyName ?? string.Empty,
                        NameEN = item.OperatingPolicyNameEN ?? string.Empty
                    }
                    : null,
                CenterType = item.CenterId != null ? new DTOCenterType
                {
                    Id = item.CenterId,
                    Name = item.CenterName ?? string.Empty,
                    NameEN = item.CenterNameEN ?? string.Empty
                } : null
            };
        }
        catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
        {
            throw new InvalidOperationException($"Error mapping school: Property not found or invalid. {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Unexpected error mapping school: {ex.Message}", ex);
        }
    }

    private static DTOFacility MapFacilityFromResult(dynamic item)
    {
        return new DTOFacility
        {
            Id = item.Id,
            Name = item.Name
        };
    }

    private static DTOFacility MapSatelliteFromResult(dynamic item)
    {
        return new DTOFacility
        {
            Id = item.Id,
            Name = item.Name
        };
    }
}

