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
/// Repositorio para el dashboard AESAN
/// </summary>
public class AesanDashboardRepository(DapperContext context, ILoggingService loggingService, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IAesanDashboardRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILoggingService _logger = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Obtiene las métricas del dashboard AESAN
    /// </summary>
    /// <param name="userId">ID del usuario (opcional)</param>
    /// <returns>Métricas del dashboard</returns>
    public async Task<AesanDashboardResponse> GetDashboardMetrics(string userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String, ParameterDirection.Input);

            var result = await dbConnection.QueryFirstOrDefaultAsync<dynamic>("100_GetAesanDashboardMetrics", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                var error = new Exception($"No se encontraron métricas del dashboard para el usuario: {userId}");
                await _logger.LogError(error, "No se encontraron métricas del dashboard");
                return new AesanDashboardResponse();
            }

            // Mapear el resultado a AesanDashboardResponse
            var dashboardResponse = new AesanDashboardResponse
            {
                AgencyStatusCounts = new AgencyStatusCounts
                {
                    PendingValidationCount = result.PendingValidationCount ?? 0,
                    OrientationCount = result.OrientationCount ?? 0,
                    ApprovedCount = result.ApprovedCount ?? 0,
                    RejectedCount = result.RejectedCount ?? 0
                },
                TotalAgencies = result.TotalAgenciesCount ?? 0,
                LastUpdated = result.LastUpdated ?? DateTime.UtcNow
            };

            return dashboardResponse;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener métricas del dashboard AESAN");
            throw;
        }
    }
}
