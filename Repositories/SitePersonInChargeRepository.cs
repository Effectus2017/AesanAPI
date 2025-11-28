using Api.Data;
using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Api.Services;
using Dapper;
using System.Collections.Generic;
using System.Data;

namespace Api.Repositories;

/// <summary>
/// Repositorio para gestionar la información de Persona a Cargo de los sitios
/// </summary>
public class SitePersonInChargeRepository(DapperContext context, ILoggingService loggingService) : ISitePersonInChargeRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILoggingService _logger = loggingService ?? throw new ArgumentNullException(nameof(loggingService));

    /// <summary>
    /// Inserta información de Persona a Cargo para un sitio
    /// </summary>
    public async Task<int> InsertSitePersonInCharge(int siteId, SitePersonInChargeRequest request, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        IDbConnection? dbConnection = null;
        var shouldDisposeConnection = connection == null;

        try
        {
            _logger.LogInformation($"Insertando información de Persona a Cargo para el sitio {siteId}");

            dbConnection = connection ?? _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@firstName", request.FirstName, DbType.String);
            parameters.Add("@middleName", request.MiddleName, DbType.String);
            parameters.Add("@fatherLastName", request.FatherLastName, DbType.String);
            parameters.Add("@motherLastName", request.MotherLastName, DbType.String);
            parameters.Add("@sitePhone", request.SitePhone, DbType.String);
            parameters.Add("@extension", request.Extension, DbType.String);
            parameters.Add("@mobilePhone", request.MobilePhone, DbType.String);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSitePersonInCharge", parameters, transaction, commandType: CommandType.StoredProcedure);

            int id = parameters.Get<int>("@id");

            _logger.LogInformation($"Información de Persona a Cargo insertada exitosamente con ID {id} para el sitio {siteId}");
            return id;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al insertar información de Persona a Cargo", new Dictionary<string, string>
            {
                { "SiteId", siteId.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new Exception($"Error al insertar información de Persona a Cargo: {ex.Message}", ex);
        }
        finally
        {
            if (shouldDisposeConnection && dbConnection != null)
            {
                dbConnection.Dispose();
            }
        }
    }

    /// <summary>
    /// Actualiza información de Persona a Cargo para un sitio
    /// </summary>
    public async Task<bool> UpdateSitePersonInCharge(int siteId, SitePersonInChargeRequest request)
    {
        try
        {
            _logger.LogInformation($"Actualizando información de Persona a Cargo para el sitio {siteId}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);
            parameters.Add("@firstName", request.FirstName, DbType.String);
            parameters.Add("@middleName", request.MiddleName, DbType.String);
            parameters.Add("@fatherLastName", request.FatherLastName, DbType.String);
            parameters.Add("@motherLastName", request.MotherLastName, DbType.String);
            parameters.Add("@sitePhone", request.SitePhone, DbType.String);
            parameters.Add("@extension", request.Extension, DbType.String);
            parameters.Add("@mobilePhone", request.MobilePhone, DbType.String);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await connection.ExecuteAsync("100_UpdateSitePersonInCharge", parameters, commandType: CommandType.StoredProcedure);

            int rowsAffected = parameters.Get<int>("@rowsAffected");

            _logger.LogInformation($"Información de Persona a Cargo actualizada exitosamente para el sitio {siteId}. Filas afectadas: {rowsAffected}");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al actualizar información de Persona a Cargo", new Dictionary<string, string>
            {
                { "SiteId", siteId.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new Exception($"Error al actualizar información de Persona a Cargo: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene información de Persona a Cargo por SiteId
    /// </summary>
    public async Task<SitePersonInChargeResponse?> GetSitePersonInChargeBySiteId(int siteId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo información de Persona a Cargo para el sitio {siteId}");

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@siteId", siteId, DbType.Int32);

            var result = await connection.QueryFirstOrDefaultAsync<SitePersonInChargeResponse>(
                "100_GetSitePersonInChargeBySiteId", 
                parameters, 
                commandType: CommandType.StoredProcedure);

            if (result != null)
            {
                _logger.LogInformation($"Información de Persona a Cargo encontrada para el sitio {siteId}");
            }
            else
            {
                _logger.LogInformation($"No se encontró información de Persona a Cargo para el sitio {siteId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            await _logger.LogError(ex, "Error al obtener información de Persona a Cargo", new Dictionary<string, string>
            {
                { "SiteId", siteId.ToString() },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new Exception($"Error al obtener información de Persona a Cargo: {ex.Message}", ex);
        }
    }
}

