using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Api.Repositories;

public class AgencyStatusHistoryRepository(DapperContext context, ILogger<AgencyStatusHistoryRepository> logger) : IAgencyStatusHistoryRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<AgencyStatusHistoryRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<(IReadOnlyList<DTOAgencyStatusHistory> Items, int TotalCount)> GetAgencyStatusHistoryPagedAsync(
        int agencyId,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 20;

            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@from", from, DbType.DateTime2);
            parameters.Add("@to", to, DbType.DateTime2);
            parameters.Add("@page", page, DbType.Int32);
            parameters.Add("@pagesize", pageSize, DbType.Int32);

            var rows = (await db.QueryAsync<DTOAgencyStatusHistory>(
                "100_GetAgencyStatusHistoryPaged",
                parameters,
                commandType: CommandType.StoredProcedure)).ToList();

            int totalCount = rows.Count > 0 && rows[0].TotalCount.HasValue ? rows[0].TotalCount.Value : 0;
            foreach (var row in rows)
            {
                row.TotalCount = null;
            }

            return (rows, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de estados de agencia {AgencyId}", agencyId);
            throw;
        }
    }
}
