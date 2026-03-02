using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Models.Response;
using Api.Services;
using Api.Services.Mappers;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class StaffRepository(
    DapperContext context,
    ILogger<StaffRepository> logger,
    IMemoryCache cache,
    IOptions<ApplicationSettings> appSettings,
    MappingService mappingService,
    IAuditLogger auditLogger,
    ISiteStaffRepository siteStaffRepository,
    IAgencyRepository agencyRepository,
    MessageTemplateService messageTemplateService
) : IStaffRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<StaffRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMemoryCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly ApplicationSettings _appSettings = appSettings.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));
    private readonly IAuditLogger _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
    private readonly ISiteStaffRepository _siteStaffRepository = siteStaffRepository ?? throw new ArgumentNullException(nameof(siteStaffRepository));
    private readonly IAgencyRepository _agencyRepository = agencyRepository ?? throw new ArgumentNullException(nameof(agencyRepository));
    private readonly MessageTemplateService _messageTemplateService = messageTemplateService ?? throw new ArgumentNullException(nameof(messageTemplateService));

    /// <summary>
    /// Obtiene un miembro del staff por su ID
    /// </summary>
    /// <param name="id">El ID del miembro del staff</param>
    /// <returns>El miembro del staff</returns>
    public async Task<dynamic> GetStaffById(int id)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@id", id, DbType.Int32);

            using var result = await dbConnection.QueryMultipleAsync("101_GetStaffById", param, commandType: CommandType.StoredProcedure);

            var data = await result.ReadFirstOrDefaultAsync<dynamic>();

            if (data == null)
            {
                return null;
            }

            var staff = _mappingService.MapStaffDetails(data);

            // Segundo result set: SalaryOrigins (tabla StaffSalaryOrigin), mismo patrón que boardExecutiveAuthority en 113/114_GetAgencyById
            var salaryOrigins = await result.ReadAsync<dynamic>();

            staff.SalaryOrigins = _mappingService.MapOptionSelections(salaryOrigins);

            // Tercer result set: Contratos por clasificación (StaffContractByClassification), mismo patrón que segundo result set
            var contracts = await result.ReadAsync<dynamic>();
            staff.ClassificationContracts = StaffMapper.MapContractByClassificationList(contracts);

            return staff;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el miembro del staff con ID {StaffId}", id);
            throw new Exception($"Error al obtener el miembro del staff con ID {id}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene un miembro del staff por su UserId
    /// </summary>
    /// <param name="userId">El UserId del usuario asociado al staff</param>
    /// <returns>El miembro del staff o null si no existe</returns>
    public async Task<Staff?> GetStaffByUserId(string userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new DynamicParameters();
            param.Add("@userId", userId, DbType.String);

            var result = await dbConnection.QueryFirstOrDefaultAsync<Staff>("100_GetStaffByUserId", param, commandType: CommandType.StoredProcedure);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el miembro del staff con UserId {UserId}", userId);
            throw new Exception($"Error al obtener el miembro del staff con UserId {userId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los miembros del staff de la base de datos
    /// </summary>
    /// <param name="take">El número de miembros del staff a obtener</param>
    /// <param name="skip">El número de miembros del staff a saltar</param>
    /// <param name="name">El nombre del miembro del staff a buscar</param>
    /// <param name="alls">Si se deben obtener todos los miembros del staff</param>
    /// <param name="excludeRelated">Si es true, excluye staff ya relacionado en StaffRelationship (usado en modal Add)</param>
    /// <param name="isList">DEPRECATED - Usar excludeRelated en su lugar (mantenido por compatibilidad)</param>
    /// <param name="staffTypeId">ID del tipo de staff para filtrar</param>
    /// <returns>Los miembros del staff</returns>
    public async Task<dynamic> GetAllStaffFromDb(int take, int skip, string name, bool alls, bool excludeRelated, bool isList, int? staffTypeId = null, int? agencyId = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@take", take, DbType.Int32);
            param.Add("@skip", skip, DbType.Int32);
            param.Add("@name", name, DbType.String);
            param.Add("@alls", alls, DbType.Boolean);
            param.Add("@staffTypeId", staffTypeId, DbType.Int32);
            param.Add("@agencyId", agencyId == 0 ? null : agencyId, DbType.Int32);
            param.Add("@excludeRelated", excludeRelated, DbType.Boolean);

            if (isList)
            {
                var result = await dbConnection.QueryMultipleAsync("100_GetAllStaff", param, commandType: CommandType.StoredProcedure);

                if (result == null)
                    return null;

                var data = result.Read<dynamic>()
                    .Select(_mappingService.MapStaffDropdownItem)
                    .OfType<StaffDropdownItemResponse>()
                    .ToList();
                return data;
            }
            else
            {
                var result = await dbConnection.QueryMultipleAsync("100_GetAllStaff", param, commandType: CommandType.StoredProcedure);

                if (result == null)
                    return null;

                var data = result.Read<dynamic>()
                    .Select(_mappingService.MapStaffList)
                    .OfType<StaffTableResponse>()
                    .ToList();
                var count = result.Read<int>().FirstOrDefault();

                return new PagedResult<StaffTableResponse> { Data = data, Count = count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los miembros del staff");
            throw new Exception($"Error al obtener los miembros del staff: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Inserta un nuevo miembro del staff en la base de datos
    /// </summary>
    /// <param name="staffRequest">Datos del miembro del staff a insertar</param>
    /// <param name="staffId">ID del staff creado (solo si la inserción fue exitosa)</param>
    /// <returns>ID del staff creado</returns>
    public async Task<int> InsertStaff(StaffRequest staffRequest)
    {
        try
        {
            _logger.LogInformation("Insertando nuevo miembro del staff - Email: {Email}, PositionId: {PositionId}, StaffTypeId: {StaffTypeId}, StatusId: {StatusId}, UserId: {UserId}", staffRequest.Email, staffRequest.PositionId, staffRequest.StaffTypeId, staffRequest.StatusId, staffRequest.UserId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@firstName", staffRequest.FirstName ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@middleName", staffRequest.MiddleName ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@fatherLastName", staffRequest.FatherLastName ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@motherLastName", staffRequest.MotherLastName ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@statusId", staffRequest.StatusId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@positionId", staffRequest.PositionId, DbType.Int32, ParameterDirection.Input);
            // StaffTypeId: usar 1 por defecto si es 0
            var finalStaffTypeId = staffRequest.StaffTypeId == 0 ? 1 : staffRequest.StaffTypeId;
            parameters.Add("@staffTypeId", finalStaffTypeId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@staffClassificationId", staffRequest.StaffClassificationId, DbType.Int32, ParameterDirection.Input);
            // Fechas seguras para SQL Server
            parameters.Add("@contractStartDate", staffRequest.ContractStartDate?.Year >= 1753 ? staffRequest.ContractStartDate : DBNull.Value, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@contractEndDate", staffRequest.ContractEndDate?.Year >= 1753 ? staffRequest.ContractEndDate : DBNull.Value, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@birthDate", staffRequest.BirthDate?.Year >= 1753 ? staffRequest.BirthDate : DBNull.Value, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@email", staffRequest.Email ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@phoneNumber", staffRequest.PhoneNumber ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@postalAddress", staffRequest.PostalAddress ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@cityId", staffRequest.CityId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@regionId", staffRequest.RegionId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@zipCode", staffRequest.ZipCode ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@agencyId", staffRequest.AgencyId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@comments", staffRequest.Comments ?? "", DbType.String, ParameterDirection.Input);
            parameters.Add("@userId", staffRequest.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("@reviewResultId", staffRequest.ReviewResultId, DbType.Int32, ParameterDirection.Input);
            // Fecha de revisión segura
            parameters.Add("@reviewDate", staffRequest.ReviewDate?.Year >= 1753 ? staffRequest.ReviewDate : DBNull.Value, DbType.DateTime, ParameterDirection.Input);
            parameters.Add("@reviewJustification", staffRequest.ReviewJustification ?? "", DbType.String, ParameterDirection.Input);
            // Agregar parámetros para miembros de la junta
            parameters.Add("@tenureDuration", staffRequest.TenureDuration, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@tenureDurationUnitId", staffRequest.TenureDurationUnitId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@receivesProgramSalaryId", staffRequest.ReceivesProgramSalaryId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("101_InsertStaff", parameters, commandType: CommandType.StoredProcedure);

            int staffId = parameters.Get<int>("@id");

            if (staffRequest.SalaryOrigins != null)
            {
                foreach (var item in staffRequest.SalaryOrigins)
                {
                    await InsertStaffSalaryOrigin(staffId, item.Id, dbConnection, null);
                }
            }

            {
                // Si se proporcionó una sitio, crear la asociación
                if (staffRequest.SiteId.HasValue && staffRequest.SiteId.Value > 0)
                {
                    _logger.LogInformation("Asignando staff {StaffId} a la sitio {SiteId}", staffId, staffRequest.SiteId.Value);

                    var siteStaffRequest = new SiteStaffRequest
                    {
                        SiteId = staffRequest.SiteId.Value,
                        StaffId = staffId,
                        IsPrimary = staffRequest.IsPrimary,
                        Comments = $"Asignación creada automáticamente al crear el staff",
                    };

                    try
                    {
                        await _siteStaffRepository.AssignStaffToSite(siteStaffRequest);
                        _logger.LogInformation("Staff {StaffId} asignado exitosamente a la sitio {SiteId}", staffId, staffRequest.SiteId.Value);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error al asignar staff {StaffId} a la sitio {SiteId}, pero el staff fue creado exitosamente", staffId, staffRequest.SiteId.Value);
                        // No lanzar excepción aquí porque el staff ya fue creado exitosamente
                    }
                }
            }

            if (staffRequest.ClassificationContracts != null)
            {
                await DeleteStaffContractByClassification(staffId, dbConnection, null);
                foreach (var contract in staffRequest.ClassificationContracts)
                {
                    await InsertStaffContractByClassification(staffId, contract, dbConnection, null);
                }
            }

            if (staffId <= 0)
            {
                throw new Exception($"Error al insertar el miembro del staff: El stored procedure no retornó un ID válido. Esto puede indicar un error de foreign key constraint o un problema con los datos enviados.");
            }

            return staffId;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al insertar el miembro del staff: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza un miembro del staff existente en la base de datos
    /// </summary>
    /// <param name="staffRequest">Datos del miembro del staff a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateStaff(StaffRequest staffRequest)
    {
        try
        {
            _logger.LogInformation("Actualizando miembro del staff con ID {StaffId}", staffRequest.Id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", staffRequest.Id, DbType.Int32);
            parameters.Add("@firstName", staffRequest.FirstName ?? "", DbType.String);
            parameters.Add("@middleName", staffRequest.MiddleName ?? "", DbType.String);
            parameters.Add("@fatherLastName", staffRequest.FatherLastName ?? "", DbType.String);
            parameters.Add("@motherLastName", staffRequest.MotherLastName ?? "", DbType.String);
            parameters.Add("@statusId", staffRequest.StatusId, DbType.Int32);
            parameters.Add("@positionId", staffRequest.PositionId, DbType.Int32);
            parameters.Add("@staffTypeId", staffRequest.StaffTypeId, DbType.Int32);
            parameters.Add("@staffClassificationId", staffRequest.StaffClassificationId, DbType.Int32);

            // Fechas seguras para SQL Server
            parameters.Add("@contractStartDate", staffRequest.ContractStartDate?.Year >= 1753 ? staffRequest.ContractStartDate : DBNull.Value, DbType.DateTime);
            parameters.Add("@contractEndDate", staffRequest.ContractEndDate?.Year >= 1753 ? staffRequest.ContractEndDate : DBNull.Value, DbType.DateTime);
            parameters.Add("@birthDate", staffRequest.BirthDate?.Year >= 1753 ? staffRequest.BirthDate : DBNull.Value, DbType.DateTime);

            parameters.Add("@email", staffRequest.Email ?? "", DbType.String);
            parameters.Add("@phoneNumber", staffRequest.PhoneNumber ?? "", DbType.String);
            parameters.Add("@postalAddress", staffRequest.PostalAddress ?? "", DbType.String);
            parameters.Add("@cityId", staffRequest.CityId, DbType.Int32);
            parameters.Add("@regionId", staffRequest.RegionId, DbType.Int32);
            parameters.Add("@zipCode", staffRequest.ZipCode ?? "", DbType.String);
            parameters.Add("@agencyId", staffRequest.AgencyId, DbType.Int32);
            parameters.Add("@comments", staffRequest.Comments ?? "", DbType.String);
            parameters.Add("@userId", staffRequest.UserId, DbType.String);
            parameters.Add("@isActive", staffRequest.IsActive, DbType.Boolean);
            parameters.Add("@reviewResultId", staffRequest.ReviewResultId);
            // Fecha de revisión segura
            parameters.Add("@reviewDate", staffRequest.ReviewDate?.Year >= 1753 ? staffRequest.ReviewDate : DBNull.Value, DbType.DateTime);
            parameters.Add("@reviewJustification", staffRequest.ReviewJustification ?? "", DbType.String);
            // Agregar parámetros para miembros de la junta
            parameters.Add("@tenureDuration", staffRequest.TenureDuration, DbType.Int32);
            parameters.Add("@tenureDurationUnitId", staffRequest.TenureDurationUnitId, DbType.Int32);
            parameters.Add("@receivesProgramSalaryId", staffRequest.ReceivesProgramSalaryId, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("101_UpdateStaff", parameters, commandType: CommandType.StoredProcedure);

            int rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                await DeleteStaffSalaryOrigin(staffRequest.Id.Value, dbConnection, null);

                if (staffRequest.SalaryOrigins != null)
                {
                    foreach (var item in staffRequest.SalaryOrigins)
                    {
                        await InsertStaffSalaryOrigin(staffRequest.Id.Value, item.Id, dbConnection, null);
                    }
                }

                if (staffRequest.ClassificationContracts != null)
                {
                    await DeleteStaffContractByClassification(staffRequest.Id.Value, dbConnection, null);

                    foreach (var contract in staffRequest.ClassificationContracts)
                    {
                        await InsertStaffContractByClassification(staffRequest.Id.Value, contract, dbConnection, null);
                    }
                }

                InvalidateCache(staffRequest.Id.Value);
                // Manejar la asignación de sitio
                await HandleSchoolAssignmentUpdate(staffRequest.Id.Value, staffRequest.SiteId, staffRequest.IsPrimary);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el miembro del staff con ID {StaffId}", staffRequest.Id);
            throw new Exception($"Error al actualizar el miembro del staff con ID {staffRequest.Id}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Elimina todos los contratos por clasificación de un staff (tabla StaffContractByClassification).
    /// </summary>
    private async Task DeleteStaffContractByClassification(int staffId, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@staffid", staffId, DbType.Int32);

            await dbConnection.ExecuteAsync("100_DeleteStaffContractByClassification", parameters, transaction, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar contratos por clasificación para staff {StaffId}", staffId);
            throw new Exception($"Error al eliminar contratos por clasificación del staff: {ex.Message}", ex);
        }
        finally
        {
            if (shouldDisposeConnection)
                dbConnection.Dispose();
        }
    }

    /// <summary>
    /// Inserta un contrato por clasificación para un staff (tabla StaffContractByClassification).
    /// </summary>
    private async Task InsertStaffContractByClassification(int staffId, StaffClassificationContractItemRequest contract, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@staffid", staffId, DbType.Int32);
            parameters.Add("@staffclassificationid", contract.StaffClassificationId, DbType.Int32);
            parameters.Add("@positionid", contract.PositionId, DbType.Int32);
            parameters.Add("@contractstartdate", contract.ContractStartDate?.Year >= 1753 ? contract.ContractStartDate : null, DbType.DateTime);
            parameters.Add("@contractenddate", contract.ContractEndDate?.Year >= 1753 ? contract.ContractEndDate : null, DbType.DateTime);
            var scheduleFrom = ParseTimeToTimeSpan(contract.ScheduleFrom);
            var scheduleTo = ParseTimeToTimeSpan(contract.ScheduleTo);
            parameters.Add("@schedulefrom", (object?)scheduleFrom ?? DBNull.Value, DbType.Time);
            parameters.Add("@scheduleto", (object?)scheduleTo ?? DBNull.Value, DbType.Time);
            parameters.Add("@id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await dbConnection.ExecuteAsync("100_InsertStaffContractByClassification", parameters, transaction, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar contrato por clasificación para staff {StaffId}", staffId);
            throw new Exception($"Error al insertar contrato por clasificación del staff: {ex.Message}", ex);
        }
        finally
        {
            if (shouldDisposeConnection)
                dbConnection.Dispose();
        }
    }

    /// <summary>
    /// Inserta un origen de salario para un staff (tabla StaffSalaryOrigin). Usa 100_InsertStaffSalaryOrigin.
    /// </summary>
    private async Task InsertStaffSalaryOrigin(int staffId, int optionSelectionId, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@staffid", staffId, DbType.Int32);
            parameters.Add("@optionselectionid", optionSelectionId, DbType.Int32);

            await dbConnection.ExecuteAsync("100_InsertStaffSalaryOrigin", parameters, transaction, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar origen de salario para staff {StaffId}", staffId);
            throw new Exception($"Error al insertar origen de salario del staff: {ex.Message}", ex);
        }
        finally
        {
            if (shouldDisposeConnection)
                dbConnection.Dispose();
        }
    }

    /// <summary>
    /// Elimina todos los orígenes de salario de un staff. Usa 100_UpdateStaffSalaryOrigin con lista vacía.
    /// </summary>
    private async Task DeleteStaffSalaryOrigin(int staffId, IDbConnection? connection = null, IDbTransaction? transaction = null)
    {
        var dbConnection = connection ?? _context.CreateConnection();
        var shouldDisposeConnection = connection == null;

        try
        {
            var parameters = new DynamicParameters();
            parameters.Add("@staffid", staffId, DbType.Int32);
            parameters.Add("@optionselectionids", "", DbType.String);

            await dbConnection.ExecuteAsync("100_UpdateStaffSalaryOrigin", parameters, transaction, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar orígenes de salario para staff {StaffId}", staffId);
            throw new Exception($"Error al eliminar orígenes de salario del staff: {ex.Message}", ex);
        }
        finally
        {
            if (shouldDisposeConnection)
                dbConnection.Dispose();
        }
    }

    private static TimeSpan? ParseTimeToTimeSpan(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (TimeSpan.TryParse(value, out var t)) return t;
        if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{1,2}:\d{2}$"))
        {
            var parts = value.Split(':');
            if (int.TryParse(parts[0], out var h) && int.TryParse(parts[1], out var m))
                return new TimeSpan(h, m, 0);
        }
        return null;
    }

    /// <summary>
    /// Elimina un miembro del staff de la base de datos (baja lógica)
    /// </summary>
    /// <param name="id">El ID del miembro del staff a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    public async Task<bool> DeleteStaff(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando miembro del staff con ID {StaffId}", id);

            // Obtener el registro actual para auditoría
            var currentStaff = await GetStaffById(id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id, DbType.Int32);

            var rowsAffected = await dbConnection.ExecuteAsync("100_DeleteStaff", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                // Registrar en auditoría
                await _auditLogger.LogChangeAsync(
                    "Staff",
                    id.ToString(),
                    "DELETE",
                    "SYSTEM", // No tenemos userId en DeleteStaff, usar SYSTEM
                    currentStaff, // oldEntity
                    null, // newEntity (no hay para DELETE)
                    "Miembro del staff eliminado",
                    "StaffDeletion"
                );

                InvalidateCache(id);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el miembro del staff con ID {StaffId}", id);
            throw new Exception($"Error al eliminar el miembro del staff con ID {id}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Elimina completamente un miembro del staff y todas sus relaciones, incluyendo la agencia si es propietario
    /// </summary>
    /// <param name="id">El ID del miembro del staff a eliminar</param>
    /// <returns>True si se eliminó correctamente</returns>
    public async Task<bool> BulkDeleteStaff(int id)
    {
        try
        {
            _logger.LogInformation("Eliminando completamente el miembro del staff con ID {StaffId} y todas sus relaciones", id);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@staffId", id, DbType.Int32);
            parameters.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("101_BulkDeleteStaff", parameters, commandType: CommandType.StoredProcedure);

            var result = parameters.Get<int>("@returnValue");

            if (result > 0)
            {
                InvalidateCache(id);
                _logger.LogInformation("Staff {StaffId} y todas sus relaciones eliminadas exitosamente", id);
                return true;
            }

            _logger.LogWarning("No se pudo eliminar el staff con ID {StaffId}", id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar completamente el miembro del staff con ID {StaffId}: {Message}", id, ex.Message);
            throw new Exception($"Error al eliminar completamente el miembro del staff con ID {id}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza solo la imagen del staff
    /// </summary>
    /// <param name="staffId">El ID del staff</param>
    /// <param name="imageUrl">La nueva URL de la imagen</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateStaffImage(int staffId, string? imageUrl)
    {
        try
        {
            _logger.LogInformation("Actualizando imagen del staff con ID {StaffId}", staffId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@staffId", staffId, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@imageURL", imageUrl ?? "", DbType.String, ParameterDirection.Input);

            var rowsAffected = await dbConnection.ExecuteAsync("100_UpdateStaffImage", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                _logger.LogInformation("Imagen del staff con ID {StaffId} actualizada exitosamente", staffId);

                // Invalidar cache para este staff
                InvalidateCache(staffId);

                return true;
            }
            else
            {
                _logger.LogWarning("No se pudo actualizar la imagen del staff con ID {StaffId}", staffId);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la imagen del staff con ID {StaffId}", staffId);
            throw new Exception($"Error al actualizar la imagen del staff con ID {staffId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Convierte un miembro del staff en usuario del sistema
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>True si se convirtió correctamente</returns>
    public async Task<bool> ConvertStaffToUser(int staffId, string userId)
    {
        try
        {
            _logger.LogInformation("Convirtiendo miembro del staff con ID {StaffId} a usuario", staffId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@staffId", staffId, DbType.Int32);
            parameters.Add("@userId", userId, DbType.String);

            var rowsAffected = await dbConnection.ExecuteAsync("100_ConvertStaffToUser", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                InvalidateCache(staffId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al convertir miembro del staff con ID {StaffId} a usuario", staffId);
            throw new Exception($"Error al convertir miembro del staff con ID {staffId} a usuario: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Actualiza el estado activo de un miembro del staff
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    /// <param name="isActive">Nuevo estado activo</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateStaffActiveStatus(int staffId, bool isActive)
    {
        try
        {
            _logger.LogInformation("Actualizando estado activo del miembro del staff con ID {StaffId}", staffId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@staffId", staffId, DbType.Int32);
            parameters.Add("@isActive", isActive, DbType.Boolean);

            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await dbConnection.ExecuteAsync("100_UpdateStaffActiveStatus", parameters, commandType: CommandType.StoredProcedure);

            int rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0 && !isActive)
            {
                // Si se inactivó el personal, enviar notificación al evaluador asignado
                await NotifyEvaluatorOnStaffInactivation(staffId);

                // Notificar también al SuperAdmin si el usuario tiene cuenta de NUTRE (UserId no nulo)
                await NotifySuperAdminIfNutreUser(staffId);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado activo del miembro del staff con ID {StaffId}", staffId);
            throw new Exception($"Error al actualizar estado activo del miembro del staff con ID {staffId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Notifica al evaluador cuando un personal es inactivado
    /// </summary>
    /// <param name="staffId">ID del personal inactivado</param>
    private async Task NotifyEvaluatorOnStaffInactivation(int staffId)
    {
        try
        {
            // Obtener información del staff usando tipos específicos
            var staffResult = await GetStaffById(staffId);
            if (staffResult == null)
            {
                _logger.LogWarning("NotifyEvaluatorOnStaffInactivation - Staff con ID {StaffId} no encontrado", staffId);
                return;
            }

            // Convertir a DTOStaff (GetStaffById ya retorna DTOStaff mapeado)
            if (staffResult is not DTOStaff staff)
            {
                _logger.LogWarning("NotifyEvaluatorOnStaffInactivation - No se pudo convertir el staff con ID {StaffId} a DTOStaff", staffId);
                return;
            }

            if (!staff.AgencyId.HasValue)
            {
                _logger.LogWarning("NotifyEvaluatorOnStaffInactivation - Staff {StaffId} no tiene AgencyId", staffId);
                return;
            }

            // Obtener evaluador de la agencia
            var evaluatorUserId = await _agencyRepository.GetEvaluatorUserIdByAgencyId(staff.AgencyId.Value);
            if (string.IsNullOrEmpty(evaluatorUserId))
            {
                _logger.LogWarning("NotifyEvaluatorOnStaffInactivation - No se encontró evaluador asignado para la agencia {AgencyId}", staff.AgencyId.Value);
                return;
            }

            // Construir nombre completo del staff
            var staffNameParts = new List<string> { staff.FirstName };
            if (!string.IsNullOrWhiteSpace(staff.MiddleName))
            {
                staffNameParts.Add(staff.MiddleName);
            }
            staffNameParts.Add(staff.FatherLastName);
            if (!string.IsNullOrWhiteSpace(staff.MotherLastName))
            {
                staffNameParts.Add(staff.MotherLastName);
            }
            var staffName = string.Join(" ", staffNameParts);

            // Preparar variables para los templates
            var variables = new Dictionary<string, string>
            {
                { "StaffName", staffName },
                { "AgencyName", staff.AgencyName ?? "" },
                { "InactiveDate", DateTime.Now.ToString("dd/MM/yyyy") },
            };

            // Enviar mensaje interno Y email usando templates separados
            _logger.LogInformation("NotifyEvaluatorOnStaffInactivation - Enviando notificación al evaluador {EvaluatorUserId} para staff {StaffId}", evaluatorUserId, staffId);

            await _messageTemplateService.SendMessageAndEmailFromTemplate(messageTemplateKey: "StaffInactivated", emailTemplateKey: "StaffInactivated", recipientUserId: evaluatorUserId, variables: variables, language: "es");

            _logger.LogInformation("NotifyEvaluatorOnStaffInactivation - Notificación enviada exitosamente para staff {StaffId}", staffId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación al evaluador para staff {StaffId}: {Message}", staffId, ex.Message);
            // No lanzar excepción para no fallar la operación principal
        }
    }

    /// <summary>
    /// Notifica a los Super Admins cuando un personal con usuario NUTRE es inactivado
    /// </summary>
    /// <param name="staffId">ID del personal inactivado</param>
    private async Task NotifySuperAdminIfNutreUser(int staffId)
    {
        try
        {
            // 1. Obtener información del staff
            DTOStaff staff = await GetStaffById(staffId);

            if (staff == null)
            {
                _logger.LogWarning("NotifySuperAdminIfNutreUser - Staff con ID {StaffId} no encontrado", staffId);
                return;
            }
            
            //2. Verificar si tiene usuario NUTRE asociado
            if (string.IsNullOrEmpty(staff.UserId))
            {
                return;
            }

            // 3. Obtener IDs de usuarios SuperAdmin / Administrator
            var superAdminIds = await GetSuperAdminUserIds();

            if (superAdminIds.Count == 0)
            {
                _logger.LogWarning("NotifySuperAdminIfNutreUser - No se encontraron Super Admins para notificar sobre staff {StaffId}", staffId);
                return;
            }

            // 4. Preparar variables para el mensaje
            // Construir nombre completo del staff
            var staffNameParts = new List<string> { staff.FirstName };
            if (!string.IsNullOrWhiteSpace(staff.MiddleName))
            {
                staffNameParts.Add(staff.MiddleName);
            }
            staffNameParts.Add(staff.FatherLastName);
            if (!string.IsNullOrWhiteSpace(staff.MotherLastName))
            {
                staffNameParts.Add(staff.MotherLastName);
            }
            var staffName = string.Join(" ", staffNameParts);

            var variables = new Dictionary<string, string>
            {
                { "StaffName", staffName },
                { "AgencyName", staff.AgencyName ?? "Agencia Desconocida" },
                { "InactiveDate", DateTime.Now.ToString("dd/MM/yyyy") },
                { "UserEmail", staff.Email }, // Variable adicional por si el template la usa
            };

            // 5. Enviar notificaciones a cada Super Admin
            _logger.LogInformation("NotifySuperAdminIfNutreUser - Enviando notificación a {Count} Super Admins para staff {StaffId}", superAdminIds.Count, staffId);

            foreach (var adminId in superAdminIds)
            {
                await _messageTemplateService.SendMessageAndEmailFromTemplate(
                    messageTemplateKey: "StaffInactivated", // Usamos el mismo template o uno específico si existiera
                    emailTemplateKey: "StaffInactivated",
                    recipientUserId: adminId,
                    variables: variables,
                    language: "es"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar notificación a Super Admins para staff {StaffId}: {Message}", staffId, ex.Message);
        }
    }

    /// <summary>
    /// Obtiene los IDs de usuarios con rol SuperAdmin o Administrator
    /// </summary>
    private async Task<List<string>> GetSuperAdminUserIds()
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            string sql =
                @"
                SELECT DISTINCT u.Id
                FROM AspNetUsers u
                INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                WHERE r.Name IN ('SuperAdmin', 'Administrator')
                AND u.IsActive = 1";

            var result = await dbConnection.QueryAsync<string>(sql);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener IDs de Super Admins");
            return new List<string>();
        }
    }

    /// <summary>
    /// Actualiza solo el AgencyId de un miembro del staff
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    /// <param name="agencyId">Nuevo ID de agencia</param>
    /// <returns>True si se actualizó correctamente</returns>
    public async Task<bool> UpdateStaffAgencyId(int staffId, int agencyId)
    {
        try
        {
            _logger.LogInformation("Actualizando AgencyId del staff con ID {StaffId} a {AgencyId}", staffId, agencyId);

            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@staffId", staffId, DbType.Int32);
            parameters.Add("@agencyId", agencyId, DbType.Int32);

            var rowsAffected = await dbConnection.ExecuteAsync("100_UpdateStaffAgencyId", parameters, commandType: CommandType.StoredProcedure);

            if (rowsAffected > 0)
            {
                InvalidateCache(staffId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el AgencyId del staff con ID {StaffId}", staffId);
            throw new Exception($"Error al actualizar el AgencyId del staff con ID {staffId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene todos los miembros del staff de una agencia específica
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <param name="take">El número de miembros del staff a obtener</param>
    /// <param name="skip">El número de miembros del staff a saltar</param>
    /// <param name="name">El nombre del miembro del staff a buscar</param>
    /// <param name="staffTypeId">ID del tipo de staff para filtrar</param>
    /// <returns>Los miembros del staff de la agencia</returns>
    public async Task<dynamic> GetStaffByAgency(int agencyId, int take, int skip, string name, int? staffTypeId = null)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new DynamicParameters();
            param.Add("@agencyId", agencyId, DbType.Int32);
            param.Add("@take", take, DbType.Int32);
            param.Add("@skip", skip, DbType.Int32);
            param.Add("@name", name, DbType.String);
            param.Add("@staffTypeId", staffTypeId, DbType.Int32);

            var result = await dbConnection.QueryMultipleAsync("100_GetStaffByAgency", param, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null;
            }

            var data = result.Read<dynamic>()
                .Select(_mappingService.MapStaffList)
                .OfType<StaffTableResponse>()
                .ToList();
            var count = result.Read<int>().FirstOrDefault();

            return new PagedResult<StaffTableResponse> { Data = data, Count = count };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los miembros del staff de la agencia {AgencyId}", agencyId);
            throw new Exception($"Error al obtener los miembros del staff de la agencia {agencyId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene el historial de auditoría de un miembro del staff
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    /// <param name="limit">Límite de registros a retornar</param>
    /// <returns>Lista de registros de auditoría</returns>
    public async Task<List<AuditTrailDto>> GetStaffAuditHistory(int staffId, int limit = 100)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@StaffId", staffId, DbType.Int32);
            parameters.Add("@Limit", limit, DbType.Int32);

            var result = await dbConnection.QueryAsync<AuditTrailDto>("103_GetStaffAuditHistory", parameters, commandType: CommandType.StoredProcedure);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial de auditoría para staff {StaffId}", staffId);
            throw new Exception($"Error al obtener historial de auditoría para staff {staffId}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Maneja la actualización de la asignación de sitio para un staff
    /// </summary>
    /// <param name="staffId">ID del staff</param>
    /// <param name="newSiteId">Nuevo ID de sitio (null si no hay sitio)</param>
    /// <param name="isPrimary">Si es asignación principal</param>
    private async Task HandleSchoolAssignmentUpdate(int staffId, int? newSiteId, bool isPrimary)
    {
        try
        {
            // Obtener asignaciones actuales del staff
            var currentAssignments = await _siteStaffRepository.GetSitesByStaff(staffId);
            var currentAssignment = currentAssignments.FirstOrDefault(a => a.IsActive);

            // Caso 1: No hay asignación actual y no se proporcionó nueva sitio → No hacer nada
            if (currentAssignment == null && (!newSiteId.HasValue || newSiteId.Value <= 0))
            {
                _logger.LogInformation("No hay cambios en la asignación de sitio para el staff {StaffId}", staffId);
                return;
            }

            // Caso 2: No hay asignación actual pero se proporcionó una sitio → Crear nueva
            if (currentAssignment == null && newSiteId.HasValue && newSiteId.Value > 0)
            {
                _logger.LogInformation("Creando nueva asignación: Staff {StaffId} → Sitio {SiteId}", staffId, newSiteId.Value);

                var siteStaffRequest = new SiteStaffRequest
                {
                    SiteId = newSiteId.Value,
                    StaffId = staffId,
                    IsPrimary = isPrimary,
                    Comments = $"Asignación actualizada automáticamente",
                };

                await _siteStaffRepository.AssignStaffToSite(siteStaffRequest);
                _logger.LogInformation("Asignación creada exitosamente");
                return;
            }

            // Caso 3: Hay asignación actual pero no se proporcionó sitio → Desasignar
            if (currentAssignment != null && (!newSiteId.HasValue || newSiteId.Value <= 0))
            {
                _logger.LogInformation("Eliminando asignación: Staff {StaffId} de Sitio {SiteId}", staffId, currentAssignment.SiteId);
                await _siteStaffRepository.UnassignStaffFromSite(currentAssignment.SiteId, staffId);
                _logger.LogInformation("Asignación eliminada exitosamente");
                return;
            }

            // Caso 4: Hay asignación actual y cambió la sitio → Desasignar anterior y crear nueva
            if (currentAssignment != null && newSiteId.HasValue && newSiteId.Value > 0 && currentAssignment.SiteId != newSiteId.Value)
            {
                _logger.LogInformation("Cambiando asignación: Staff {StaffId} de Sitio {OldSiteId} → {NewSiteId}", staffId, currentAssignment.SiteId, newSiteId.Value);

                // Desasignar de la sitio anterior
                await _siteStaffRepository.UnassignStaffFromSite(currentAssignment.SiteId, staffId);

                // Asignar a la nueva sitio
                var siteStaffRequest = new SiteStaffRequest
                {
                    SiteId = newSiteId.Value,
                    StaffId = staffId,
                    IsPrimary = isPrimary,
                    Comments = $"Asignación actualizada automáticamente",
                };

                await _siteStaffRepository.AssignStaffToSite(siteStaffRequest);
                _logger.LogInformation("Asignación actualizada exitosamente");
                return;
            }

            // Caso 5: Misma sitio pero cambió isPrimary → Actualizar asignación existente
            if (currentAssignment != null && newSiteId.HasValue && newSiteId.Value > 0 && currentAssignment.SiteId == newSiteId.Value)
            {
                // Verificar si cambió algo
                bool isPrimaryChanged = currentAssignment.IsPrimary != isPrimary;

                if (isPrimaryChanged)
                {
                    _logger.LogInformation("Actualizando asignación existente: Staff {StaffId} en Sitio {SiteId}", staffId, newSiteId.Value);

                    var updateRequest = new UpdateSiteStaffRequest { IsPrimary = isPrimary, Comments = $"Asignación actualizada automáticamente" };

                    await _siteStaffRepository.UpdateSiteStaff(currentAssignment.Id, updateRequest);
                    _logger.LogInformation("Asignación actualizada exitosamente");
                }
                else
                {
                    _logger.LogInformation("No hay cambios en la asignación de sitio para el staff {StaffId}", staffId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error al manejar la asignación de sitio para el staff {StaffId}, pero la actualización del staff fue exitosa", staffId);
            // No lanzar excepción aquí porque el staff ya fue actualizado exitosamente
        }
    }

    /// <summary>
    /// Invalida el caché para un miembro del staff específico
    /// </summary>
    /// <param name="staffId">ID del miembro del staff</param>
    private void InvalidateCache(int staffId)
    {
        try
        {
            _cache.Remove(string.Format(_appSettings.Cache.Keys.Staff, 0, 0, "", false, null));
            _cache.Remove(_appSettings.Cache.Keys.Staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al invalidar caché para miembro del staff con ID {StaffId}", staffId);
        }
    }
}
