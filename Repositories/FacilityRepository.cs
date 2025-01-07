using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class FacilityRepository(DapperContext context, ILogger<FacilityRepository> logger) : IFacilityRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<FacilityRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene una instalación por su ID.
    /// </summary>
    /// <param name="id">El ID de la instalación a obtener.</param>
    /// <returns>La instalación encontrada.</returns>
    public async Task<dynamic> GetFacilityById(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo instalación por ID: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryMultipleAsync("100_GetFacilityById", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadSingleAsync<DTOFacility>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la instalación por ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las instalaciones.
    /// </summary>
    /// <param name="take">El número de instalaciones a tomar.</param>
    /// <param name="skip">El número de instalaciones a saltar.</param>
    /// <param name="name">El nombre de la instalación a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las instalaciones.</param>
    /// <returns>Una lista de instalaciones.</returns>
    public async Task<dynamic> GetAllFacilities(int take, int skip, string name, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las instalaciones");
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllFacilities", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOFacility>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las instalaciones");
            throw;
        }
    }

    /// <summary>
    /// Inserta una nueva instalación.
    /// </summary>
    /// <param name="facility">La instalación a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertFacility(DTOFacility facility)
    {
        try
        {
            _logger.LogInformation("Insertando instalación: {Name}", facility.Name);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", facility.Name, DbType.String);
            parameters.Add("@id", facility.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertFacility", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");
            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la instalación: {Name}", facility.Name);
            throw;
        }
    }

    /// <summary>
    /// Actualiza una instalación existente.
    /// </summary>
    /// <param name="facility">La instalación a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateFacility(DTOFacility facility)
    {
        try
        {
            _logger.LogInformation("Actualizando instalación: {Name}", facility.Name);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", facility.Id, DbType.Int32);
            parameters.Add("@name", facility.Name, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateFacility", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la instalación: {Name}", facility.Name);
            throw;
        }
    }

    /// <summary>
    /// Elimina una instalación por su ID.
    /// </summary>
    /// <param name="id">El ID de la instalación a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteFacility(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando instalación: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteFacility", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la instalación: {Id}", id);
            throw;
        }
    }
}