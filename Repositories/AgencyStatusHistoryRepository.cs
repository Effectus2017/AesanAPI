using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Api.Models.Response;
using Api.Services;
using Dapper;

namespace Api.Repositories;

/// <summary>
/// Repositorio para el historial de estados de agencia.
/// </summary>
public class AgencyStatusHistoryRepository(DapperContext context, ILoggingService loggingService) : IAgencyStatusHistoryRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILoggingService _logger = loggingService ?? throw new ArgumentNullException(nameof(loggingService));

    /// <summary>
    /// Obtiene el historial de estados de agencia paginado.
    /// </summary>
    /// <param name="take">Número de registros a tomar.</param>
    /// <param name="skip">Número de registros a saltar.</param>
    /// <param name="agencyId">ID de la agencia (opcional).</param>
    /// <param name="from">Fecha desde (opcional).</param>
    /// <param name="to">Fecha hasta (opcional).</param>
    /// <returns>Lista de historial y total de registros.</returns>
    public async Task<dynamic> GetAgencyStatusHistoryPaged(int take, int skip, int? agencyId, DateTime? from, DateTime? to)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@agencyid", agencyId, DbType.Int32);
            parameters.Add("@from", from, DbType.DateTime2);
            parameters.Add("@to", to, DbType.DateTime2);

            var result = await db.QueryMultipleAsync("100_GetAllAgencyStatusHistory", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return new { data = new List<AgencyStatusHistoryResponse>(), count = 0 };
            }

            var data = result.Read<AgencyStatusHistoryResponse>().ToList();
            var count = result.ReadFirstOrDefault<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener historial de estados de agencia");
            throw;
        }
    }
}
