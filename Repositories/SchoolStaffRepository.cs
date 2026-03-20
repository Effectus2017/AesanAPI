using Api.Data;
using Api.Interfaces;
using Api.Models.Errors;
using Api.Models.Request;
using Api.Models.Response;
using Api.Services;
using Dapper;
using System.Data;

namespace Api.Repositories;

/// <summary>
/// Repositorio para asignaciones de empleados a escuelas (SchoolStaff).
/// </summary>
public class SchoolStaffRepository(DapperContext context, ILoggingService loggingService) : ISchoolStaffRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILoggingService _logger = loggingService ?? throw new ArgumentNullException(nameof(loggingService));

    /// <inheritdoc />
    public async Task<IEnumerable<SchoolStaffResponse>> GetStaffBySchool(int schoolId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo empleados asignados a la escuela {schoolId}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", schoolId, DbType.Int32);

            var result = await connection.QueryAsync<SchoolStaffResponse>("100_GetStaffBySchool", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation($"Se encontraron {result.Count()} empleados asignados a la escuela {schoolId}");
            return result;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener empleados por escuela", new Dictionary<string, string>
            {
                { "SchoolId", schoolId.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener empleados por escuela", ex);
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<SchoolStaffResponse>> GetSchoolsByStaff(int staffId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo escuelas asignadas al empleado {staffId}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@staffId", staffId, DbType.Int32);

            var result = await connection.QueryAsync<SchoolStaffResponse>("100_GetSchoolsByStaff", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation($"Se encontraron {result.Count()} escuelas asignadas al empleado {staffId}");

            return result;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener escuelas por empleado", new Dictionary<string, string>
            {
                { "StaffId", staffId.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener escuelas por empleado", ex);
        }
    }

    /// <inheritdoc />
    public async Task<int> AssignStaffToSchool(SchoolStaffRequest request)
    {
        try
        {
            _logger.LogInformation($"Asignando empleado {request.StaffId} a la escuela {request.SchoolId}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@schoolId", request.SchoolId, DbType.Int32);
            parameters.Add("@staffId", request.StaffId, DbType.Int32);
            parameters.Add("@isPrimary", request.IsPrimary, DbType.Boolean);
            parameters.Add("@startDate", request.StartDate, DbType.Date);
            parameters.Add("@endDate", request.EndDate, DbType.Date);
            parameters.Add("@comments", request.Comments, DbType.String);

            var result = await connection.QuerySingleAsync<int>("100_AssignStaffToSchool", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation($"Empleado {request.StaffId} asignado a la escuela {request.SchoolId} con ID {result}");
            return result;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al asignar empleado a escuela", new Dictionary<string, string>
            {
                { "SchoolId", request?.SchoolId.ToString() ?? "null" },
                { "StaffId", request?.StaffId.ToString() ?? "null" },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al asignar empleado {request?.StaffId} a la escuela {request?.SchoolId}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> UnassignStaffFromSchool(int schoolId, int staffId)
    {
        try
        {
            _logger.LogInformation($"Desasignando empleado {staffId} de la escuela {schoolId}");

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
                _logger.LogInformation($"Empleado {staffId} desasignado de la escuela {schoolId}");
            }
            else
            {
                _logger.LogWarning($"No se pudo desasignar el empleado {staffId} de la escuela {schoolId}");
                throw new ApiException(ErrorCode.ENTITY_NOT_FOUND, $"No se pudo desasignar el empleado {staffId} de la escuela {schoolId}");
            }

            return success;
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al desasignar empleado de escuela", new Dictionary<string, string>
            {
                { "SchoolId", schoolId.ToString() },
                { "StaffId", staffId.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al desasignar empleado {staffId} de la escuela {schoolId}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> UpdateSchoolStaff(int id, UpdateSchoolStaffRequest request)
    {
        try
        {
            _logger.LogInformation($"Actualizando asignación SchoolStaff {id}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
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
                _logger.LogInformation($"Asignación {id} actualizada");
            }
            else
            {
                _logger.LogWarning($"No se pudo actualizar la asignación {id}");
                throw new ApiException(ErrorCode.ENTITY_NOT_FOUND, $"No se pudo actualizar la asignación {id}");
            }

            return success;
        }
        catch (ApiException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al actualizar asignación SchoolStaff", new Dictionary<string, string>
            {
                { "Id", id.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al actualizar la asignación {id}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<SchoolStaffResponse?> GetSchoolStaffById(int id)
    {
        try
        {
            _logger.LogInformation($"Obteniendo asignación SchoolStaff {id}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var result = await connection.QuerySingleOrDefaultAsync<SchoolStaffResponse>(
                "100_GetSchoolStaffById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

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
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener la asignación {id}", ex);
        }
    }
}
