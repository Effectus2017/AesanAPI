using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Api.Models.Errors;

namespace Api.Repositories;

/// <summary>
/// Repository for managing message templates in the database
/// Repositorio para gestionar templates de mensajes en la base de datos
/// </summary>
public class MessageTemplateRepository(
    DapperContext context,
    ILogger<MessageTemplateRepository> logger,
    IMemoryCache cache,
    IOptions<ApplicationSettings> appSettings) : IMessageTemplateRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<MessageTemplateRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Gets a single message template by its ID
    /// Obtiene un template de mensaje por su ID
    /// </summary>
    public async Task<MessageTemplateResponse?> GetMessageTemplateById(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryFirstOrDefaultAsync<MessageTemplateResponse>(
                "100_GetMessageTemplateById",
                parameters,
                commandType: CommandType.StoredProcedure);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el template de mensaje por ID");
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Gets a message template by its template key
    /// Obtiene un template de mensaje por su clave
    /// </summary>
    public async Task<MessageTemplateResponse?> GetMessageTemplateByKey(string templateKey)
    {
        try
        {
            string cacheKey = string.Format(_appSettings.Cache.Keys.MessageTemplateByKey, templateKey);
            return await _cache.CacheQuery(
                cacheKey,
                async () =>
                {
                    using IDbConnection db = _context.CreateConnection();
                    var parameters = new DynamicParameters();
                    parameters.Add("@templateKey", templateKey, DbType.String);
                    var result = await db.QueryFirstOrDefaultAsync<MessageTemplateResponse>(
                        "100_GetMessageTemplateByKey",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                    return result;
                },
                _logger,
                _appSettings,
                TimeSpan.FromMinutes(30)
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener template de mensaje por clave: {TemplateKey}", templateKey);
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Gets all message templates with pagination and filtering
    /// Obtiene todos los templates de mensaje con paginación y filtrado
    /// </summary>
    public async Task<dynamic> GetAllMessageTemplates(int take, int skip, string? templateKey, string? description, bool alls)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@templateKey", templateKey, DbType.String);
            parameters.Add("@purpose", description, DbType.String); // El parámetro description se mapea a @purpose en el SP
            parameters.Add("@alls", alls, DbType.Boolean);

            using var result = await db.QueryMultipleAsync("100_GetAllMessageTemplates", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var data = result.Read<MessageTemplateResponse>().ToList();
            var count = result.ReadFirstOrDefault<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los templates de mensaje");
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Inserts a new message template into the database
    /// Inserta un nuevo template de mensaje en la base de datos
    /// </summary>
    public async Task<bool> InsertMessageTemplate(MessageTemplateRequest messageTemplate, string? createdBy = null)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@templateKey", messageTemplate.TemplateKey, DbType.String);
            parameters.Add("@titleES", messageTemplate.TitleES, DbType.String);
            parameters.Add("@titleEN", messageTemplate.TitleEN, DbType.String);
            parameters.Add("@bodyES", messageTemplate.BodyES, DbType.String);
            parameters.Add("@bodyEN", messageTemplate.BodyEN, DbType.String);
            parameters.Add("@icon", messageTemplate.Icon, DbType.String);
            parameters.Add("@image", messageTemplate.Image, DbType.String);
            parameters.Add("@link", messageTemplate.Link, DbType.String);
            parameters.Add("@useRouter", messageTemplate.UseRouter, DbType.Boolean);
            parameters.Add("@purposeES", messageTemplate.PurposeES, DbType.String);
            parameters.Add("@purposeEN", messageTemplate.PurposeEN, DbType.String);
            parameters.Add("@isActive", messageTemplate.IsActive, DbType.Boolean);
            parameters.Add("@createdBy", createdBy, DbType.String);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertMessageTemplate", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");

            if (id > 0)
            {
                InvalidateCache();
            }

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el template de mensaje");
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Updates an existing message template
    /// Actualiza un template de mensaje existente
    /// </summary>
    public async Task<bool> UpdateMessageTemplate(MessageTemplateRequest messageTemplate, string? updatedBy = null)
    {
        try
        {
            if (messageTemplate.Id == null || messageTemplate.Id == 0)
            {
                _logger.LogWarning("Intento de actualizar template sin ID");
                return false;
            }

            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", messageTemplate.Id, DbType.Int32);
            parameters.Add("@titleES", messageTemplate.TitleES, DbType.String);
            parameters.Add("@titleEN", messageTemplate.TitleEN, DbType.String);
            parameters.Add("@bodyES", messageTemplate.BodyES, DbType.String);
            parameters.Add("@bodyEN", messageTemplate.BodyEN, DbType.String);
            parameters.Add("@icon", messageTemplate.Icon, DbType.String);
            parameters.Add("@image", messageTemplate.Image, DbType.String);
            parameters.Add("@link", messageTemplate.Link, DbType.String);
            parameters.Add("@useRouter", messageTemplate.UseRouter, DbType.Boolean);
            parameters.Add("@purposeES", messageTemplate.PurposeES, DbType.String);
            parameters.Add("@purposeEN", messageTemplate.PurposeEN, DbType.String);
            parameters.Add("@isActive", messageTemplate.IsActive, DbType.Boolean);
            parameters.Add("@updatedBy", updatedBy, DbType.String);
            var rowsAffected = await db.ExecuteAsync("100_UpdateMessageTemplate", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                InvalidateCache();
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el template de mensaje");
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Invalidates the cache for message templates
    /// Invalida el cache para templates de mensaje
    /// </summary>
    private void InvalidateCache()
    {
        _cache.Remove("MessageTemplates");
        _logger.LogInformation("Cache invalidado para MessageTemplate Repository");
    }
}

