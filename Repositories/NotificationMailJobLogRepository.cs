using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Errors;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Api.Repositories;

public class NotificationMailJobLogRepository(DapperContext context, ILogger<NotificationMailJobLogRepository> logger) : INotificationMailJobLogRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<NotificationMailJobLogRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<int> InsertAsync(NotificationMailJobLogRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@jobname", request.JobName, DbType.String);
            parameters.Add("@startedat", request.StartedAt, DbType.DateTime2);
            parameters.Add("@finishedat", request.FinishedAt, DbType.DateTime2);
            parameters.Add("@status", request.Status, DbType.String);
            parameters.Add("@message", request.Message, DbType.String);

            var result = await db.QueryFirstOrDefaultAsync<dynamic>(
                new CommandDefinition(
                    "100_InsertNotificationMailJobLog",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));

            if (result != null && result.id != null)
            {
                return (int)result.id;
            }

            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar NotificationMailJobLog");
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al insertar NotificationMailJobLog", ex);
        }
    }
}
