using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class AgencyStatusRepository(DapperContext context, ILogger<AgencyStatusRepository> logger) : IAgencyStatusRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<AgencyStatusRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene un estado de agencia por su ID.
    /// </summary>
    /// <param name="id">El ID del estado a obtener.</param>
    /// <returns>El estado encontrado.</returns>
    public async Task<dynamic> GetAgencyStatusById(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo estado de agencia por ID: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryMultipleAsync("100_GetAgencyStatusById", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadSingleAsync<DTOAgencyStatus>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el estado de agencia por ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los estados de agencia.
    /// </summary>
    /// <param name="take">El número de estados a tomar.</param>
    /// <param name="skip">El número de estados a saltar.</param>
    /// <param name="name">El nombre del estado a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los estados.</param>
    /// <returns>Una lista de estados de agencia.</returns>
    public async Task<dynamic> GetAllAgencyStatuses(int take, int skip, string name, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los estados de agencia");
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllAgencyStatuses", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOAgencyStatus>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los estados de agencia");
            throw;
        }
    }

    /// <summary>
    /// Inserta un nuevo estado de agencia.
    /// </summary>
    /// <param name="status">El estado a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertAgencyStatus(DTOAgencyStatus status)
    {
        try
        {
            _logger.LogInformation("Insertando estado de agencia: {Name}", status.Name);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", status.Name, DbType.String);
            parameters.Add("@id", status.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertAgencyStatus", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");
            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el estado de agencia: {Name}", status.Name);
            throw;
        }
    }

    /// <summary>
    /// Actualiza un estado de agencia existente.
    /// </summary>
    /// <param name="status">El estado a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateAgencyStatus(DTOAgencyStatus status)
    {
        try
        {
            _logger.LogInformation("Actualizando estado de agencia: {Name}", status.Name);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", status.Id, DbType.Int32);
            parameters.Add("@name", status.Name, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateAgencyStatus", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado de agencia: {Name}", status.Name);
            throw;
        }
    }

    /// <summary>
    /// Elimina un estado de agencia por su ID.
    /// </summary>
    /// <param name="id">El ID del estado a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteAgencyStatus(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando estado de agencia: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteAgencyStatus", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el estado de agencia: {Id}", id);
            throw;
        }
    }
}