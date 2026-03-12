using System.Data;
using Dapper;
using Api.Data;
using Api.Interfaces;
using Api.Models.Errors;
using Api.Models.Request;
using Api.Models.Response;

namespace Api.Repositories;

/// <summary>
/// Repositorio para la gestión de calendario de citas de agencias
/// Implementa las operaciones CRUD
/// </summary>
public class AgencyCalendarRepository(DapperContext context) : IAgencyCalendarRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<AgencyCalendarResponse> GetAppointments(int agencyId, int? month = null, int? year = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AgencyId", agencyId, DbType.Int32);
            parameters.Add("@Month", month, DbType.Int32);
            parameters.Add("@Year", year, DbType.Int32);

            using var multi = await dbConnection.QueryMultipleAsync("100_GetAgencyAppointments", parameters, commandType: CommandType.StoredProcedure);

            // Obtener información de la agencia
            var agencyInfo = await multi.ReadFirstOrDefaultAsync<AgencyCalendarResponse>();
            if (agencyInfo == null)
            {
                return new AgencyCalendarResponse
                {
                    AgencyId = agencyId,
                    AgencyName = "Agencia no encontrada",
                    Appointments = new List<AgencyAppointmentResponse>()
                };
            }

            // Obtener citas
            var appointments = await multi.ReadAsync<AgencyAppointmentResponse>();
            
            var response = new AgencyCalendarResponse
            {
                AgencyId = agencyInfo.AgencyId,
                AgencyName = agencyInfo.AgencyName,
                Appointments = appointments.ToList()
            };

            return response;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener citas para la agencia {agencyId}", ex);
        }
    }

    public async Task<int?> CreateAppointment(AgencyAppointmentRequest request, string? userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AgencyId", request.AgencyId, DbType.Int32);
            parameters.Add("@AppointmentDate", request.AppointmentDate.Date, DbType.Date);
            parameters.Add("@StartTime", TimeSpan.Parse(request.StartTime), DbType.Time);
            parameters.Add("@EndTime", TimeSpan.Parse(request.EndTime), DbType.Time);
            parameters.Add("@Comments", request.Comment, DbType.String);
            parameters.Add("@UserId", userId ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertAgencyAppointment", parameters, commandType: CommandType.StoredProcedure);

            var newId = parameters.Get<int?>("@Id");
            return newId;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al crear cita para la agencia {request.AgencyId}", ex);
        }
    }

    public async Task<bool> UpdateAppointment(int id, AgencyAppointmentRequest request, string? userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);
            parameters.Add("@AppointmentDate", request.AppointmentDate.Date, DbType.Date);
            parameters.Add("@StartTime", TimeSpan.Parse(request.StartTime), DbType.Time);
            parameters.Add("@EndTime", TimeSpan.Parse(request.EndTime), DbType.Time);
            parameters.Add("@Comments", request.Comment, DbType.String);
            parameters.Add("@UserId", userId ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_UpdateAgencyAppointment", parameters, commandType: CommandType.StoredProcedure);

            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al actualizar la cita {id}", ex);
        }
    }

    public async Task<bool> DeleteAppointment(int id, string? userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Id", id, DbType.Int32);
            parameters.Add("@UserId", userId ?? (object)DBNull.Value, DbType.String);

            // Nota: 100_DeleteAgencyAppointment ya no devuelve return value en el scope anterior,
            // pero internamente hace un soft delete con @Id, no hay parametro output
            // Asi que validamos rows afectadas globalmente
            
            var rowsAffected = await dbConnection.ExecuteAsync("100_DeleteAgencyAppointment", parameters, commandType: CommandType.StoredProcedure);

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al eliminar la cita {id}", ex);
        }
    }
}
