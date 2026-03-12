using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models.Request;
using Dapper;
using Api.Models.Errors;

namespace Api.Repositories;

public class LogApplicationRepository(DapperContext context) : ILogApplicationRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<long> InsertAsync(LogApplicationRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@category", request.Category, DbType.String);
            parameters.Add("@level", request.Level, DbType.String);
            parameters.Add("@message", request.Message, DbType.String);
            parameters.Add("@payload", request.Payload, DbType.String);
            parameters.Add("@userid", request.UserId, DbType.String);
            parameters.Add("@status", request.Status, DbType.String);

            var result = await db.QueryFirstOrDefaultAsync<dynamic>(
                new CommandDefinition(
                    "100_InsertLogApplication",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: cancellationToken));

            if (result != null && result.id != null)
            {
                return (long)result.id;
            }

            return 0;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al insertar LogApplication", ex);
        }
    }
}
