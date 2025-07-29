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

public class EmployeeRepository(
    DapperContext context,
    ILogger<EmployeeRepository> logger,
    IMemoryCache cache,
    IOptions<ApplicationSettings> appSettings) : IEmployeeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<EmployeeRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene un empleado por su ID
    /// </summary>
    /// <param name="id">El ID del empleado</param>
    /// <returns>El empleado</returns>
    public async Task<dynamic> GetEmployeeById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QueryFirstOrDefaultAsync<DTOEmployee>(
                "100_GetEmployeeById",
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
            _logger.LogError(ex, "Error al obtener el empleado con ID {EmployeeId}", id);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los empleados de la base de datos
    /// </summary>
    /// <param name="take">El número de empleados a obtener</param>
    /// <param name="skip">El número de empleados a saltar</param>
    /// <param name="name">El nombre del empleado a buscar</param>
    /// <param name="alls">Si se deben obtener todos los empleados</param>
    /// <param name="isList">Si es para lista simple (dropdowns)</param>
    /// <returns>Los empleados</returns>
    public async Task<dynamic> GetAllEmployeesFromDb(int take, int skip, string name, bool alls, bool isList)
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
                string cacheKey = string.Format(_appSettings.Cache.Keys.Employees, take, skip, name, alls);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        var result = await dbConnection.QueryMultipleAsync("100_GetEmployees", param, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(MapEmployeeListFromResult).ToList();
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
                    "100_GetEmployees",
                    param,
                    commandType: CommandType.StoredProcedure
                );

                if (result == null)
                {
                    return null;
                }

                var data = result.Read<dynamic>().Select(MapEmployeeFromResult).ToList();
                var count = result.Read<int>().Single();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los empleados");
            throw new Exception(ex.Message);
        }
    }



    /// <summary>
    /// Inserta un nuevo empleado en la base de datos
    /// </summary>
    /// <param name="employeeRequest">Datos del empleado a insertar</param>
    /// <returns>True si se insertó correctamente</returns>
    public async Task<bool> InsertEmployee(EmployeeRequest employeeRequest)
    {
        try
        {
            _logger.LogInformation("Insertando nuevo empleado");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@firstName", employeeRequest.FirstName, DbType.String, ParameterDirection.Input);
            parameters.Add("@middleName", employeeRequest.MiddleName, DbType.String, ParameterDirection.Input);
            parameters.Add("@fatherLastName", employeeRequest.FatherLastName, DbType.String, ParameterDirection.Input);
            parameters.Add("@motherLastName", employeeRequest.MotherLastName, DbType.String, ParameterDirection.Input);
            parameters.Add("@statusId", employeeRequest.StatusId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@positionId", employeeRequest.PositionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@birthDate", employeeRequest.BirthDate, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@email", employeeRequest.Email, DbType.String, ParameterDirection.Input);
            parameters.Add("@postalAddress", employeeRequest.PostalAddress, DbType.String, ParameterDirection.Input);
            parameters.Add("@cityId", employeeRequest.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@regionId", employeeRequest.RegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@areaCode", employeeRequest.AreaCode, DbType.String, ParameterDirection.Input);
            parameters.Add("@comments", employeeRequest.Comments, DbType.String, ParameterDirection.Input);
            parameters.Add("@userId", employeeRequest.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertEmployee", parameters, commandType: CommandType.StoredProcedure);

            var employeeId = parameters.Get<int>("@id");

            InvalidateCache(employeeId);

            return employeeId > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el empleado");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un empleado existente en la base de datos
    /// </summary>
    /// <param name="employeeRequest">Datos del empleado a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateEmployee(EmployeeRequest employeeRequest)
    {
        try
        {
            _logger.LogInformation("Actualizando empleado con ID {EmployeeId}", employeeRequest.Id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", employeeRequest.Id, DbType.Int32);
            parameters.Add("@firstName", employeeRequest.FirstName, DbType.String);
            parameters.Add("@middleName", employeeRequest.MiddleName, DbType.String);
            parameters.Add("@fatherLastName", employeeRequest.FatherLastName, DbType.String);
            parameters.Add("@motherLastName", employeeRequest.MotherLastName, DbType.String);
            parameters.Add("@statusId", employeeRequest.StatusId, DbType.Int32);
            parameters.Add("@positionId", employeeRequest.PositionId, DbType.Int32);
            parameters.Add("@birthDate", employeeRequest.BirthDate, DbType.DateTime);
            parameters.Add("@email", employeeRequest.Email, DbType.String);
            parameters.Add("@postalAddress", employeeRequest.PostalAddress, DbType.String);
            parameters.Add("@cityId", employeeRequest.CityId, DbType.Int32);
            parameters.Add("@regionId", employeeRequest.RegionId, DbType.Int32);
            parameters.Add("@areaCode", employeeRequest.AreaCode, DbType.String);
            parameters.Add("@comments", employeeRequest.Comments, DbType.String);
            parameters.Add("@userId", employeeRequest.UserId, DbType.String);
            parameters.Add("@isActive", employeeRequest.IsActive, DbType.Boolean);

            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("100_UpdateEmployee", parameters, commandType: CommandType.StoredProcedure);

            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(employeeRequest.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el empleado con ID {EmployeeId}", employeeRequest.Id);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Elimina un empleado de la base de datos (baja lógica)
    /// </summary>
    /// <param name="id">El ID del empleado a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    public async Task<bool> DeleteEmployee(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando empleado con ID {EmployeeId}", id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var rowsAffected = await dbConnection.ExecuteAsync(
                "100_DeleteEmployee",
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
            _logger.LogError(ex, "Error al eliminar el empleado con ID {EmployeeId}", id);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Convierte un empleado en usuario del sistema
    /// </summary>
    /// <param name="employeeId">ID del empleado</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>True si se convirtió correctamente</returns>
    public async Task<bool> ConvertEmployeeToUser(int employeeId, string userId)
    {
        try
        {
            _logger.LogInformation("Convirtiendo empleado con ID {EmployeeId} a usuario", employeeId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@employeeId", employeeId, DbType.Int32);
            parameters.Add("@userId", userId, DbType.String);

            var rowsAffected = await dbConnection.ExecuteAsync(
                "100_ConvertEmployeeToUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (rowsAffected > 0)
            {
                InvalidateCache(employeeId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al convertir empleado con ID {EmployeeId} a usuario", employeeId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Verifica si existe un empleado principal
    /// </summary>
    /// <returns>True si existe un empleado principal</returns>
    public async Task<bool> HasMainEmployee()
    {
        try
        {
            _logger.LogInformation("Verificando si existe empleado principal");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();

            var result = await dbConnection.QueryFirstOrDefaultAsync<int>(
                "100_HasMainEmployee",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si existe empleado principal");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza el estado activo de un empleado
    /// </summary>
    /// <param name="employeeId">ID del empleado</param>
    /// <param name="isActive">Nuevo estado activo</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateEmployeeActiveStatus(int employeeId, bool isActive)
    {
        try
        {
            _logger.LogInformation("Actualizando estado activo del empleado con ID {EmployeeId} a {IsActive}", employeeId, isActive);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@employeeId", employeeId, DbType.Int32);
            parameters.Add("@isActive", isActive, DbType.Boolean);

            var rowsAffected = await dbConnection.ExecuteAsync(
                "100_UpdateEmployeeActiveStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (rowsAffected > 0)
            {
                InvalidateCache(employeeId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado activo del empleado con ID {EmployeeId}", employeeId);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Invalida el caché para el empleado
    /// </summary>
    /// <param name="employeeId">ID del empleado</param>
    private void InvalidateCache(int? employeeId = null)
    {
        if (employeeId.HasValue)
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.Employees, 0, 0, "", false));
        }

        // Invalidar listas completas
        _cache.Remove(_appSettings.Cache.Keys.Employees);

        _logger.LogInformation("Cache invalidado para Employee Repository");
    }

    /// <summary>
    /// Mapea el resultado de la consulta a una lista de empleados
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Lista de empleados</returns>
    private static DTOEmployee MapEmployeeListFromResult(dynamic result)
    {
        return new DTOEmployee
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
            IsActive = result.IsActive
        };
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un empleado
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Empleado</returns>
    private static DTOEmployee MapEmployeeFromResult(dynamic result)
    {
        return new DTOEmployee
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
            IsActive = result.IsActive
        };
    }
}