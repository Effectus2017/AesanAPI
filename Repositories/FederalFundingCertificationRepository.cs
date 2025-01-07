using System.Data;
using Api.Data;
using Api.Interfaces;
using Api.Models;
using Dapper;

namespace Api.Repositories;

public class FederalFundingCertificationRepository(DapperContext context, ILogger<FederalFundingCertificationRepository> logger) : IFederalFundingCertificationRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<FederalFundingCertificationRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene una certificación de fondos federales por su ID.
    /// </summary>
    /// <param name="id">El ID de la certificación a obtener.</param>
    /// <returns>La certificación encontrada.</returns>
    public async Task<dynamic> GetFederalFundingCertificationById(int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo certificación de fondos federales por ID: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            var result = await db.QueryMultipleAsync("100_GetFederalFundingCertificationById", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadSingleAsync<DTOFederalFundingCertification>();
            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la certificación de fondos federales por ID: {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Obtiene todas las certificaciones de fondos federales.
    /// </summary>
    /// <param name="take">El número de certificaciones a tomar.</param>
    /// <param name="skip">El número de certificaciones a saltar.</param>
    /// <param name="description">La descripción de la certificación a buscar.</param>
    /// <param name="alls">Si se deben obtener todas las certificaciones.</param>
    /// <returns>Una lista de certificaciones de fondos federales.</returns>
    public async Task<dynamic> GetAllFederalFundingCertifications(int take, int skip, string description, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todas las certificaciones de fondos federales");
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@description", description, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            var result = await db.QueryMultipleAsync("100_GetAllFederalFundingCertifications", parameters, commandType: CommandType.StoredProcedure);
            var data = await result.ReadAsync<DTOFederalFundingCertification>();
            var count = await result.ReadSingleAsync<int>();
            return new { data, count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las certificaciones de fondos federales");
            throw;
        }
    }

    /// <summary>
    /// Inserta una nueva certificación de fondos federales.
    /// </summary>
    /// <param name="certification">La certificación a insertar.</param>
    /// <returns>True si la inserción es exitosa, false en caso contrario.</returns>
    public async Task<bool> InsertFederalFundingCertification(DTOFederalFundingCertification certification)
    {
        try
        {
            _logger.LogInformation("Insertando certificación de fondos federales: {Description}", certification.Description);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@fundingAmount", certification.FundingAmount, DbType.Decimal);
            parameters.Add("@description", certification.Description, DbType.String);
            parameters.Add("@id", certification.Id, DbType.Int32, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_InsertFederalFundingCertification", parameters, commandType: CommandType.StoredProcedure);
            var id = parameters.Get<int>("@id");
            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la certificación de fondos federales: {Description}", certification.Description);
            throw;
        }
    }

    /// <summary>
    /// Actualiza una certificación de fondos federales existente.
    /// </summary>
    /// <param name="certification">La certificación a actualizar.</param>
    /// <returns>True si la actualización es exitosa, false en caso contrario.</returns>
    public async Task<bool> UpdateFederalFundingCertification(DTOFederalFundingCertification certification)
    {
        try
        {
            _logger.LogInformation("Actualizando certificación de fondos federales: {Description}", certification.Description);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", certification.Id, DbType.Int32);
            parameters.Add("@fundingAmount", certification.FundingAmount, DbType.Decimal);
            parameters.Add("@description", certification.Description, DbType.String);
            parameters.Add("@updatedAt", DateTime.UtcNow, DbType.DateTime);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_UpdateFederalFundingCertification", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la certificación de fondos federales: {Description}", certification.Description);
            throw;
        }
    }

    /// <summary>
    /// Elimina una certificación de fondos federales por su ID.
    /// </summary>
    /// <param name="id">El ID de la certificación a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    public async Task<bool> DeleteFederalFundingCertification(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando certificación de fondos federales: {Id}", id);
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("100_DeleteFederalFundingCertification", parameters, commandType: CommandType.StoredProcedure);
            var rowsAffected = parameters.Get<int>("@rowsAffected");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la certificación de fondos federales: {Id}", id);
            throw;
        }
    }
}