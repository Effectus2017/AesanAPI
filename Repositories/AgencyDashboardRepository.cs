using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Api.Models.Response;
using Api.Services;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

/// <summary>
/// Repositorio para el dashboard de agencia
/// </summary>
public class AgencyDashboardRepository(DapperContext context, ILoggingService loggingService, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IAgencyDashboardRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILoggingService _logger = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene las métricas del dashboard de agencia
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>Métricas del dashboard</returns>
    public async Task<AgencyDashboardResponse> GetDashboardMetrics(int agencyId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@agencyId", agencyId, DbType.Int32, ParameterDirection.Input);

            var result = await dbConnection.QueryFirstOrDefaultAsync<dynamic>("100_GetAgencyDashboardMetrics", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                var error = new Exception($"No se encontraron métricas del dashboard para la agencia: {agencyId}");
                await _logger.LogError(error, "No se encontraron métricas del dashboard");
                return new AgencyDashboardResponse();
            }

            // Mapear el resultado a AgencyDashboardResponse
            var dashboardResponse = new AgencyDashboardResponse
            {
                TotalSites = result.TotalSites ?? 0,
                TotalSchools = result.TotalSchools ?? 0,
                LastUpdated = result.LastUpdated ?? DateTime.UtcNow
            };

            return dashboardResponse;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener métricas del dashboard de agencia");
            throw;
        }
    }
}

