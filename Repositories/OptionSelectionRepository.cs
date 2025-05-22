using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

/// <summary>
/// Repository for managing option selections in the database
/// Repositorio para gestionar selecciones de opciones en la base de datos
/// </summary>
public class OptionSelectionRepository(DapperContext context, ILogger<OptionSelectionRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IOptionSelectionRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<OptionSelectionRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));

    /// <summary>
    /// Gets a single option selection by its ID
    /// Obtiene una selección de opción por su ID
    /// </summary>
    /// <param name="id">The ID of the option selection to retrieve/El ID de la selección de opción a recuperar</param>
    /// <returns>The option selection data/Los datos de la selección de opción</returns>
    public async Task<dynamic> GetOptionSelectionById(int id)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);
        var result = await db.QueryMultipleAsync("100_GetOptionSelectionById", parameters, commandType: CommandType.StoredProcedure);
        var data = await result.ReadSingleAsync<DTOOptionSelection>();
        return data;
    }

    /// <summary>
    /// Gets all option selections with pagination and filtering
    /// Obtiene todas las selecciones de opciones con paginación y filtrado
    /// </summary>
    /// <param name="take">Number of records to take/Número de registros a tomar</param>
    /// <param name="skip">Number of records to skip/Número de registros a omitir</param>
    /// <param name="name">Name filter (optional)/Filtro de nombre (opcional)</param>
    /// <param name="alls">Whether to get all records ignoring pagination/Si obtener todos los registros ignorando la paginación</param>
    /// <returns>Object containing the list of option selections and total count/Objeto que contiene la lista de selecciones de opciones y el conteo total</returns>
    public async Task<dynamic> GetAllOptionSelections(int take, int skip, string name, string optionKey, bool alls)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@optionKey", optionKey, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllOptionSelections", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOOptionSelection>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las opciones de selección/Error getting option selections");
            throw;
        }
    }

    /// <summary>
    /// Inserts a new option selection into the database
    /// Inserta una nueva selección de opción en la base de datos
    /// </summary>
    /// <param name="optionSelection">The option selection data to insert/Los datos de la selección de opción a insertar</param>
    /// <returns>True if insertion was successful/Verdadero si la inserción fue exitosa</returns>
    public async Task<bool> InsertOptionSelection(DTOOptionSelection optionSelection)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@name", optionSelection.Name, DbType.String);
            parameters.Add("@nameEN", optionSelection.NameEN, DbType.String);
            parameters.Add("@optionKey", optionSelection.OptionKey, DbType.String);
            parameters.Add("@isActive", optionSelection.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", optionSelection.DisplayOrder, DbType.Int32);
            parameters.Add("@id", optionSelection.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertOptionSelection", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");

            if (id > 0)
            {
                InvalidateCache(id);
            }

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la opción de selección/Error inserting option selection");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing option selection
    /// Actualiza una selección de opción existente
    /// </summary>
    /// <param name="optionSelection">The option selection data to update/Los datos de la selección de opción a actualizar</param>
    /// <returns>True if update was successful/Verdadero si la actualización fue exitosa</returns>
    public async Task<bool> UpdateOptionSelection(DTOOptionSelection optionSelection)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", optionSelection.Id, DbType.Int32);
            parameters.Add("@name", optionSelection.Name, DbType.String);
            parameters.Add("@nameEN", optionSelection.NameEN, DbType.String);
            parameters.Add("@optionKey", optionSelection.OptionKey, DbType.String);
            parameters.Add("@isActive", optionSelection.IsActive, DbType.Boolean);
            parameters.Add("@displayOrder", optionSelection.DisplayOrder, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateOptionSelection", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(optionSelection.Id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la opción de selección/Error updating option selection");
            throw;
        }
    }

    /// <summary>
    /// Updates the display order of an option selection
    /// Actualiza el orden de visualización de una selección de opción
    /// </summary>
    /// <param name="optionSelectionId">ID of the option selection to update/ID de la selección de opción a actualizar</param>
    /// <param name="displayOrder">New display order value/Nuevo valor de orden de visualización</param>
    /// <returns>True if update was successful/Verdadero si la actualización fue exitosa</returns>
    public async Task<bool> UpdateOptionSelectionDisplayOrder(int optionSelectionId, int displayOrder)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@optionSelectionId", optionSelectionId, DbType.Int32);
            parameters.Add("@displayOrder", displayOrder, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("105_UpdateOptionSelectionDisplayOrder", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(optionSelectionId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el orden de visualización de la opción de selección/Error updating option selection display order");
            throw;
        }
    }

    /// <summary>
    /// Deletes an option selection by ID
    /// Elimina una selección de opción por ID
    /// </summary>
    /// <param name="id">ID of the option selection to delete/ID de la selección de opción a eliminar</param>
    /// <returns>True if deletion was successful/Verdadero si la eliminación fue exitosa</returns>
    public async Task<bool> DeleteOptionSelection(int id)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteOptionSelection", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                InvalidateCache(id);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la opción de selección");
            throw;
        }
    }

    /// <summary>
    /// Invalidates the cache for option selections
    /// </summary>
    /// <param name="optionSelectionId">Optional specific option selection ID to invalidate</param>
    private void InvalidateCache(int? optionSelectionId = null)
    {
        if (optionSelectionId.HasValue)
        {
            _cache.Remove($"OptionSelection_{optionSelectionId}");
        }
        _cache.Remove("OptionSelections");
        _logger.LogInformation("Cache invalidado para OptionSelection Repository");
    }
}
