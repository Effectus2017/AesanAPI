using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Api.Repositories;

/// <summary>
/// Repositorio para gestionar períodos de programas
/// </summary>
public class ProgramPeriodRepository(DapperContext context, ILogger<ProgramPeriodRepository> logger) : IProgramPeriodRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<ProgramPeriodRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene todos los períodos de un programa
    /// </summary>
    public async Task<IEnumerable<ProgramPeriodResponse>> GetProgramPeriodsByProgramId(int programId)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@programId", programId, DbType.Int32);

            var result = await connection.QueryAsync<ProgramPeriodResponse>(
                "100_GetProgramPeriodsByProgramId",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result ?? Enumerable.Empty<ProgramPeriodResponse>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener períodos del programa {ProgramId}", programId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene un período específico de un programa por año
    /// </summary>
    public async Task<ProgramPeriodResponse?> GetProgramPeriodByProgramIdAndYear(int programId, int year)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@programId", programId, DbType.Int32);
            parameters.Add("@year", year, DbType.Int32);

            var result = await connection.QueryFirstOrDefaultAsync<ProgramPeriodResponse>(
                "100_GetProgramPeriodByProgramIdAndYear",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener período del programa {ProgramId} para el año {Year}", programId, year);
            throw;
        }
    }

    /// <summary>
    /// Inserta un nuevo período de programa
    /// </summary>
    public async Task<int> InsertProgramPeriod(ProgramPeriodRequest request)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@programId", request.ProgramId, DbType.Int32);
            parameters.Add("@year", request.Year, DbType.Int32);
            parameters.Add("@startDate", request.StartDate, DbType.Date);
            parameters.Add("@endDate", request.EndDate, DbType.Date);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "100_InsertProgramPeriod",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return parameters.Get<int>("@id");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar período de programa");
            throw;
        }
    }

    /// <summary>
    /// Actualiza un período de programa existente
    /// </summary>
    public async Task<bool> UpdateProgramPeriod(ProgramPeriodRequest request)
    {
        try
        {
            if (!request.Id.HasValue)
            {
                throw new ArgumentException("El ID del período es requerido para actualizar");
            }

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", request.Id.Value, DbType.Int32);
            parameters.Add("@programId", request.ProgramId, DbType.Int32);
            parameters.Add("@year", request.Year, DbType.Int32);
            parameters.Add("@startDate", request.StartDate, DbType.Date);
            parameters.Add("@endDate", request.EndDate, DbType.Date);
            parameters.Add("@isActive", request.IsActive, DbType.Boolean);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await connection.ExecuteAsync(
                "100_UpdateProgramPeriod",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar período de programa {Id}", request.Id);
            throw;
        }
    }
}

