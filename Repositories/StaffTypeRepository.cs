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

public class StaffTypeRepository(DapperContext context, ILogger<StaffTypeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IStaffTypeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<StaffTypeRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene un tipo de staff por su ID
    /// </summary>
    /// <param name="id">El ID del tipo de staff</param>
    /// <returns>El tipo de staff</returns>
    public async Task<dynamic> GetStaffTypeById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QueryFirstOrDefaultAsync<DTOStaffType>("100_GetStaffTypeById", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de staff con ID {StaffTypeId}", id);
            throw; // Preservar la excepción original con toda la información
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de staff de la base de datos
    /// </summary>
    /// <param name="take">El número de tipos de staff a obtener</param>
    /// <param name="skip">El número de tipos de staff a saltar</param>
    /// <param name="name">El nombre del tipo de staff a buscar</param>
    /// <param name="alls">Si se deben obtener todos los tipos de staff</param>
    /// <param name="isList">Si es para lista simple (dropdown)</param>
    /// <returns>Los tipos de staff</returns>
    public async Task<dynamic> GetAllStaffTypesFromDb(int take, int skip, string name, bool alls, bool isList)
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
                string cacheKey = string.Format(_appSettings.Cache.Keys.StaffTypes, take, skip, name, alls);

                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        var result = await dbConnection.QueryMultipleAsync("100_GetStaffTypes", param, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(MapStaffTypeListFromResult).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(30)
                );
            }
            else
            {
                var result = await dbConnection.QueryMultipleAsync("100_GetStaffTypes", param, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var data = result.Read<dynamic>().Select(MapStaffTypeFromResult).ToList();
                var count = result.Read<int>().FirstOrDefault();

                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los tipos de staff. Parámetros: take={Take}, skip={Skip}, name={Name}, alls={Alls}, isList={IsList}", take, skip, name, alls, isList);
            throw; // Preservar la excepción original con toda la información
        }
    }

    /// <summary>
    /// Inserta un nuevo tipo de staff en la base de datos
    /// </summary>
    /// <param name="staffTypeRequest">Datos del tipo de staff a insertar</param>
    /// <returns>True si se insertó correctamente</returns>
    public async Task<bool> InsertStaffType(StaffTypeRequest staffTypeRequest)
    {
        try
        {
            _logger.LogInformation("Insertando nuevo tipo de staff");

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", staffTypeRequest.Name, DbType.String, ParameterDirection.Input);
            parameters.Add("@nameEn", staffTypeRequest.NameEn, DbType.String, ParameterDirection.Input);
            parameters.Add("@displayOrder", staffTypeRequest.DisplayOrder, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertStaffType", parameters, commandType: CommandType.StoredProcedure);

            var staffTypeId = parameters.Get<int>("@id");

            InvalidateCache(staffTypeId);

            return staffTypeId > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el tipo de staff. Datos: Name={Name}, NameEn={NameEn}, DisplayOrder={DisplayOrder}",
                staffTypeRequest.Name, staffTypeRequest.NameEn, staffTypeRequest.DisplayOrder);
            throw; // Preservar la excepción original con toda la información
        }
    }

    /// <summary>
    /// Actualiza un tipo de staff existente en la base de datos
    /// </summary>
    /// <param name="staffTypeRequest">Datos del tipo de staff a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateStaffType(StaffTypeRequest staffTypeRequest)
    {
        try
        {
            _logger.LogInformation("Actualizando tipo de staff con ID {StaffTypeId}", staffTypeRequest.Id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", staffTypeRequest.Id, DbType.Int32);
            parameters.Add("@name", staffTypeRequest.Name, DbType.String);
            parameters.Add("@nameEn", staffTypeRequest.NameEn, DbType.String);
            parameters.Add("@displayOrder", staffTypeRequest.DisplayOrder, DbType.Int32);
            parameters.Add("@isActive", staffTypeRequest.IsActive, DbType.Boolean);

            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("100_UpdateStaffType", parameters, commandType: CommandType.StoredProcedure);

            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(staffTypeRequest.Id.Value);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de staff con ID {StaffTypeId}. Datos: Name={Name}, NameEn={NameEn}, DisplayOrder={DisplayOrder}, IsActive={IsActive}",
            staffTypeRequest.Id, staffTypeRequest.Name, staffTypeRequest.NameEn, staffTypeRequest.DisplayOrder, staffTypeRequest.IsActive);
            throw; // Preservar la excepción original con toda la información
        }
    }

    /// <summary>
    /// Elimina un tipo de staff de la base de datos (baja lógica)
    /// </summary>
    /// <param name="id">El ID del tipo de staff a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    public async Task<bool> DeleteStaffType(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando tipo de staff con ID {StaffTypeId}", id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var rowsAffected = await dbConnection.ExecuteAsync(
                "100_DeleteStaffType",
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
            _logger.LogError(ex, "Error al eliminar el tipo de staff con ID {StaffTypeId}", id);
            throw; // Preservar la excepción original con toda la información
        }
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un objeto StaffType para lista
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Objeto StaffType mapeado</returns>
    private static dynamic MapStaffTypeListFromResult(dynamic result)
    {
        return new
        {
            result.Id,
            result.Name,
            result.NameEn,
            result.DisplayOrder,
            result.IsActive,
            result.CreatedAt,
            result.UpdatedAt
        };
    }

    /// <summary>
    /// Mapea el resultado de la consulta a un objeto StaffType
    /// </summary>
    /// <param name="result">Resultado de la consulta</param>
    /// <returns>Objeto StaffType mapeado</returns>
    private static dynamic MapStaffTypeFromResult(dynamic result)
    {
        return new
        {
            result.Id,
            result.Name,
            result.NameEn,
            result.DisplayOrder,
            result.IsActive,
            result.CreatedAt,
            result.UpdatedAt
        };
    }

    /// <summary>
    /// Invalida el caché para un tipo de staff específico
    /// </summary>
    /// <param name="staffTypeId">ID del tipo de staff</param>
    private void InvalidateCache(int staffTypeId)
    {
        try
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.StaffTypes, 0, 0, "", false));
            _cache.Remove(_appSettings.Cache.Keys.StaffTypes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al invalidar caché para tipo de staff con ID {StaffTypeId}", staffTypeId);
        }
    }
}