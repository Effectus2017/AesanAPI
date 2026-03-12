using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.Errors;
using Api.Models.Request;
using Api.Models.Response;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class StaffTypeRepository(DapperContext context, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : IStaffTypeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

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
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener el tipo de staff con ID {id}", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de staff de la base de datos
    /// </summary>
    /// <param name="take">El número de tipos de staff a obtener</param>
    /// <param name="skip">El número de tipos de staff a saltar</param>
    /// <param name="name">El nombre del tipo de staff a buscar</param>
    /// <param name="alls">Si se deben obtener todos los tipos de staff</param>
    /// <param name="forDropdown">Si es para lista simple (dropdown)</param>
    /// <returns>Los tipos de staff</returns>
    public async Task<dynamic> GetAllStaffTypesFromDb(int take, int skip, string name, bool alls, bool forDropdown)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@take", take, DbType.Int32);
            param.Add("@skip", skip, DbType.Int32);
            param.Add("@name", name, DbType.String);
            param.Add("@alls", alls, DbType.Boolean);

            if (forDropdown)
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

                        var data = result.Read<dynamic>().Select(_mappingService.MapStaffTypeList).OfType<StaffTypeDropdownItemResponse>().ToList();
                        return data;
                    },
                    _appSettings,
                    TimeSpan.FromMinutes(1)
                );
            }
            else
            {
                var result = await dbConnection.QueryMultipleAsync("100_GetStaffTypes", param, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var data = result.Read<dynamic>().Select(_mappingService.MapStaffTypeList).OfType<StaffTypeDropdownItemResponse>().ToList();
                var count = result.Read<int>().FirstOrDefault();

                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener los tipos de staff. Parámetros: take={take}, skip={skip}, name={name}, alls={alls}, forDropdown={forDropdown}", ex);
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
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al insertar el tipo de staff. Name={staffTypeRequest.Name}, NameEn={staffTypeRequest.NameEn}, DisplayOrder={staffTypeRequest.DisplayOrder}", ex);
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
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al actualizar el tipo de staff con ID {staffTypeRequest.Id}. Name={staffTypeRequest.Name}, NameEn={staffTypeRequest.NameEn}, DisplayOrder={staffTypeRequest.DisplayOrder}, IsActive={staffTypeRequest.IsActive}", ex);
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
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al eliminar el tipo de staff con ID {id}", ex);
        }
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
        catch
        {
            // No relanzar para no alterar el flujo si falla la invalidación de caché
        }
    }
}