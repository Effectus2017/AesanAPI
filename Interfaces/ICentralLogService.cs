using Api.Models.Request;

namespace Api.Interfaces;

/// <summary>
/// Servicio central de logging del portal AESAN.
/// Siempre que se deba loguear nueva información se usa este servicio.
/// No se escriben logs en otras tablas ni archivos fuera de este sistema.
/// </summary>
public interface ICentralLogService
{
    /// <summary>
    /// Registra un cambio de auditoría (CRUD). Delega en IAuditLogger.
    /// </summary>
    Task LogAuditChangeAsync(string tableName, string entityId, string action, string userId,
        object? oldEntity = null, object? newEntity = null, string? reason = null,
        string? businessContext = null, Dictionary<string, object>? metadata = null);

    /// <summary>
    /// Inicia una operación masiva de auditoría. Delega en IAuditLogger.
    /// </summary>
    Task<Guid> LogAuditStartBulkAsync(string operationType, string tableName, string userId,
        string? description = null, string? sourceSystem = null);

    /// <summary>
    /// Completa una operación masiva de auditoría. Delega en IAuditLogger.
    /// </summary>
    Task LogAuditCompleteBulkAsync(Guid operationId, int totalRecords, int successfulRecords, int failedRecords);

    /// <summary>
    /// Registra un envío de correo. Delega en IEmailLogRepository.
    /// </summary>
    Task<int> LogEmailAsync(EmailLogRequest request);

    /// <summary>
    /// Registra una ejecución de job. Delega en INotificationMailJobLogRepository.
    /// </summary>
    Task<int> LogJobAsync(NotificationMailJobLogRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registra un error o evento de aplicación. Escribe en LogApplication.
    /// Sustituye a ELMAH para errores de aplicación.
    /// </summary>
    Task<long> LogApplicationAsync(string level, string message, string? payloadJson = null, string? userId = null, string? status = null, CancellationToken cancellationToken = default);
}
