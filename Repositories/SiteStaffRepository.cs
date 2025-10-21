using Api.Data;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Services;
using Dapper;
using Microsoft.Data.SqlClient;
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
    public async Task<IEnumerable<DTOSiteStaff>> GetStaffBySite(int siteId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo empleados asignados al sitio {siteId}");

            if (siteId <= 0)
            {
                throw new ArgumentException("El ID del sitio debe ser mayor a 0", nameof(siteId));
            }

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);

            var result = await connection.QueryAsync<DTOSiteStaff>("100_GetStaffBySite", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation($"Se encontraron {result.Count()} empleados asignados al sitio {siteId}");
            return result;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al obtener empleados por sitio: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los sitios asignados a un empleado específico
    /// </summary>
    public async Task<IEnumerable<DTOSiteStaff>> GetSitesByStaff(int staffId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo sitios asignados al empleado {staffId}");

            if (staffId <= 0)
            {
                throw new ArgumentException("El ID del empleado debe ser mayor a 0", nameof(staffId));
            }

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@staffId", staffId, DbType.Int32);

            var result = await connection.QueryAsync<DTOSiteStaff>("100_GetSitesByStaff", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation($"Se encontraron {result.Count()} sitios asignados al empleado {staffId}");

            return result;
        }
        catch (Exception ex)
        {
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

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "La solicitud de asignación no puede ser nula");
            }

            if (request.SiteId <= 0)
            {
                throw new ArgumentException("El ID del sitio debe ser mayor a 0", nameof(request.SiteId));
            }

            if (request.StaffId <= 0)
            {
                throw new ArgumentException("El ID del empleado debe ser mayor a 0", nameof(request.StaffId));
            }

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", request.SiteId, DbType.Int32);
            parameters.Add("@staffId", request.StaffId, DbType.Int32);
            parameters.Add("@assignmentTypeId", request.AssignmentTypeId, DbType.Int32);
            parameters.Add("@isPrimary", request.IsPrimary, DbType.Boolean);
            parameters.Add("@startDate", request.StartDate, DbType.Date);
            parameters.Add("@endDate", request.EndDate, DbType.Date);
            parameters.Add("@comments", request.Comments, DbType.String);

            var result = await connection.QuerySingleAsync<int>(
                "100_AssignStaffToSite",
                parameters,
                commandType: CommandType.StoredProcedure
            );

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
            throw;
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

            if (siteId <= 0)
            {
                throw new ArgumentException("El ID del sitio debe ser mayor a 0", nameof(siteId));
            }

            if (staffId <= 0)
            {
                throw new ArgumentException("El ID del empleado debe ser mayor a 0", nameof(staffId));
            }

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
            throw;
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

            if (id <= 0)
            {
                throw new ArgumentException("El ID de la asignación debe ser mayor a 0", nameof(id));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "La solicitud de actualización no puede ser nula");
            }

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@assignmentTypeId", request.AssignmentTypeId, DbType.Int32);
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
            throw;
        }
    }

    /// <summary>
    /// Obtiene una asignación específica por su ID
    /// </summary>
    public async Task<DTOSiteStaff?> GetSiteStaffById(int id)
    {
        try
        {
            _logger.LogInformation($"Obteniendo asignación {id}");

            if (id <= 0)
            {
                throw new ArgumentException("El ID de la asignación debe ser mayor a 0", nameof(id));
            }

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await connection.QuerySingleOrDefaultAsync<DTOSiteStaff>(
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
            throw;
        }
    }
}
