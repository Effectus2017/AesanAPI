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

/// <summary>
/// Repositorio para operaciones con School
/// </summary>
public class SchoolRepository(DapperContext context, ILogger<SchoolRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, MappingService mappingService) : ISchoolRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SchoolRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Obtiene una escuela por su ID
    /// </summary>
    public async Task<SchoolResponse> GetSchoolById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QueryFirstOrDefaultAsync<dynamic>("100_GetSchoolById", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null!;
            }

            return _mappingService.MapSchool(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la escuela por ID {Id}: {Message}", id, ex.Message);
            throw new Exception($"Error al obtener la escuela: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todas las escuelas con paginación y filtros
    /// </summary>
    public async Task<dynamic> GetAllSchools(int take, int skip, string? name, int? agencyId, bool alls)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@alls", alls, DbType.Boolean);

            using var result = await dbConnection.QueryMultipleAsync("100_GetAllSchools", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return new { data = Array.Empty<SchoolResponse>(), count = 0 };
            }

            var schools = result.Read<dynamic>().Select(_mappingService.MapSchool).ToList();
            var count = result.ReadFirstOrDefault<int>();

            return new { data = schools, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las escuelas: {Message}", ex.Message);
            throw new Exception($"Error al obtener las escuelas: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Inserta una nueva escuela
    /// </summary>
    public async Task<bool> InsertSchool(SchoolRequest request)
    {
        try
        {
            // Validaciones de negocio
            if (request.AgencyId <= 0)
            {
                throw new ArgumentException("El ID de la agencia debe ser válido");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("El nombre de la escuela es requerido");
            }

            if (request.Name.Length < 2)
            {
                throw new ArgumentException("El nombre de la escuela debe tener al menos 2 caracteres");
            }

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", request.AgencyId, DbType.Int32);
            parameters.Add("@name", request.Name.Trim(), DbType.String);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSchool", parameters, commandType: CommandType.StoredProcedure);

            int id = parameters.Get<int>("@id");

            InvalidateCache();
            _logger.LogInformation("Escuela creada exitosamente: ID {Id}, Nombre {Name}", id, request.Name);

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la escuela {Name} para la agencia {AgencyId}: {Message}", request.Name, request.AgencyId, ex.Message);
            throw new Exception($"Error al insertar la escuela: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza una escuela existente
    /// </summary>
    public async Task<bool> UpdateSchool(SchoolRequest request)
    {
        try
        {
            // Validaciones de negocio
            if (!request.Id.HasValue || request.Id.Value <= 0)
            {
                throw new ArgumentException("El ID de la escuela es requerido para actualizar");
            }

            if (request.AgencyId <= 0)
            {
                throw new ArgumentException("El ID de la agencia debe ser válido");
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new ArgumentException("El nombre de la escuela es requerido");
            }

            if (request.Name.Length < 2)
            {
                throw new ArgumentException("El nombre de la escuela debe tener al menos 2 caracteres");
            }

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", request.Id.Value, DbType.Int32);
            parameters.Add("@agencyId", request.AgencyId, DbType.Int32);
            parameters.Add("@name", request.Name.Trim(), DbType.String);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("100_UpdateSchool", parameters, commandType: CommandType.StoredProcedure);

            int rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected == 0)
            {
                _logger.LogWarning("No se encontró la escuela con ID {Id} para actualizar", request.Id.Value);
                throw new Exception("No se encontró la escuela para actualizar");
            }

            InvalidateCache();
            _logger.LogInformation("Escuela actualizada exitosamente: ID {Id}, Nombre {Name}", request.Id.Value, request.Name);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la escuela ID {Id}: {Message}", request.Id, ex.Message);
            throw new Exception($"Error al actualizar la escuela: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Elimina una escuela (soft delete)
    /// </summary>
    public async Task<bool> DeleteSchool(int id)
    {
        try
        {
            // Validaciones de negocio
            if (id <= 0)
            {
                throw new ArgumentException("El ID de la escuela debe ser válido");
            }

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("100_DeleteSchool", parameters, commandType: CommandType.StoredProcedure);

            int rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected == 0)
            {
                _logger.LogWarning("No se encontró la escuela con ID {Id} para eliminar", id);
                throw new Exception("No se encontró la escuela para eliminar");
            }

            InvalidateCache();
            _logger.LogInformation("Escuela eliminada exitosamente: ID {Id}", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la escuela ID {Id}: {Message}", id, ex.Message);
            throw new Exception($"Error al eliminar la escuela: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todas las escuelas de una agencia específica con paginación y filtros
    /// </summary>
    public async Task<dynamic> GetSchoolsByAgencyId(int agencyId, int take, int skip, string? name)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);

            using var result = await dbConnection.QueryMultipleAsync("100_GetSchoolsByAgencyId", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return new { data = Array.Empty<SchoolResponse>(), count = 0 };
            }

            var schools = result.Read<dynamic>().Select(_mappingService.MapSchool).ToList();
            var count = result.ReadFirstOrDefault<int>();

            // Debug: verificar que SitesCount se mapea correctamente
            _logger.LogInformation("Total schools retrieved: {Count}", schools.Count);

            return new { data = schools, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener escuelas por agencia {AgencyId}: {Message}", agencyId, ex.Message);
            throw new Exception($"Error al obtener las escuelas de la agencia: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Invalida la caché para las escuelas
    /// </summary>
    private void InvalidateCache()
    {
        _cache.Remove(_appSettings.Cache.Keys.Schools);
        _logger.LogInformation("Cache invalidado para School Repository");
    }
}
