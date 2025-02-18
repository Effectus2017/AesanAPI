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

namespace Api.Repositories;

public class UserRepository(UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IOptions<ApplicationSettings> appSettings,
    IMapper mapper,
    ILogger<UserRepository> logger,
    DapperContext context,
    IEmailService emailService,
    IAgencyRepository agencyRepository) : IUserRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;
    private readonly ApplicationSettings _appSettings = appSettings.Value;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UserRepository> _logger = logger;
    private readonly IEmailService _emailService = emailService;
    private readonly IAgencyRepository _agencyRepository = agencyRepository;
    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    /// <param name="id">El ID del usuario</param>
    /// <returns>El usuario</returns>
    public DTOUser GetUserById(string userId)
    {
        try
        {
            var _result = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefault(x => x.Id == userId);
            var _complete = _mapper.Map<User, DTOUser>(_result);
            return _complete;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
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

    // <summary>
    /// Obtiene todos los usuarios de la base de datos usando un Stored Procedure
    /// </summary>
    /// <param name="take">El número de usuarios a obtener</param>
    /// <param name="skip">El número de usuarios a saltar</param>
    /// <param name="name">El nombre del usuario a buscar</param>
    /// <returns>Una lista de usuarios con el conteo total</returns>
    public async Task<DTOUserResponse> GetAllUsersFromDbWithSP(int take, int skip, string name, int? agencyId = null)
    {
        try
        {
            _logger.LogInformation("Obteniendo usuarios con SP");
            using IDbConnection db = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@take", take, DbType.Int32);
            parameters.Add("@skip", skip, DbType.Int32);
            parameters.Add("@name", name, DbType.String);
            parameters.Add("@agencyId", agencyId, DbType.Int32);

            var result = await db.QueryMultipleAsync("100_GetAllUsersFromDb", parameters, commandType: CommandType.StoredProcedure);
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
                Roles = new List<string> { u.RoleName }
            }).ToList();

            return new DTOUserResponse
            {
                Data = users,
                Count = count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios con SP");
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
    public dynamic GetAllProgramsFromDb(int take, int skip, string name, bool alls)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los programas de la base de datos");

            using IDbConnection dbConnection = _context.CreateConnection();

            var param = new { take, skip, name, alls };

            var _result = dbConnection.QueryMultiple("100_GetPrograms", param, commandType: CommandType.StoredProcedure);

            if (_result == null)
            {
                return null;
            }

            var programs = _result.Read<dynamic>().Select(item => new DTOProgram
            {
                Id = item.Id,
                Name = item.Name,
                Description = item.Description
            }).ToList();

            var count = _result.Read<int>().Single();

            var _complete = new { data = programs, count };

            return _complete;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
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
            _logger.LogInformation("Iniciando sesión en el sistema");

            User? _user = await _userManager.FindByNameAsync(model.UserName);

            if (_user == null)
            {
                return new UnauthorizedObjectResult(new { Message = "Usuario no encontrado" });
            }

            if (_user.IsTemporalPasswordActived)
            {
                return new ConflictObjectResult(new { Message = "La contraseña temporal es válida." });
            }

#if !DEBUG
            if (!_user.IsActive)
            {
                return new BadRequestObjectResult(new { Message = "El usuario está deshabilitado." });
            }
#endif

#if !DEBUG
            if (!_user.EmailConfirmed)
            {
                return new BadRequestObjectResult(new { Message = "El correo electrónico no está confirmado." });
            }
#endif

            var passwordValid = await _userManager.CheckPasswordAsync(_user, model.Password);

            if (!passwordValid)
            {
                return new UnauthorizedObjectResult(new { Message = "Contraseña incorrecta" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var days = 2;

            // Obtener los roles del usuario
            var roles = await _userManager.GetRolesAsync(_user);

            _logger.LogInformation("Obteniendo la agencia del usuario");

            // Obtener la agencia del usuario
            var agency = _user.AgencyId != null && _user.AgencyId != 0 ? await _agencyRepository.GetAgencyById(_user.AgencyId.Value) : null;
            var userPrograms = await _agencyRepository.GetAgencyProgramsByUserId(_user.Id);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GetClaims(_user, roles, agency, userPrograms),
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
    private static ClaimsIdentity GetClaims(User user, IList<string> roles, DTOAgency agency, List<DTOProgram> userPrograms)
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
            claims.AddClaim(new Claim("programs", string.Join(",", agency.Programs.Select(p => p.Name))));
            claims.AddClaim(new Claim("programIds", string.Join(",", agency.Programs.Select(p => p.Id.ToString()))));

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

            _logger.LogInformation("Contraseña temporal: {temporaryPassword}", temporaryPassword);

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

                _logger.LogInformation("Insertando la contrasea temporal en la base de datos: {temporaryPassword}", temporaryPassword);
                await InsertTemporaryPassword(user.Id, temporaryPassword);
            }

            _logger.LogInformation("Insertando la agencia en la base de datos");

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

            // Actualizar el usuario con el ID de la agencia
            user.AgencyId = agencyId;
            var updateResult = await _userManager.UpdateAsync(user);

            // Si falla la actualización, eliminar el usuario creado en Identity
            if (!updateResult.Succeeded)
            {
                await RemoveUserAndAgencyRelatedDataByEmail(model.User.Email);
                return new BadRequestObjectResult(updateResult.Errors);
            }

            // Enviar correo con la contraseña temporal y bienvenida
            await _emailService.SendWelcomeAgencyEmail(model, temporaryPassword);

            return new OkObjectResult(new { Message = "Usuario registrado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar usuario");

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
    public async Task<dynamic> RegisterUser(DTOUser model, string role)
    {

        try
        {

#if !DEBUG
            // Generar una contraseña temporal
            var temporaryPassword = Utilities.GenerateTemporaryPassword();
#else
            // Usar la contraseña temporal 9c272156 si estamos en debug
            var temporaryPassword = "9c272156";
#endif

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
                IsTemporalPasswordActived = true
            };

            var result = await _userManager.CreateAsync(user, temporaryPassword);

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

            // Enviar correo con la contraseña temporal
            await InsertTemporaryPassword(user.Id, temporaryPassword);
            await _emailService.SendTemporaryPasswordEmail(model.Email, temporaryPassword);

            return new OkObjectResult(new { Message = "Usuario registrado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar usuario");
            return new BadRequestObjectResult(new { Message = "Error al registrar usuario", Error = ex.Message });
        }
    }

    /// ------------------------------------------------------------------------------------------------
    /// Métodos para cambiar la contraseña y actualizar el avatar
    /// ------------------------------------------------------------------------------------------------

    /// <summary>
    /// Cambia la contraseña de un usuario
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
            _logger.LogError(ex, "Error al cambiar la contraseña");
            return new BadRequestObjectResult(new { Message = "Error al cambiar la contraseña", Error = ex.Message });
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
            _logger.LogError(ex, "Error al generar el token de restablecimiento de contraseña");
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
            user.UpdatedAt = DateTimeOffset.Now;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result.Errors);
            }

            return new OkObjectResult(new { Message = "Avatar actualizado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el avatar del usuario");
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
            user.UpdatedAt = DateTimeOffset.Now;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result.Errors);
            }

            return new OkObjectResult(new { Message = "Usuario deshabilitado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al deshabilitar el usuario");
            return new BadRequestObjectResult(new { Message = "Error al deshabilitar el usuario", Error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un usuario y sus datos asociados por su correo electrónico
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
            // Eliminar la agencia asociada al usuario
            if (user.AgencyId != null)
            {
                var agency = user.AgencyId.Value;
                await _agencyRepository.DeleteAgency(agency);
            }
            // Finalmente, eliminar el usuario
            await _userManager.DeleteAsync(user);

            return new OkObjectResult(new { Message = "Usuario y datos asociados eliminados exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el usuario y sus datos asociados por correo electrónico");
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
            _logger.LogError(ex, "Error al insertar la contraseña temporal");
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
            _logger.LogError(ex, "Error al obtener la contraseña temporal");
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
            _logger.LogError(ex, "Error al eliminar la contraseña temporal");
            throw new Exception("No se pudo eliminar la contraseña temporal", ex);
        }
    }

    /// <summary>
    /// Resetea la contraseña de un usuario usando la contraseña temporal
    /// </summary>
    /// <param name="model">Modelo con email, contraseña temporal y nueva contraseña</param>
    /// <returns>El resultado de la operación</returns>
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        try
        {
            // Buscar el usuario por email
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new NotFoundObjectResult(new { Message = "Usuario no encontrado" });
            }

            // Verificar si el usuario tiene una contraseña temporal activa
            if (!user.IsTemporalPasswordActived)
            {
                return new BadRequestObjectResult(new { Message = "No hay una contraseña temporal activa para este usuario" });
            }

            // Obtener la contraseña temporal almacenada
            var storedTempPassword = await GetTemporaryPassword(user.Id);

            if (storedTempPassword == null || storedTempPassword != model.TemporaryPassword)
            {
                return new BadRequestObjectResult(new { Message = "La contraseña temporal es inválida" });
            }

            // Remover la contraseña actual
            var removePasswordResult = await _userManager.RemovePasswordAsync(user);
            if (!removePasswordResult.Succeeded)
            {
                return new BadRequestObjectResult(removePasswordResult.Errors);
            }

            // Establecer la nueva contraseña
            var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                return new BadRequestObjectResult(addPasswordResult.Errors);
            }

            // Actualizar el estado de la contraseña temporal
            user.IsTemporalPasswordActived = false;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return new BadRequestObjectResult(updateResult.Errors);
            }

            // Eliminar la contraseña temporal de la base de datos
            await DeleteTemporaryPassword(user.Id);

            return new OkObjectResult(new { Message = "Contraseña actualizada exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al resetear la contraseña");
            return new BadRequestObjectResult(new { Message = "Error al resetear la contraseña", Error = ex.Message });
        }
    }
}

