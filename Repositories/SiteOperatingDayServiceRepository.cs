using System.Data;
using Dapper;
using Api.Data;
using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.Extensions.Logging;

namespace Api.Repositories;

/// <summary>
/// Repositorio para la gestión de servicios de alimentación por día de funcionamiento
/// Implementa las operaciones CRUD para servicios relacionados con días operativos
/// </summary>
public class SiteOperatingDayServiceRepository(DapperContext context, ILogger<SiteOperatingDayServiceRepository> logger) : ISiteOperatingDayServiceRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<SiteOperatingDayServiceRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene todos los servicios de un día de funcionamiento
    /// </summary>
    public async Task<List<SiteOperatingDayServiceResponse>> GetServicesByOperatingDay(int operatingDayId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@operatingDayId", operatingDayId, DbType.Int32);

            var services = await dbConnection.QueryAsync<SiteOperatingDayServiceResponse>("100_GetSiteOperatingDayServices", parameters, commandType: CommandType.StoredProcedure);

            var servicesList = services.ToList();

            _logger.LogInformation("Se obtuvieron {Count} servicios para el día de funcionamiento {OperatingDayId}",
                servicesList.Count, operatingDayId);

            return servicesList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener servicios para el día de funcionamiento {OperatingDayId}", operatingDayId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene un servicio por su ID
    /// </summary>
    public async Task<SiteOperatingDayServiceResponse?> GetServiceById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var service = await dbConnection.QueryFirstOrDefaultAsync<SiteOperatingDayServiceResponse>("100_GetSiteOperatingDayServiceById", parameters, commandType: CommandType.StoredProcedure);

            return service;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener servicio con ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Crea un nuevo servicio para un día de funcionamiento
    /// </summary>
    public async Task<int> CreateService(SiteOperatingDayServiceRequest request)
    {
        try
        {
            if (request.OperatingDayId == null || request.OperatingDayId <= 0)
            {
                throw new ArgumentException("OperatingDayId es requerido para crear un servicio", nameof(request));
            }

            // Validar horarios antes de crear
            var isValid = await ValidateServiceTimeRange(request.OperatingDayId.Value, request.StartTime, request.EndTime);
            if (!isValid)
            {
                throw new ArgumentException("Los horarios del servicio deben estar dentro del rango del día de funcionamiento");
            }

            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@operatingDayId", request.OperatingDayId, DbType.Int32);
            parameters.Add("@serviceTypeId", request.ServiceTypeId, DbType.Int32);
            parameters.Add("@childGroupId", request.ChildGroupId, DbType.Int32);
            parameters.Add("@startTime", request.StartTime, DbType.Time);
            parameters.Add("@endTime", request.EndTime, DbType.Time);
            parameters.Add("@isEnabled", request.IsEnabled, DbType.Boolean);
            parameters.Add("@comment", request.Comment, DbType.String);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSiteOperatingDayService", parameters, commandType: CommandType.StoredProcedure);

            var id = parameters.Get<int>("@id");

            _logger.LogInformation("Servicio creado con ID {Id} para el día de funcionamiento {OperatingDayId}",
                id, request.OperatingDayId);

            return id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear servicio para el día de funcionamiento {OperatingDayId}",
                request.OperatingDayId);
            throw;
        }
    }

    /// <summary>
    /// Crea múltiples servicios en batch para optimizar performance
    /// </summary>
    public async Task<int> CreateServicesBatch(List<SiteOperatingDayServiceRequest> requests, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var shouldDisposeConnection = connection == null;
        IDbConnection? dbConnection = null;

        try
        {
            if (requests == null || requests.Count == 0)
            {
                return 0;
            }

            // Validaciones básicas antes de enviar a BD
            foreach (var request in requests)
            {
                if (request.OperatingDayId == null || request.OperatingDayId <= 0)
                {
                    throw new ArgumentException($"OperatingDayId es requerido para todos los servicios");
                }
            }

            dbConnection = connection ?? _context.CreateConnection();

            // Crear DataTable para Table-Valued Parameter
            var dataTable = new DataTable();
            dataTable.Columns.Add("OperatingDayId", typeof(int));
            dataTable.Columns.Add("ServiceTypeId", typeof(int));
            dataTable.Columns.Add("ChildGroupId", typeof(int));
            dataTable.Columns.Add("StartTime", typeof(TimeSpan));
            dataTable.Columns.Add("EndTime", typeof(TimeSpan));
            dataTable.Columns.Add("IsEnabled", typeof(bool));
            dataTable.Columns.Add("Comment", typeof(string));

            // Llenar DataTable
            foreach (var request in requests)
            {
                dataTable.Rows.Add(
                    request.OperatingDayId!.Value,
                    request.ServiceTypeId,
                    request.ChildGroupId ?? (object)DBNull.Value,
                    request.StartTime,
                    request.EndTime,
                    request.IsEnabled,
                    request.Comment ?? (object)DBNull.Value
                );
            }

            var parameters = new DynamicParameters();
            parameters.Add("@services", dataTable.AsTableValuedParameter("SiteOperatingDayServiceBatchType"));
            parameters.Add("@rowsInserted", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertSiteOperatingDayServicesBatch", parameters, transaction, commandType: CommandType.StoredProcedure);

            var rowsInserted = parameters.Get<int>("@rowsInserted");

            _logger.LogInformation("Se crearon {RowsInserted} servicios en batch de {TotalRequests} solicitados",
                rowsInserted, requests.Count);

            return rowsInserted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear servicios en batch");
            throw;
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
    /// Actualiza un servicio existente
    /// </summary>
    public async Task<bool> UpdateService(int id, SiteOperatingDayServiceRequest request)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@startTime", request.StartTime != default ? request.StartTime : (TimeSpan?)null, DbType.Time);
            parameters.Add("@endTime", request.EndTime != default ? request.EndTime : (TimeSpan?)null, DbType.Time);
            parameters.Add("@isEnabled", request.IsEnabled, DbType.Boolean);
            parameters.Add("@comment", request.Comment, DbType.String);

            await dbConnection.ExecuteAsync("100_UpdateSiteOperatingDayService", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation("Servicio con ID {Id} actualizado exitosamente", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar servicio con ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Elimina un servicio
    /// </summary>
    public async Task<bool> DeleteService(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            await dbConnection.ExecuteAsync("100_DeleteSiteOperatingDayService", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation("Servicio con ID {Id} eliminado exitosamente", id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar servicio con ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Habilita o deshabilita un servicio
    /// </summary>
    public async Task<bool> ToggleService(int id, bool isEnabled)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);
            parameters.Add("@isEnabled", isEnabled, DbType.Boolean);

            await dbConnection.ExecuteAsync("100_ToggleSiteOperatingDayService", parameters, commandType: CommandType.StoredProcedure);

            _logger.LogInformation("Servicio con ID {Id} {Status} exitosamente",
                id, isEnabled ? "habilitado" : "deshabilitado");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cambiar estado del servicio con ID {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Valida que los horarios del servicio estén dentro del rango del día de funcionamiento
    /// </summary>
    public async Task<bool> ValidateServiceTimeRange(int operatingDayId, TimeSpan startTime, TimeSpan endTime)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@operatingDayId", operatingDayId, DbType.Int32);
            parameters.Add("@startTime", startTime, DbType.Time);
            parameters.Add("@endTime", endTime, DbType.Time);
            parameters.Add("@isValid", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_ValidateServiceTimeRange", parameters, commandType: CommandType.StoredProcedure);

            var isValid = parameters.Get<bool>("@isValid");

            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar horarios del servicio para el día {OperatingDayId}", operatingDayId);
            throw;
        }
    }
}

