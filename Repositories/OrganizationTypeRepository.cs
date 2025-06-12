using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Api.Models.DTO;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class OrganizationTypeRepository(DapperContext context, ILogger<OrganizationTypeRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings) : IOrganizationTypeRepository
{
    private readonly DapperContext _context = context;
    private readonly ILogger<OrganizationTypeRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings.Value;

    /// <summary>
    /// Obtiene un tipo de organización por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de organización a obtener.</param>
    /// <returns>El tipo de organización encontrado.</returns>
    public async Task<dynamic> GetOrganizationTypeById(int id)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);
        var result = await db.QueryFirstOrDefaultAsync<DTOOrganizationType>("100_GetOrganizationTypeById", parameters, commandType: CommandType.StoredProcedure);
        return result;
    }

    /// <summary>
    /// Obtiene todos los tipos de organización.
    /// </summary>
    /// <param name="take">El número de tipos de organización a tomar.</param>
    /// <param name="skip">El número de tipos de organización a saltar.</param>
    /// <param name="name">El nombre del tipo de organización a buscar.</param>
    /// <param name="alls">Si se deben obtener todos los tipos de organización.</param>
    /// <returns>Una lista de tipos de organización.</returns>
    public async Task<dynamic> GetAllOrganizationTypes(int take, int skip, string name, bool alls)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@take", take, DbType.Int32);
        parameters.Add("@skip", skip, DbType.Int32);
        parameters.Add("@name", name, DbType.String);
        parameters.Add("@alls", alls, DbType.Boolean);
        var result = await db.QueryMultipleAsync("100_GetAllOrganizationTypes", parameters, commandType: CommandType.StoredProcedure);
        var data = await result.ReadAsync<DTOOrganizationType>();
        var count = await result.ReadSingleAsync<int>();
        return new { data, count };
    }

    /// <summary>
    /// Inserta un nuevo tipo de organización.
    /// </summary>
    /// <param name="organizationType">El tipo de organización a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertOrganizationType(DTOOrganizationType organizationType)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@name", organizationType.Name, DbType.String);
        parameters.Add("@nameEN", organizationType.NameEN, DbType.String);
        parameters.Add("@isActive", organizationType.IsActive, DbType.Boolean);
        parameters.Add("@displayOrder", organizationType.DisplayOrder, DbType.Int32);
        parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);
        await db.ExecuteAsync("100_InsertOrganizationType", parameters, commandType: CommandType.StoredProcedure);
        var id = parameters.Get<int>("@id");
        return id > 0;
    }

    /// <summary>
    /// Actualiza un tipo de organización existente.
    /// </summary>
    /// <param name="organizationType">El tipo de organización a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateOrganizationType(DTOOrganizationType organizationType)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@id", organizationType.Id, DbType.Int32);
        parameters.Add("@name", organizationType.Name, DbType.String);
        parameters.Add("@nameEN", organizationType.NameEN, DbType.String);
        parameters.Add("@isActive", organizationType.IsActive, DbType.Boolean);
        parameters.Add("@displayOrder", organizationType.DisplayOrder, DbType.Int32);
        var affected = await db.ExecuteAsync("100_UpdateOrganizationType", parameters, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }

    /// <summary>
    /// Elimina un tipo de organización por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de organización a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteOrganizationType(int id)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@id", id, DbType.Int32);
        var affected = await db.ExecuteAsync("100_DeleteOrganizationType", parameters, commandType: CommandType.StoredProcedure);
        return affected > 0;
    }
}