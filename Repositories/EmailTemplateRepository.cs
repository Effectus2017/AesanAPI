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
/// Repository for managing email templates in the database
/// Repositorio para gestionar templates de email en la base de datos
/// </summary>
public class EmailTemplateRepository(
    DapperContext context,
    ILogger<EmailTemplateRepository> logger,
    IMemoryCache cache,
    IOptions<ApplicationSettings> appSettings) : IEmailTemplateRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<EmailTemplateRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Gets a single email template by its ID
    /// Obtiene un template de email por su ID
    /// </summary>
    /// <param name="id">The ID of the email template to retrieve/El ID del template de email a recuperar</param>
    /// <returns>The email template data/Los datos del template de email</returns>
    public async Task<EmailTemplateResponse?> GetEmailTemplateById(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryFirstOrDefaultAsync<EmailTemplateResponse>(
                "100_GetEmailTemplateById",
                parameters,
                commandType: CommandType.StoredProcedure);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el template de email por ID/Error getting email template by ID");
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
        finally
        {
            _logger.LogInformation("Fin de la ejecución de GetEmailTemplateById");
        }
    }

    /// <summary>
    /// Gets an email template by its template key
    /// Obtiene un template de email por su clave
    /// </summary>
    /// <param name="templateKey">The template key to filter by/La clave del template para filtrar</param>
    /// <returns>Email template matching the key/Template de email que coincide con la clave</returns>
    public async Task<EmailTemplateResponse?> GetEmailTemplateByKey(string templateKey)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@templateKey", templateKey, DbType.String);
            var result = await db.QueryFirstOrDefaultAsync<EmailTemplateResponse>(
                "100_GetEmailTemplateByKey",
                parameters,
                commandType: CommandType.StoredProcedure);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener template de email por clave: {TemplateKey}", templateKey);
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Gets all email templates with pagination and filtering
    /// Obtiene todos los templates de email con paginación y filtrado
    /// </summary>
    /// <param name="take">Number of records to take/Número de registros a tomar</param>
    /// <param name="skip">Number of records to skip/Número de registros a omitir</param>
    /// <param name="templateKey">Template key filter (optional)/Filtro de clave de template (opcional)</param>
    /// <param name="description">Description filter (optional)/Filtro de descripción (opcional)</param>
    /// <param name="alls">Whether to get all records ignoring pagination/Si obtener todos los registros ignorando la paginación</param>
    /// <returns>Object containing the list of email templates and total count/Objeto que contiene la lista de templates de email y el conteo total</returns>
    public async Task<dynamic> GetAllEmailTemplates(int take, int skip, string? templateKey, string? description, bool alls)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@templateKey", templateKey, DbType.String);
            parameters.Add("@description", description, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);

            using var result = await db.QueryMultipleAsync("100_GetAllEmailTemplates", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var data = result.Read<EmailTemplateResponse>().ToList();
            var count = result.ReadFirstOrDefault<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los templates de email/Error getting email templates");
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Inserts a new email template into the database
    /// Inserta un nuevo template de email en la base de datos
    /// </summary>
    /// <param name="emailTemplate">The email template data to insert/Los datos del template de email a insertar</param>
    /// <param name="createdBy">User ID who creates the template/ID del usuario que crea el template</param>
    /// <returns>True if insertion was successful/Verdadero si la inserción fue exitosa</returns>
    public async Task<bool> InsertEmailTemplate(EmailTemplateRequest emailTemplate, string? createdBy = null)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@templateKey", emailTemplate.TemplateKey, DbType.String);
            parameters.Add("@subjectES", emailTemplate.SubjectES, DbType.String);
            parameters.Add("@subjectEN", emailTemplate.SubjectEN, DbType.String);
            parameters.Add("@bodyES", emailTemplate.BodyES, DbType.String);
            parameters.Add("@bodyEN", emailTemplate.BodyEN, DbType.String);
            parameters.Add("@description", emailTemplate.Description, DbType.String);
            parameters.Add("@descriptionEN", emailTemplate.DescriptionEN, DbType.String);
            parameters.Add("@isActive", emailTemplate.IsActive, DbType.Boolean);
            parameters.Add("@createdBy", createdBy, DbType.String);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertEmailTemplate", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");

            if (id > 0)
            {
                InvalidateCache();
            }

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el template de email/Error inserting email template");
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Updates an existing email template
    /// Actualiza un template de email existente
    /// </summary>
    /// <param name="emailTemplate">The email template data to update/Los datos del template de email a actualizar</param>
    /// <param name="updatedBy">User ID who updates the template/ID del usuario que actualiza el template</param>
    /// <returns>True if update was successful/Verdadero si la actualización fue exitosa</returns>
    public async Task<bool> UpdateEmailTemplate(EmailTemplateRequest emailTemplate, string? updatedBy = null)
    {
        try
        {
            if (emailTemplate.Id == null || emailTemplate.Id == 0)
            {
                _logger.LogWarning("Intento de actualizar template sin ID");
                return false;
            }

            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", emailTemplate.Id, DbType.Int32);
            parameters.Add("@subjectES", emailTemplate.SubjectES, DbType.String);
            parameters.Add("@subjectEN", emailTemplate.SubjectEN, DbType.String);
            parameters.Add("@bodyES", emailTemplate.BodyES, DbType.String);
            parameters.Add("@bodyEN", emailTemplate.BodyEN, DbType.String);
            parameters.Add("@description", emailTemplate.Description, DbType.String);
            parameters.Add("@descriptionEN", emailTemplate.DescriptionEN, DbType.String);
            parameters.Add("@isActive", emailTemplate.IsActive, DbType.Boolean);
            parameters.Add("@updatedBy", updatedBy, DbType.String);
            var rowsAffected = await db.ExecuteAsync("100_UpdateEmailTemplate", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                InvalidateCache();
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el template de email/Error updating email template");
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
    }

    /// <summary>
    /// Invalidates the cache for email templates
    /// Invalida el cache para templates de email
    /// </summary>
    private void InvalidateCache()
    {
        _cache.Remove("EmailTemplates");
        // Invalidar todos los caches por key usando patrón
        _cache.RemoveByPattern("EmailTemplate_", _logger);
        _logger.LogInformation("Cache invalidado para EmailTemplate Repository");
    }

    /// <summary>
    /// Invalidates the cache for a specific email template by key
    /// Invalida el cache para un template de email específico por su clave
    /// </summary>
    /// <param name="templateKey">The template key to invalidate/La clave del template a invalidar</param>
    public void InvalidateCacheByKey(string templateKey)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.EmailTemplateByKey, templateKey);
        _cache.Remove(cacheKey);
        _logger.LogInformation("Cache invalidado para EmailTemplate con key: {TemplateKey}", templateKey);
    }
}

