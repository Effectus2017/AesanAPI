using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Services;

/// <summary>
/// Servicio de consulta del centro de logs (solo lectura).
/// Ejecuta los SPs paginados por categoría y devuelve DTO unificado.
/// </summary>
public class LogsQueryService(DapperContext context) : ILogsQueryService
{
    private static readonly HashSet<string> ValidCategories = new(StringComparer.OrdinalIgnoreCase)
        { "Audit", "Email", "Job", "Application" };

    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<(IReadOnlyList<CentralLogEntryDto> Items, int TotalCount)> GetLogsPagedAsync(
        string category,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(category) || !ValidCategories.Contains(category))
        {
            return (Array.Empty<CentralLogEntryDto>(), 0);
        }

        var storedProcedure = category.ToLowerInvariant() switch
        {
            "audit" => "101_GetAuditTrailPaged",
            "email" => "101_GetEmailLogPaged",
            "job" => "101_GetNotificationMailJobLogPaged",
            "application" => "101_GetLogApplicationPaged",
            _ => null
        };

        if (storedProcedure == null)
        {
            return (Array.Empty<CentralLogEntryDto>(), 0);
        }

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 20;

        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@from", from, DbType.DateTime2);
        parameters.Add("@to", to, DbType.DateTime2);
        parameters.Add("@page", page, DbType.Int32);
        parameters.Add("@pagesize", pageSize, DbType.Int32);

        var rows = (await db.QueryAsync<CentralLogEntryDto>(
            storedProcedure,
            parameters,
            commandType: CommandType.StoredProcedure
        )).ToList();

        int totalCount = rows.Count > 0 && rows[0].TotalCount.HasValue ? rows[0].TotalCount.Value : 0;

        // No exponer TotalCount en los items que devolvemos al API
        foreach (var row in rows)
        {
            row.TotalCount = null;
        }

        return (rows, totalCount);
    }
}
