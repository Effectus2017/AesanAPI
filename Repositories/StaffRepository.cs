using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class StaffRepository(
    DapperContext context,
    ILogger<StaffRepository> logger,
    IMemoryCache cache,
    IOptions<ApplicationSettings> appSettings) : IStaffRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<StaffRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene un miembro del staff por su ID
    /// </summary>
    /// <param name="id">El ID del miembro del staff</param>
    /// <returns>El miembro del staff</returns>
    public async Task<dynamic> GetStaffById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QueryFirstOrDefaultAsync<DTOStaff>(
                "100_GetStaffById",
                param,
                commandType: CommandType.StoredProcedure
            );

            if (result == null)
            {
                return null;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el miembro del staff con ID {StaffId}", id);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los miembros del staff de la base de datos
    /// </summary>
    /// <param name="take">El número de miembros del staff a obtener</param>
    /// <param name="skip">El número de miembros del staff a saltar</param>
    /// <param name="name">El nombre del miembro del staff a buscar</param>
    /// <param name="alls">Si se deben obtener todos los miembros del staff</param>
    /// <param name="isList">Si es para lista simple (dropdown)</param>
    /// <returns>Los miembros del staff</returns>
    public async Task<dynamic> GetAllStaffFromDb(int take, int skip, string name, bool alls, bool isList)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@take", take, DbType.Int32);
            param.Add("@skip", skip, DbType.Int32);
            param.Add("@name", name, DbType.String);
            param.Add("@alls", alls, DbType.Boolean);

            if (isList)
            {
                string cacheKey = string.Format(_appSettings.Cache.Keys.Staff, take, skip, name, alls);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        var result = await dbConnection.QueryMultipleAsync("100_GetStaff", param, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(MapStaffListFromResult).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(30)
                );
            }
            else
            {
                var result = await dbConnection.QueryMultipleAsync(
                    "100_GetStaff",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                {
                    return null;
                }

                var data = result.Read<dynamic>().Select(MapStaffFromResult).ToList();
                var count = result.Read<int>().FirstOrDefault();

                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los miembros del staff");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Inserta un nuevo miembro del staff en la base de datos
    /// </summary>
    /// <param name="staffRequest">Datos del miembro del staff a insertar</param>
    /// <returns>True si se insertó correctamente</returns>
    public async Task<bool> InsertStaff(StaffRequest staffRequest)
    {
        try
        {
            _logger.LogInformation("Insertando nuevo miembro del staff");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@firstName", staffRequest.FirstName ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@middleName", staffRequest.MiddleName ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@fatherLastName", staffRequest.FatherLastName ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@motherLastName", staffRequest.MotherLastName ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@statusId", staffRequest.StatusId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@positionId", staffRequest.PositionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@staffTypeId", staffRequest.StaffTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@staffClassificationId", staffRequest.StaffClassificationId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@birthDate", staffRequest.BirthDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@email", staffRequest.Email ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@postalAddress", staffRequest.PostalAddress ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@cityId", staffRequest.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@regionId", staffRequest.RegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@areaCode", staffRequest.AreaCode ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@comments", staffRequest.Comments ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@userId", staffRequest.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("@reviewResultId", staffRequest.ReviewResultId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@reviewDate", staffRequest.ReviewDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@reviewJustification", staffRequest.ReviewJustification ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertStaff", parameters, commandType: CommandType.StoredProcedure);

            var staffId = parameters.Get<int>("@id");

            InvalidateCache(staffId);

            return staffId > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el miembro del staff");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un miembro del staff existente en la base de datos
    /// </summary>
    /// <param name="staffRequest">Datos del miembro del staff a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateStaff(StaffRequest staffRequest)
    {
        try
        {
            _logger.LogInformation("Actualizando miembro del staff con ID {StaffId}", staffRequest.Id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", staffRequest.Id, DbType.Int32);
            parameters.Add("@firstName", staffRequest.FirstName ?? "", DbType.String);
            parameters.Add("@middleName", staffRequest.MiddleName ?? "", DbType.String);
            parameters.Add("@fatherLastName", staffRequest.FatherLastName ?? "", DbType.String);
            parameters.Add("@motherLastName", staffRequest.MotherLastName ?? "", DbType.String);
            parameters.Add("@statusId", staffRequest.StatusId, DbType.Int32);
            parameters.Add("@positionId", staffRequest.PositionId, DbType.Int32);
            parameters.Add("@staffTypeId", staffRequest.StaffTypeId, DbType.Int32);
            parameters.Add("@staffClassificationId", staffRequest.StaffClassificationId, DbType.Int32);
            parameters.Add("@birthDate", staffRequest.BirthDate, DbType.DateTime);
            parameters.Add("@email", staffRequest.Email ?? "", DbType.String);
            parameters.Add("@postalAddress", staffRequest.PostalAddress ?? "", DbType.String);
            parameters.Add("@cityId", staffRequest.CityId, DbType.Int32);
            parameters.Add("@regionId", staffRequest.RegionId, DbType.Int32);
            parameters.Add("@areaCode", staffRequest.AreaCode ?? "", DbType.String);
            parameters.Add("@comments", staffRequest.Comments ?? "", DbType.String);
            parameters.Add("@userId", staffRequest.UserId, DbType.String);
            parameters.Add("@isActive", staffRequest.IsActive, DbType.Boolean);
            parameters.Add("@reviewResultId", staffRequest.ReviewResultId, DbType.Int32);
            parameters.Add("@reviewDate", staffRequest.ReviewDate, DbType.DateTime);
            parameters.Add("@reviewJustification", staffRequest.ReviewJustification ?? "", DbType.String);

            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("100_UpdateStaff", parameters, commandType: CommandType.StoredProcedure);

            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(staffRequest.Id.Value);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el miembro del staff con ID {StaffId}", staffRequest.Id);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un miembro del staff de la base de datos (baja lógica)
    /// </summary>
    /// <param name="id">El ID del miembro del staff a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    public async Task<bool> DeleteStaff(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando miembro del staff con ID {StaffId}", id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var rowsAffected = await dbConnection.ExecuteAsync(
                "100_DeleteStaff",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (rowsAffected > 0)
            {
                InvalidateCache(id);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el miembro del staff con ID {StaffId}", id);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Convierte un miembro del staff en usuario del sistema
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>True si se convirtió correctamente</returns>
    public async Task<bool> ConvertStaffToUser(int staffId, string userId)
    {
        try
        {
            _logger.LogInformation("Convirtiendo miembro del staff con ID {StaffId} a usuario", staffId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@staffId", staffId, DbType.Int32);
            parameters.Add("@userId", userId, DbType.String);

            var rowsAffected = await dbConnection.ExecuteAsync(
                "100_ConvertStaffToUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (rowsAffected > 0)
            {
                InvalidateCache(staffId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al convertir miembro del staff con ID {StaffId} a usuario", staffId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza el estado activo de un miembro del staff
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    /// <param name="isActive">Nuevo estado activo</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateStaffActiveStatus(int staffId, bool isActive)
    {
        try
        {
            _logger.LogInformation("Actualizando estado activo del miembro del staff con ID {StaffId}", staffId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@staffId", staffId, DbType.Int32);
            parameters.Add("@isActive", isActive, DbType.Boolean);

            var rowsAffected = await dbConnection.ExecuteAsync(
                "100_UpdateStaffActiveStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (rowsAffected > 0)
            {
                InvalidateCache(staffId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado activo del miembro del staff con ID {StaffId}", staffId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un objeto Staff para lista
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Objeto Staff mapeado</returns>
    private static dynamic MapStaffListFromResult(dynamic result)
    {
        return new
        {
            Id = result.Id,
            FirstName = result.FirstName,
            MiddleName = result.MiddleName,
            FatherLastName = result.FatherLastName,
            MotherLastName = result.MotherLastName,
            StatusName = result.StatusName,
            PositionName = result.PositionName,
            StaffTypeName = result.StaffTypeName,
            StaffTypeNameEn = result.StaffTypeNameEn,
            Email = result.Email,
            CityName = result.CityName,
            RegionName = result.RegionName,
            UserName = result.UserName,
            IsActive = result.IsActive
        };
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un objeto Staff
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Objeto Staff mapeado</returns>
    private static dynamic MapStaffFromResult(dynamic result)
    {
        return new
        {
            Id = result.Id,
            FirstName = result.FirstName,
            MiddleName = result.MiddleName,
            FatherLastName = result.FatherLastName,
            MotherLastName = result.MotherLastName,
            StatusId = result.StatusId,
            StatusName = result.StatusName,
            PositionId = result.PositionId,
            PositionName = result.PositionName,
            StaffTypeId = result.StaffTypeId,
            StaffTypeName = result.StaffTypeName,
            StaffTypeNameEn = result.StaffTypeNameEn,
            BirthDate = result.BirthDate,
            Email = result.Email,
            PostalAddress = result.PostalAddress,
            CityId = result.CityId,
            CityName = result.CityName,
            RegionId = result.RegionId,
            RegionName = result.RegionName,
            AreaCode = result.AreaCode,
            Comments = result.Comments,
            UserId = result.UserId,
            UserName = result.UserName,
            CreatedAt = result.CreatedAt,
            UpdatedAt = result.UpdatedAt,
            IsActive = result.IsActive,
            ReviewResultId = result.ReviewResultId,
            ReviewDate = result.ReviewDate,
            ReviewJustification = result.ReviewJustification
        };
    }

    /// <summary>
    /// Invalida el caché para un miembro del staff específico
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    private void InvalidateCache(int staffId)
    {
        try
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.Staff, 0, 0, "", false));
            _cache.Remove(_appSettings.Cache.Keys.Staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al invalidar caché para miembro del staff con ID {StaffId}", staffId);
        }
    }
}