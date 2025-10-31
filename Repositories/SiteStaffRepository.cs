using Api.Data;
using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Api.Services;
using Dapper;
using System.Collections.Generic;
using System.Data;

namespace Api.Repositories;

/// <summary>
/// Repositorio para gestionar las asignaciones de empleados a sitios
/// </summary>
public class SiteStaffRepository(DapperContext context, ILoggingService loggingService) : ISiteStaffRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILoggingService _logger = loggingService ?? throw new ArgumentNullException(nameof(loggingService));

    /// <summary>
    /// Obtiene todos los empleados asignados a un sitio específico
    /// </summary>
    public async Task<IEnumerable<SiteStaffResponse>> GetStaffBySite(int siteId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo empleados asignados al sitio {siteId}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);

            var result = await connection.QueryAsync<SiteStaffResponse>("100_GetStaffBySite", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation($"Se encontraron {result.Count()} empleados asignados al sitio {siteId}");
            return result;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener empleados por sitio", new Dictionary<string, string>
            {
                { "SiteId", siteId.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new Exception($"Error al obtener empleados por sitio: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los sitios asignados a un empleado específico
    /// </summary>
    public async Task<IEnumerable<SiteStaffResponse>> GetSitesByStaff(int staffId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo sitios asignados al empleado {staffId}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@staffId", staffId, DbType.Int32);

            var result = await connection.QueryAsync<SiteStaffResponse>("100_GetSitesByStaff", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation($"Se encontraron {result.Count()} sitios asignados al empleado {staffId}");

            return result;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener sitios por empleado", new Dictionary<string, string>
            {
                { "StaffId", staffId.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new Exception($"Error al obtener sitios por empleado: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Asigna un empleado a un sitio
    /// </summary>
    public async Task<int> AssignStaffToSite(SiteStaffRequest request)
    {
        try
        {
            _logger.LogInformation($"Asignando empleado {request.StaffId} al sitio {request.SiteId}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", request.SiteId, DbType.Int32);
            parameters.Add("@staffId", request.StaffId, DbType.Int32);
            parameters.Add("@isPrimary", request.IsPrimary, DbType.Boolean);
            parameters.Add("@startDate", request.StartDate, DbType.Date);
            parameters.Add("@endDate", request.EndDate, DbType.Date);
            parameters.Add("@comments", request.Comments, DbType.String);

            var result = await connection.QuerySingleAsync<int>("100_AssignStaffToSite", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation($"Empleado {request.StaffId} asignado exitosamente al sitio {request.SiteId} con ID {result}");
            return result;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al asignar empleado a sitio", new Dictionary<string, string>
            {
                { "SiteId", request?.SiteId.ToString() ?? "null" },
                { "StaffId", request?.StaffId.ToString() ?? "null" },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new Exception($"Error al asignar empleado {request?.StaffId} al sitio {request?.SiteId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Desasigna un empleado de un sitio
    /// </summary>
    public async Task<bool> UnassignStaffFromSite(int siteId, int staffId)
    {
        try
        {
            _logger.LogInformation($"Desasignando empleado {staffId} del sitio {siteId}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@staffId", staffId, DbType.Int32);

            var result = await connection.QuerySingleAsync<int>(
                "100_UnassignStaffFromSite",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var success = result == 1;
            if (success)
            {
                _logger.LogInformation($"Empleado {staffId} desasignado exitosamente del sitio {siteId}");
            }
            else
            {
                _logger.LogWarning($"No se pudo desasignar el empleado {staffId} del sitio {siteId}");
                throw new Exception($"No se pudo desasignar el empleado {staffId} del sitio {siteId}");
            }

            return success;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al desasignar empleado de sitio", new Dictionary<string, string>
            {
                { "SiteId", siteId.ToString() },
                { "StaffId", staffId.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new Exception($"Error al desasignar empleado {staffId} del sitio {siteId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza una asignación existente
    /// </summary>
    public async Task<bool> UpdateSiteStaff(int id, UpdateSiteStaffRequest request)
    {
        try
        {
            _logger.LogInformation($"Actualizando asignación {id}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@isPrimary", request.IsPrimary, DbType.Boolean);
            parameters.Add("@startDate", request.StartDate, DbType.Date);
            parameters.Add("@endDate", request.EndDate, DbType.Date);
            parameters.Add("@comments", request.Comments, DbType.String);

            var result = await connection.QuerySingleAsync<int>(
                "100_UpdateSiteStaff",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var success = result == 1;
            if (success)
            {
                _logger.LogInformation($"Asignación {id} actualizada exitosamente");
            }
            else
            {
                _logger.LogWarning($"No se pudo actualizar la asignación {id}");
                throw new Exception($"No se pudo actualizar la asignación {id}");
            }

            return success;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al actualizar asignación", new Dictionary<string, string>
            {
                { "Id", id.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new Exception($"Error al actualizar la asignación {id}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene una asignación específica por su ID
    /// </summary>
    public async Task<SiteStaffResponse?> GetSiteStaffById(int id)
    {
        try
        {
            _logger.LogInformation($"Obteniendo asignación {id}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await connection.QuerySingleOrDefaultAsync<SiteStaffResponse>(
                "100_GetSiteStaffById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            if (result != null)
            {
                _logger.LogInformation($"Asignación {id} encontrada exitosamente");
            }
            else
            {
                _logger.LogInformation($"No se encontró la asignación {id}");
            }

            return result;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener asignación por ID", new Dictionary<string, string>
            {
                { "Id", id.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new Exception($"Error al obtener la asignación {id}: {ex.Message}", ex);
        }
    }
}
