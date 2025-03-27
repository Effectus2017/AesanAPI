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
    public async Task<IActionResult> GetUserById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo usuario con ID: {UserId}", queryParameters.UserId);
                DTOUser _result = await _unitOfWork.UserRepository.GetUserById(queryParameters.UserId);
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
                dynamic _result = await _unitOfWork.UserRepository.GetAllUsersFromDbWithSP(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.AgencyId, queryParameters.Roles);
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
    public async Task<IActionResult> AddUserToDb([FromBody] DTOUser entity, [FromQuery] QueryParameters queryParameters)
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

                var result = await _unitOfWork.UserRepository.RegisterUser(entity, entity.Roles.FirstOrDefault() ?? "Monitor", queryParameters.AgencyId);
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
    /// Actualiza modelo de usuario
    /// </summary>
    /// <param name="entity">Modelo a actualizar</param>
    /// <response code="200">Modelo no actualizado</response>
    /// <response code="202">Modelo actualizado correctamente</response>
    /// <response code="400">Incapaz actualizar el modelo</response>
    [HttpPut("update-user-from-db")]
    public async Task<IActionResult> Put([FromBody] DTOUser entity)
    {
        try
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
            }

            if (entity == null)
            {
                return BadRequest(new { Message = "La entidad no puede ser nula" });
            }

            bool result = await _unitOfWork.UserRepository.Update(entity);

            if (result)
            {
                _logger.LogInformation("Usuario actualizado exitosamente: {@DTOUser}", entity);
                return StatusCode(StatusCodes.Status200OK, new { Valid = true, Message = "Usuario actualizado exitosamente" });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { Valid = false, Message = "No se pudo actualizar el usuario" });
            }
        }
        catch (Exception ex)
        {

            return StatusCode(StatusCodes.Status500InternalServerError, new { Valid = false, Message = "Error interno al actualizar el usuario", Detail = ex.Message });
        }
    }

    /// <summary>
    /// Elimina usuario de la base de datos
    /// </summary>
    /// <param name="userId">El Id del usuario</param>
    /// <returns></returns>
    [HttpDelete("delete-user-from-db")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (queryParameters != null)
            {
                bool _result = await _unitOfWork.UserRepository.Delete(queryParameters.UserId);

                return _result
                    ? StatusCode(StatusCodes.Status202Accepted, new { Valid = _result, Message = "Delete successfully" })
                    : StatusCode(StatusCodes.Status200OK, new { Valid = _result, Message = "Not deleted" });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { Valid = false, Message = "UserId is required" });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Para que un usuario pueda cambiar su contraseña por si mismo
    /// </summary>
    /// <param name="model"></param>
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                bool _result = await _unitOfWork.UserRepository.ChangePassword(queryParameters.UserId, queryParameters.Password, queryParameters.NewPassword);

                return _result
                    ? StatusCode(StatusCodes.Status202Accepted, new { Valid = true, Message = "Updated successfully" })
                    : StatusCode(StatusCodes.Status200OK, new { Valid = false, Message = "Not updated" });
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Para que un administrador pueda resetear la contraseña de un usuario
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                bool _result = await _unitOfWork.UserRepository.ResetPassword(queryParameters.UserId);

                return _result
                    ? StatusCode(StatusCodes.Status202Accepted, new { Valid = true, Message = "Updated successfully" })
                    : StatusCode(StatusCodes.Status200OK, new { Valid = false, Message = "Not updated" });
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Actualiza la contraseña temporal de un usuario
    /// </summary>
    /// <param name="queryParameters">Parámetros con UserId y NewPassword</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("update-temporal-password")]
    [SwaggerOperation(Summary = "Actualiza la contraseña temporal de un usuario", Description = "Permite a un administrador actualizar la contraseña temporal de un usuario.")]
    public async Task<IActionResult> UpdateTemporalPassword([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                bool _result = await _unitOfWork.UserRepository.UpdateTemporalPassword(queryParameters.Email, queryParameters.NewPassword, queryParameters.TemporaryPassword);

                return _result
                    ? StatusCode(StatusCodes.Status202Accepted, new { Valid = true, Message = "Updated successfully" })
                    : StatusCode(StatusCodes.Status200OK, new { Valid = false, Message = "Not updated" });
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Permite a un administrador forzar una nueva contraseña para un usuario
    /// </summary>
    /// <param name="queryParameters">Parámetros con UserId y NewPassword</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("force-password")]
    [SwaggerOperation(Summary = "Fuerza una nueva contraseña para un usuario", Description = "Permite a un administrador asignar una nueva contraseña a un usuario.")]
    public async Task<IActionResult> ForcePassword([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(queryParameters.UserId))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "UserId y NewPassword son requeridos." });
                }

                bool result = await _unitOfWork.UserRepository.ForcePassword(queryParameters.UserId);

                if (result)
                {
                    return StatusCode(
                        StatusCodes.Status200OK,
                        new { Valid = true, Message = "Contraseña actualizada exitosamente" }
                    );
                }
                else
                {
                    return StatusCode(
                        StatusCodes.Status400BadRequest,
                        new { Valid = false, Message = "No se pudo actualizar la contraseña" }
                    );
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al forzar contraseña. Tipo: {ErrorType}, Mensaje: {ErrorMessage}",
                ex.GetType().Name,
                ex.Message
            );
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

}
