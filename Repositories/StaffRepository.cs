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
    /// Agrega un parámetro de fecha de forma segura, convirtiendo fechas inválidas a NULL
    /// </summary>
    private void AddSafeDateParameter(DynamicParameters parameters, string parameterName, DateTime? dateValue, ParameterDirection direction = ParameterDirection.Input)
    {
        if (!dateValue.HasValue)
        {
            parameters.Add(parameterName, DBNull.Value, DbType.DateTime, direction);
            return;
        }

        var date = dateValue.Value;

        // SQL Server acepta fechas desde 1753-01-01 hasta 9999-12-31
        if (date >= new DateTime(1753, 1, 1) && date <= new DateTime(9999, 12, 31))
        {
            parameters.Add(parameterName, date, DbType.DateTime, direction);
        }
        else
        {
            // Si la fecha está fuera del rango válido, usar NULL
            parameters.Add(parameterName, DBNull.Value, DbType.DateTime, direction);

            // Solo loguear si es una fecha realmente problemática (no DateTime.MinValue/MaxValue)
            if (date != DateTime.MinValue && date != DateTime.MaxValue)
            {
                _logger.LogWarning("Fecha fuera del rango SQL Server en {ParameterName}: {DateValue}", parameterName, date);
            }
        }
    }

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

            var result = await dbConnection.QueryFirstOrDefaultAsync<dynamic>("100_GetStaffById", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            return MapStaffDetailsFromResult(result);
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
    /// <param name="staffTypeId">ID del tipo de staff para filtrar</param>
    /// <returns>Los miembros del staff</returns>
    public async Task<dynamic> GetAllStaffFromDb(int take, int skip, string name, bool alls, bool isList, int? staffTypeId = null, int? agencyId = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@take", take, DbType.Int32);
            param.Add("@skip", skip, DbType.Int32);
            param.Add("@name", name, DbType.String);
            param.Add("@alls", alls, DbType.Boolean);
            param.Add("@staffTypeId", staffTypeId, DbType.Int32);
            param.Add("@agencyId", agencyId, DbType.Int32);

            if (isList)
            {
                string cacheKey = string.Format(_appSettings.Cache.Keys.Staff, take, skip, name, alls, staffTypeId, agencyId);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        var result = await dbConnection.QueryMultipleAsync("100_GetAllStaff", param, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(MapStaffListFromResult).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(1)
                );
            }
            else
            {
                var result = await dbConnection.QueryMultipleAsync("100_GetAllStaff", param, commandType: CommandType.StoredProcedure);

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
            // Fechas seguras para SQL Server
            parameters.Add("@contractStartDate", staffRequest.ContractStartDate?.Year >= 1753 ? staffRequest.ContractStartDate : DBNull.Value, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@contractEndDate", staffRequest.ContractEndDate?.Year >= 1753 ? staffRequest.ContractEndDate : DBNull.Value, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@birthDate", staffRequest.BirthDate?.Year >= 1753 ? staffRequest.BirthDate : DBNull.Value, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@email", staffRequest.Email ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@postalAddress", staffRequest.PostalAddress ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@cityId", staffRequest.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@regionId", staffRequest.RegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@areaCode", staffRequest.AreaCode ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@agencyId", staffRequest.AgencyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@comments", staffRequest.Comments ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@userId", staffRequest.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("@reviewResultId", staffRequest.ReviewResultId, DbType.Int32, ParameterDirection.Input);
            // Fecha de revisión segura
            parameters.Add("@reviewDate", staffRequest.ReviewDate?.Year >= 1753 ? staffRequest.ReviewDate : DBNull.Value, DbType.DateTime, ParameterDirection.Input);
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

            // Fechas seguras para SQL Server
            parameters.Add("@contractStartDate", staffRequest.ContractStartDate?.Year >= 1753 ? staffRequest.ContractStartDate : DBNull.Value, DbType.DateTime);
            parameters.Add("@contractEndDate", staffRequest.ContractEndDate?.Year >= 1753 ? staffRequest.ContractEndDate : DBNull.Value, DbType.DateTime);
            parameters.Add("@birthDate", staffRequest.BirthDate?.Year >= 1753 ? staffRequest.BirthDate : DBNull.Value, DbType.DateTime);

            parameters.Add("@email", staffRequest.Email ?? "", DbType.String);
            parameters.Add("@postalAddress", staffRequest.PostalAddress ?? "", DbType.String);
            parameters.Add("@cityId", staffRequest.CityId, DbType.Int32);
            parameters.Add("@regionId", staffRequest.RegionId, DbType.Int32);
            parameters.Add("@areaCode", staffRequest.AreaCode ?? "", DbType.String);
            parameters.Add("@agencyId", staffRequest.AgencyId, DbType.Int32);
            parameters.Add("@comments", staffRequest.Comments ?? "", DbType.String);
            parameters.Add("@userId", staffRequest.UserId, DbType.String);
            parameters.Add("@isActive", staffRequest.IsActive, DbType.Boolean);
            parameters.Add("@reviewResultId", staffRequest.ReviewResultId);

            // Fecha de revisión segura
            parameters.Add("@reviewDate", staffRequest.ReviewDate?.Year >= 1753 ? staffRequest.ReviewDate : DBNull.Value, DbType.DateTime);

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

            var rowsAffected = await dbConnection.ExecuteAsync("100_ConvertStaffToUser", parameters, commandType: CommandType.StoredProcedure);

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

            var rowsAffected = await dbConnection.ExecuteAsync("100_UpdateStaffActiveStatus", parameters, commandType: CommandType.StoredProcedure);

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
    /// Maps query result to a Staff list item
    /// </summary>
    /// <param name="result">Query result</param>
    /// <returns>Mapped Staff list item</returns>
    private static dynamic MapStaffListFromResult(dynamic result)
    {
        return new
        {
            result.Id,
            result.FirstName,
            result.MiddleName,
            result.FatherLastName,
            result.MotherLastName,
            result.StatusName,
            result.StatusNameEN,
            result.PositionName,
            result.PositionNameEN,
            result.StaffTypeId,
            result.StaffTypeName,
            result.StaffTypeNameEn,
            result.StaffClassificationId,
            result.StaffClassificationName,
            result.StaffClassificationNameEn,
            result.ContractStartDate,
            result.ContractEndDate,
            result.Email,
            result.CityName,
            result.RegionName,
            result.AgencyId,
            result.AgencyName,
            result.UserName,
            result.IsActive
        };
    }

    /// <summary>
    /// Maps query result to a Staff object
    /// </summary>
    /// <param name="result">Query result</param>
    /// <returns>Mapped Staff object</returns>
    private static dynamic MapStaffFromResult(dynamic result)
    {
        return new
        {
            result.Id,
            result.FirstName,
            result.MiddleName,
            result.FatherLastName,
            result.MotherLastName,
            result.StatusId,
            result.StatusName,
            result.StatusNameEN,
            result.PositionId,
            result.PositionName,
            result.PositionNameEN,
            result.StaffTypeId,
            result.StaffTypeName,
            result.StaffTypeNameEn,
            result.StaffClassificationId,
            result.StaffClassificationName,
            result.StaffClassificationNameEn,
            result.ContractStartDate,
            result.ContractEndDate,
            result.BirthDate,
            result.Email,
            result.PostalAddress,
            result.CityId,
            result.CityName,
            result.RegionId,
            result.RegionName,
            result.AreaCode,
            result.AgencyId,
            result.AgencyName,
            result.Comments,
            result.UserId,
            result.UserName,
            result.CreatedAt,
            result.UpdatedAt,
            result.IsActive,
            result.ReviewResultId,
            result.ReviewDate,
            result.ReviewJustification
        };
    }

    /// <summary>
    /// Maps GetById result to a DTOStaff with nested relations
    /// </summary>
    /// <param name="item">Dynamic result item</param>
    /// <returns>DTOStaff</returns>
    private static DTOStaff MapStaffDetailsFromResult(dynamic item)
    {
        return new DTOStaff
        {
            Id = item.Id,
            FirstName = item.FirstName ?? string.Empty,
            MiddleName = item.MiddleName,
            FatherLastName = item.FatherLastName ?? string.Empty,
            MotherLastName = item.MotherLastName ?? string.Empty,
            StatusId = item.StatusId ?? 0,
            StatusName = item.StatusName ?? string.Empty,
            PositionId = item.PositionId ?? 0,
            PositionName = item.PositionName ?? string.Empty,
            StaffTypeId = item.StaffTypeId ?? 0,
            StaffTypeName = item.StaffTypeName ?? string.Empty,
            StaffTypeNameEn = item.StaffTypeNameEn ?? string.Empty,
            StaffClassificationId = item.StaffClassificationId,
            StaffClassificationName = item.StaffClassificationName ?? string.Empty,
            StaffClassificationNameEn = item.StaffClassificationNameEn ?? string.Empty,
            ContractStartDate = item.ContractStartDate,
            ContractEndDate = item.ContractEndDate,
            BirthDate = item.BirthDate ?? DateTime.MinValue,
            Email = item.Email ?? string.Empty,
            PostalAddress = item.PostalAddress ?? string.Empty,
            CityId = item.CityId ?? 0,
            CityName = item.CityName ?? string.Empty,
            RegionId = item.RegionId ?? 0,
            RegionName = item.RegionName ?? string.Empty,
            AreaCode = item.AreaCode ?? string.Empty,
            AgencyId = item.AgencyId,
            AgencyName = item.AgencyName ?? string.Empty,
            Comments = item.Comments,
            UserId = item.UserId,
            UserName = item.UserName,
            CreatedAt = item.CreatedAt ?? DateTime.Now,
            UpdatedAt = item.UpdatedAt,
            IsActive = item.IsActive ?? true,
            ReviewResultId = item.ReviewResultId,
            ReviewDate = item.ReviewDate,
            ReviewJustification = item.ReviewJustification,

            City = new DTOCity
            {
                Id = item.CityId ?? 0,
                Name = item.CityName ?? string.Empty
            },
            Region = new DTORegion
            {
                Id = item.RegionId ?? 0,
                Name = item.RegionName ?? string.Empty
            },
            Status = new DTOOptionSelection
            {
                Id = item.StatusId ?? 0,
                Name = item.StatusName ?? string.Empty,
                NameEN = item.StatusNameEN ?? string.Empty,
            },
            Position = new DTOOptionSelection
            {
                Id = item.PositionId ?? 0,
                Name = item.PositionName ?? string.Empty,
                NameEN = item.PositionNameEN ?? string.Empty,
            },
            StaffType = new DTOStaffType
            {
                Id = item.StaffTypeId ?? 0,
                Name = item.StaffTypeName ?? string.Empty,
                NameEn = item.StaffTypeNameEn ?? string.Empty,
            },
            StaffClassification = item.StaffClassificationId != null ? new DTOStaffClassification
            {
                Id = item.StaffClassificationId,
                Name = item.StaffClassificationName ?? string.Empty,
                NameEn = item.StaffClassificationNameEn ?? string.Empty,
            } : null
        };
    }

    /// <summary>
    /// Obtiene todos los miembros del staff de una agencia específica
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <param name="take">El número de miembros del staff a obtener</param>
    /// <param name="skip">El número de miembros del staff a saltar</param>
    /// <param name="name">El nombre del miembro del staff a buscar</param>
    /// <param name="staffTypeId">ID del tipo de staff para filtrar</param>
    /// <returns>Los miembros del staff de la agencia</returns>
    public async Task<dynamic> GetStaffByAgency(int agencyId, int take, int skip, string name, int? staffTypeId = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@agencyId", agencyId, DbType.Int32);
            param.Add("@take", take, DbType.Int32);
            param.Add("@skip", skip, DbType.Int32);
            param.Add("@name", name, DbType.String);
            param.Add("@staffTypeId", staffTypeId, DbType.Int32);

            var result = await dbConnection.QueryMultipleAsync("100_GetStaffByAgency", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var data = result.Read<dynamic>().Select(MapStaffFromResult).ToList();
            var count = result.Read<int>().FirstOrDefault();

            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los miembros del staff de la agencia {AgencyId}", agencyId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Invalida el caché para un miembro del staff específico
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    private void InvalidateCache(int staffId)
    {
        try
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.Staff, 0, 0, "", false, null));
            _cache.Remove(_appSettings.Cache.Keys.Staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al invalidar caché para miembro del staff con ID {StaffId}", staffId);
        }
    }
}