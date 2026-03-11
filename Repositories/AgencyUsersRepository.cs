using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Services;
using Dapper;
using Api.Models.Errors;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class AgencyUsersRepository(DapperContext context, ILogger<AgencyUsersRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, IEmailService emailService, Lazy<IUserRepository> userRepository, Lazy<IAgencyRepository> agencyRepository, MappingService mappingService) : IAgencyUsersRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<AgencyUsersRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly Lazy<IUserRepository> _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly Lazy<IAgencyRepository> _agencyRepository = agencyRepository ?? throw new ArgumentNullException(nameof(agencyRepository));
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Obtiene la agencia asignada a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <returns>La agencia asignada al usuario</returns>
    public async Task<dynamic> GetUserAssignedAgency(string userId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);
            var result = await db.QueryFirstOrDefaultAsync<DTOAgencyUser>("104_GetUserAssignedAgency", parameters, commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                return null!;
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la agencia asignada al usuario {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene las agencias asignadas a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="take">Número de registros a tomar</param>
    /// <param name="skip">Número de registros a saltar</param>
    /// <param name="alls">Si se deben obtener todas las agencias</param>
    /// <returns>Lista de agencias asignadas al usuario</returns>
    public async Task<dynamic> GetUserAssignedAgencies(string userId, int take, int skip, bool alls, bool forDropdown)
    {
        return await GetUserAssignedAgenciesV2(userId, take, skip, alls, forDropdown);
    }


    /// <summary>
    /// Desasigna una agencia de un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>True si la desasignación fue exitosa</returns>
    public async Task<bool> UnassignAgencyFromUser(string userId, int agencyId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("101_UnassignAgencyToUser", parameters, commandType: CommandType.StoredProcedure);

            var rowsAffected = parameters.Get<int>("@rowsAffected");

            if (rowsAffected > 0)
            {
                // Invalidar caché
                InvalidateCache(userId);
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desasignar la agencia del usuario");
            throw;
        }
    }

    /// <summary>
    /// Actualiza la agencia principal a la que pertenece un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="agencyId">ID de la nueva agencia</param>
    /// <param name="assignedBy">ID del usuario que realiza el cambio</param>
    /// <returns>True si la actualización fue exitosa</returns>
    public async Task<bool> UpdateUserMainAgency(string userId, int agencyId, string assignedBy)
    {
        return await UpdateUserMainAgencyV2(userId, agencyId, assignedBy);
    }

    private void InvalidateCache(string userId)
    {
        // Invalidar listas completas
        _cache.Remove(string.Format(_appSettings.Cache.Keys.AgencyUsers, userId, "*", "*"));
        _logger.LogInformation("Cache invalidado para AgencyUsers Repository");
    }

    // =============================================
    // MÉTODOS V2 - Nueva estructura con AgencyAssignmentType
    // =============================================

    /// <summary>
    /// Obtiene el AgencyAssignmentType del usuario según su rol (resuelto en DB por 100_GetAgencyAssignmentTypeByUserId).
    /// </summary>
    public async Task<string> CalculateAgencyAssignmentTypeFromRole(string userId)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@userid", userId, DbType.String);

        var agencyAssignmentType = await db.QueryFirstOrDefaultAsync<string>(
            "100_GetAgencyAssignmentTypeByUserId",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        if (string.IsNullOrEmpty(agencyAssignmentType))
        {
            throw new ApiException(ErrorCode.UNEXPECTED_ERROR, $"No se puede determinar AgencyAssignmentType para el usuario {userId} (sin rol o rol no configurado en RoleAssignmentCategory).");
        }

        return agencyAssignmentType;
    }

    /// <summary>
    /// Asigna una agencia a un usuario usando AgencyAssignmentType
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="agencyId">ID de la agencia</param>
    /// <param name="assignedBy">ID del usuario que asigna</param>
    /// <param name="agencyAssignmentType">Tipo de asignación (AGENCY_OWNER, AGENCY_STAFF, NUTRE_COORDINATOR, etc.)</param>
    /// <returns>True si la asignación fue exitosa</returns>
    public async Task<bool> AssignAgencyToUser(string userId, int agencyId, string assignedBy, string agencyAssignmentType)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@assignedBy", assignedBy, DbType.String);
            parameters.Add("@agencyAssignmentType", agencyAssignmentType, DbType.String);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("102_AssignAgencyToUser", parameters, commandType: CommandType.StoredProcedure);

            var id = parameters.Get<int>("@Id");

            if (id > 0)
            {
                // Solo enviar correo si es una asignación de monitoreo (NUTRE_%)
                if (agencyAssignmentType.StartsWith("NUTRE_", StringComparison.OrdinalIgnoreCase))
                {
                    var user = await _userRepository.Value.GetUserById(userId);
                    var agency = await _agencyRepository.Value.GetAgencyById(agencyId);

                    if (user != null && agency != null)
                    {
                        await _emailService.SendAgencyAssignmentEmail(user, agency);
                        _logger.LogInformation($"Correo de asignación enviado al usuario {userId} para monitorear la agencia {agencyId}");
                    }
                }

                InvalidateCache(userId);
            }

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar la agencia al usuario");
            throw;
        }
    }

    /// <summary>
    /// Obtiene las agencias asignadas a un usuario (V2)
    /// </summary>
    public async Task<dynamic> GetUserAssignedAgenciesV2(string userId, int take, int skip, bool alls, bool forDropdown)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@userId", userId, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);

            if (forDropdown)
            {
                string cacheKey = string.Format(_appSettings.Cache.Keys.AgencyUsers, userId, take, skip);
                return await _cache.CacheQuery(
                    cacheKey,
                    async () =>
                    {
                        using var result = await db.QueryMultipleAsync("101_GetUserAssignedAgencies", parameters, commandType: CommandType.StoredProcedure);

                        if (result == null)
                        {
                            return [];
                        }

                        var data = result.Read<dynamic>().Select(_mappingService.MapAgencyUserList).ToList();
                        return data;
                    },
                    _logger,
                    _appSettings,
                    TimeSpan.FromMinutes(1)
                );
            }
            else
            {
                using var result = await db.QueryMultipleAsync("101_GetUserAssignedAgencies", parameters, commandType: CommandType.StoredProcedure);

                if (result == null)
                {
                    return null!;
                }

                var data = result.Read<dynamic>().Select(_mappingService.MapAgencyUser).ToList();
                var count = result.ReadFirstOrDefault<int>();
                return new { data, count };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las agencias asignadas al usuario {UserId} con V2", userId);
            throw;
        }
    }

    /// <summary>
    /// Actualiza la agencia principal a la que pertenece un usuario (V2)
    /// </summary>
    public async Task<bool> UpdateUserMainAgencyV2(string userId, int agencyId, string assignedBy)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@assignedBy", assignedBy, DbType.String);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync("102_UpdateUserMainAgency", parameters, commandType: CommandType.StoredProcedure);

            var id = parameters.Get<int>("@Id");

            if (id > 0)
            {
                InvalidateCache(userId);
            }

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la agencia principal del usuario con V2");
            throw;
        }
    }

}