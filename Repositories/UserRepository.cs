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
    IEmailService emailService) : IUserRepository
{
    private readonly DapperContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<Role> _roleManager = roleManager;
    private readonly ApplicationSettings _appSettings = appSettings.Value;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<UserRepository> _logger = logger;
    private readonly IEmailService _emailService = emailService;

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
            User? _user = await _userManager.FindByNameAsync(model.UserName);

            if (_user == null)
            {
                return new UnauthorizedResult();
            }

            var roles = await _userManager.GetRolesAsync(_user);

            var passwordValid = await _userManager.CheckPasswordAsync(_user, model.Password);

            if (!passwordValid)
            {
                return new UnauthorizedResult();
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var days = 2;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GetClaims(_user, roles),
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
    private static ClaimsIdentity GetClaims(User user, IList<string> roles)
    {
        try
        {
            var claims = new ClaimsIdentity(new Claim[] { new(ClaimTypes.NameIdentifier, user.Id), new(ClaimTypes.Name, user.UserName ?? "") });

            foreach (var role in roles)
            {
                claims.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            //claims.AddClaim(new Claim("avatar", user.ImageURL));
            claims.AddClaim(new Claim("name", user.FirstName ?? ""));
            claims.AddClaim(new Claim("lastName", user.FatherLastName ?? ""));
            claims.AddClaim(new Claim("email", user.Email ?? ""));
            claims.AddClaim(new Claim("agency", "AESAN"));

            return claims;
        }
        catch (Exception ex)
        {
            throw new Exception("Error al obtener los claims del usuario", ex);
        }
    }

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
            // Generar una contraseña temporal
            var temporaryPassword = Utilities.GenerateTemporaryPassword();

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
                EmailConfirmed = true // Podría ser false si se requiere confirmación por correo
            };

            var result = await _userManager.CreateAsync(user, temporaryPassword);

            _logger.LogInformation("Contraseña temporal: {temporaryPassword}", temporaryPassword);

            if (!result.Succeeded)
            {
                await DeleteUserByEmailAsync(model.User.Email);
                return new BadRequestObjectResult(result.Errors);
            }
            else
            {
                // Asignar el rol (asumiendo que el rol es "User")
                var resultRole = await _userManager.AddToRoleAsync(user, "User");

                if (!resultRole.Succeeded)
                {
                    await DeleteUserByEmailAsync(model.User.Email);
                    return new BadRequestObjectResult(resultRole.Errors);
                }
            }

            // Parámetros para la inserción de la agencia
            var parameters = new DynamicParameters();
            parameters.Add("@Name", model.Agency.Name, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("@StatusId", model.Agency.StatusId, dbType: DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@ProgramId", model.Agency.ProgramId, dbType: DbType.Int32, direction: ParameterDirection.Input);

            // Datos de la Agencia
            parameters.Add("@SdrNumber", model.Agency.SdrNumber, dbType: DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@UieNumber", model.Agency.UieNumber, dbType: DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@EinNumber", model.Agency.EinNumber, dbType: DbType.Int32, direction: ParameterDirection.Input);

            // Datos de la Ciudad y Región
            parameters.Add("@CityId", model.Agency.CityId, dbType: DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@RegionId", model.Agency.RegionId, dbType: DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@Latitude", model.Agency.Latitude, dbType: DbType.Double, direction: ParameterDirection.Input);
            parameters.Add("@Longitude", model.Agency.Longitude, dbType: DbType.Double, direction: ParameterDirection.Input);

            // Dirección y Teléfono
            parameters.Add("@Address", model.Agency.Address, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("@ZipCode", model.Agency.ZipCode, dbType: DbType.Int32, direction: ParameterDirection.Input);
            parameters.Add("@PostalAddress", model.Agency.PostalAddress, dbType: DbType.String, direction: ParameterDirection.Input);
            parameters.Add("@Phone", model.Agency.Phone, dbType: DbType.String, direction: ParameterDirection.Input);

            // Datos del Contacto
            parameters.Add("@Email", model.User.Email, dbType: DbType.String, direction: ParameterDirection.Input);

            // Datos de elegibilidad
            parameters.Add("@NonProfit", model.Agency.NonProfit, dbType: DbType.Boolean, direction: ParameterDirection.Input);
            parameters.Add("@FederalFundsDenied", model.Agency.FederalFundsDenied, dbType: DbType.Boolean, direction: ParameterDirection.Input);
            parameters.Add("@StateFundsDenied", model.Agency.StateFundsDenied, dbType: DbType.Boolean, direction: ParameterDirection.Input);

            // Id de la Agencia
            parameters.Add("@Id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            // Insertar en la tabla Agency
            using IDbConnection dbConnection = _context.CreateConnection();

            await dbConnection.ExecuteAsync("100_InsertAgency", parameters, commandType: CommandType.StoredProcedure);
            // Obtener el ID de la agencia insertada
            int agencyId = parameters.Get<int>("@Id");
            // Si el ID de la agencia es 0, significa que hubo un error en la inserción
            if (agencyId == 0)
            {
                // Si falla la inserción, eliminar el usuario creado en Identity
                await DeleteUserByEmailAsync(model.User.Email);
                return new BadRequestObjectResult(new { Message = "Error al insertar el usuario en la tabla Agency" });
            }

            // Actualizar el usuario con el ID de la agencia
            user.AgencyId = agencyId;
            var updateResult = await _userManager.UpdateAsync(user);
            // Si falla la actualización, eliminar el usuario creado en Identity
            if (!updateResult.Succeeded)
            {
                await DeleteUserByEmailAsync(model.User.Email);
                return new BadRequestObjectResult(updateResult.Errors);
            }

            // Enviar correo con la contraseña temporal
            await _emailService.SendTemporaryPasswordEmail(model.User.Email, temporaryPassword);

            return new OkObjectResult(new { Message = "Usuario registrado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar usuario");

            if (user != null)
            {
                await DeleteUserByEmailAsync(model.User.Email);
            }

            return new BadRequestObjectResult(new { Message = "Error al registrar usuario", Error = ex.Message });
        }
    }

    /// <summary>
    /// Elimina un usuario por su correo electrónico
    /// </summary>
    /// <param name="email">El correo electrónico del usuario</param>
    /// <returns>El resultado de la operación</returns>
    public async Task<dynamic> DeleteUserByEmailAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
                await _userManager.RemoveFromRoleAsync(user, "User");
            }
            return new OkObjectResult(new { Message = "Usuario eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

}
