using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models.Response;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Api.Repositories;

/// <summary>
/// Repositorio para tipos de servicio por programa (AESAN-257).
/// </summary>
public class ServiceTypeRepository(DapperContext context, ILogger<ServiceTypeRepository> logger) : IServiceTypeRepository
{
    private readonly DapperContext _context = context;
    private readonly ILogger<ServiceTypeRepository> _logger = logger;

    /// <inheritdoc />
    public async Task<IEnumerable<ServiceTypeByProgramResponse>> GetServiceTypesByProgram(int programId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@programid", programId, DbType.Int32);

            var result = await db.QueryAsync<ServiceTypeByProgramResponse>(
                "100_GetServiceTypesByProgram",
                parameters,
                commandType: CommandType.StoredProcedure);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tipos de servicio para el programa {ProgramId}", programId);
            throw;
        }
    }
}
