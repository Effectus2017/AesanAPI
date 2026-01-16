using System.Data;
using Api.Data;
using Api.Extensions;
using Api.Interfaces;
using Api.Models;
using Api.Services;
using Dapper;
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
        return await GetUserAssignedAgencyV2(userId);
    }

    /// <summary>
    /// Obtiene las agencias asignadas a un usuario
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="take">Número de registros a tomar</param>
    /// <param name="skip">Número de registros a saltar</param>
    /// <param name="alls">Si se deben obtener todas las agencias</param>
    /// <returns>Lista de agencias asignadas al usuario</returns>
    public async Task<dynamic> GetUserAssignedAgencies(string userId, int take, int skip, bool alls, bool isList)
    {
        return await GetUserAssignedAgenciesV2(userId, take, skip, alls, isList);
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
    /// Obtiene el rol del usuario desde AspNetUserRoles
    /// </summary>
    private async Task<string?> GetUserRole(string userId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);
            
            var roleName = await db.QueryFirstOrDefaultAsync<string>(
                @"SELECT TOP 1 r.Name
                  FROM AspNetUserRoles ur
                  INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
                  WHERE ur.UserId = @userId",
                parameters
            );
            
            return roleName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el rol del usuario {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// Obtiene el AssignmentCategory del rol desde RoleAssignmentCategory
    /// </summary>
    private async Task<string?> GetAssignmentCategoryFromRole(string roleName)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@roleName", roleName, DbType.String);
            
            var assignmentCategory = await db.QueryFirstOrDefaultAsync<string>(
                @"SELECT rac.AssignmentCategory
                  FROM RoleAssignmentCategory rac
                  INNER JOIN AspNetRoles r ON rac.RoleId = r.Id
                  WHERE r.Name = @roleName",
                parameters
            );
            
            return assignmentCategory;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener AssignmentCategory para el rol {RoleName}", roleName);
            return null;
        }
    }

    /// <summary>
    /// Verifica si el rol del usuario puede ser owner
    /// </summary>
    private async Task<bool> CanUserBeOwner(string userId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);
            
            var canBeOwner = await db.QueryFirstOrDefaultAsync<bool?>(
                @"SELECT rac.CanBeOwner
                  FROM RoleAssignmentCategory rac
                  INNER JOIN AspNetUserRoles ur ON rac.RoleId = ur.RoleId
                  WHERE ur.UserId = @userId",
                parameters
            );
            
            return canBeOwner ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si el usuario {UserId} puede ser owner", userId);
            return false;
        }
    }

    /// <summary>
    /// Calcula el AgencyAssignmentType apropiado basado en el rol del usuario
    /// </summary>
    public async Task<string> CalculateAgencyAssignmentTypeFromRole(string userId)
    {
        // Obtener el rol del usuario desde AspNetUserRoles
        var userRole = await GetUserRole(userId);
        
        if (string.IsNullOrEmpty(userRole))
        {
            throw new Exception($"El usuario {userId} no tiene un rol asignado");
        }
        
        // Obtener AssignmentCategory del rol desde RoleAssignmentCategory
        var assignmentCategory = await GetAssignmentCategoryFromRole(userRole);
        
        // Calcular AgencyAssignmentType según AssignmentCategory
        if (assignmentCategory == "AGENCY")
        {
            // Para roles de agencia, verificar si puede ser owner
            var canBeOwner = await CanUserBeOwner(userId);
            return canBeOwner ? "AGENCY_OWNER" : "AGENCY_STAFF";
        }
        else if (assignmentCategory == "NUTRE")
        {
            // Determinar tipo específico según el nombre del rol
            if (userRole.Contains("Coordinador", StringComparison.OrdinalIgnoreCase) || 
                userRole.Contains("Coordinator", StringComparison.OrdinalIgnoreCase))
                return "NUTRE_COORDINATOR";
            else if (userRole.Contains("Evaluador", StringComparison.OrdinalIgnoreCase) || 
                     userRole.Contains("Evaluator", StringComparison.OrdinalIgnoreCase))
                return "NUTRE_EVALUATOR";
            else if (userRole.Contains("Admin", StringComparison.OrdinalIgnoreCase) || 
                     userRole.Contains("Administrador", StringComparison.OrdinalIgnoreCase))
                return "NUTRE_ADMIN";
            else if (userRole.Contains("Contaduría", StringComparison.OrdinalIgnoreCase) || 
                     userRole.Contains("Accounting", StringComparison.OrdinalIgnoreCase) ||
                     userRole.Contains("Contable", StringComparison.OrdinalIgnoreCase))
                return "NUTRE_ACCOUNTING";
            else
                return "NUTRE_EVALUATOR"; // Default para roles NUTRE
        }
        
        throw new Exception($"No se puede determinar AgencyAssignmentType para el rol: {userRole}");
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
    /// Obtiene la agencia asignada a un usuario (V2)
    /// </summary>
    public async Task<dynamic> GetUserAssignedAgencyV2(string userId)
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
            _logger.LogError(ex, "Error al obtener la agencia asignada al usuario {UserId} con V2", userId);
            throw;
        }
    }

    /// <summary>
    /// Obtiene las agencias asignadas a un usuario (V2)
    /// </summary>
    public async Task<dynamic> GetUserAssignedAgenciesV2(string userId, int take, int skip, bool alls, bool isList)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@userId", userId, DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);

            if (isList)
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