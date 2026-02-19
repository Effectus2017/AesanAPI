using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Api.Repositories;

public class EmailLogRepository(DapperContext context, ILogger<EmailLogRepository> logger) : IEmailLogRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<EmailLogRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Inserta un nuevo registro de envío de correo electrónico
    /// </summary>
    public async Task<int> InsertEmailLog(EmailLogRequest request)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@recipientemail", request.RecipientEmail, DbType.String);
            parameters.Add("@subject", request.Subject, DbType.String);
            parameters.Add("@emailtype", request.EmailType, DbType.String);
            parameters.Add("@status", request.Status, DbType.String);
            parameters.Add("@errormessage", request.ErrorMessage, DbType.String);
            parameters.Add("@sentat", request.SentAt, DbType.DateTime);
            parameters.Add("@attemptedat", request.AttemptedAt, DbType.DateTime);
            parameters.Add("@userid", request.UserId, DbType.String);
            parameters.Add("@agencyid", request.AgencyId, DbType.Int32);
            parameters.Add("@emailtemplatekey", request.EmailTemplateKey, DbType.String);
            parameters.Add("@retrycount", request.RetryCount, DbType.Int32);
            parameters.Add("@originalemaillogid", request.OriginalEmailLogId, DbType.Int32);
            parameters.Add("@createdby", request.CreatedBy, DbType.String);

            var result = await db.QueryFirstOrDefaultAsync<dynamic>(
                "100_InsertEmailLog",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result != null && result.id != null)
            {
                return (int)result.id;
            }

            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar log de correo electrónico");
            throw new Exception($"Error al insertar log de correo electrónico: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los logs de correo electrónico de un usuario específico
    /// </summary>
    public async Task<List<DTOEmailLog>> GetEmailLogsByUserId(string userId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userid", userId, DbType.String);

            var result = await db.QueryAsync<DTOEmailLog>(
                "100_GetEmailLogsByUserId",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener logs de correo por usuario {userId}");
            throw new Exception($"Error al obtener logs de correo por usuario: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los logs de correo electrónico para un email específico
    /// </summary>
    public async Task<List<DTOEmailLog>> GetEmailLogsByEmail(string email)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@email", email, DbType.String);

            var result = await db.QueryAsync<DTOEmailLog>(
                "100_GetEmailLogsByEmail",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener logs de correo por email {email}");
            throw new Exception($"Error al obtener logs de correo por email: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene un log de correo electrónico específico por su ID
    /// </summary>
    public async Task<DTOEmailLog?> GetEmailLogById(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await db.QueryFirstOrDefaultAsync<DTOEmailLog>(
                "100_GetEmailLogById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener log de correo por ID {id}");
            throw new Exception($"Error al obtener log de correo por ID: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza el estado de un log de correo electrónico
    /// </summary>
    public async Task<bool> UpdateEmailLogStatus(int id, string status, string? errorMessage = null)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@status", status, DbType.String);
            parameters.Add("@errormessage", errorMessage, DbType.String);
            parameters.Add("@sentat", status == "Sent" ? DateTime.Now : (DateTime?)null, DbType.DateTime);

            var result = await db.QueryFirstOrDefaultAsync<dynamic>(
                "100_UpdateEmailLogStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result != null && result.rowsAffected != null)
            {
                return (int)result.rowsAffected > 0;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al actualizar estado del log de correo {id}");
            throw new Exception($"Error al actualizar estado del log de correo: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene los logs de correos fallidos para un email específico (o todos si email es null)
    /// </summary>
    public async Task<List<DTOEmailLog>> GetFailedEmailLogs(string? email = null)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@email", email, DbType.String);

            var result = await db.QueryAsync<DTOEmailLog>(
                "100_GetFailedEmailLogs",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener logs de correos fallidos");
            throw new Exception($"Error al obtener logs de correos fallidos: {ex.Message}", ex);
        }
    }
}
