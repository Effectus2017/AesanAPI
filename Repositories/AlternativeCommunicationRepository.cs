using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class AlternativeCommunicationRepository(DapperContext context, ILogger<AlternativeCommunicationRepository> logger) : IAlternativeCommunicationRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<AlternativeCommunicationRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene una comunicación alternativa por su ID.
    /// </summary>
    /// <param name="id">El ID de la comunicación alternativa a obtener.</param>
    /// <returns>La comunicación alternativa encontrada.</returns>
    public async Task<dynamic> GetAlternativeCommunicationById(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo comunicación alternativa por ID: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            var result = await db.QueryMultipleAsync("100_GetAlternativeCommunicationById", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadSingleAsync<DTOAlternativeCommunication>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la comunicación alternativa por ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las comunicaciones alternativas.
    /// </summary>
    /// <param name="take">El número de comunicaciones alternativas a tomar.</param>
    /// <param name="skip">El número de comunicaciones alternativas a saltar.</param>
    /// <param name="name">El nombre de la comunicación alternativa a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las comunicaciones alternativas.</param>
    /// <returns>Una lista de comunicaciones alternativas.</returns>
    public async Task<dynamic> GetAllAlternativeCommunications(int take, int skip, string name, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las comunicaciones alternativas");
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllAlternativeCommunications", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOAlternativeCommunication>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las comunicaciones alternativas");
            throw;
        }
    }

    /// <summary>
    /// Inserta una nueva comunicación alternativa.
    /// </summary>
    /// <param name="alternativeCommunication">La comunicación alternativa a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertAlternativeCommunication(DTOAlternativeCommunication alternativeCommunication)
    {
        try
        {
            _logger.LogInformation("Insertando comunicación alternativa: {Name}", alternativeCommunication.Name);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", alternativeCommunication.Name, DbType.String);
            parameters.Add("@id", alternativeCommunication.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertAlternativeCommunication", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");
            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la comunicación alternativa: {Name}", alternativeCommunication.Name);
            throw;
        }
    }

    /// <summary>
    /// Actualiza una comunicación alternativa existente.
    /// </summary>
    /// <param name="alternativeCommunication">La comunicación alternativa a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateAlternativeCommunication(DTOAlternativeCommunication alternativeCommunication)
    {
        try
        {
            _logger.LogInformation("Actualizando comunicación alternativa: {Name}", alternativeCommunication.Name);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", alternativeCommunication.Id, DbType.Int32);
            parameters.Add("@name", alternativeCommunication.Name, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateAlternativeCommunication", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la comunicación alternativa: {Name}", alternativeCommunication.Name);
            throw;
        }
    }

    /// <summary>
    /// Elimina una comunicación alternativa por su ID.
    /// </summary>
    /// <param name="id">El ID de la comunicación alternativa a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteAlternativeCommunication(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando comunicación alternativa: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteAlternativeCommunication", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la comunicación alternativa: {Id}", id);
            throw;
        }
    }
}