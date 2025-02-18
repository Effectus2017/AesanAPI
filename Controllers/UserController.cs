using System.Threading.Tasks;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("user")]
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los usuarios.
/// Proporciona endpoints para la gestión de usuarios, roles y programas,
/// incluyendo el registro de usuarios y agencias.
/// </summary>
public class UserController(IUnitOfWork unitOfWork, ILogger<UserController> logger) : Controller
{
    private readonly ILogger<UserController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// ------------------------------------------------------------------------------------------------
    /// Métodos para obtener información de usuarios
    /// ------------------------------------------------------------------------------------------------

    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    /// <param name="id">El ID del usuario</param>
    /// <returns>El usuario</returns>
    [HttpGet("get-user-by-id")]
    [SwaggerOperation(Summary = "Obtiene un usuario por su ID", Description = "Devuelve un usuario basado en el ID proporcionado.")]
    public IActionResult GetUserById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo usuario con ID: {UserId}", queryParameters.UserId);
                DTOUser _result = _unitOfWork.UserRepository.GetUserById(queryParameters.UserId);
                return _result != null ? StatusCode(StatusCodes.Status200OK, _result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Obtiene todos los usuarios de la base de datos
    /// </summary>
    /// <returns>Lista de usuarios</returns>
    [HttpGet("get-all-users-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los usuarios de la base de datos", Description = "Devuelve una lista de todos los usuarios.")]
    public IActionResult GetAllUsersFromDb([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo lista de usuarios con parámetros: {@QueryParameters}", queryParameters);
                dynamic _result = _unitOfWork.UserRepository.GetAllUsersFromDb(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.UserId);

                return _result != null ? StatusCode(StatusCodes.Status200OK, _result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Obtiene todos los roles de la base de datos
    /// </summary>
    /// <returns>Los roles</returns>
    [HttpGet("get-all-roles-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los roles de la base de datos", Description = "Devuelve una lista de todos los roles.")]
    public IActionResult GetAllRolesFromDb()
    {
        try
        {
            if (ModelState.IsValid)
            {
                dynamic _result = _unitOfWork.UserRepository.GetAllRolesFromDb();
                return _result != null ? StatusCode(StatusCodes.Status200OK, _result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Obtiene todos los programas de la base de datos usando un Stored Procedure
    /// </summary>
    /// <returns>Los programas</returns>
    [HttpGet("get-all-users-from-db-with-sp")]
    [SwaggerOperation(Summary = "Obtiene todos los programas de la base de datos usando un Stored Procedure", Description = "Devuelve una lista de todos los programas.")]
    public async Task<IActionResult> GetAllUsersFromDbWithSP([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                dynamic _result = await _unitOfWork.UserRepository.GetAllUsersFromDbWithSP(queryParameters.Take, queryParameters.Skip, queryParameters.Name);
                return _result != null ? StatusCode(StatusCodes.Status200OK, _result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// ------------------------------------------------------------------------------------------------
    /// Método para registrar un usuario
    /// ------------------------------------------------------------------------------------------------

    /// <summary>
    /// Aplicacion al programa de auspiciadores, creacion de agencia y usuario
    /// </summary>
    /// <param name="model">El modelo de registro de usuario</param>
    /// <returns>El resultado del registro</returns>
    [HttpPost("register-user-agency")]
    [SwaggerOperation(Summary = "Aplicación al programa de auspiciadores", Description = "Crea una agencia y un usuario en el sistema.")]
    public async Task<IActionResult> RegisterUserAgency([FromBody] UserAgencyRequest model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Registrando usuario en la agencia: {@UserAgencyRequest}", model);

                if (model.Agency == null || model.User == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Los campos 'Agency' y 'User' son requeridos." });
                }

                var result = await _unitOfWork.UserRepository.RegisterUserAgency(model);
                return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Agrega un usuario a la base de datos
    /// </summary>
    /// <param name="entity">El usuario a agregar</param>
    /// <returns>El resultado de la operación</returns>
    [HttpPost("add-user-to-db")]
    [SwaggerOperation(Summary = "Agrega un usuario a la base de datos", Description = "Agrega un usuario a la base de datos.")]
#if !DEBUG
    [Authorize(Roles = "Administrator")]
#endif
    public async Task<IActionResult> AddUserToDb([FromBody] DTOUser entity)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Agregando usuario a la base de datos: {@DTOUser}", entity);

                if (entity == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "El campo 'entity' es requerido." });
                }

                if (entity.Roles == null || entity.Roles.Count == 0)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "El campo 'Roles' es requerido." });
                }

                var result = await _unitOfWork.UserRepository.RegisterUser(entity, entity.Roles.FirstOrDefault() ?? "Monitor");
                return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);

            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }
}
