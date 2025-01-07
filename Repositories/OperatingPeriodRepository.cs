using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class OperatingPeriodRepository(DapperContext context, ILogger<OperatingPeriodRepository> logger) : IOperatingPeriodRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<OperatingPeriodRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene un período operativo por su ID.
    /// </summary>
    /// <param name="id">El ID del período operativo a obtener.</param>
    /// <returns>El período operativo encontrado.</returns>
    public async Task<dynamic> GetOperatingPeriodById(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo período operativo por ID: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryMultipleAsync("100_GetOperatingPeriodById", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadSingleAsync<DTOOperatingPeriod>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el período operativo por ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los períodos operativos.
    /// </summary>
    /// <param name="take">El número de períodos operativos a tomar.</param>
    /// <param name="skip">El número de períodos operativos a saltar.</param>
    /// <param name="name">El nombre del período operativo a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los períodos operativos.</param>
    /// <returns>Una lista de períodos operativos.</returns>
    public async Task<dynamic> GetAllOperatingPeriods(int take, int skip, string name, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los períodos operativos");
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllOperatingPeriods", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOOperatingPeriod>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los períodos operativos");
            throw;
        }
    }

    /// <summary>
    /// Inserta un nuevo período operativo.
    /// </summary>
    /// <param name="operatingPeriod">El período operativo a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertOperatingPeriod(DTOOperatingPeriod operatingPeriod)
    {
        try
        {
            _logger.LogInformation("Insertando período operativo: {Name}", operatingPeriod.Name);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", operatingPeriod.Name, DbType.String);
            parameters.Add("@id", operatingPeriod.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertOperatingPeriod", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");
            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el período operativo: {Name}", operatingPeriod.Name);
            throw;
        }
    }

    /// <summary>
    /// Actualiza un período operativo existente.
    /// </summary>
    /// <param name="operatingPeriod">El período operativo a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateOperatingPeriod(DTOOperatingPeriod operatingPeriod)
    {
        try
        {
            _logger.LogInformation("Actualizando período operativo: {Name}", operatingPeriod.Name);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", operatingPeriod.Id, DbType.Int32);
            parameters.Add("@name", operatingPeriod.Name, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateOperatingPeriod", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el período operativo: {Name}", operatingPeriod.Name);
            throw;
        }
    }

    /// <summary>
    /// Elimina un período operativo por su ID.
    /// </summary>
    /// <param name="id">El ID del período operativo a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteOperatingPeriod(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando período operativo: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteOperatingPeriod", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el período operativo: {Id}", id);
            throw;
        }
    }
}