using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class MealTypeRepository(DapperContext context, ILogger<MealTypeRepository> logger) : IMealTypeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<MealTypeRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene un tipo de comida por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de comida a obtener.</param>
    /// <returns>El tipo de comida encontrado.</returns>
    public async Task<dynamic> GetMealTypeById(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo tipo de comida por ID: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Id", id, DbType.Int32);
            var result = await db.QueryMultipleAsync("100_GetMealTypeById", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadSingleAsync<DTOMealType>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de comida por ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de comida.
    /// </summary>
    /// <param name="take">El número de tipos de comida a tomar.</param>
    /// <param name="skip">El número de tipos de comida a saltar.</param>
    /// <param name="name">El nombre del tipo de comida a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los tipos de comida.</param>
    /// <returns>Una lista de tipos de comida.</returns>
    public async Task<dynamic> GetAllMealTypes(int take, int skip, string name, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los tipos de comida");
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllMealTypes", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOMealType>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de comida");
            throw;
        }
    }

    /// <summary>
    /// Inserta un nuevo tipo de comida.
    /// </summary>
    /// <param name="mealType">El tipo de comida a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertMealType(DTOMealType mealType)
    {
        try
        {
            _logger.LogInformation("Insertando tipo de comida: {Name}", mealType.Name);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", mealType.Name, DbType.String);
            parameters.Add("@id", mealType.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertMealType", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");
            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el tipo de comida: {Name}", mealType.Name);
            throw;
        }
    }

    /// <summary>
    /// Actualiza un tipo de comida existente.
    /// </summary>
    /// <param name="mealType">El tipo de comida a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateMealType(DTOMealType mealType)
    {
        try
        {
            _logger.LogInformation("Actualizando tipo de comida: {Name}", mealType.Name);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", mealType.Id, DbType.Int32);
            parameters.Add("@name", mealType.Name, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateMealType", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de comida: {Name}", mealType.Name);
            throw;
        }
    }

    /// <summary>
    /// Elimina un tipo de comida por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de comida a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteMealType(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando tipo de comida: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteMealType", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de comida: {Id}", id);
            throw;
        }
    }
}