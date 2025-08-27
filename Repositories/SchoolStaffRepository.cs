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
public class SchoolStaffRepository(DapperContext context, ILoggingService loggingService) : ISchoolStaffRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILoggingService _logger = loggingService ?? throw new ArgumentNullException(nameof(loggingService));

    /// <summary>
    /// Obtiene todos los empleados asignados a un sitio específico
    /// </summary>
    public async Task<IEnumerable<DTOSchoolStaff>> GetStaffBySchool(int schoolId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo empleados asignados al sitio {schoolId}");

            if (schoolId <= 0)
            {
                throw new ArgumentException("El ID del sitio debe ser mayor a 0", nameof(schoolId));
            }

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32);

            var result = await connection.QueryAsync<DTOSchoolStaff>(
                "100_GetStaffBySchool",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            _logger.LogInformation($"Se encontraron {result.Count()} empleados asignados al sitio {schoolId}");
            return result;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener empleados por sitio", new Dictionary<string, string>
            {
                { "SchoolId", schoolId.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los sitios asignados a un empleado específico
    /// </summary>
    public async Task<IEnumerable<DTOSchoolStaff>> GetSchoolsByStaff(int staffId)
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

            var result = await connection.QueryAsync<DTOSchoolStaff>(
                "100_GetSchoolsByStaff",
                parameters,
                commandType: CommandType.StoredProcedure
            );

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
            throw;
        }
    }

    /// <summary>
    /// Asigna un empleado a un sitio
    /// </summary>
    public async Task<int> AssignStaffToSchool(SchoolStaffRequest request)
    {
        try
        {
            _logger.LogInformation($"Asignando empleado {request.StaffId} al sitio {request.SchoolId}");

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "La solicitud de asignación no puede ser nula");
            }

            if (request.SchoolId <= 0)
            {
                throw new ArgumentException("El ID del sitio debe ser mayor a 0", nameof(request.SchoolId));
            }

            if (request.StaffId <= 0)
            {
                throw new ArgumentException("El ID del empleado debe ser mayor a 0", nameof(request.StaffId));
            }

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", request.SchoolId, DbType.Int32);
            parameters.Add("@staffId", request.StaffId, DbType.Int32);
            parameters.Add("@assignmentTypeId", request.AssignmentTypeId, DbType.Int32);
            parameters.Add("@isPrimary", request.IsPrimary, DbType.Boolean);
            parameters.Add("@startDate", request.StartDate, DbType.Date);
            parameters.Add("@endDate", request.EndDate, DbType.Date);
            parameters.Add("@comments", request.Comments, DbType.String);

            var result = await connection.QuerySingleAsync<int>(
                "100_AssignStaffToSchool",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            _logger.LogInformation($"Empleado {request.StaffId} asignado exitosamente al sitio {request.SchoolId} con ID {result}");
            return result;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al asignar empleado a sitio", new Dictionary<string, string>
            {
                { "SchoolId", request?.SchoolId.ToString() ?? "null" },
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
    public async Task<bool> UnassignStaffFromSchool(int schoolId, int staffId)
    {
        try
        {
            _logger.LogInformation($"Desasignando empleado {staffId} del sitio {schoolId}");

            if (schoolId <= 0)
            {
                throw new ArgumentException("El ID del sitio debe ser mayor a 0", nameof(schoolId));
            }

            if (staffId <= 0)
            {
                throw new ArgumentException("El ID del empleado debe ser mayor a 0", nameof(staffId));
            }

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32);
            parameters.Add("@staffId", staffId, DbType.Int32);

            var result = await connection.QuerySingleAsync<int>(
                "100_UnassignStaffFromSchool",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var success = result == 1;
            if (success)
            {
                _logger.LogInformation($"Empleado {staffId} desasignado exitosamente del sitio {schoolId}");
            }
            else
            {
                _logger.LogWarning($"No se pudo desasignar el empleado {staffId} del sitio {schoolId}");
            }

            return success;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al desasignar empleado de sitio", new Dictionary<string, string>
            {
                { "SchoolId", schoolId.ToString() },
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
    public async Task<bool> UpdateSchoolStaff(int id, UpdateSchoolStaffRequest request)
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
                "100_UpdateSchoolStaff",
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
    public async Task<DTOSchoolStaff?> GetSchoolStaffById(int id)
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

            var result = await connection.QuerySingleOrDefaultAsync<DTOSchoolStaff>(
                "100_GetSchoolStaffById",
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
