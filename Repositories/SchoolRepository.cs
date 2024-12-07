using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class SchoolRepository(DapperContext context, ILogger<SchoolRepository> logger) : ISchoolRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SchoolRepository> _logger = logger;

    public async Task<dynamic> GetAllSchools(int take, int skip, string name, bool alls)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new { take, skip, name, alls };

            var result = await dbConnection.QueryMultipleAsync("100_GetSchools", parameters, commandType: CommandType.StoredProcedure);

            var schools = result.Read<dynamic>().Select(item => new DTOSchool
            {
                Id = item.Id,
                Name = item.Name,
                EducationLevel = new DTOEducationLevel { Id = item.EducationLevelId, Name = item.EducationLevelName },
                OperatingPeriod = new DTOOperatingPeriod { Id = item.OperatingPeriodId, Name = item.OperatingPeriodName },
                Address = item.Address,
                City = new DTOCity { Id = item.CityId, Name = item.CityName },
                Region = new DTORegion { Id = item.RegionId, Name = item.RegionName },
                ZipCode = item.ZipCode,
                OrganizationType = new DTOOrganizationType { Id = item.OrganizationTypeId, Name = item.OrganizationTypeName }
            }).ToList();

            var count = result.Read<int>().Single();

            return new { data = schools, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las escuelas");
            throw new Exception(ex.Message);
        }
    }

    public async Task<dynamic> GetSchoolById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new { Id = id };

            var result = await dbConnection.QueryMultipleAsync("100_GetSchoolById", parameters, commandType: CommandType.StoredProcedure);

            var schoolData = await result.ReadFirstOrDefaultAsync<dynamic>();
            if (schoolData == null) return null;

            var school = new
            {
                schoolData.Id,
                schoolData.Name,
                EducationLevel = new DTOEducationLevel { Id = schoolData.EducationLevelId, Name = schoolData.EducationLevelName },
                OperatingPeriod = new DTOOperatingPeriod { Id = schoolData.OperatingPeriodId, Name = schoolData.OperatingPeriodName },
                schoolData.Address,
                City = new DTOCity { Id = schoolData.CityId, Name = schoolData.CityName },
                Region = new DTORegion { Id = schoolData.RegionId, Name = schoolData.RegionName },
                schoolData.ZipCode,
                OrganizationType = new DTOOrganizationType { Id = schoolData.OrganizationTypeId, Name = schoolData.OrganizationTypeName }
            };

            //school.Facilities = (await GetSchoolFacilities(id)).ToList();
            //school.MealTypes = (await GetSchoolMealTypes(id)).ToList();

            return school;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la escuela");
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<DTOFacility>> GetSchoolFacilities(int schoolId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new { SchoolId = schoolId };

            var facilities = await dbConnection.QueryAsync<DTOFacility>(
                "SELECT f.Id, f.Name FROM Facility f " +
                "INNER JOIN SchoolFacility sf ON f.Id = sf.FacilityId " +
                "WHERE sf.SchoolId = @SchoolId",
                parameters
            );

            return facilities.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las facilidades de la escuela");
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<DTOMealType>> GetSchoolMealTypes(int schoolId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new { SchoolId = schoolId };

            var mealTypes = await dbConnection.QueryAsync<DTOMealType>(
                "SELECT mt.Id, mt.Name FROM MealType mt " +
                "INNER JOIN SchoolMealRequest smr ON mt.Id = smr.MealTypeId " +
                "WHERE smr.SchoolId = @SchoolId",
                parameters
            );

            return mealTypes.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de comida de la escuela");
            throw new Exception(ex.Message);
        }
    }

    public async Task<int> InsertSchool(SchoolRequest request)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@Name", request.Name);
            parameters.Add("@EducationLevelId", request.EducationLevelId);
            parameters.Add("@OperatingPeriodId", request.OperatingPeriodId);
            parameters.Add("@Address", request.Address);
            parameters.Add("@CityId", request.CityId);
            parameters.Add("@RegionId", request.RegionId);
            parameters.Add("@ZipCode", request.ZipCode);
            parameters.Add("@OrganizationTypeId", request.OrganizationTypeId);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSchool", parameters, commandType: CommandType.StoredProcedure);

            int schoolId = parameters.Get<int>("@Id");

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
                        "INSERT INTO SchoolMealRequest (SchoolId, MealTypeId) VALUES (@SchoolId, @MealTypeId)",
                        new { SchoolId = schoolId, MealTypeId = mealTypeId }
                    );
                }
            }

            return schoolId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la escuela");
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> UpdateSchool(int id, SchoolRequest request)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            parameters.Add("@Id", id);
            parameters.Add("@Name", request.Name);
            parameters.Add("@EducationLevelId", request.EducationLevelId);
            parameters.Add("@OperatingPeriodId", request.OperatingPeriodId);
            parameters.Add("@Address", request.Address);
            parameters.Add("@CityId", request.CityId);
            parameters.Add("@RegionId", request.RegionId);
            parameters.Add("@ZipCode", request.ZipCode);
            parameters.Add("@OrganizationTypeId", request.OrganizationTypeId);

            await dbConnection.ExecuteAsync("100_UpdateSchool", parameters, commandType: CommandType.StoredProcedure);

            // Actualizar facilidades
            await dbConnection.ExecuteAsync("DELETE FROM SchoolFacility WHERE SchoolId = @SchoolId", new { SchoolId = id });
            if (request.FacilityIds != null && request.FacilityIds.Any())
            {
                foreach (var facilityId in request.FacilityIds)
                {
                    await dbConnection.ExecuteAsync(
                        "INSERT INTO SchoolFacility (SchoolId, FacilityId) VALUES (@SchoolId, @FacilityId)",
                        new { SchoolId = id, FacilityId = facilityId }
                    );
                }
            }

            // Actualizar tipos de comida
            await dbConnection.ExecuteAsync("DELETE FROM SchoolMealRequest WHERE SchoolId = @SchoolId", new { SchoolId = id });
            if (request.MealTypeIds != null && request.MealTypeIds.Any())
            {
                foreach (var mealTypeId in request.MealTypeIds)
                {
                    await dbConnection.ExecuteAsync(
                        "INSERT INTO SchoolMealRequest (SchoolId, MealTypeId) VALUES (@SchoolId, @MealTypeId)",
                        new { SchoolId = id, MealTypeId = mealTypeId }
                    );
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la escuela");
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> DeleteSchool(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new { Id = id };

            await dbConnection.ExecuteAsync("100_DeleteSchool", parameters, commandType: CommandType.StoredProcedure);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la escuela");
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<DTOMealType>> GetAllMealTypes()
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var result = await dbConnection.QueryAsync<DTOMealType>("SELECT * FROM MealType ORDER BY Name");
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de comida");
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<DTOOrganizationType>> GetAllOrganizationTypes()
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var result = await dbConnection.QueryAsync<DTOOrganizationType>("SELECT * FROM OrganizationType ORDER BY Name");
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de organización");
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<DTOFacility>> GetAllFacilities()
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var result = await dbConnection.QueryAsync<DTOFacility>("SELECT * FROM Facility ORDER BY Name");
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las facilidades");
            throw new Exception(ex.Message);
        }
    }
}