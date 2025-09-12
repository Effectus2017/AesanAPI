namespace Api.Interfaces;

/// <summary>
/// Interfaz para el logger de auditoría que registra cambios en las operaciones CRUD
/// </summary>
public interface IAuditLogger
{
    /// <summary>
    /// Registra un cambio individual en la auditoría
    /// </summary>
    /// <param name="tableName">Nombre de la tabla afectada</param>
    /// <param name="entityId">ID de la entidad afectada</param>
    /// <param name="action">Acción realizada (INSERT, UPDATE, DELETE)</param>
    /// <param name="userId">ID del usuario que realizó la acción</param>
    /// <param name="oldEntity">Entidad anterior (para UPDATE/DELETE)</param>
    /// <param name="newEntity">Entidad nueva (para INSERT/UPDATE)</param>
    /// <param name="reason">Razón del cambio</param>
    /// <param name="businessContext">Contexto de negocio</param>
    /// <param name="metadata">Metadatos adicionales</param>
    Task LogChangeAsync(string tableName, string entityId, string action, string userId,
        object? oldEntity = null, object? newEntity = null, string? reason = null,
        string? businessContext = null, Dictionary<string, object>? metadata = null);

    /// <summary>
    /// Inicia una operación masiva y retorna el OperationId
    /// </summary>
    /// <param name="operationType">Tipo de operación masiva</param>
    /// <param name="tableName">Nombre de la tabla afectada</param>
    /// <param name="userId">ID del usuario que realiza la operación</param>
    /// <param name="description">Descripción de la operación</param>
    /// <param name="sourceSystem">Sistema origen</param>
    Task<Guid> StartBulkOperationAsync(string operationType, string tableName, string userId,
        string? description = null, string? sourceSystem = null);

    /// <summary>
    /// Completa una operación masiva con estadísticas
    /// </summary>
    /// <param name="operationId">ID de la operación</param>
    /// <param name="totalRecords">Total de registros procesados</param>
    /// <param name="successfulRecords">Registros exitosos</param>
    /// <param name="failedRecords">Registros fallidos</param>
    Task CompleteBulkOperationAsync(Guid operationId, int totalRecords, int successfulRecords, int failedRecords);
}
