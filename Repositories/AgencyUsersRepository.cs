using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Dapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Api.Repositories;

public class AgencyUsersRepository(DapperContext context, ILogger<AgencyUsersRepository> logger, IMemoryCache cache, IOptions<ApplicationSettings> appSettings, IEmailService emailService, Lazy<IUserRepository> userRepository, Lazy<IAgencyRepository> agencyRepository) : IAgencyUsersRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly ILogger<AgencyUsersRepository> _logger = logger;
    private readonly IMemoryCache _cache = cache;
    private readonly ApplicationSettings _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly Lazy<IUserRepository> _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    private readonly Lazy<IAgencyRepository> _agencyRepository = agencyRepository ?? throw new ArgumentNullException(nameof(agencyRepository));

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
            var agency = await db.QueryFirstOrDefaultAsync<dynamic>("103_GetUserAssignedAgency", parameters, commandType: CommandType.StoredProcedure);
            return agency;
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
    /// <returns>Lista de agencias asignadas al usuario</returns>
    public async Task<dynamic> GetUserAssignedAgencies(string userId, int take, int skip)
    {
        string cacheKey = string.Format(_appSettings.Cache.Keys.AgencyUsers, userId, take, skip);

        return await _cache.CacheQuery(
            cacheKey,
            async () =>
            {
                using IDbConnection db = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@userId", userId, DbType.String);
                parameters.Add("@take", take, DbType.Int32);
                parameters.Add("@skip", skip, DbType.Int32);

                var result = await db.QueryMultipleAsync(
                    "100_GetUserAssignedAgencies",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                var data = await result.ReadAsync<dynamic>();
                var count = await result.ReadSingleAsync<int>();
                return new { data, count };
            },
            _logger,
            _appSettings
        );
    }

    /// <summary>
    /// Asigna una agencia a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>True si la asignación fue exitosa</returns>
    public async Task<bool> AssignAgencyToUser(string userId, int agencyId, string assignedBy, bool isOwner = false, bool isMonitor = false)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@assignedBy", assignedBy, DbType.String);
            parameters.Add("@isOwner", isOwner, DbType.Boolean);
            parameters.Add("@isMonitor", isMonitor, DbType.Boolean);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "101_AssignAgencyToUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var id = parameters.Get<int>("@Id");

            if (id > 0)
            {
                // Solo enviar correo si es una asignación de monitoreo
                if (isMonitor)
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

            await db.ExecuteAsync(
                "101_UnassignAgencyToUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

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
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);
            parameters.Add("@agencyId", agencyId, DbType.Int32);
            parameters.Add("@assignedBy", assignedBy, DbType.String);
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);

            await db.ExecuteAsync(
                "101_UpdateUserMainAgency", // Necesitaremos crear este SP
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var id = parameters.Get<int>("@Id");

            if (id > 0)
            {
                // Solo invalidar caché
                InvalidateCache(userId);
            }

            return id > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la agencia principal del usuario");
            throw;
        }
    }

    private void InvalidateCache(string userId)
    {
        // Invalidar listas completas
        _cache.Remove(string.Format(_appSettings.Cache.Keys.AgencyUsers, userId, "*", "*"));
        _logger.LogInformation("Cache invalidado para AgencyUsers Repository");
    }
}