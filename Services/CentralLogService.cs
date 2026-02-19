using Api.Interfaces;
using Api.Models.Request;

namespace Api.Services;

/// <summary>
/// Implementación del servicio central de logging.
/// Delega en IAuditLogger, IEmailLogRepository, INotificationMailJobLogRepository e ILogApplicationRepository.
/// </summary>
public class CentralLogService(
    IAuditLogger auditLogger,
    IEmailLogRepository emailLogRepository,
    INotificationMailJobLogRepository notificationMailJobLogRepository,
    ILogApplicationRepository logApplicationRepository) : ICentralLogService
{
    private readonly IAuditLogger _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
    private readonly IEmailLogRepository _emailLogRepository = emailLogRepository ?? throw new ArgumentNullException(nameof(emailLogRepository));
    private readonly INotificationMailJobLogRepository _notificationMailJobLogRepository = notificationMailJobLogRepository ?? throw new ArgumentNullException(nameof(notificationMailJobLogRepository));
    private readonly ILogApplicationRepository _logApplicationRepository = logApplicationRepository ?? throw new ArgumentNullException(nameof(logApplicationRepository));

    public Task LogAuditChangeAsync(string tableName, string entityId, string action, string userId,
        object? oldEntity = null, object? newEntity = null, string? reason = null,
        string? businessContext = null, Dictionary<string, object>? metadata = null)
    {
        return _auditLogger.LogChangeAsync(tableName, entityId, action, userId, oldEntity, newEntity, reason, businessContext, metadata);
    }

    public Task<Guid> LogAuditStartBulkAsync(string operationType, string tableName, string userId,
        string? description = null, string? sourceSystem = null)
    {
        return _auditLogger.StartBulkOperationAsync(operationType, tableName, userId, description, sourceSystem);
    }

    public Task LogAuditCompleteBulkAsync(Guid operationId, int totalRecords, int successfulRecords, int failedRecords)
    {
        return _auditLogger.CompleteBulkOperationAsync(operationId, totalRecords, successfulRecords, failedRecords);
    }

    public async Task<int> LogEmailAsync(EmailLogRequest request)
    {
        return await _emailLogRepository.InsertEmailLog(request);
    }

    public async Task<int> LogJobAsync(NotificationMailJobLogRequest request, CancellationToken cancellationToken = default)
    {
        return await _notificationMailJobLogRepository.InsertAsync(request, cancellationToken);
    }

    public async Task<long> LogApplicationAsync(string level, string message, string? payloadJson = null, string? userId = null, string? status = null, CancellationToken cancellationToken = default)
    {
        var request = new LogApplicationRequest
        {
            Category = "Application",
            Level = level,
            Message = message,
            Payload = payloadJson,
            UserId = userId,
            Status = status
        };
        return await _logApplicationRepository.InsertAsync(request, cancellationToken);
    }
}
