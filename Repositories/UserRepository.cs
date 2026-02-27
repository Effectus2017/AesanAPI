using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Models.Response;
using Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Api.Data;
using System.Data;
using Dapper;
using Api.Services;

namespace Api.Repositories;

public class UserRepository(UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IOptions<ApplicationSettings> appSettings,
    IConfiguration configuration,
    IMapper mapper,
    ILoggingService loggingService,
    DapperContext context,
    IEmailService emailService,
    IAgencyRepository agencyRepository,
    IAgencyUsersRepository agencyUsersRepository,
    IStaffRepository staffRepository,
    IProgramRepository programRepository,
    MappingService mappingService,
    IAuditLogger auditLogger) : IUserRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IAuditLogger _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;
    private readonly ApplicationSettings _appSettings = appSettings.Value;
    private readonly IConfiguration _configuration = configuration;
    private readonly IMapper _mapper = mapper;
    private readonly ILoggingService _loggingService = loggingService;
    private readonly IEmailService _emailService = emailService;
    private readonly IAgencyRepository _agencyRepository = agencyRepository;
    private readonly IAgencyUsersRepository _agencyUsersRepository = agencyUsersRepository;
    private readonly IStaffRepository _staffRepository = staffRepository;
    private readonly IProgramRepository _programRepository = programRepository;
    private readonly MappingService _mappingService = mappingService ?? throw new ArgumentNullException(nameof(mappingService));

    /// <summary>
    /// Nombres de rol que requieren agencia asignada (coherentes con 104_GetUserAssignedAgency).
    /// </summary>
    private static readonly HashSet<string> AgencyRoleNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "agency_administrator",
        "agency_user"
    };

    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    /// <param name="id">El ID del usuario</param>
    /// <returns>El usuario</returns>
    public async Task<DTOUser> GetUserById(string userId)
    {
        try
        {
            var user = _userManager.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                return null;
            }

            var dtoUser = _mapper.Map<User, DTOUser>(user);

            // Obtener la agencia asignada al usuario
            var agency = await _agencyUsersRepository.GetUserAssignedAgency(userId);

            if (agency != null)
            {
                dtoUser.AgencyId = agency.Id;
                dtoUser.AgencyName = agency.Name;
            }

            return dtoUser;
        }
        catch (Exception ex)
        {
            var properties = new Dictionary<string, string>
            {
                { "UserId", userId },
                { "ErrorMessage", ex.Message }
            };
            await _loggingService.LogError(ex, "Error al obtener usuario", properties);
            throw new Exception($"Error al obtener el usuario: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene un usuario por su ID usando un Stored Procedure
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>El usuario con datos completos desde Staff y Agency</returns>
    public async Task<DTOUser> GetUserByIdWithSP(string userId)
    {
        try
        {
            _loggingService.LogInformation("Obteniendo usuario por ID con SP", new Dictionary<string, string>
            {
                { "UserId", userId }
            });

            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);

            // Ejecutar el SP que retorna dos resultados
            var result = await db.QueryMultipleAsync("110_GetUserById", parameters, commandType: CommandType.StoredProcedure);

            // Leer el primer resultado: datos del usuario
            var userFromDb = await result.ReadFirstOrDefaultAsync<DTOUserById>();

            if (userFromDb == null)
            {
                _loggingService.LogWarning("Usuario no encontrado con SP", new Dictionary<string, string>
                {
                    { "UserId", userId }
                });
                return null;
            }

            // Leer el segundo resultado: roles del usuario (con isprimary, validfrom, validto, isvigent)
            var userRoles = (await result.ReadAsync<DTOUserRole>()).ToList();

            // Leer el tercer resultado: programas asignados al usuario
            var userProgramsList = (await result.ReadAsync<DTOProgram>()).ToList();

            var roleDisplayMap = _roleManager.Roles
                .ToDictionary(r => r.Id, r => new { r.DisplayName, r.DisplayNameEN });

            foreach (var role in userRoles)
            {
                if (roleDisplayMap.TryGetValue(role.Id, out var info))
                {
                    role.DisplayName = info.DisplayName;
                    role.DisplayNameEN = info.DisplayNameEN;
                }
            }

            var primaryRole = userRoles.FirstOrDefault(r => r.IsPrimary);
            var secondaryRolesList = userRoles.Where(r => !r.IsPrimary).Select(r => new DTOUserSecondaryRole
            {
                RoleId = r.Id,
                RoleName = r.DisplayName,
                Comment = r.Comment,
                ValidFrom = r.ValidFrom ?? DateTime.MinValue,
                ValidTo = r.ValidTo ?? DateTime.MinValue,
                IsActive = r.IsVigent
            }).ToList();

            var roleNamesForDisplay = new List<string>();
            if (primaryRole != null) roleNamesForDisplay.Add(primaryRole.DisplayName);
            foreach (var sec in userRoles.Where(r => !r.IsPrimary && r.IsVigent))
                roleNamesForDisplay.Add(sec.DisplayName);

            var dtoUser = new DTOUser
            {
                Id = userFromDb.Id,
                UserName = userFromDb.UserName,
                Email = userFromDb.Email,
                FirstName = userFromDb.FirstName,
                MiddleName = userFromDb.MiddleName,
                FatherLastName = userFromDb.FatherLastName,
                MotherLastName = userFromDb.MotherLastName,
                AdministrationTitle = userFromDb.AdministrationTitle,
                PhoneNumber = userFromDb.PhoneNumber,
                ImageURL = userFromDb.ImageURL,
                IsActive = userFromDb.IsActive,
                IsTemporalPasswordActived = userFromDb.IsTemporalPasswordActived,
                EmailConfirmed = userFromDb.EmailConfirmed,
                AgencyId = userFromDb.AgencyId,
                AgencyName = userFromDb.AgencyName,
                ProgramId = userFromDb.ProgramId,
                ProgramName = userFromDb.ProgramName,
                Programs = userProgramsList.Count > 0 ? userProgramsList : null,
                Roles = roleNamesForDisplay,
                RolesDisplay = string.Join(", ", roleNamesForDisplay),
                Role = primaryRole ?? userRoles.FirstOrDefault(),
                PrimaryRoleName = primaryRole?.Name,
                SecondaryRoles = secondaryRolesList,
                Agency = userFromDb.AgencyId.HasValue && userFromDb.AgencyId.Value != 0 ? new AgencyResponse { Id = userFromDb.AgencyId.Value, Name = userFromDb.AgencyName } : null
            };

            _loggingService.LogInformation("Usuario obtenido exitosamente con SP", new Dictionary<string, string>
            {
                { "UserId", userId },
                { "Email", userFromDb.Email },
                { "FirstName", userFromDb.FirstName },
                { "LastName", userFromDb.FatherLastName },
                { "RolesCount", userRoles.Count.ToString() },
                { "RoleName", userRoles.FirstOrDefault()?.Name ?? "Sin rol" }
            });

            return dtoUser;
        }
        catch (Exception ex)
        {
            var properties = new Dictionary<string, string>
            {
                { "UserId", userId },
                { "ErrorMessage", ex.Message }
            };
            await _loggingService.LogError(ex, "Error al obtener usuario con SP", properties);
            throw new Exception($"Error al obtener el usuario con SP: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Obtiene los programas asignados al usuario desde UserProgram (Admin Portal). Usado para claims JWT.
    /// </summary>
    private async Task<List<DTOProgram>> GetUserProgramsByUserId(string userId)
    {
        try
        {
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@userId", userId, DbType.String);
            var result = await db.QueryAsync<DTOProgram>("100_GetUserProgramsByUserId", parameters, commandType: CommandType.StoredProcedure);
            return result?.ToList() ?? [];
        }
        catch (Exception ex)
        {
            _ = _loggingService.LogError(ex, "Error al obtener programas del usuario desde UserProgram para {UserId}", new Dictionary<string, string> { { "UserId", userId } });
            return [];
        }
    }

    /// <summary>
    /// Obtiene todos los usuarios de la base de datos
    /// </summary>
    /// <param name="take">El número de usuarios a obtener</param>
    /// <param name="skip">El número de usuarios a saltar</param>
    /// <param name="name">El nombre del usuario</param>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>Una lista de usuarios</returns>
    public dynamic GetAllUsersFromDb(int take, int skip, string name, string userId, bool isList)
    {
        try
        {
            if (isList)
            {
                var _result = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToList();
                var _currentUser = _mapper.Map<List<User>, List<DTOUser>>(_result);
                return _currentUser;
            }
            else
            {
                var _result = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Skip(skip).Take(take).ToList();
                var _count = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToList().Count;
                var _currentUser = _mapper.Map<List<User>, List<DTOUser>>(_result);
                var _complete = new { data = _currentUser, count = _count };
                return _complete;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los usuarios de la base de datos usando un Stored Procedure
    /// </summary>
    /// <param name="take">El número de usuarios a obtener</param>
    /// <param name="skip">El número de usuarios a saltar</param>
    /// <param name="name">El nombre del usuario a buscar</param>
    /// <param name="agencyId">ID de la agencia para filtrar</param>
    /// <param name="isList">Si es true, retorna solo la lista sin paginación</param>
    /// <param name="roles">Lista de roles para filtrar</param>
    /// <param name="alls">Si es true, retorna todos los usuarios sin filtros ni paginación</param>
    /// <param name="excludeAdministrators">Si es true, excluye usuarios con rol Administrator o Super-Administrator</param>
    /// <returns>Una lista de usuarios con el conteo total</returns>
    public async Task<dynamic> GetAllUsersFromDbWithSP(int take, int skip, string name, int? agencyId = null, bool isList = false, List<string> roles = null, bool alls = false, bool excludeAdministrators = false)
    {
        try
        {
            _loggingService.LogInformation("Obteniendo usuarios con SP");
            var rolesForSp = roles != null && roles.Count > 0 ? roles : null;

            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@agencyId", agencyId == 0 ? null : agencyId, DbType.Int32);
            parameters.Add("@roles", rolesForSp == null ? null : string.Join(",", rolesForSp), DbType.String);
            parameters.Add("@alls", alls, DbType.Boolean);
            parameters.Add("@excludeAdministrators", excludeAdministrators, DbType.Boolean);

            var result = await db.QueryMultipleAsync("109_GetAllUsersFromDb", parameters, commandType: CommandType.StoredProcedure);
            var rows = result.Read<DTOUserListItem>().ToList();
            var count = result.ReadFirstOrDefault<int>();

            var roleDisplayMap = _roleManager.Roles
                .ToDictionary(r => r.Id, r => new { r.DisplayName, r.DisplayNameEN });

            var grouped = rows.GroupBy(r => r.Id);

            var data = grouped.Select(g =>
            {
                var first = g.First();
                var user = _mappingService.MapUser(first);

                if (user.Role != null && roleDisplayMap.TryGetValue(user.Role.Id, out var roleInfo))
                {
                    user.Role.DisplayName = roleInfo.DisplayName;
                    user.Role.DisplayNameEN = roleInfo.DisplayNameEN;
                }

                var roleDisplayNames = g
                    .Where(x => x.RoleId != null)
                    .Select(x => roleDisplayMap.TryGetValue(x.RoleId!, out var info) ? info.DisplayName : x.RoleName)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct()
                    .ToList();
                user.Roles = roleDisplayNames;
                user.RolesDisplay = string.Join(", ", roleDisplayNames);
                return user;
            }).ToList();

            if (isList)
            {
                return data;
            }

            return new { data, count };
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al obtener usuarios con SP");
            throw;
        }
    }


    /// <summary>
    /// Obtiene todos los roles de la base de datos
    /// </summary>
    /// <param name="take">El número de roles a obtener</param>
    /// <param name="skip">El número de roles a saltar</param>
    /// <param name="name">El nombre del rol</param>
    /// <returns>Una lista de roles</returns>
    public dynamic GetAllRolesFromDb()
    {
        try
        {
            var _result = _roleManager.Roles.OrderBy(r => r.Name).ToList();
            var _count = _result.Count;
            var _currentRoles = _mapper.Map<List<Role>, List<DTORole>>(_result);
            var _complete = new { data = _currentRoles, count = _count };

            return _complete;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene los permisos de un usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>Los permisos del usuario</returns>
    public async Task<dynamic> GetPermissionsByUserId(string userId)
    {
        using var db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.String);
        var permissions = await db.QueryAsync<dynamic>("100_GetUserPermissions", parameters, commandType: CommandType.StoredProcedure);

        if (permissions == null)
        {
            return null;
        }

        return permissions;
    }

    /// ------------------------------------------------------------------------------------------------
    /// Métodos para la autenticación
    /// ------------------------------------------------------------------------------------------------

    /// <summary>
    /// Obtiene los nombres de roles AESAN (IsAesanRole=1) desde la base de datos.
    /// </summary>
    public async Task<IList<string>> GetAesanRoleNames()
    {
        var roles = await _roleManager.Roles
            .Where(r => r.IsAesanRole)
            .OrderBy(r => r.Name)
            .Select(r => r.Name!)
            .ToListAsync();
        return roles;
    }

    /// <summary>
    /// Obtiene los roles AESAN con Name (clave), DisplayName y DisplayNameEN para la UI.
    /// </summary>
    public async Task<List<DTOAesanRoleItem>> GetAesanRoles()
    {
        var roles = await _roleManager.Roles
            .Where(r => r.IsAesanRole)
            .OrderBy(r => r.Name)
            .Select(r => new DTOAesanRoleItem
            {
                Name = r.Name ?? "",
                DisplayName = r.DisplayName,
                DisplayNameEN = r.DisplayNameEN
            })
            .ToListAsync();
        return roles;
    }

    /// <summary>
    /// Obtiene roles AESAN disponibles para asignar como roles secundarios.
    /// Excluye el rol primario y roles secundarios ya asignados.
    /// </summary>
    public async Task<List<DTOAesanRoleItem>> GetAvailableSecondaryRoles(string? primaryRoleId, List<string>? excludeRoleIds)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@primaryroleid", primaryRoleId, DbType.String);
        parameters.Add("@excluderoleids", excludeRoleIds != null && excludeRoleIds.Any() 
            ? string.Join(",", excludeRoleIds) 
            : null, DbType.String);

        var roles = await db.QueryAsync<DTOAesanRoleItem>(
            "100_GetAvailableSecondaryRoles",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return roles.ToList();
    }

    /// <summary>
    /// Obtiene todos los roles asignados a un usuario específico (primario + secundarios activos).
    /// </summary>
    public async Task<List<DTOAesanRoleItem>> GetUserRoles(string userId)
    {
        using IDbConnection db = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@userid", userId, DbType.String);

        var roles = await db.QueryAsync<DTOAesanRoleItem>(
            "100_GetUserRoles",
            parameters,
            commandType: CommandType.StoredProcedure
        );

        return roles.ToList();
    }

    /// <summary>
    /// Obtiene los nombres de roles efectivos del usuario: rol principal + roles secundarios vigentes (hoy entre ValidFrom y ValidTo).
    /// </summary>
    public async Task<List<string>> GetEffectiveRoleNamesForUser(string userId)
    {
        using IDbConnection db = _context.CreateConnection();
        const string sql = @"
            SELECT r.Name
            FROM AspNetUserRoles ur
            INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE ur.UserId = @userId AND (ur.IsActive = 1 OR ur.IsActive IS NULL)
              AND (ur.IsPrimary = 1 OR (ur.ValidFrom IS NOT NULL AND ur.ValidTo IS NOT NULL
                   AND CAST(GETDATE() AS DATE) >= ur.ValidFrom AND CAST(GETDATE() AS DATE) <= ur.ValidTo))";
        var names = await db.QueryAsync<string>(sql, new { userId });
        return names?.Where(n => !string.IsNullOrEmpty(n)).ToList() ?? [];
    }

    /// <summary>
    /// Obtiene los emails de usuarios que tienen el rol indicado (ej. Administrator para notificaciones).
    /// </summary>
    public async Task<List<string>> GetUserEmailsByRoleNameAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null) return new List<string>();
        var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
        return usersInRole?.Where(u => !string.IsNullOrWhiteSpace(u.Email)).Select(u => u.Email!).Distinct().ToList() ?? new List<string>();
    }

    /// <summary>
    /// Obtiene el Id del rol por nombre.
    /// </summary>
    public async Task<string?> GetRoleIdByNameAsync(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        return role?.Id;
    }

    /// <summary>
    /// Obtiene la fecha tope (ValidTo) del rol secundario para el usuario, o null si es rol principal o no existe.
    /// </summary>
    public async Task<DateTime?> GetRoleValidToForUser(string userId, string roleName)
    {
        using IDbConnection db = _context.CreateConnection();
        const string sql = @"
            SELECT ur.ValidTo
            FROM AspNetUserRoles ur
            INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE ur.UserId = @userId AND r.Name = @roleName AND ur.IsPrimary = 0
              AND (ur.IsActive = 1 OR ur.IsActive IS NULL)";
        var validTo = await db.QueryFirstOrDefaultAsync<DateTime?>(sql, new { userId, roleName });
        return validTo;
    }

    /// <summary>
    /// Inicia sesión en el sistema
    /// </summary>
    /// <param name="model">El modelo de inicio de sesión</param>
    /// <returns>El token de acceso</returns>
    public async Task<dynamic> Login(LoginRequest model)
    {
        try
        {
            _loggingService.LogInformation("Iniciando sesión en el sistema");

            User? _user = await _userManager.FindByNameAsync(model.UserName);

            // Si el usuario no existe, se debe devolver un error
            if (_user == null)
            {
                return new UnauthorizedObjectResult(new { Message = "Usuario no encontrado" });
            }

            // Si la contraseña temporal está activa, se debe cambiar
            if (_user.IsTemporalPasswordActived)
            {
                return new ConflictObjectResult(new { Message = "La contraseña temporera es válida." });
            }

#if !DEBUG

            // Si el usuario está deshabilitado, se debe devolver un error
            if (!_user.IsActive)
            {
                return new BadRequestObjectResult(new { Message = "El usuario está deshabilitado." });
            }

            // Si el correo electrónico no está confirmado, se debe devolver un error
            // if (!_user.EmailConfirmed)
            // {
            //     return new BadRequestObjectResult(new { Message = "El correo electrónico no está confirmado." });
            // }
#endif

            // Verificar si la contraseña es correcta
            var passwordValid = await _userManager.CheckPasswordAsync(_user, model.Password);

            if (!passwordValid)
            {
                return new UnauthorizedObjectResult(new { Message = "Contraseña incorrecta" });
            }

            // Obtener roles efectivos (principal + secundarios vigentes)
            var roles = await GetEffectiveRoleNamesForUser(_user.Id);

            if (roles == null || roles.Count == 0)
            {
                _loggingService.LogWarning($"Usuario {_user.UserName} intentó iniciar sesión sin roles asignados o vigentes");
                return new BadRequestObjectResult(new { Message = "El usuario no tiene un rol asignado o vigente. Por favor, contacte al administrador del sistema." });
            }

            var aesanRoleNames = await GetAesanRoleNames();
            var aesanRoles = roles.Where(r => aesanRoleNames.Contains(r)).ToList();

            IList<string> rolesToUse = roles;
            List<string>? rolesForSelection = null;

            if (aesanRoles.Count >= 2)
            {
                rolesToUse = [aesanRoles[0]];
                rolesForSelection = aesanRoles;
            }

            var primaryRole = rolesToUse?.FirstOrDefault();

            if (!string.IsNullOrEmpty(primaryRole) && AgencyRoleNames.Contains(primaryRole))
            {
                var agency = await _agencyUsersRepository.GetUserAssignedAgency(_user.Id);
                if (agency == null)
                {
                    _loggingService.LogWarning($"Usuario {_user.UserName} con rol de agencia no tiene agencia asignada");
                    return new ObjectResult(new { Message = "No tiene una agencia asignada. Contacte al administrador del sistema." }) { StatusCode = 403 };
                }
            }

            var roleValidTo = await GetRoleValidToForUser(_user.Id, rolesToUse.FirstOrDefault() ?? "");
            var (access_token, expires_in) = await GenerateTokenForUserAsync(_user, rolesToUse, rolesForSelection, roleValidTo);

            if (rolesForSelection != null)
            {
                return new { token_type = "Bearer", access_token, expires_in, roles = rolesForSelection };
            }

            return new { token_type = "Bearer", access_token, expires_in };
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }

    /// <summary>
    /// Genera un token JWT para el usuario con los roles especificados.
    /// Reutilizable para Login y SelectRole.
    /// </summary>
    /// <param name="allAesanRolesForSelection">Cuando el usuario tiene 2+ roles AESAN, lista de roles disponibles para cambio en el menú; se incluye como claim "roles" en el JWT.</param>
    /// <param name="roleValidTo">Si el rol actual es secundario, fecha tope de vigencia (claim "roleValidTo" en el JWT).</param>
    private async Task<(string access_token, double expires_in)> GenerateTokenForUserAsync(User user, IList<string> rolesToUse, IList<string>? allAesanRolesForSelection = null, DateTime? roleValidTo = null)
    {
        var permissions = await GetPermissionsByUserId(user.Id);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"));
        var days = 2;
        var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
        var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

        var agency = await _agencyUsersRepository.GetUserAssignedAgency(user.Id);
        var userProgramsFromAdmin = await GetUserProgramsByUserId(user.Id);
        var userPrograms = (userProgramsFromAdmin != null && userProgramsFromAdmin.Any())
            ? userProgramsFromAdmin
            : (await _agencyRepository.GetAgencyProgramsByUserId(user.Id) as List<DTOProgram>) ?? new List<DTOProgram>();

        Staff? staff = null;
        try
        {
            staff = await _staffRepository.GetStaffByUserId(user.Id);
        }
        catch (Exception ex)
        {
            _loggingService.LogWarning($"No se pudo obtener Staff para el usuario {user.Id}: {ex.Message}");
        }

        var currentRoleKey = rolesToUse?.FirstOrDefault();
        string? roleDisplay = null;
        string? roleDisplayEN = null;

        if (!string.IsNullOrEmpty(currentRoleKey))
        {
            var roleEntity = await _roleManager.FindByNameAsync(currentRoleKey);

            if (roleEntity != null)
            {
                roleDisplay = roleEntity.DisplayName;
                roleDisplayEN = roleEntity.DisplayNameEN;
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GetClaims(user, rolesToUse, agency, userPrograms, permissions, staff, allAesanRolesForSelection, roleValidTo, roleDisplay, roleDisplayEN),
            Expires = DateTime.UtcNow.AddDays(days),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var access_token = tokenHandler.WriteToken(token);
        var expires_in = TimeSpan.FromDays(days).TotalSeconds;

        return (access_token, expires_in);
    }

    /// <summary>
    /// Genera un nuevo token con el rol seleccionado para usuarios multi-rol AESAN.
    /// Valida que el rol sea efectivo (principal o secundario vigente) e incluye roleValidTo en el JWT si es secundario.
    /// </summary>
    public async Task<dynamic> SelectRole(string userId, string role)
    {
        try
        {
            var aesanRoleNames = await GetAesanRoleNames();

            if (string.IsNullOrWhiteSpace(role) || !aesanRoleNames.Contains(role))
            {
                return new BadRequestObjectResult(new { Message = "Rol no válido para selección." });
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new UnauthorizedObjectResult(new { Message = "Usuario no encontrado" });
            }

            var effectiveRoles = await GetEffectiveRoleNamesForUser(userId);

            var hasAesanRole = effectiveRoles != null && effectiveRoles.Any(r => aesanRoleNames.Contains(r));

            if (!hasAesanRole)
            {
                return new ObjectResult(new { Message = "Cambiar rol solo está disponible para usuarios AESAN." }) { StatusCode = 403 };
            }

            if (effectiveRoles == null || !effectiveRoles.Contains(role))
            {
                return new BadRequestObjectResult(new { Message = "El usuario no tiene asignado el rol indicado o el rol secundario ya no está vigente." });
            }

            var userAgency = await _agencyUsersRepository.GetUserAssignedAgency(user.Id);

            if (AgencyRoleNames.Contains(role) && userAgency == null)
            {
                return new ObjectResult(new { Message = "No tiene una agencia asignada. Contacte al administrador del sistema." }) { StatusCode = 403 };
            }

            var aesanRoles = effectiveRoles.Where(r => aesanRoleNames.Contains(r)).ToList();
            var roleValidTo = await GetRoleValidToForUser(userId, role);
            var (access_token, expires_in) = await GenerateTokenForUserAsync(user, new List<string> { role }, aesanRoles, roleValidTo);

            await _auditLogger.LogChangeAsync(
                tableName: "UserSession",
                entityId: userId,
                action: "ROLE_SWITCH",
                userId,
                newEntity: new { Role = role },
                businessContext: "RoleSwitch");

            return new { token_type = "Bearer", access_token, expires_in };
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene los claims del usuario
    /// </summary>
    /// <param name="user">El usuario</param>
    /// <param name="roles">Los roles del usuario</param>
    /// <param name="agency">La agencia del usuario</param>
    /// <param name="userPrograms">Los programas del usuario</param>
    /// <param name="permissions">Los permisos del usuario</param>
    /// <param name="staff">Los datos de Staff asociados al usuario (opcional)</param>
    /// <param name="allAesanRolesForSelection">Lista de roles AESAN para cambio en el menú (claim "roles" en JWT)</param>
    /// <param name="roleValidTo">Si el rol actual es secundario, fecha tope de vigencia (claim "roleValidTo" en el JWT)</param>
    /// <param name="roleDisplay">Nombre a mostrar del rol actual (español)</param>
    /// <param name="roleDisplayEN">Nombre a mostrar del rol actual (inglés)</param>
    /// <returns>Los claims del usuario</returns>
    private static ClaimsIdentity GetClaims(User user, IList<string> roles, dynamic agency, List<DTOProgram> userPrograms, dynamic permissions, Staff? staff = null, IList<string>? allAesanRolesForSelection = null, DateTime? roleValidTo = null, string? roleDisplay = null, string? roleDisplayEN = null)
    {
        try
        {
            var claims = new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, user.Id), new(ClaimTypes.Name, user.UserName ?? "") });

            foreach (var role in roles)
            {
                claims.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            if (allAesanRolesForSelection != null && allAesanRolesForSelection.Count > 0)
            {
                claims.AddClaim(new Claim("roles", string.Join(",", allAesanRolesForSelection)));
            }

            if (roleValidTo.HasValue)
            {
                claims.AddClaim(new Claim("roleValidTo", roleValidTo.Value.ToString("o")));
            }

            if (!string.IsNullOrEmpty(roleDisplay))
            {
                claims.AddClaim(new Claim("roleDisplay", roleDisplay));
            }
            if (!string.IsNullOrEmpty(roleDisplayEN))
            {
                claims.AddClaim(new Claim("roleDisplayEN", roleDisplayEN));
            }

            // Obtener name y lastName de Staff si existe
            var name = staff?.FirstName ?? "";
            var lastName = "";
            if (staff != null)
            {
                var lastNameParts = new List<string>();
                if (!string.IsNullOrWhiteSpace(staff.FatherLastName))
                {
                    lastNameParts.Add(staff.FatherLastName);
                }
                if (!string.IsNullOrWhiteSpace(staff.MotherLastName))
                {
                    lastNameParts.Add(staff.MotherLastName);
                }
                lastName = string.Join(" ", lastNameParts);
            }

            var programsList = userPrograms ?? new List<DTOProgram>();
            var programNames = string.Join(",", programsList.Select(p => p.Name));
            var programIds = string.Join(",", programsList.Select(p => p.Id.ToString()));

            // Roles NUTRE "por programa" (coordinator, evaluator) reciben claims de programas
            if (roles.Any(r => r != null && (r.Contains("coordinator", StringComparison.OrdinalIgnoreCase) || r.Contains("evaluator", StringComparison.OrdinalIgnoreCase))))
            {
                claims.AddClaim(new Claim("userId", user.Id));
                claims.AddClaim(new Claim("name", name));
                claims.AddClaim(new Claim("lastName", lastName));
                claims.AddClaim(new Claim("email", user.Email ?? ""));
                claims.AddClaim(new Claim("programs", programNames));
                claims.AddClaim(new Claim("programIds", programIds));

                return claims;
            }

            // Los datos personales ahora vienen de Staff, no de User
            claims.AddClaim(new Claim("name", name));
            claims.AddClaim(new Claim("lastName", lastName));
            claims.AddClaim(new Claim("email", user.Email ?? ""));

            // Usuarios con roles administrativos pueden no tener agencia asignada
            if (agency != null)
            {
                claims.AddClaim(new Claim("agency", agency.Name ?? ""));
                claims.AddClaim(new Claim("agencyId", agency.Id.ToString()));
            }
            else
            {
                claims.AddClaim(new Claim("agency", ""));
                claims.AddClaim(new Claim("agencyId", ""));
            }

            claims.AddClaim(new Claim("programs", programNames));
            claims.AddClaim(new Claim("programIds", programIds));

            if (permissions != null)
            {
                foreach (var permission in permissions)
                {
                    claims.AddClaim(new Claim("permissions", permission.ValueKey));
                }
            }

            return claims;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los claims del usuario", ex);
        }
    }

    /// ------------------------------------------------------------------------------------------------
    /// Métodos para registrar un usuario
    /// ------------------------------------------------------------------------------------------------

    /// <summary>
    /// Registra un usuario en el sistema
    /// </summary>
    /// <param name="model">El modelo de registro de usuario</param>
    /// <returns>El resultado de la operación</returns>
    public async Task<dynamic> RegisterUserAgency(UserAgencyRequest model)
    {
        User? user = null;
        Staff? staff = null;

        try
        {
            // Verificar si el email ya existe ANTES de intentar crear el usuario
            var emailExists = await EmailExists(model.Staff.Email);
            if (emailExists)
            {
                _loggingService.LogWarning("Intento de registro con email existente", new Dictionary<string, string>
                {
                    { "Email", model.Staff.Email }
                });
                return new BadRequestObjectResult(new { Message = "El correo electrónico ya está registrado en el sistema." });
            }

            // Verificar si el IUE ya existe
            if (model.Agency.UieNumber > 0)
            {
                var uieExists = await _agencyRepository.UieNumberExists(model.Agency.UieNumber);
                if (uieExists)
                {
                    _loggingService.LogWarning("Intento de registro con IUE existente", new Dictionary<string, string>
                    {
                        { "UieNumber", model.Agency.UieNumber.ToString() }
                    });
                    return new BadRequestObjectResult(new { Message = "Este Identificador Único de Entidad (IUE) ya está registrado en el sistema." });
                }
            }

            // Verificar si el SDR ya existe
            if (model.Agency.SdrNumber > 0)
            {
                var sdrExists = await _agencyRepository.SdrNumberExists(model.Agency.SdrNumber);
                if (sdrExists)
                {
                    _loggingService.LogWarning("Intento de registro con SDR existente", new Dictionary<string, string>
                    {
                        { "SdrNumber", model.Agency.SdrNumber.ToString() }
                    });
                    return new BadRequestObjectResult(new { Message = "Este Número de Registro del Departamento de Estado (SDR) ya está registrado en el sistema." });
                }
            }

            // Verificar si el EIN ya existe
            if (model.Agency.EinNumber > 0)
            {
                var einExists = await _agencyRepository.EinNumberExists(model.Agency.EinNumber);
                if (einExists)
                {
                    _loggingService.LogWarning("Intento de registro con EIN existente", new Dictionary<string, string>
                    {
                        { "EinNumber", model.Agency.EinNumber.ToString() }
                    });
                    return new BadRequestObjectResult(new { Message = "Este Número de Seguro Social Patronal (EIN) ya está registrado en el sistema." });
                }
            }

#if !DEBUG
            // Generar una contraseña temporal
            var temporaryPassword = Utilities.GenerateTemporaryPassword();
#else
            // Usar la contraseña temporal 9c272156 si estamos en debug
            var temporaryPassword = "9c272156";
#endif

            // 1. Crear el usuario de Identity (solo datos de login)
            user = new User
            {
                UserName = model.Staff.Email,
                Email = model.Staff.Email,
                IsActive = true,
                IsTemporalPasswordActived = true,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, temporaryPassword);

            _loggingService.LogInformation("Insertando la contraseña temporal en la base de datos", new Dictionary<string, string>
            {
                { "TemporaryPassword", temporaryPassword }
            });

            if (!result.Succeeded)
            {
                // Solo eliminar si el usuario fue creado (aunque falló la operación)
                // Si el email ya existe, CreateAsync falla pero no crea el usuario, así que no hay nada que eliminar
                if (user != null && !string.IsNullOrEmpty(user.Id))
                {
                    await RemoveUserAndAgencyRelatedDataByUserId(user.Id);
                }
                return new BadRequestObjectResult(result.Errors);
            }

            // 2. Crear registro en Staff (datos personales) - SIN AgencyId por ahora
            var staffRequest = new StaffRequest
            {
                FirstName = model.Staff.FirstName,
                MiddleName = model.Staff.MiddleName,
                FatherLastName = model.Staff.FatherLastName,
                MotherLastName = model.Staff.MotherLastName,
                Email = model.Staff.Email,
                PhoneNumber = model.Staff.PhoneNumber,
                BirthDate = new DateTime(2000, 1, 1), // Fecha por defecto para sign-up
                PostalAddress = model.Staff.PostalAddress,
                CityId = model.Staff.CityId,
                RegionId = model.Staff.RegionId,
                ZipCode = model.Staff.ZipCode,
                StaffTypeId = model.Staff.StaffTypeId,
                StaffClassificationId = model.Staff.StaffClassificationId,
                StatusId = model.Staff.StatusId,
                PositionId = model.Staff.PositionId,
                UserId = user.Id, // Relación con el usuario creado
                IsActive = model.Staff.IsActive
            };

            // Insertar el staff en la base de datos y obtener el ID
            var staffId = await _staffRepository.InsertStaff(staffRequest);

            if (staffId == 0)
            {
                await RemoveUserAndAgencyRelatedDataByUserId(user.Id);
                return new BadRequestObjectResult(new { Message = "Error al insertar staff" });
            }

            // 3. Asignar el rol (asumiendo que el rol es "agency_administrator")
            var resultRole = await _userManager.AddToRoleAsync(user, "agency_administrator");

            if (!resultRole.Succeeded)
            {
                await RemoveUserAndAgencyRelatedDataByUserId(user.Id);
                return new BadRequestObjectResult(resultRole.Errors);
            }

            // Si el rol es "Evaluador", agregar automáticamente a todos los programas activos
            // (Nota: En este caso el rol es fijo "agency_administrator", pero si puede variar, verificar aquí)
            // Por ahora, este método solo crea usuarios con rol "agency_administrator"

            _loggingService.LogInformation("Insertando la contraseña temporal en la base de datos", new Dictionary<string, string> { { "temporaryPassword", temporaryPassword } });
            await InsertTemporaryPassword(user.Id, temporaryPassword);

            _loggingService.LogInformation("Insertando la agencia en la base de datos");

            // Generar código único de agencia automáticamente
            var existingCodes = await GetExistingAgencyCodes();
            var generatedAgencyCode = Utilities.GenerateSimpleAgencyCode(existingCodes);

            // Asignar el código generado a la agencia
            model.Agency.AgencyCode = generatedAgencyCode;

            _loggingService.LogInformation($"Código de agencia generado: {generatedAgencyCode}", new Dictionary<string, string>
            {
                { "AgencyName", model.Agency.Name },
                { "GeneratedCode", generatedAgencyCode }
            });

            // Insertar la agencia
            int agencyId = await _agencyRepository.InsertAgency(model.Agency);

            // Verificar si la agencia se insertó correctamente
            if (agencyId == 0)
            {
                // Si falla la inserción, eliminar el usuario creado en Identity
                await RemoveUserAndAgencyRelatedDataByUserId(user.Id);
                return new BadRequestObjectResult(new { Message = "Error al insertar el usuario en la tabla Agency" });
            }

            // Insertar los programas de la agencia
            foreach (var programId in model.Agency.Programs)
            {
                await _agencyRepository.InsertAgencyProgram(agencyId, programId);
            }

            // 4. Asignar agencia al staff (datos personales)
            // Nota: El staff se crea sin agencia inicialmente, se puede actualizar después si es necesario
            // Por ahora, la relación se mantiene a través del UserId en Staff

            // 5. Asignar agencia a usuario (para compatibilidad con sistema existente)
            if (agencyId != 0)
            {
                // El creador de la agencia siempre es AGENCY_OWNER
                string agencyAssignmentType = "AGENCY_OWNER";
                await _agencyUsersRepository.AssignAgencyToUser(user.Id, agencyId, user.Id, agencyAssignmentType);

                // 6. Actualizar solo el AgencyId del staff
                bool staffUpdated = await _staffRepository.UpdateStaffAgencyId(staffId, agencyId);
                if (!staffUpdated)
                {
                    _loggingService.LogWarning("No se pudo actualizar el AgencyId del staff", new Dictionary<string, string>
                    {
                        { "StaffEmail", model.Staff.Email },
                        { "AgencyId", agencyId.ToString() }
                    });
                }
            }
            else
            {
                await RemoveUserAndAgencyRelatedDataByUserId(user.Id);
                return new BadRequestObjectResult(new { Message = "Error al insertar el usuario en la tabla Agency" });
            }

            // Enviar correo con la contraseña temporal y bienvenida
            await _emailService.SendWelcomeAgencyEmail(model, temporaryPassword, user.Id);

            // Asignar permisos CRUD de escuelas, staff y sitios al usuario
            await AssignSchoolCrudPermissionsToUserAsync(user.Id);
            await AssignStaffCrudPermissionsToUserAsync(user.Id);
            await AssignSiteCrudPermissionsToUserAsync(user.Id);

            return new OkObjectResult(new { Message = "Usuario registrado exitosamente" });
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Error al registrar usuario");

            if (user != null && !string.IsNullOrEmpty(user.Id))
            {
                await RemoveUserAndAgencyRelatedDataByUserId(user.Id);
            }

            return new BadRequestObjectResult(new { Message = "Error al registrar usuario", Error = ex.Message });
        }
    }

    /// <summary>
    /// Registra un usuario en el sistema
    /// </summary>
    /// <param name="model">El modelo de registro de usuario</param>
    /// <param name="roles">Los roles del usuario</param>
    /// <returns>El resultado de la operación</returns>
    public async Task<dynamic> RegisterUser(DTOUser model, List<string> roles, int agencyId)
    {
        try
        {
            // 1. Crear usuario en Identity (solo datos de login)
            User? user = new()
            {
                UserName = !string.IsNullOrWhiteSpace(model.UserName) ? model.UserName : model.Email,
                Email = model.Email,
                EmailConfirmed = model.EmailConfirmed,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                IsTemporalPasswordActived = model.IsTemporalPasswordActived,
                IsActive = model.IsActive
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result.Errors);
            }

            // 2. Crear registro en Staff (datos personales)
            var staffRequest = new StaffRequest
            {
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                FatherLastName = model.FatherLastName,
                MotherLastName = model.MotherLastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                ImageURL = model.ImageURL, // URL de la imagen/avatar
                // AdministrationTitle removido - ahora se maneja a través de PositionId
                BirthDate = DateTime.Now, // Campo requerido, usar fecha por defecto
                PostalAddress = "Dirección por definir",
                CityId = 0, // Por defecto
                RegionId = 0, // Por defecto
                ZipCode = "00901", // Código postal por defecto para PR
                StaffTypeId = 1, // Empleado por defecto
                StatusId = 1, // Activo por defecto
                PositionId = 0, // Sin posición específica por defecto - se puede actualizar después
                UserId = user.Id, // Relación con el usuario creado
                AgencyId = agencyId, // Asignar agencia directamente
                IsActive = true
            };

            var staffId = await _staffRepository.InsertStaff(staffRequest);

            if (staffId == 0)
            {
                await RemoveUserAndAgencyRelatedDataByUserId(user.Id);
                return new BadRequestObjectResult(new { Message = "Error al insertar staff" });
            }

            // 3. Asignar roles: flujo principal+secundarios (114) o legacy (113)
            using (var db = _context.CreateConnection())
            {
                if (!string.IsNullOrWhiteSpace(model.PrimaryRoleName))
                {
                    var roleParams = new DynamicParameters();
                    roleParams.Add("@userId", user.Id, DbType.String);
                    roleParams.Add("@primaryRoleName", model.PrimaryRoleName.Trim(), DbType.String);
                    string? secondaryJson = null;
                    if (model.SecondaryRoles != null && model.SecondaryRoles.Any())
                    {
                        var items = model.SecondaryRoles
                            .Where(s => !string.IsNullOrWhiteSpace(s.RoleName))
                            .Select(s => new { roleName = s.RoleName.Trim(), comment = s.Comment ?? (string?)null, validFrom = s.ValidFrom.ToString("yyyy-MM-dd"), validTo = s.ValidTo.ToString("yyyy-MM-dd") })
                            .ToList();
                        secondaryJson = System.Text.Json.JsonSerializer.Serialize(items);
                    }
                    roleParams.Add("@secondaryRolesJson", secondaryJson, DbType.String);
                    roleParams.Add("@assignedBy", (string?)null, DbType.String);
                    await db.ExecuteAsync("100_UpdateUserRolesPrimarySecondary", roleParams, commandType: CommandType.StoredProcedure);
                }
                else
                {
                    var rolesToAssign = roles?.Where(r => !string.IsNullOrWhiteSpace(r)).ToList() ?? [];
                    var roleNamesCsv = string.Join(",", rolesToAssign);
                    var roleParams = new DynamicParameters();
                    roleParams.Add("@userId", user.Id, DbType.String);
                    roleParams.Add("@roleNames", roleNamesCsv, DbType.String);
                    await db.ExecuteAsync("113_InsertUserRoles", roleParams, commandType: CommandType.StoredProcedure);
                }
            }

            // Asignar la agencia al usuario
            // Calcular AgencyAssignmentType según el rol del usuario
            string agencyAssignmentType = await _agencyUsersRepository.CalculateAgencyAssignmentTypeFromRole(user.Id);
            await _agencyUsersRepository.AssignAgencyToUser(user.Id, agencyId, user.Id, agencyAssignmentType);

            // Asignar programas al usuario si vienen en el modelo (Admin add user)
            var addProgramIds = model.ProgramIds ?? (model.ProgramId.HasValue && model.ProgramId.Value > 0 ? new List<int> { model.ProgramId.Value } : null);
            if (addProgramIds != null && addProgramIds.Any())
            {
                using (var dbProgram = _context.CreateConnection())
                {
                    foreach (var programId in addProgramIds.Where(id => id > 0))
                    {
                        var programParams = new DynamicParameters();
                        programParams.Add("@userId", user.Id, DbType.String);
                        programParams.Add("@programId", programId, DbType.Int32);
                        await dbProgram.ExecuteAsync(
                            "INSERT INTO UserProgram (UserId, ProgramId, IsActive, CreatedAt) VALUES (@userId, @programId, 1, GETUTCDATE())",
                            programParams);
                    }
                }
            }

            // Enviar correo con la contraseña temporal
            await InsertTemporaryPassword(user.Id, model.Password);
            await _emailService.SendTemporaryPasswordEmail(model.Email, model.Password);

            return new OkObjectResult(new { Message = "Usuario registrado exitosamente" });
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Error al registrar usuario");
            return new BadRequestObjectResult(new { Message = "Error al registrar usuario", Error = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza un usuario usando Stored Procedure
    /// </summary>
    /// <param name="entity">El usuario</param>
    /// <param name="currentUserId">ID del usuario que está realizando la actualización</param>
    /// <returns>True si se actualiza correctamente, false en caso contrario</returns>
    public async Task<dynamic> UpdateWithSP(DTOUser entity, string currentUserId)
    {
        try
        {
            _loggingService.LogInformation("Actualizando usuario con SP", new Dictionary<string, string>
            {
                { "UserId", entity.Id },
                { "Email", entity.Email }
            });

            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();

            // Parámetros de Identity User
            parameters.Add("@userId", entity.Id, DbType.String);
            parameters.Add("@email", entity.Email, DbType.String);
            parameters.Add("@emailConfirmed", entity.EmailConfirmed, DbType.Boolean);
            parameters.Add("@isActive", entity.IsActive, DbType.Boolean);
            parameters.Add("@isTemporalPasswordActived", entity.IsTemporalPasswordActived, DbType.Boolean);

            // Parámetros de Staff (solo campos disponibles en UI)
            parameters.Add("@firstName", entity.FirstName, DbType.String);
            parameters.Add("@middleName", entity.MiddleName ?? "", DbType.String);
            parameters.Add("@fatherLastName", entity.FatherLastName, DbType.String);
            parameters.Add("@motherLastName", entity.MotherLastName, DbType.String);
            parameters.Add("@phoneNumber", entity.PhoneNumber, DbType.String);
            parameters.Add("@agencyId", entity.AgencyId, DbType.Int32);

            string? rolesForLog = null;
            // Roles: flujo principal+secundarios o legacy (roleNames)
            if (!string.IsNullOrWhiteSpace(entity.PrimaryRoleName))
            {
                parameters.Add("@primaryRoleName", entity.PrimaryRoleName.Trim(), DbType.String);
                rolesForLog = entity.PrimaryRoleName.Trim();
                if (entity.SecondaryRoles != null && entity.SecondaryRoles.Any())
                {
                    var items = entity.SecondaryRoles
                        .Where(s => !string.IsNullOrWhiteSpace(s.RoleName))
                        .Select(s => new { roleName = s.RoleName.Trim(), comment = s.Comment ?? (string?)null, validFrom = s.ValidFrom.ToString("yyyy-MM-dd"), validTo = s.ValidTo.ToString("yyyy-MM-dd") })
                        .ToList();
                    var secondaryJson = System.Text.Json.JsonSerializer.Serialize(items);
                    parameters.Add("@secondaryRolesJson", secondaryJson, DbType.String);
                    rolesForLog += "," + string.Join(",", items.Select(i => i.roleName));
                }
                else
                    parameters.Add("@secondaryRolesJson", null, DbType.String);
                parameters.Add("@roleNames", null, DbType.String);
            }
            else
            {
                var roleNamesList = GetRoleNamesFromEntity(entity);
                if (roleNamesList == null || !roleNamesList.Any())
                {
                    var user = await _userManager.FindByIdAsync(entity.Id);
                    if (user != null)
                    {
                        var currentRoles = await _userManager.GetRolesAsync(user);
                        roleNamesList = currentRoles.ToList();
                    }
                    if (roleNamesList == null || !roleNamesList.Any())
                        throw new Exception($"El usuario {entity.Id} no tiene un rol asignado");
                }
                rolesForLog = string.Join(",", roleNamesList.Where(r => !string.IsNullOrWhiteSpace(r)));
                parameters.Add("@primaryRoleName", null, DbType.String);
                parameters.Add("@secondaryRolesJson", null, DbType.String);
                parameters.Add("@roleNames", rolesForLog, DbType.String);
            }

            // Programas asignados al usuario (IDs separados por coma, ej. "1,2,3")
            var programIds = entity.ProgramIds ?? (entity.Programs?.Select(p => p.Id).ToList());
            if (programIds == null && entity.ProgramId.HasValue && entity.ProgramId.Value > 0)
                programIds = new List<int> { entity.ProgramId.Value };
            var programIdsCsv = programIds != null && programIds.Any()
                ? string.Join(",", programIds.Where(id => id > 0))
                : null;
            parameters.Add("@programIds", programIdsCsv, DbType.String);

            // Parámetro de usuario que realiza la asignación
            parameters.Add("@assignedBy", currentUserId, DbType.String);

            // Usar SP 112 con soporte para múltiples roles y programa
            var result = await db.QueryFirstOrDefaultAsync<int>("112_UpdateUser", parameters, commandType: CommandType.StoredProcedure);

            if (result == 1)
            {
                _loggingService.LogInformation("Usuario actualizado exitosamente con SP", new Dictionary<string, string>
                {
                    { "UserId", entity.Id },
                    { "Email", entity.Email },
                    { "RoleNames", rolesForLog ?? "Sin rol" }
                });
                return true;
            }
            else
            {
                await _loggingService.LogError(new Exception("No se pudo actualizar el usuario"), "Error al actualizar usuario con SP", new Dictionary<string, string>
                {
                    { "UserId", entity.Id }
                });
                return false;
            }
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error crítico al actualizar usuario con SP", new Dictionary<string, string>
            {
                { "UserId", entity.Id },
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            throw new Exception($"Error al actualizar el usuario con SP: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Extrae la lista de nombres de rol desde la entidad (Roles o Role).
    /// </summary>
    private static List<string>? GetRoleNamesFromEntity(DTOUser entity)
    {
        if (entity.Roles != null && entity.Roles.Any())
        {
            return entity.Roles.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(entity.Role?.Name))
        {
            return [entity.Role.Name];
        }

        return null;
    }

    /// <summary>
    /// Actualiza un usuario (método original mantenido para compatibilidad)
    /// </summary>
    /// <param name="entity">El usuario</param>
    /// <returns>True si se actualiza correctamente, false en caso contrario</returns>
    public async Task<dynamic> Update(DTOUser entity)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(entity.Id);

            if (user == null)
            {
                return false;
            }

            // Registrar los cambios que se van a realizar
            var changes = new Dictionary<string, (string Old, string New)>();
            if (user.Email != entity.Email) changes.Add("Email", (user.Email, entity.Email));
            // Nota: Los datos personales ahora se manejan a través de Staff, no de User
            // Solo se actualizan las propiedades de Identity que aún existen en User

            // Actualizar propiedades de Identity
            user.Email = entity.Email;
            user.EmailConfirmed = entity.EmailConfirmed;
            // Nota: Los datos personales (FirstName, FatherLastName, etc.) ahora se manejan a través de Staff
            // Se puede implementar la actualización de Staff aquí si es necesario

            try
            {
                var resultUser = await _userManager.UpdateAsync(user);

                if (!resultUser.Succeeded)
                {
                    return false;
                }

                if (entity.Role != null)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var currentRole = currentRoles.FirstOrDefault();
                    var newRole = entity.Role.Name;

                    if (currentRole != newRole)
                    {

                        var isInRole = await _userManager.IsInRoleAsync(user, newRole);

                        if (!isInRole)
                        {
                            if (currentRole != null)
                            {
                                var resultDelete = await _userManager.RemoveFromRoleAsync(user, currentRole);

                                if (!resultDelete.Succeeded)
                                {
                                    return false;
                                }
                            }

                            var resultAdd = await _userManager.AddToRoleAsync(user, newRole);

                            if (!resultAdd.Succeeded)
                            {
                                return false;
                            }

                        }
                    }
                }

                if (entity.AgencyId != null)
                {
                    // Usar el nuevo método para actualizar la agencia principal
                    await _agencyUsersRepository.UpdateUserMainAgency(user.Id, entity.AgencyId.Value, user.Id);
                }

                return true;
            }
            catch (Exception ex)
            {
                await _loggingService.LogError(ex, "Error inesperado al actualizar usuario", new Dictionary<string, string>
                {
                    { "UserId", entity.Id },
                    { "ErrorType", ex.GetType().Name },
                    { "ErrorMessage", ex.Message },
                    { "StackTrace", ex.StackTrace }
                });

                if (ex.InnerException != null)
                {
                    await _loggingService.LogError(ex.InnerException, "Inner Exception para usuario {UserId}: Tipo: {ErrorType}. Mensaje: {ErrorMessage}", new Dictionary<string, string> { { "UserId", entity.Id }, { "ErrorType", ex.InnerException.GetType().Name }, { "ErrorMessage", ex.InnerException.Message } });
                }

                throw;
            }
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error crítico al procesar la actualización del usuario. Tipo de error: {ErrorType}. Mensaje: {ErrorMessage}. Stack Trace: {StackTrace}", new Dictionary<string, string> { { "ErrorType", ex.GetType().Name }, { "ErrorMessage", ex.Message }, { "StackTrace", ex.StackTrace } });

            if (ex.InnerException != null)
            {
                await _loggingService.LogError(ex.InnerException, "Inner Exception: Tipo: {ErrorType}. Mensaje: {ErrorMessage}", new Dictionary<string, string> { { "ErrorType", ex.InnerException.GetType().Name }, { "ErrorMessage", ex.InnerException.Message } });
            }

            throw new Exception($"Error al actualizar el usuario: {ex.Message}", ex);
        }
    }


    /// <summary>
    /// Elimina un usuario
    /// </summary>
    /// <param name="id">El ID del usuario</param>
    /// <returns>True si se elimina correctamente, false en caso contrario</returns>
    public async Task<dynamic> Delete(string id)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return false;
            }

            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (result.Succeeded)
            {

                var assignedAgencies = await _agencyUsersRepository.GetUserAssignedAgencies(user.Id, 100, 0, false, false);

                foreach (var agency in assignedAgencies.data)
                {
                    await _agencyUsersRepository.UnassignAgencyFromUser(user.Id, agency.Id);
                }

                result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    /// ------------------------------------------------------------------------------------------------
    /// Métodos para cambiar la contraseña y actualizar el avatar
    /// ------------------------------------------------------------------------------------------------

    /// <summary>
    /// Para que un usuario pueda cambiar su contraseña
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <param name="model">El modelo que contiene la nueva contraseña</param>
    /// <returns>El resultado de la operación</returns>
    public async Task<dynamic> ChangePassword(string userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new NotFoundObjectResult(new { Message = "Usuario no encontrado" });
            }

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result.Errors);
            }

            return new OkObjectResult(new { Message = "Contraseña cambiada exitosamente" });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al cambiar la contraseña");
            return new BadRequestObjectResult(new { Message = "Error al cambiar la contraseña", Error = ex.Message });
        }
    }

    /// <summary>
    /// Resetea la contraseña de un usuario usando la contraseña temporal
    /// </summary>
    /// <param name="model">Modelo con email, contraseña temporal y nueva contraseña</param>
    /// <returns>El resultado de la operación</returns>
    public async Task<bool> ResetPassword(string userId)
    {
        try
        {
            // Buscar el usuario por email
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

#if !DEBUG
            // Generar una contraseña temporal
            var temporaryPassword = Utilities.GenerateTemporaryPassword();
#else
            // Usar la contraseña temporal 9c272156 si estamos en debug
            var temporaryPassword = "9c272156";
#endif

            // Remover la contraseña actual
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);

            // Establecer la nueva contraseña
            var addPasswordResult = await _userManager.AddPasswordAsync(user, temporaryPassword);

            if (!addPasswordResult.Succeeded)
            {
                return false;
            }

            // Actualizar el estado de la contraseña temporal
            user.IsTemporalPasswordActived = true;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return false;
            }

            // Enviar correo con la contraseña temporal
            await InsertTemporaryPassword(user.Id, temporaryPassword);
            await _emailService.SendTemporaryPasswordEmail(user.Email, temporaryPassword);

            return true;
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al resetear la contraseña");
            return false;
        }
    }

    /// <summary>
    /// Actualiza la contraseña temporal de un usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <param name="newPassword">La nueva contraseña</param>
    /// <returns>True si se actualiza correctamente, false en caso contrario</returns>
    public async Task<bool> UpdateTemporalPassword(string email, string newPassword, string temporaryPassword)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return false;
            }

            // Verificar si la contraseña temporal está activa
            if (!user.IsTemporalPasswordActived)
            {
                return false;
            }

            // comparar la contraseña temporal es valida 
            var temporaryPasswordResult = await _userManager.CheckPasswordAsync(user, temporaryPassword);

            if (!temporaryPasswordResult)
            {
                return false;
            }

            // Remover la contraseña actual
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);

            if (!removePasswordResult.Succeeded)
            {
                return false;
            }

            // Establecer la nueva contraseña
            var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);

            if (!addPasswordResult.Succeeded)
            {
                return false;
            }

            user.IsTemporalPasswordActived = false;
            user.UpdatedAt = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al actualizar la contraseña temporal");
            return false;
        }
    }

    /// <summary>
    /// Genera un token de restablecimiento de contraseña
    /// </summary>
    /// <param name="email">El correo electrónico del usuario</param>
    /// <returns>El token de restablecimiento de contraseña</returns>
    public async Task<string> GeneratePasswordResetToken(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                throw new Exception("Usuario no encontrado");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al generar el token de restablecimiento de contraseña", new Dictionary<string, string> { { "Email", email } });
            throw new Exception("No se pudo generar el token de restablecimiento de contraseña", ex);
        }
    }

    /// <summary>
    /// Actualiza el avatar del usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <param name="imageUrl">La nueva URL de la imagen del avatar</param>
    /// <returns>El resultado de la operación</returns>
    public async Task<dynamic> UpdateUserAvatar(string userId, string imageUrl)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new NotFoundObjectResult(new { Message = "Usuario no encontrado" });
            }

            // Nota: La imagen ahora se maneja a través de Staff, no de User
            // Se puede implementar la actualización de Staff aquí si es necesario
            // Por ahora, solo actualizamos la fecha de actualización del usuario
            user.UpdatedAt = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result.Errors);
            }

            return new OkObjectResult(new { Message = "Avatar actualizado exitosamente (nota: la imagen ahora se maneja a través de Staff)" });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al actualizar el avatar del usuario");
            return new BadRequestObjectResult(new { Message = "Error al actualizar el avatar", Error = ex.Message });
        }
    }

    /// <summary>
    /// Deshabilita un usuario por su ID
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>El resultado de la operación</returns>
    public async Task<dynamic> DisableUser(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new NotFoundObjectResult(new { Message = "Usuario no encontrado" });
            }

            user.IsActive = false; // Deshabilitar el usuario
            user.UpdatedAt = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result.Errors);
            }

            return new OkObjectResult(new { Message = "Usuario deshabilitado exitosamente" });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al deshabilitar el usuario");
            return new BadRequestObjectResult(new { Message = "Error al deshabilitar el usuario", Error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un usuario y todos sus datos relacionados por UserId
    /// Método más seguro para rollback durante el registro
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>El resultado de la operación</returns>
    public async Task<dynamic> RemoveUserAndAgencyRelatedDataByUserId(string userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return new NotFoundObjectResult(new { Message = "Usuario no encontrado" });
            }

            // Eliminar el usuario de su rol de Administrador (solo si tiene el rol)
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains("agency_administrator"))
            {
                await _userManager.RemoveFromRoleAsync(user, "agency_administrator");
            }

            // Eliminar la contraseña temporal del usuario
            await DeleteTemporaryPassword(user.Id);

            // Obtener la agencia principal del usuario
            var agency = await _agencyUsersRepository.GetUserAssignedAgency(user.Id);

            // Si el usuario tiene una agencia asignada, eliminarla
            if (agency != null)
            {
                await _agencyRepository.DeleteAgency(agency.Id);
            }

            // Finalmente, eliminar el usuario
            await _userManager.DeleteAsync(user);

            return new OkObjectResult(new { Message = "Usuario y datos asociados eliminados exitosamente" });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al eliminar el usuario y sus datos asociados por UserId", new Dictionary<string, string>
            {
                { "UserId", userId }
            });
            return new BadRequestObjectResult(new { Message = "Error al eliminar el usuario", Error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un usuario y todos sus datos relacionados por correo electrónico
    /// SOLO debe usarse para limpiar usuarios recién creados durante el registro
    /// NOTA: Este método está deprecado en favor de RemoveUserAndAgencyRelatedDataByUserId
    /// </summary>
    /// <param name="email">El correo electrónico del usuario</param>
    /// <returns>El resultado de la operación</returns>
    public async Task<dynamic> RemoveUserAndAgencyRelatedDataByEmail(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new NotFoundObjectResult(new { Message = "Usuario no encontrado" });
            }

            // Verificar si el usuario fue creado recientemente (en los últimos 5 minutos)
            // Esto previene eliminar usuarios existentes accidentalmente
            // Nota: IdentityUser no tiene CreatedAt en el modelo, pero la tabla sí lo tiene
            // Por seguridad, solo eliminamos si el usuario no tiene agencias asignadas o roles establecidos
            var userRoles = await _userManager.GetRolesAsync(user);
            var agency = await _agencyUsersRepository.GetUserAssignedAgency(user.Id);

            // Si el usuario tiene roles establecidos o agencias asignadas, puede ser un usuario existente
            // Solo proceder si parece ser un usuario recién creado (sin agencias y solo con el rol agency_administrator)
            if (userRoles.Count > 1 || (agency != null && agency.Id > 0))
            {
                _loggingService.LogWarning("Intento de eliminar usuario existente durante rollback", new Dictionary<string, string>
                {
                    { "Email", email },
                    { "UserId", user.Id },
                    { "RolesCount", userRoles.Count.ToString() },
                    { "HasAgency", (agency != null).ToString() }
                });
                return new BadRequestObjectResult(new { Message = "No se puede eliminar un usuario existente durante el rollback" });
            }

            // Eliminar el usuario de su rol de Administrador (solo si tiene el rol)
            if (userRoles.Contains("agency_administrator"))
            {
                await _userManager.RemoveFromRoleAsync(user, "agency_administrator");
            }

            // Eliminar la contraseña temporal del usuario
            await DeleteTemporaryPassword(user.Id);

            // Si el usuario tiene una agencia asignada, eliminarla
            if (agency != null)
            {
                await _agencyRepository.DeleteAgency(agency.Id);
            }

            // Finalmente, eliminar el usuario
            await _userManager.DeleteAsync(user);

            return new OkObjectResult(new { Message = "Usuario y datos asociados eliminados exitosamente" });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al eliminar el usuario y sus datos asociados por correo electrónico", new Dictionary<string, string>
            {
                { "Email", email }
            });
            return new BadRequestObjectResult(new { Message = "Error al eliminar el usuario", Error = ex.Message });
        }
    }

    /// ------------------------------------------------------------------------------------------------
    /// Métodos para insertar y eliminar contraseñas temporales
    /// ------------------------------------------------------------------------------------------------

    /// <summary>
    /// Inserta una contraseña temporal en la base de datos
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <param name="temporaryPassword">La contraseña temporal</param>
    /// <returns>El resultado de la operación</returns>
    public async Task InsertTemporaryPassword(string userId, string temporaryPassword)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new { UserId = userId, TemporaryPassword = temporaryPassword };
            await dbConnection.ExecuteAsync("100_InsertTemporaryPassword", param, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al insertar la contraseña temporal");
            throw new Exception("No se pudo insertar la contraseña temporal", ex);
        }
    }


    /// <summary>
    /// Obtiene la contraseña temporal de un usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>La contraseña temporal o null si no existe</returns>
    public async Task<string?> GetTemporaryPassword(string userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new { UserId = userId };
            return await dbConnection.QueryFirstOrDefaultAsync<string>("100_GetTemporaryPassword", param, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al obtener la contraseña temporal");
            throw new Exception("No se pudo obtener la contraseña temporal", ex);
        }
    }

    /// <summary>
    /// Elimina una contraseña temporal por el ID del usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>El resultado de la operación</returns>
    public async Task DeleteTemporaryPassword(string userId)
    {
        try
        {
            using IDbConnection dbConnection = _context.CreateConnection();
            var param = new { UserId = userId };
            await dbConnection.ExecuteAsync("100_DeleteTemporaryPassword", param, commandType: CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al eliminar la contraseña temporal");
            throw new Exception("No se pudo eliminar la contraseña temporal", ex);
        }
    }

    /// <summary>
    /// Fuerza una nueva contraseña para un usuario y envía un correo con las credenciales
    /// </summary>
    /// <param name="userId">ID del usuario</param>
    /// <param name="newPassword">Nueva contraseña</param>
    /// <returns>True si la operación fue exitosa</returns>
    public async Task<bool> ForcePassword(string userId)
    {
        try
        {

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            // En producción, usar la contraseña proporcionada
            var password = Utilities.GenerateTemporaryPassword();

            // Remover la contraseña actual
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);

            if (!removePasswordResult.Succeeded)
            {
                return false;
            }

            // Establecer la nueva contraseña
            var addPasswordResult = await _userManager.AddPasswordAsync(user, password);

            if (!addPasswordResult.Succeeded)
            {
                return false;
            }

            // Actualizar el estado del usuario
            user.IsTemporalPasswordActived = true;
            user.UpdatedAt = DateTime.Now;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                return false;
            }

            var dtoUser = _mapper.Map<DTOUser>(user);
            await _emailService.SendPasswordChangedEmail(dtoUser, password);

            return true;
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al forzar contraseña para usuario {UserId}", new Dictionary<string, string> { { "UserId", userId } });

            throw;
        }
    }

    /// <summary>
    /// Genera un token para restablecer la contraseña y envía un correo electrónico
    /// </summary>
    public async Task<bool> GeneratePasswordResetTokenAndSendEmail(string email)
    {
        try
        {
            // Generar un token seguro
            var token = await _userManager.GeneratePasswordResetTokenAsync(await _userManager.FindByEmailAsync(email));
            var expirationDate = DateTime.UtcNow.AddMinutes(30); // Token válido por 30 minutos

            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@email", email, DbType.String);
            parameters.Add("@token", token, DbType.String);
            parameters.Add("@expirationDate", expirationDate, DbType.DateTime);

            var result = await connection.ExecuteScalarAsync<int>(
                "109_GeneratePasswordResetToken",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            switch (result)
            {
                case 0: // Éxito
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user != null)
                    {
                        var webUrl = Utilities.GetUrl(_appSettings);
                        var resetLink = $"{webUrl}reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
                        await _emailService.SendPasswordResetEmail(email, resetLink);
                        _loggingService.LogInformation("Email de restablecimiento enviado", new Dictionary<string, string> { { "Email", email } });
                        return true;
                    }
                    break;
                case -1:
                    await _loggingService.LogError(new Exception("Token inválido"), "Token inválido generado", new Dictionary<string, string> { { "Email", email } });
                    break;
                case -2:
                    await _loggingService.LogError(new Exception("Token inválido"), "Token inválido generado", new Dictionary<string, string> { { "Email", email } });
                    break;
                case -3:
                    await _loggingService.LogError(new Exception("Token expirado"), "Token expirado", new Dictionary<string, string> { { "Email", email } });
                    break;
                case -4:
                    await _loggingService.LogError(new Exception("Usuario no encontrado o inactivo"), "Usuario no encontrado o inactivo: {Email}", new Dictionary<string, string> { { "Email", email } });
                    break;
                case -5:
                    await _loggingService.LogError(new Exception("Demasiados intentos de restablecimiento"), "Demasiados intentos de restablecimiento para {Email}", new Dictionary<string, string> { { "Email", email } });
                    break;
                case -99:
                    await _loggingService.LogError(new Exception("Error general"), "Error general al generar token", new Dictionary<string, string> { { "Email", email } });
                    break;
                default:
                    await _loggingService.LogError(new Exception("Error desconocido"), "Error desconocido al generar token", new Dictionary<string, string> { { "Email", email }, { "Result", result.ToString() } });
                    break;
            }

            return false;
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al generar token de restablecimiento para {Email}", new Dictionary<string, string> { { "Email", email } });
            return false;
        }
    }

    /// <summary>
    /// Valida un token de restablecimiento de contraseña
    /// </summary>
    public async Task<bool> ValidatePasswordResetToken(string email, string token)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@email", email, DbType.String);
            parameters.Add("@token", token, DbType.String);

            var result = await connection.ExecuteScalarAsync<int>(
                "110_ValidatePasswordResetToken",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            switch (result)
            {
                case 0: // Éxito
                    _loggingService.LogInformation("Token validado exitosamente para {Email}", new Dictionary<string, string> { { "Email", email } });
                    return true;
                case -1:
                    _loggingService.LogWarning("Email inválido: {Email}", new Dictionary<string, string> { { "Email", email } });
                    break;
                case -2:
                    _loggingService.LogWarning("Token inválido para {Email}", new Dictionary<string, string> { { "Email", email } });
                    break;
                case -3:
                    _loggingService.LogWarning("Usuario no encontrado o inactivo: {Email}", new Dictionary<string, string> { { "Email", email } });
                    break;
                case -4:
                    _loggingService.LogWarning("Token no encontrado para {Email}", new Dictionary<string, string> { { "Email", email } });
                    break;
                case -5:
                    await _loggingService.LogError(new Exception("Token expirado"), "Token expirado para {Email}", new Dictionary<string, string> { { "Email", email } });
                    break;
                case -99:
                    await _loggingService.LogError(new Exception("Error general"), "Error general al validar token para {Email}", new Dictionary<string, string> { { "Email", email } });
                    break;
                default:
                    await _loggingService.LogError(new Exception("Error desconocido"), "Error desconocido al validar token", new Dictionary<string, string> { { "Email", email }, { "Result", result.ToString() } });
                    break;
            }

            return false;
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al validar token de restablecimiento para {Email}", new Dictionary<string, string> { { "Email", email } });
            return false;
        }
    }

    /// <summary>
    /// Restablece la contraseña usando un token válido
    /// </summary>
    public async Task<bool> ResetPasswordWithToken(string email, string token, string newPassword)
    {
        try
        {
            // Validar el token primero
            if (!await ValidatePasswordResetToken(email, token))
            {
                return false;
            }

            // Buscar el usuario
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return false;
            }

            // Generar token de reset de Identity
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Cambiar la contraseña
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (result.Succeeded)
            {
                // Desactivar contraseña temporal si estaba activa
                if (user.IsTemporalPasswordActived)
                {
                    user.IsTemporalPasswordActived = false;
                    await _userManager.UpdateAsync(user);
                }
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al restablecer contraseña para {Email}", new Dictionary<string, string> { { "Email", email } });
            throw;
        }
    }

    /// <summary>
    /// Asigna permisos CRUD de escuelas a un usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    private async Task AssignSchoolCrudPermissionsToUserAsync(string userId)
    {
        using var db = _context.CreateConnection();
        // Llama al SP que asigna los permisos CRUD de escuelas
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.String, size: 450);
        await db.ExecuteAsync("100_AssignSchoolCrudPermissionsToUser", parameters, commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// Asigna permisos CRUD de staff a un usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    private async Task AssignStaffCrudPermissionsToUserAsync(string userId)
    {
        using var db = _context.CreateConnection();
        // Llama al SP que asigna los permisos CRUD de staff
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.String, size: 450);
        await db.ExecuteAsync("100_AssignStaffCrudPermissionsToUser", parameters, commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// Asigna permisos CRUD de sitios a un usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    private async Task AssignSiteCrudPermissionsToUserAsync(string userId)
    {
        using var db = _context.CreateConnection();
        // Llama al SP que asigna los permisos CRUD de sitios
        var parameters = new DynamicParameters();
        parameters.Add("@userId", userId, DbType.String, size: 450);
        await db.ExecuteAsync("100_AssignSiteCrudPermissionsToUser", parameters, commandType: CommandType.StoredProcedure);
    }

    /// <summary>
    /// Obtiene todos los códigos de agencias existentes
    /// </summary>
    /// <returns>Lista de códigos de agencias</returns>
    private async Task<List<string>> GetExistingAgencyCodes()
    {
        try
        {
            using var connection = _context.CreateConnection();
            var codes = await connection.QueryAsync<string>("112_GetExistingAgencyCodes", commandType: CommandType.StoredProcedure);
            return codes.ToList();
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al obtener códigos de agencias existentes");
            throw;
        }
    }

    /// <summary>
    /// Verifica si un correo electrónico ya existe en el sistema
    /// </summary>
    /// <param name="email">El correo electrónico a verificar</param>
    /// <returns>True si el correo existe, False si no existe</returns>
    public async Task<bool> EmailExists(string email)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            // Normalizar el email para comparación (case-insensitive)
            var normalizedEmail = email.Trim().ToLowerInvariant();

            // 1. Verificar en AspNetUsers (Identity)
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                _loggingService.LogInformation("Correo encontrado en AspNetUsers", new Dictionary<string, string>
                {
                    { "Email", email }
                });
                return true;
            }

            // 2. Verificar en Staff table usando Stored Procedure
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@email", normalizedEmail, DbType.String);
            parameters.Add("@exists", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            await db.ExecuteAsync("100_CheckStaffEmailExists", parameters, commandType: CommandType.StoredProcedure);

            var staffExists = parameters.Get<bool>("@exists");

            if (staffExists)
            {
                _loggingService.LogInformation("Correo encontrado en Staff", new Dictionary<string, string>
                {
                    { "Email", email }
                });
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al verificar si el correo existe", new Dictionary<string, string>
            {
                { "Email", email }
            });
            // En caso de error, retornar false para no bloquear el registro
            return false;
        }
    }

}

