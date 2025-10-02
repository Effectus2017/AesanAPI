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
/// Repositorio para la gestión de calendario de funcionamiento de escuelas
/// Implementa las operaciones CRUD y específicas para días de funcionamiento
/// </summary>
public class SchoolCalendarRepository(DapperContext context, ILogger<SchoolCalendarRepository> logger) : ISchoolCalendarRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SchoolCalendarRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene todos los días de funcionamiento de una escuela específica
    /// </summary>
    public async Task<SchoolCalendarResponse> GetOperatingDays(int schoolId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32);

            using var multi = await dbConnection.QueryMultipleAsync("100_GetSchoolOperatingDays", parameters, commandType: CommandType.StoredProcedure);

            // Obtener información de la escuela
            var schoolInfo = await multi.ReadFirstOrDefaultAsync<SchoolInfoResponse>();
            if (schoolInfo == null)
            {
                _logger.LogWarning("No se encontró la escuela con ID {SchoolId}", schoolId);
                return new SchoolCalendarResponse
                {
                    schoolId = schoolId,
                    schoolName = "Escuela no encontrada",
                    operatingDays = new List<OperatingDayResponse>()
                };
            }

            // Obtener días de funcionamiento
            var operatingDays = await multi.ReadAsync<OperatingDayResponse>();

            var response = new SchoolCalendarResponse
            {
                schoolId = schoolInfo.SchoolId,
                schoolName = schoolInfo.SchoolName,
                operatingDays = operatingDays.ToList()
            };

            _logger.LogInformation("Se obtuvieron {Count} días de funcionamiento para la escuela {SchoolId}",
                operatingDays.Count(), schoolId);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener días de funcionamiento para la escuela {SchoolId}", schoolId);
            throw;
        }
    }

    /// <summary>
    /// Alterna el estado de funcionamiento de un día específico
    /// </summary>
    public async Task<bool> ToggleOperatingDay(SchoolOperatingDayRequest request)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", request.schoolId, DbType.Int32);
            parameters.Add("@operatingDate", request.operatingDate.Date, DbType.Date);
            parameters.Add("@startTime", request.startTime, DbType.Time);
            parameters.Add("@endTime", request.endTime, DbType.Time);
            parameters.Add("@isWeekendOverride", request.isWeekendOverride, DbType.Boolean);
            parameters.Add("@isExcluded", request.isExcluded, DbType.Boolean);
            parameters.Add("@comment", request.comment, DbType.String);

            var result = await dbConnection.QuerySingleAsync<int>("100_ToggleSchoolOperatingDay", parameters, commandType: CommandType.StoredProcedure);

            var success = result > 0;

            _logger.LogInformation("Toggle de día de funcionamiento para escuela {SchoolId} en fecha {Date}: {Success}",
                request.schoolId, request.operatingDate.Date, success ? "Exitoso" : "Fallido");

            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al alternar día de funcionamiento para la escuela {SchoolId} en fecha {Date}",
                request.schoolId, request.operatingDate.Date);
            throw;
        }
    }

    /// <summary>
    /// Actualiza un día de funcionamiento existente
    /// </summary>
    public async Task<bool> UpdateOperatingDay(int id, SchoolOperatingDayRequest request)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@startTime", request.startTime, DbType.Time);
            parameters.Add("@endTime", request.endTime, DbType.Time);
            parameters.Add("@isWeekendOverride", request.isWeekendOverride, DbType.Boolean);
            parameters.Add("@isExcluded", request.isExcluded, DbType.Boolean);
            parameters.Add("@comment", request.comment, DbType.String);

            var result = await dbConnection.QuerySingleAsync<int>("100_UpdateSchoolOperatingDay", parameters, commandType: CommandType.StoredProcedure);

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
    public async Task<bool> BulkUpdateOperatingDays(int schoolId, List<SchoolOperatingDayRequest> requests)
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
                    _logger.LogError(ex, "Error al procesar día de funcionamiento para escuela {SchoolId} en fecha {Date}",
                        request.schoolId, request.operatingDate.Date);
                }
            }

            var allSuccessful = successCount == totalCount;

            _logger.LogInformation("Actualización en lote para escuela {SchoolId}: {SuccessCount}/{TotalCount} exitosos",
                schoolId, successCount, totalCount);

            return allSuccessful;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en actualización en lote para escuela {SchoolId}", schoolId);
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

            var query = "DELETE FROM SchoolOperatingDays WHERE Id = @Id";

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await dbConnection.ExecuteAsync(query, parameters);

            var success = result > 0;

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

    /// <summary>
    /// Verifica si una escuela existe
    /// </summary>
    public async Task<bool> SchoolExists(int schoolId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var query = "SELECT COUNT(1) FROM School WHERE Id = @SchoolId";

            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32);

            var count = await dbConnection.QuerySingleAsync<int>(query, parameters);

            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia de la escuela {SchoolId}", schoolId);
            throw;
        }
    }
}
