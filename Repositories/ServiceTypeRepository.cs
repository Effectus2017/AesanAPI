using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models.Response;
using Dapper;
using Api.Models.Errors;

namespace Api.Repositories;

/// <summary>
/// Repositorio para tipos de servicio por programa (AESAN-257).
/// </summary>
public class ServiceTypeRepository(DapperContext context) : IServiceTypeRepository
{
    private readonly DapperContext _context = context;

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
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener tipos de servicio para el programa {programId}", ex);
        }
    }
}
