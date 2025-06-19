using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Api.Interfaces;
using Api.Models;
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
    IMapper mapper,
    ILoggingService loggingService,
    DapperContext context,
    IEmailService emailService,
    IAgencyRepository agencyRepository,
    IAgencyUsersRepository agencyUsersRepository) : IUserRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;
    private readonly ApplicationSettings _appSettings = appSettings.Value;
    private readonly IMapper _mapper = mapper;
    private readonly ILoggingService _loggingService = loggingService;
    private readonly IEmailService _emailService = emailService;
    private readonly IAgencyRepository _agencyRepository = agencyRepository;
    private readonly IAgencyUsersRepository _agencyUsersRepository = agencyUsersRepository;
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
    /// Obtiene todos los usuarios de la base de datos
    /// </summary>
    /// <param name="take">El número de usuarios a obtener</param>
    /// <param name="skip">El número de usuarios a saltar</param>
    /// <param name="name">El nombre del usuario</param>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>Una lista de usuarios</returns>
    public dynamic GetAllUsersFromDb(int take, int skip, string name, string userId)
    {
        try
        {
            var _result = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).Skip(skip).Take(take).ToList();
            var _count = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ToList().Count;
            var _currentUser = _mapper.Map<List<User>, List<DTOUser>>(_result);
            var _complete = new { data = _currentUser, count = _count };
            return _complete;
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
    /// <returns>Una lista de usuarios con el conteo total</returns>
    public async Task<DTOUserResponse> GetAllUsersFromDbWithSP(int take, int skip, string name, int? agencyId = null, List<string> roles = null)
    {
        try
        {
            _loggingService.LogInformation("Obteniendo usuarios con SP");
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@agencyId", agencyId == 0 ? null : agencyId, DbType.Int32);

            // Add roles parameter if provided
            if (roles != null && roles.Count != 0)
            {
                parameters.Add("@roles", string.Join(",", roles), DbType.String);
            }

            var result = await db.QueryMultipleAsync("108_GetAllUsersFromDb", parameters, commandType: CommandType.StoredProcedure);
            var usersFromDb = await result.ReadAsync<DTOUserDB>();
            var count = await result.ReadSingleAsync<int>();

            var users = usersFromDb.Select(u => new DTOUser
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                MiddleName = u.MiddleName,
                FatherLastName = u.FatherLastName,
                MotherLastName = u.MotherLastName,
                AdministrationTitle = u.AdministrationTitle,
                PhoneNumber = u.PhoneNumber,
                ImageURL = u.ImageURL,
                IsActive = u.IsActive,
                IsTemporalPasswordActived = u.IsTemporalPasswordActived,
                EmailConfirmed = u.EmailConfirmed,
                Roles = [u.RoleName]
            }).ToList();

            return new DTOUserResponse
            {
                Data = users,
                Count = count
            };
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
            var _result = _roleManager.Roles.ToList();
            var _count = _roleManager.Roles.ToList().Count;
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
    /// Obtiene todos los programas de la base de datos
    /// </summary>
    /// <param name="take">El número de programas a obtener</param>
    /// <param name="skip">El número de programas a saltar</param>
    /// <param name="name">El nombre del programa</param>
    /// <returns>Una lista de programas</returns>
    // public dynamic GetAllProgramsFromDb(int take, int skip, string name, bool alls)
    // {
    //     try
    //     {
    //         _loggingService.LogInformation("Obteniendo todos los programas de la base de datos");

    //         using IDbConnection dbConnection = _context.CreateConnection();

    //         var param = new { take, skip, name, alls };

    //         var _result = dbConnection.QueryMultiple("100_GetPrograms", param, commandType: CommandType.StoredProcedure);

    //         if (_result == null)
    //         {
    //             return null;
    //         }

    //         var programs = _result.Read<dynamic>().Select(item => new DTOProgram
    //         {
    //             Id = item.Id,
    //             Name = item.Name,
    //             Description = item.Description
    //         }).ToList();

    //         var count = _result.Read<int>().Single();

    //         var _complete = new { data = programs, count };

    //         return _complete;
    //     }
    //     catch (Exception ex)
    //     {
    //         throw new Exception(ex.Message);
    //     }
    // }

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
                return new ConflictObjectResult(new { Message = "La contraseña temporal es válida." });
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

            // Generar el token de acceso
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var days = 2;

            // Obtener los roles del usuario
            var roles = await _userManager.GetRolesAsync(_user);
            var permissions = await GetPermissionsByUserId(_user.Id);

            _loggingService.LogInformation("Obteniendo la agencia del usuario");

            // Obtener la agencia del usuario
            var agency = await _agencyUsersRepository.GetUserAssignedAgency(_user.Id);
            var userPrograms = await _agencyRepository.GetAgencyProgramsByUserId(_user.Id);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GetClaims(_user, roles, agency, userPrograms, permissions),
                Expires = DateTime.UtcNow.AddDays(days),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var access_token = tokenHandler.WriteToken(token);

            var expires_in = TimeSpan.FromDays(days).TotalSeconds;

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
    /// <returns>Los claims del usuario</returns>
    private static ClaimsIdentity GetClaims(User user, IList<string> roles, dynamic agency, List<DTOProgram> userPrograms, dynamic permissions)
    {
        try
        {
            var claims = new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, user.Id), new(ClaimTypes.Name, user.UserName ?? "") });

            foreach (var role in roles)
            {
                claims.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            if (roles.Contains("Monitor"))
            {
                claims.AddClaim(new Claim("userId", user.Id));
                claims.AddClaim(new Claim("name", user.FirstName ?? ""));
                claims.AddClaim(new Claim("lastName", user.FatherLastName ?? ""));
                claims.AddClaim(new Claim("email", user.Email ?? ""));
                claims.AddClaim(new Claim("programs", string.Join(",", userPrograms.Select(p => p.Name))));
                claims.AddClaim(new Claim("programIds", string.Join(",", userPrograms.Select(p => p.Id.ToString()))));

                return claims;
            }

            //claims.AddClaim(new Claim("avatar", user.ImageURL));
            claims.AddClaim(new Claim("name", user.FirstName ?? ""));
            claims.AddClaim(new Claim("lastName", user.FatherLastName ?? ""));
            claims.AddClaim(new Claim("email", user.Email ?? ""));
            claims.AddClaim(new Claim("agency", agency.Name ?? ""));
            claims.AddClaim(new Claim("agencyId", agency.Id.ToString()));

            claims.AddClaim(new Claim("programs", string.Join(",", userPrograms.Select(p => p.Name))));
            claims.AddClaim(new Claim("programIds", string.Join(",", userPrograms.Select(p => p.Id.ToString()))));

            if (permissions != null)
            {
                foreach (var permission in permissions)
                {
                    claims.AddClaim(new Claim("permissions", permission.Name));
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

        try
        {

#if !DEBUG
            // Generar una contraseña temporal
            var temporaryPassword = Utilities.GenerateTemporaryPassword();
#else
            // Usar la contraseña temporal 9c272156 si estamos en debug
            var temporaryPassword = "9c272156";
#endif

            // Crear el usuario de Identity
            user = new User
            {
                UserName = model.User.Email,
                Email = model.User.Email,
                FirstName = model.User.FirstName,
                MiddleName = model.User.MiddleName,
                FatherLastName = model.User.FatherLastName,
                MotherLastName = model.User.MotherLastName,
                AdministrationTitle = model.User.AdministrationTitle,
                PhoneNumber = model.User.PhoneNumber,
                IsActive = true,
                IsTemporalPasswordActived = true,
                EmailConfirmed = false // Podría ser false si se requiere confirmación por correo
            };

            var result = await _userManager.CreateAsync(user, temporaryPassword);

            _loggingService.LogInformation("Insertando la contraseña temporal en la base de datos", new Dictionary<string, string>
            {
                { "TemporaryPassword", temporaryPassword }
            });

            if (!result.Succeeded)
            {
                await RemoveUserAndAgencyRelatedDataByEmail(model.User.Email);
                return new BadRequestObjectResult(result.Errors);
            }
            else
            {
                // Asignar el rol (asumiendo que el rol es "Agency-Administrator")
                var resultRole = await _userManager.AddToRoleAsync(user, "Agency-Administrator");

                if (!resultRole.Succeeded)
                {
                    await RemoveUserAndAgencyRelatedDataByEmail(model.User.Email);
                    return new BadRequestObjectResult(resultRole.Errors);
                }

                _loggingService.LogInformation("Insertando la contraseña temporal en la base de datos", new Dictionary<string, string> { { "temporaryPassword", temporaryPassword } });
                await InsertTemporaryPassword(user.Id, temporaryPassword);
            }

            _loggingService.LogInformation("Insertando la agencia en la base de datos");

            // Insertar la agencia
            int agencyId = await _agencyRepository.InsertAgency(model.Agency);

            // Verificar si la agencia se insertó correctamente
            if (agencyId == 0)
            {
                // Si falla la inserción, eliminar el usuario creado en Identity
                await RemoveUserAndAgencyRelatedDataByEmail(model.User.Email);
                return new BadRequestObjectResult(new { Message = "Error al insertar el usuario en la tabla Agency" });
            }

            // Insertar los programas de la agencia
            foreach (var programId in model.Agency.Programs)
            {
                await _agencyRepository.InsertAgencyProgram(agencyId, programId);
            }

            // Asignar agencia a usuario
            if (agencyId != 0)
            {
                await _agencyUsersRepository.AssignAgencyToUser(user.Id, agencyId, user.Id, true);
            }
            else
            {
                await RemoveUserAndAgencyRelatedDataByEmail(model.User.Email);
                return new BadRequestObjectResult(new { Message = "Error al insertar el usuario en la tabla Agency" });
            }

            // Enviar correo con la contraseña temporal y bienvenida
            await _emailService.SendWelcomeAgencyEmail(model, temporaryPassword);

            // Asignar permisos CRUD de escuelas al usuario
            await AssignSchoolCrudPermissionsToUserAsync(user.Id);

            return new OkObjectResult(new { Message = "Usuario registrado exitosamente" });
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Error al registrar usuario");

            if (user != null)
            {
                await RemoveUserAndAgencyRelatedDataByEmail(model.User.Email);
            }

            return new BadRequestObjectResult(new { Message = "Error al registrar usuario", Error = ex.Message });
        }
    }

    /// <summary>
    /// Registra un usuario en el sistema
    /// </summary>
    /// <param name="model">El modelo de registro de usuario</param>
    /// <param name="role">El rol del usuario</param>
    /// <returns>El resultado de la operación</returns>
    public async Task<dynamic> RegisterUser(DTOUser model, string role, int agencyId)
    {

        try
        {

            User user = new()
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                MiddleName = model.MiddleName,
                FatherLastName = model.FatherLastName,
                MotherLastName = model.MotherLastName,
                AdministrationTitle = model.AdministrationTitle,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                IsTemporalPasswordActived = true,
                ImageURL = model.ImageURL,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result.Errors);
            }

            // Asignar el rol al usuario
            var resultRole = await _userManager.AddToRoleAsync(user, role);

            if (!resultRole.Succeeded)
            {
                await RemoveUserAndAgencyRelatedDataByEmail(model.Email);
                return new BadRequestObjectResult(resultRole.Errors);
            }

            // Asignar la agencia al usuario
            await _agencyUsersRepository.AssignAgencyToUser(user.Id, agencyId, user.Id, false, false);

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
    /// Actualiza un usuario
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
            if (user.FirstName != entity.FirstName) changes.Add("FirstName", (user.FirstName, entity.FirstName));
            if (user.FatherLastName != entity.FatherLastName) changes.Add("FatherLastName", (user.FatherLastName, entity.FatherLastName));
            if (user.MiddleName != entity.MiddleName) changes.Add("MiddleName", (user.MiddleName, entity.MiddleName));
            if (user.MotherLastName != entity.MotherLastName) changes.Add("MotherLastName", (user.MotherLastName, entity.MotherLastName));
            if (user.AdministrationTitle != entity.AdministrationTitle) changes.Add("AdministrationTitle", (user.AdministrationTitle, entity.AdministrationTitle));
            if (user.PhoneNumber != entity.PhoneNumber) changes.Add("PhoneNumber", (user.PhoneNumber, entity.PhoneNumber));
            if (user.ImageURL != entity.ImageURL) changes.Add("ImageURL", (user.ImageURL, entity.ImageURL));

            // Actualizar propiedades
            user.Email = entity.Email;
            user.EmailConfirmed = entity.EmailConfirmed;
            user.FirstName = !string.IsNullOrEmpty(entity.FirstName) ? entity.FirstName : user.FirstName;
            user.FatherLastName = !string.IsNullOrEmpty(entity.FatherLastName) ? entity.FatherLastName : user.FatherLastName;
            user.MiddleName = !string.IsNullOrEmpty(entity.MiddleName) ? entity.MiddleName : user.MiddleName;
            user.MotherLastName = !string.IsNullOrEmpty(entity.MotherLastName) ? entity.MotherLastName : user.MotherLastName;
            user.AdministrationTitle = !string.IsNullOrEmpty(entity.AdministrationTitle) ? entity.AdministrationTitle : user.AdministrationTitle;
            user.PhoneNumber = !string.IsNullOrEmpty(entity.PhoneNumber) ? entity.PhoneNumber : user.PhoneNumber;
            user.ImageURL = !string.IsNullOrEmpty(entity.ImageURL) ? entity.ImageURL : user.ImageURL;

            try
            {
                var resultUser = await _userManager.UpdateAsync(user);

                if (!resultUser.Succeeded)
                {
                    return false;
                }

                if (entity.Roles?.Count > 0)
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var currentRole = currentRoles.FirstOrDefault();
                    var newRole = entity.Roles[0];

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

            user.ImageURL = imageUrl;
            user.UpdatedAt = DateTime.Now;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result.Errors);
            }

            return new OkObjectResult(new { Message = "Avatar actualizado exitosamente" });
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
    /// Elimina un usuario y todos sus datos relacionados por correo electrónico
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

            // Eliminar el usuario de su rol de Administrador
            await _userManager.RemoveFromRoleAsync(user, "Agency-Administrator");

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
            await _loggingService.LogError(ex, "Error al eliminar el usuario y sus datos asociados por correo electrónico");
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
        parameters.Add("@userId", userId);
        await db.ExecuteAsync("100_AssignSchoolCrudPermissionsToUser", parameters, commandType: CommandType.StoredProcedure);
    }

}

