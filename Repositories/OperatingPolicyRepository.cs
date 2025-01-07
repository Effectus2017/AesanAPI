using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class OperatingPolicyRepository(DapperContext context, ILogger<OperatingPolicyRepository> logger) : IOperatingPolicyRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<OperatingPolicyRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene una política operativa por su ID.
    /// </summary>
    /// <param name="id">El ID de la política operativa a obtener.</param>
    /// <returns>La política operativa encontrada.</returns>
    public async Task<dynamic> GetOperatingPolicyById(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo política operativa por ID: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryMultipleAsync("100_GetOperatingPolicyById", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadSingleAsync<DTOOperatingPolicy>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la política operativa por ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las políticas operativas.
    /// </summary>
    /// <param name="take">El número de políticas operativas a tomar.</param>
    /// <param name="skip">El número de políticas operativas a saltar.</param>
    /// <param name="description">La descripción de la política operativa a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las políticas operativas.</param>
    /// <returns>Una lista de políticas operativas.</returns>
    public async Task<dynamic> GetAllOperatingPolicies(int take, int skip, string description, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las políticas operativas");
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@description", description, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllOperatingPolicies", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOOperatingPolicy>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las políticas operativas");
            throw;
        }
    }

    /// <summary>
    /// Inserta una nueva política operativa.
    /// </summary>
    /// <param name="operatingPolicy">La política operativa a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertOperatingPolicy(DTOOperatingPolicy operatingPolicy)
    {
        try
        {
            _logger.LogInformation("Insertando política operativa: {Description}", operatingPolicy.Description);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@description", operatingPolicy.Description, DbType.String);
            parameters.Add("@id", operatingPolicy.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertOperatingPolicy", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");
            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la política operativa: {Description}", operatingPolicy.Description);
            throw;
        }
    }

    /// <summary>
    /// Actualiza una política operativa existente.
    /// </summary>
    /// <param name="operatingPolicy">La política operativa a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateOperatingPolicy(DTOOperatingPolicy operatingPolicy)
    {
        try
        {
            _logger.LogInformation("Actualizando política operativa: {Description}", operatingPolicy.Description);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", operatingPolicy.Id, DbType.Int32);
            parameters.Add("@description", operatingPolicy.Description, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateOperatingPolicy", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la política operativa: {Description}", operatingPolicy.Description);
            throw;
        }
    }

    /// <summary>
    /// Elimina una política operativa por su ID.
    /// </summary>
    /// <param name="id">El ID de la política operativa a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteOperatingPolicy(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando política operativa: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteOperatingPolicy", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la política operativa: {Id}", id);
            throw;
        }
    }
}