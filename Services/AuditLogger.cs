using Api.Data;
using Api.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Text.Json;

namespace Api.Services;

/// <summary>
/// Implementación del logger de auditoría que registra cambios en las operaciones CRUD
/// </summary>
public class AuditLogger : IAuditLogger
{
    private readonly DapperContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuditLogger> _logger;

    // Configuración JSON para serialización consistente
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public AuditLogger(DapperContext context, IHttpContextAccessor httpContextAccessor, ILogger<AuditLogger> logger)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task LogChangeAsync(string tableName, string entityId, string action, string userId,
        object? oldEntity = null, object? newEntity = null, string? reason = null,
        string? businessContext = null, Dictionary<string, object>? metadata = null)
    {
        try
        {
            // Validar que userId no esté vacío
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Intento de auditoría sin userId para tabla {TableName}", tableName);
                return;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var parameters = new DynamicParameters();

            parameters.Add("@TableName", tableName);
            parameters.Add("@EntityId", entityId);
            parameters.Add("@Action", action);
            parameters.Add("@ChangedBy", userId);
            parameters.Add("@OldValues", oldEntity != null ? JsonSerializer.Serialize(oldEntity, JsonOptions) : null);
            parameters.Add("@NewValues", newEntity != null ? JsonSerializer.Serialize(newEntity, JsonOptions) : null);
            parameters.Add("@ChangedFields", GetChangedFields(oldEntity, newEntity));
            parameters.Add("@Reason", reason);
            parameters.Add("@BusinessContext", businessContext);
            parameters.Add("@IPAddress", GetClientIP(httpContext));
            parameters.Add("@UserAgent", httpContext?.Request?.Headers["User-Agent"].ToString());
            parameters.Add("@SessionId", httpContext?.Session?.Id);
            parameters.Add("@RequestId", httpContext?.TraceIdentifier);
            parameters.Add("@Metadata", metadata != null ? JsonSerializer.Serialize(metadata, JsonOptions) : null);
            parameters.Add("@Tags", GenerateTags(action, businessContext, metadata));
            parameters.Add("@OperationId", dbType: DbType.Guid, direction: ParameterDirection.Output);

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("100_LogAuditChange", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogDebug("Auditoría registrada para {TableName} {EntityId} {Action} por usuario {UserId}",
                tableName, entityId, action, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging audit change for {TableName} entity {EntityId}", tableName, entityId);
        }
    }

    public async Task<Guid> StartBulkOperationAsync(string operationType, string tableName, string userId,
        string? description = null, string? sourceSystem = null)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("UserId es requerido para operaciones de auditoría", nameof(userId));
        }

        var parameters = new DynamicParameters();
        parameters.Add("@OperationType", operationType);
        parameters.Add("@TableName", tableName);
        parameters.Add("@ChangedBy", userId);
        parameters.Add("@Description", description);
        parameters.Add("@SourceSystem", sourceSystem ?? "WebPortal");
        parameters.Add("@OperationId", dbType: DbType.Guid, direction: ParameterDirection.Output);

        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync("101_StartBulkOperation", parameters, commandType: CommandType.StoredProcedure);

        var operationId = parameters.Get<Guid>("@OperationId");
        _logger.LogInformation("Operación masiva iniciada: {OperationType} en {TableName} por usuario {UserId} - OperationId: {OperationId}",
            operationType, tableName, userId, operationId);

        return operationId;
    }

    public async Task CompleteBulkOperationAsync(Guid operationId, int totalRecords, int successfulRecords, int failedRecords)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@OperationId", operationId);
            parameters.Add("@TotalRecords", totalRecords);
            parameters.Add("@SuccessfulRecords", successfulRecords);
            parameters.Add("@FailedRecords", failedRecords);
            parameters.Add("@Status", failedRecords > 0 ? "COMPLETED_WITH_ERRORS" : "COMPLETED");

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync("102_CompleteBulkOperation", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation("Operación masiva completada: {OperationId} - Total: {Total}, Exitosos: {Successful}, Fallidos: {Failed}",
                operationId, totalRecords, successfulRecords, failedRecords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completando operación masiva {OperationId}", operationId);
        }
    }

    private string? GetChangedFields(object? oldEntity, object? newEntity)
    {
        if (oldEntity == null || newEntity == null) return null;

        var changedFields = new List<string>();
        var properties = oldEntity.GetType().GetProperties();

        foreach (var prop in properties)
        {
            var oldValue = prop.GetValue(oldEntity);
            var newValue = prop.GetValue(newEntity);

            if (!Equals(oldValue, newValue))
            {
                changedFields.Add(prop.Name);
            }
        }

        return changedFields.Count > 0 ? JsonSerializer.Serialize(changedFields) : null;
    }

    private string? GenerateTags(string action, string? businessContext, Dictionary<string, object>? metadata)
    {
        var tags = new List<string> { action };

        if (!string.IsNullOrEmpty(businessContext))
            tags.Add(businessContext);

        if (metadata?.ContainsKey("priority") == true)
            tags.Add($"priority:{metadata["priority"]}");

        return tags.Count > 0 ? string.Join(",", tags) : null;
    }

    private string? GetClientIP(HttpContext? httpContext)
    {
        if (httpContext == null) return null;

        // Intentar obtener la IP real del cliente
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

        // Verificar si hay un proxy/load balancer
        if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim();
        }
        else if (httpContext.Request.Headers.ContainsKey("X-Real-IP"))
        {
            ipAddress = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        }

        return ipAddress;
    }
}
