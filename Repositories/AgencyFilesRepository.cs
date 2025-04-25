using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

/// <summary>
/// Repositorio para manejar los archivos de agencia
/// </summary>
/// <remarks>
/// Constructor del repositorio de archivos de agencia
/// </remarks>
public class AgencyFilesRepository(DapperContext context, ILogger<AgencyFilesRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IAgencyFilesRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<AgencyFilesRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene todos los archivos asociados a una agencia
    /// </summary>
    public async Task<dynamic> GetAgencyFiles(int agencyId, int take, int skip, string name = null, string documentType = null)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@documentType", documentType, DbType.String);
            parameters.Add("@name", name, DbType.String);

            var result = await db.QueryMultipleAsync("100_GetAgencyFiles", parameters, commandType: CommandType.StoredProcedure);

            var data = await result.ReadAsync<DTOAgencyFile>();
            var count = await result.ReadSingleAsync<int>();

            // Construir las URLs completas para cada archivo
            foreach (var file in data)
            {
                if (!string.IsNullOrEmpty(file.FileUrl))
                {
                    string baseUrl = Utilities.GetUrl(_appSettings);
                    _logger.LogInformation("URL Base obtenida: {BaseUrl}", baseUrl);
                    _logger.LogInformation("Configuración actual: {@AppSettings}", _appSettings);
                    file.FileUrl = $"{baseUrl.TrimEnd('/')}/{file.FileUrl}";
                }
            }

            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los archivos de la agencia {AgencyId}", agencyId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene un archivo específico por su ID
    /// </summary>
    public async Task<DTOAgencyFile> GetAgencyFileById(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await db.QueryFirstOrDefaultAsync<DTOAgencyFile>(
                "100_GetAgencyFileById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Construir la URL completa del archivo
            if (result != null && !string.IsNullOrEmpty(result.FileUrl))
            {
                string baseUrl = Utilities.GetUrl(_appSettings);
                result.FileUrl = $"{baseUrl.TrimEnd('/')}/{result.FileUrl}";
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el archivo con ID {FileId}", id);
            throw;
        }
    }

    /// <summary>
    /// Agrega un nuevo archivo a una agencia
    /// </summary>
    public async Task<int> AddAgencyFile(AgencyFileRequest request)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", request.AgencyId, DbType.Int32);
            parameters.Add("@fileName", request.FileName, DbType.String);
            parameters.Add("@storedFileName", request.StoredFileName, DbType.String);
            parameters.Add("@fileUrl", request.FileUrl, DbType.String);
            parameters.Add("@contentType", request.ContentType, DbType.String);
            parameters.Add("@fileSize", request.FileSize, DbType.Int64);
            parameters.Add("@description", request.Description, DbType.String);
            parameters.Add("@documentType", request.DocumentType, DbType.String);
            parameters.Add("@uploadedBy", request.UploadedBy, DbType.String);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync(
                "100_InsertAgencyFile",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            int newFileId = parameters.Get<int>("@id");

            // Invalidar caché si es necesario
            InvalidateCache(request.AgencyId);

            return newFileId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar archivo a la agencia {AgencyId}", request.AgencyId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza la información de un archivo
    /// </summary>
    public async Task<bool> UpdateAgencyFile(int id, string description = null, string documentType = null)
    {
        try
        {
            // Primero obtenemos el archivo para saber a qué agencia pertenece
            var existingFile = await GetAgencyFileById(id);
            if (existingFile == null)
            {
                return false;
            }

            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@description", description, DbType.String);
            parameters.Add("@documentType", documentType, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_UpdateAgencyFile",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            int rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(existingFile.AgencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el archivo con ID {FileId}", id);
            throw;
        }
    }

    /// <summary>
    /// Elimina lógicamente un archivo (lo marca como inactivo)
    /// </summary>
    public async Task<bool> DeleteAgencyFile(int id)
    {
        try
        {
            // Primero obtenemos el archivo para saber a qué agencia pertenece
            var existingFile = await GetAgencyFileById(id);
            if (existingFile == null)
            {
                return false;
            }

            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_DeleteAgencyFile",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            int rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(existingFile.AgencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el archivo con ID {FileId}", id);
            throw;
        }
    }

    /// <summary>
    /// Invalida la caché relacionada con los archivos de una agencia
    /// </summary>
    private void InvalidateCache(int agencyId)
    {
        // Si se implementa caché para este repositorio, aquí se invalidaría
        _logger.LogInformation("Invalidando caché para archivos de la agencia {AgencyId}", agencyId);
    }

    /// <summary>
    /// Verifica un archivo de agencia
    /// </summary>
    public async Task<bool> VerifyAgencyFile(int id)
    {
        try
        {
            // Primero obtenemos el archivo para saber a qué agencia pertenece
            var existingFile = await GetAgencyFileById(id);
            if (existingFile == null)
            {
                return false;
            }

            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "100_VerifyAgencyFile",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            int rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(existingFile.AgencyId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar el archivo con ID {FileId}", id);
            throw;
        }
    }
}