using System.Data;
using Dapper;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.Extensions.Logging;

namespace Api.Repositories;

/// <summary>
/// Repositorio para la gestión de calendario de funcionamiento de sitios
/// Implementa las operaciones CRUD y específicas para días de funcionamiento
/// </summary>
public class SiteCalendarRepository(DapperContext context, ILogger<SiteCalendarRepository> logger, ISiteOperatingDayServiceRepository? serviceRepository = null) : ISiteCalendarRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SiteCalendarRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ISiteOperatingDayServiceRepository? _serviceRepository = serviceRepository;

    /// <summary>
    /// Obtiene todos los días de funcionamiento de un sitio específico
    /// Opcionalmente filtra por mes y año para mejorar el rendimiento
    /// </summary>
    public async Task<SiteCalendarResponse> GetOperatingDays(int siteId, int? month = null, int? year = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@month", month, DbType.Int32);
            parameters.Add("@year", year, DbType.Int32);

            using var multi = await dbConnection.QueryMultipleAsync("100_GetSiteOperatingDays", parameters, commandType: CommandType.StoredProcedure);

            // Obtener información del sitio
            var siteInfo = await multi.ReadFirstOrDefaultAsync<SiteInfoResponse>();
            if (siteInfo == null)
            {
                _logger.LogWarning("No se encontró el sitio con ID {SiteId}", siteId);
                return new SiteCalendarResponse
                {
                    SiteId = siteId,
                    SiteName = "Sitio no encontrado",
                    OperatingDays = new List<OperatingDayResponse>()
                };
            }

            // Obtener días de funcionamiento
            var operatingDays = await multi.ReadAsync<OperatingDayResponse>();
            var operatingDaysList = operatingDays.ToList();

            // Optimizar carga de servicios: cargar todos en una sola consulta batch
            if (_serviceRepository != null && operatingDaysList.Any())
            {
                var operatingDayIds = operatingDaysList.Select(d => d.Id).ToList();
                await LoadServicesBatch(dbConnection, operatingDaysList, operatingDayIds);
            }

            var response = new SiteCalendarResponse
            {
                SiteId = siteInfo.SiteId,
                SiteName = siteInfo.SiteName,
                OperatingDays = operatingDaysList
            };

            _logger.LogInformation("Se obtuvieron {Count} días de funcionamiento para el sitio {SiteId} (mes: {Month}, año: {Year})",
                operatingDaysList.Count, siteId, month?.ToString() ?? "todos", year?.ToString() ?? "todos");

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener días de funcionamiento para el sitio {SiteId}", siteId);
            throw;
        }
    }

    /// <summary>
    /// Carga servicios para múltiples días de funcionamiento en una sola consulta batch
    /// </summary>
    private async Task LoadServicesBatch(IDbConnection dbConnection, List<OperatingDayResponse> operatingDays, List<int> operatingDayIds)
    {
        try
        {
            // Convertir lista de IDs a string separado por comas
            var operatingDayIdsString = string.Join(",", operatingDayIds);

            var parameters = new DynamicParameters();
            parameters.Add("@operatingDayIds", operatingDayIdsString, DbType.String);

            var services = await dbConnection.QueryAsync<SiteOperatingDayServiceResponse>(
                "100_GetSiteOperatingDayServicesBatch",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            // Agrupar servicios por OperatingDayId usando un diccionario para mejor rendimiento
            var servicesByDayId = services
                .GroupBy(s => s.OperatingDayId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Asignar servicios a cada día de funcionamiento
            foreach (var day in operatingDays)
            {
                if (servicesByDayId.TryGetValue(day.Id, out var dayServices))
                {
                    day.Services = dayServices;
                }
                else
                {
                    day.Services = new List<SiteOperatingDayServiceResponse>();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al cargar servicios en batch para {Count} días de funcionamiento", operatingDayIds.Count);
            // En caso de error, inicializar lista vacía para cada día
            foreach (var day in operatingDays)
            {
                day.Services = new List<SiteOperatingDayServiceResponse>();
            }
        }
    }

    /// <summary>
    /// Alterna el estado de funcionamiento de un día específico
    /// </summary>
    public async Task<bool> ToggleOperatingDay(SiteOperatingDayRequest request)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@siteId", request.SiteId, DbType.Int32);
            parameters.Add("@operatingDate", request.OperatingDate.Date, DbType.Date);
            parameters.Add("@startTime", request.StartTime, DbType.Time);
            parameters.Add("@endTime", request.EndTime, DbType.Time);
            parameters.Add("@isWeekendOverride", request.IsWeekendOverride, DbType.Boolean);
            parameters.Add("@isExcluded", request.IsExcluded, DbType.Boolean);
            parameters.Add("@isHoliday", request.IsHoliday, DbType.Boolean);
            parameters.Add("@comment", request.Comment, DbType.String);

            var result = await dbConnection.QuerySingleAsync<int>("100_ToggleSiteOperatingDay", parameters, commandType: CommandType.StoredProcedure);

            var success = result > 0;

            _logger.LogInformation("Toggle de día de funcionamiento para sitio {SiteId} en fecha {Date}: {Success}",
                request.SiteId, request.OperatingDate.Date, success ? "Exitoso" : "Fallido");

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al alternar día de funcionamiento para el sitio {SiteId} en fecha {Date}",
                request.SiteId, request.OperatingDate.Date);
            throw;
        }
    }

    /// <summary>
    /// Actualiza un día de funcionamiento existente
    /// </summary>
    public async Task<bool> UpdateOperatingDay(int id, SiteOperatingDayRequest request)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@startTime", request.StartTime, DbType.Time);
            parameters.Add("@endTime", request.EndTime, DbType.Time);
            parameters.Add("@isWeekendOverride", request.IsWeekendOverride, DbType.Boolean);
            parameters.Add("@isExcluded", request.IsExcluded, DbType.Boolean);
            parameters.Add("@isHoliday", request.IsHoliday, DbType.Boolean);
            parameters.Add("@comment", request.Comment, DbType.String);

            var result = await dbConnection.QuerySingleAsync<int>("100_UpdateSiteOperatingDay", parameters, commandType: CommandType.StoredProcedure);

            var success = result > 0;

            _logger.LogInformation("Actualización de día de funcionamiento {Id}: {Success}",
                id, success ? "Exitosa" : "Fallida");

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar día de funcionamiento {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Actualiza múltiples días de funcionamiento en lote
    /// </summary>
    public async Task<bool> BulkUpdateOperatingDays(int siteId, List<SiteOperatingDayRequest> requests)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var successCount = 0;
            var totalCount = requests.Count;

            foreach (var request in requests)
            {
                try
                {
                    var success = await ToggleOperatingDay(request);
                    if (success) successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar día de funcionamiento para sitio {SiteId} en fecha {Date}",
                        request.SiteId, request.OperatingDate.Date);
                }
            }

            var allSuccessful = successCount == totalCount;

            _logger.LogInformation("Actualización en lote para sitio {SiteId}: {SuccessCount}/{TotalCount} exitosos",
                siteId, successCount, totalCount);

            return allSuccessful;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en actualización en lote para sitio {SiteId}", siteId);
            throw;
        }
    }

    /// <summary>
    /// Elimina un día de funcionamiento
    /// </summary>
    public async Task<bool> DeleteOperatingDay(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await dbConnection.QuerySingleAsync<dynamic>(
                "100_DeleteSiteOperatingDay",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var rowsAffected = (int)result.RowsAffected;
            var success = rowsAffected > 0;

            _logger.LogInformation("Eliminación de día de funcionamiento {Id}: {Success}",
                id, success ? "Exitosa" : "Fallida");

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar día de funcionamiento {Id}", id);
            throw;
        }
    }

}
