using System.Data;
using Dapper;
using Api.Data;
using Api.Interfaces;
using Api.Models.Errors;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.Data.SqlClient;

namespace Api.Repositories;

/// <summary>
/// Repositorio del calendario de visitas por sitio.
/// </summary>
public class SiteVisitCalendarRepository(DapperContext context) : ISiteVisitCalendarRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task<SiteVisitCalendarResponse> GetVisits(int agencyId, int siteId, int? month = null, int? year = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@agencyid", agencyId, DbType.Int32);
            parameters.Add("@siteid", siteId, DbType.Int32);
            parameters.Add("@month", month, DbType.Int32);
            parameters.Add("@year", year, DbType.Int32);

            using var multi = await dbConnection.QueryMultipleAsync("100_GetSiteVisits", parameters, commandType: CommandType.StoredProcedure);

            var agencyInfo = await multi.ReadFirstOrDefaultAsync<SiteVisitCalendarResponse>();
            if (agencyInfo == null)
            {
                return new SiteVisitCalendarResponse
                {
                    AgencyId = agencyId,
                    AgencyName = "Agencia no encontrada",
                    Visits = new List<SiteVisitResponse>()
                };
            }

            var visits = (await multi.ReadAsync<SiteVisitResponse>()).ToList();
            agencyInfo.Visits = visits;
            return agencyInfo;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener visitas para agencia {agencyId} y sitio {siteId}", ex);
        }
    }

    public async Task<IReadOnlyList<VisitTypeDropdownItemResponse>> GetVisitTypes(bool alls = false)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@alls", alls ? 1 : 0, DbType.Boolean);
            var rows = await dbConnection.QueryAsync<VisitTypeDropdownItemResponse>("100_GetVisitTypes", parameters, commandType: CommandType.StoredProcedure);
            return rows.ToList();
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al obtener tipos de visita", ex);
        }
    }

    public async Task<int?> CreateVisit(SiteVisitRequest request, string? userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@agencyid", request.AgencyId, DbType.Int32);
            parameters.Add("@siteid", request.SiteId, DbType.Int32);
            parameters.Add("@visittypeid", request.VisitTypeId, DbType.Int32);
            parameters.Add("@visitdate", request.VisitDate.Date, DbType.Date);
            parameters.Add("@starttime", TimeSpan.Parse(NormalizeTime(request.StartTime)), DbType.Time);
            parameters.Add("@endtime", TimeSpan.Parse(NormalizeTime(request.EndTime)), DbType.Time);
            parameters.Add("@comments", request.Comment, DbType.String);
            parameters.Add("@userid", userId ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSiteVisit", parameters, commandType: CommandType.StoredProcedure);
            return parameters.Get<int?>("@id");
        }
        catch (SqlException ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al crear visita para el sitio {request.SiteId}", ex);
        }
    }

    public async Task<bool> UpdateVisit(int id, SiteVisitRequest request, string? userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@agencyid", request.AgencyId, DbType.Int32);
            parameters.Add("@siteid", request.SiteId, DbType.Int32);
            parameters.Add("@visittypeid", request.VisitTypeId, DbType.Int32);
            parameters.Add("@visitdate", request.VisitDate.Date, DbType.Date);
            parameters.Add("@starttime", TimeSpan.Parse(NormalizeTime(request.StartTime)), DbType.Time);
            parameters.Add("@endtime", TimeSpan.Parse(NormalizeTime(request.EndTime)), DbType.Time);
            parameters.Add("@comments", request.Comment, DbType.String);
            parameters.Add("@userid", userId ?? (object)DBNull.Value, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_UpdateSiteVisit", parameters, commandType: CommandType.StoredProcedure);
            var rows = parameters.Get<int>("@rowsAffected");
            return rows > 0;
        }
        catch (SqlException ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al actualizar la visita {id}", ex);
        }
    }

    public async Task<bool> DeleteVisit(int id, int agencyId, string? userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@agencyid", agencyId, DbType.Int32);
            parameters.Add("@userid", userId ?? (object)DBNull.Value, DbType.String);

            var rowsAffected = await dbConnection.ExecuteAsync("100_DeleteSiteVisit", parameters, commandType: CommandType.StoredProcedure);
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al eliminar la visita {id}", ex);
        }
    }

    public static string NormalizeTime(string time)
    {
        if (string.IsNullOrWhiteSpace(time))
            return "00:00:00";
        var t = time.Trim();
        if (t.Length == 5 && t[2] == ':')
            return t + ":00";
        return t;
    }
}
