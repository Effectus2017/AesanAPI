using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Api.Models.Errors;
using Dapper;

namespace Api.Repositories;

/// <summary>
/// Repositorio para gestionar relaciones sitio-programa
/// </summary>
public class SiteProgramRepository(DapperContext context) : ISiteProgramRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));

    /// <summary>
    /// Obtiene todos los programas de un sitio
    /// </summary>
    public async Task<IEnumerable<SiteProgramResponse>> GetSiteProgramsBySiteId(int siteId)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);

            var result = await connection.QueryAsync<SiteProgramResponse>(
                "100_GetSiteProgramsBySiteId",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? Enumerable.Empty<SiteProgramResponse>();
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al obtener programas del sitio {siteId}", ex);
        }
    }

    /// <summary>
    /// Inserta una nueva relación sitio-programa
    /// </summary>
    public async Task<int> InsertSiteProgram(SiteProgramRequest request)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", request.SiteId, DbType.Int32);
            parameters.Add("@programId", request.ProgramId, DbType.Int32);
            parameters.Add("@startDate", request.StartDate, DbType.Date);
            parameters.Add("@endDate", request.EndDate, DbType.Date);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "100_InsertSiteProgram",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return parameters.Get<int>("@id");
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, "Error al insertar relación sitio-programa", ex);
        }
    }

    /// <summary>
    /// Inserta una nueva relación sitio-programa usando una conexión y transacción existentes
    /// </summary>
    public async Task<int> InsertSiteProgram(SiteProgramRequest request, IDbConnection connection, IDbTransaction transaction)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", request.SiteId, DbType.Int32);
            parameters.Add("@programId", request.ProgramId, DbType.Int32);
            parameters.Add("@startDate", request.StartDate, DbType.Date);
            parameters.Add("@endDate", request.EndDate, DbType.Date);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "100_InsertSiteProgram",
                parameters,
                transaction,
                commandType: CommandType.StoredProcedure
            );

            return parameters.Get<int>("@id");
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al insertar relación sitio-programa para SiteId={request.SiteId}, ProgramId={request.ProgramId}", ex);
        }
    }

    /// <summary>
    /// Elimina todos los programas de un sitio
    /// </summary>
    public async Task<int> DeleteSitePrograms(int siteId, IDbConnection connection, IDbTransaction transaction)
    {
        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await connection.ExecuteAsync(
                "100_DeleteSitePrograms",
                parameters,
                transaction,
                commandType: CommandType.StoredProcedure
            );

            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al eliminar programas del sitio {siteId}", ex);
        }
    }

    /// <summary>
    /// Actualiza una relación sitio-programa existente
    /// </summary>
    public async Task<bool> UpdateSiteProgram(SiteProgramRequest request)
    {
        try
        {
            if (!request.Id.HasValue)
            {
                throw new ArgumentException("El ID de la relación es requerido para actualizar");
            }

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", request.Id.Value, DbType.Int32);
            parameters.Add("@siteId", request.SiteId, DbType.Int32);
            parameters.Add("@programId", request.ProgramId, DbType.Int32);
            parameters.Add("@startDate", request.StartDate, DbType.Date);
            parameters.Add("@endDate", request.EndDate, DbType.Date);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await connection.ExecuteAsync(
                "100_UpdateSiteProgram",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"Error al actualizar relación sitio-programa {request.Id}", ex);
        }
    }
}

