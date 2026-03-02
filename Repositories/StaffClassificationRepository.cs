using System.Data;
using Api.Data;
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

public class StaffClassificationRepository(DapperContext context, ILogger<StaffClassificationRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : IStaffClassificationRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<StaffClassificationRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Obtiene una clasificación de staff por su ID
    /// </summary>
    /// <param name="id">El ID de la clasificación de staff</param>
    /// <returns>La clasificación de staff</returns>
    public async Task<dynamic> GetStaffClassificationById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QueryFirstOrDefaultAsync<DTOStaffClassification>("100_GetStaffClassificationById", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la clasificación de staff con ID {StaffClassificationId}", id);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todas las clasificaciones de staff de la base de datos
    /// </summary>
    /// <param name="take">El número de clasificaciones a obtener</param>
    /// <param name="skip">El número de clasificaciones a saltar</param>
    /// <param name="name">El nombre de la clasificación a buscar</param>
    /// <param name="alls">Si se deben obtener todas las clasificaciones</param>
    /// <param name="isList">Si es para lista simple (dropdown)</param>
    /// <returns>Las clasificaciones de staff</returns>
    public async Task<dynamic> GetAllStaffClassificationsFromDb(int take, int skip, string name, bool alls, bool isList)
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
                var result = await dbConnection.QueryMultipleAsync("100_GetAllStaffClassifications", param, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return new List<dynamic>();
                }

                var data = result.Read<dynamic>().Select(_mappingService.MapStaffClassificationList).ToList();
                return data;
            }
            else
            {
                var result = await dbConnection.QueryMultipleAsync("100_GetAllStaffClassifications", param, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null;
                }

                var data = result.Read<dynamic>().Select(_mappingService.MapStaffClassificationList).ToList();
                var count = result.Read<int>().FirstOrDefault();

                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las clasificaciones de staff. Parámetros: take={Take}, skip={Skip}, name={Name}, alls={Alls}, isList={IsList}",
                take, skip, name, alls, isList);
            throw; // Preservar la excepción original con toda la información
        }
    }

    /// <summary>
    /// Inserta una nueva clasificación de staff en la base de datos
    /// </summary>
    /// <param name="staffClassificationRequest">Datos de la clasificación de staff a insertar</param>
    /// <returns>True si se insertó correctamente</returns>
    public async Task<bool> InsertStaffClassification(StaffClassificationRequest staffClassificationRequest)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@name", staffClassificationRequest.Name, DbType.String);
            param.Add("@nameEn", staffClassificationRequest.NameEn, DbType.String);
            param.Add("@sortOrder", staffClassificationRequest.SortOrder, DbType.Int32);
            param.Add("@isActive", staffClassificationRequest.IsActive, DbType.Boolean);

            var result = await dbConnection.ExecuteAsync("100_InsertStaffClassification", param, commandType: CommandType.StoredProcedure);

            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la clasificación de staff");
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Actualiza una clasificación de staff existente en la base de datos
    /// </summary>
    /// <param name="staffClassificationRequest">Datos de la clasificación de staff a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateStaffClassification(StaffClassificationRequest staffClassificationRequest)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", staffClassificationRequest.Id, DbType.Int32);
            param.Add("@name", staffClassificationRequest.Name, DbType.String);
            param.Add("@nameEn", staffClassificationRequest.NameEn, DbType.String);
            param.Add("@sortOrder", staffClassificationRequest.SortOrder, DbType.Int32);
            param.Add("@isActive", staffClassificationRequest.IsActive, DbType.Boolean);

            var result = await dbConnection.ExecuteAsync("100_UpdateStaffClassification", param, commandType: CommandType.StoredProcedure);

            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la clasificación de staff con ID {StaffClassificationId}", staffClassificationRequest.Id);
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Elimina una clasificación de staff de la base de datos (baja lógica)
    /// </summary>
    /// <param name="id">El ID de la clasificación de staff a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    public async Task<bool> DeleteStaffClassification(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", id, DbType.Int32);

            var result = await dbConnection.ExecuteAsync("100_DeleteStaffClassification", param, commandType: CommandType.StoredProcedure);

            return result > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la clasificación de staff con ID {StaffClassificationId}", id);
            throw new Exception(ex.Message);
        }
    }

}