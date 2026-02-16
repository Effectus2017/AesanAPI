using System.Threading.Tasks;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;

namespace Api.Controllers;

[Route("user")]
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los usuarios.
/// Proporciona endpoints para la gestión de usuarios, roles y programas,
/// incluyendo el registro de usuarios y agencias.
/// </summary>
public class UserController(IUnitOfWork unitOfWork, ILoggingService loggingService, IUserRoleExtensionRequestRepository extensionRequestRepository, IEmailService emailService) : Controller
{
    private readonly ILoggingService _loggingService = loggingService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUserRoleExtensionRequestRepository _extensionRequestRepository = extensionRequestRepository;
    private readonly IEmailService _emailService = emailService;

    /// ------------------------------------------------------------------------------------------------
    /// Métodos para obtener información de usuarios
    /// ------------------------------------------------------------------------------------------------

    /// <summary>
    /// Obtiene un usuario por su ID
    /// </summary>
    /// <param name="id">El ID del usuario</param>
    /// <returns>El usuario</returns>
    // [AuthorizePermission("UserView")] // Ejemplo: proteger por permiso granular
    [HttpGet("get-user-by-id")]
    [SwaggerOperation(Summary = "Obtiene un usuario por su ID", Description = "Devuelve un usuario basado en el ID proporcionado.")]
    public async Task<IActionResult> GetUserById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _loggingService.LogInformation("Obteniendo usuario con ID: " + queryParameters.UserId, new Dictionary<string, string> { { "UserId", queryParameters.UserId } });
                DTOUser _result = await _unitOfWork.UserRepository.GetUserById(queryParameters.UserId);
                return _result != null ? StatusCode(StatusCodes.Status200OK, _result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al obtener usuario", new Dictionary<string, string> { { "UserId", queryParameters.UserId } });
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Obtiene un usuario por su ID usando Stored Procedure
    /// </summary>
    /// <param name="id">El ID del usuario</param>
    /// <returns>El usuario con datos completos desde Staff y Agency</returns>
    // [AuthorizePermission("UserView")] // Ejemplo: proteger por permiso granular
    [HttpGet("get-user-by-id-with-sp")]
    [SwaggerOperation(Summary = "Obtiene un usuario por su ID usando SP", Description = "Devuelve un usuario completo con datos desde Staff y Agency usando Stored Procedure.")]
    public async Task<IActionResult> GetUserByIdWithSP([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _loggingService.LogInformation("Obteniendo usuario con SP, ID: " + queryParameters.UserId, new Dictionary<string, string> { { "UserId", queryParameters.UserId } });
                DTOUser _result = await _unitOfWork.UserRepository.GetUserByIdWithSP(queryParameters.UserId);
                return _result != null ? StatusCode(StatusCodes.Status200OK, _result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al obtener usuario con SP", new Dictionary<string, string> { { "UserId", queryParameters.UserId } });
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Obtiene todos los usuarios de la base de datos
    /// </summary>
    /// <returns>Lista de usuarios</returns>
    // [AuthorizePermission("UserView")] // Ejemplo: proteger por permiso granular
    [HttpGet("get-all-users-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los usuarios de la base de datos", Description = "Devuelve una lista de todos los usuarios.")]
    public async Task<IActionResult> GetAllUsersFromDb([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _loggingService.LogInformation("Obteniendo lista de usuarios", new Dictionary<string, string> {
                    { "Take", queryParameters.Take.ToString() },
                    { "Skip", queryParameters.Skip.ToString() },
                    { "Name", queryParameters.Name ?? "null" }
                });
                dynamic _result = _unitOfWork.UserRepository.GetAllUsersFromDb(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.UserId, queryParameters.IsList);

                return _result != null ? StatusCode(StatusCodes.Status200OK, _result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al obtener lista de usuarios", new Dictionary<string, string> {
                { "Take", queryParameters.Take.ToString() },
                { "Skip", queryParameters.Skip.ToString() },
                { "Name", queryParameters.Name ?? "null" }
            });
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
                dynamic _result = await _unitOfWork.UserRepository.GetAllUsersFromDbWithSP(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.AgencyId, queryParameters.IsList, queryParameters.Roles, queryParameters.Alls);
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
                _loggingService.LogInformation("Registrando usuario en la agencia", new Dictionary<string, string> {
                    { "Email", model.Staff?.Email ?? "null" },
                    { "AgencyName", model.Agency?.Name ?? "null" }
                });

                if (model.Agency == null || model.Staff == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Los campos 'Agency' y 'Staff' son requeridos." });
                }

                var result = await _unitOfWork.UserRepository.RegisterUserAgency(model);
                return result != null ? StatusCode(StatusCodes.Status200OK, result) : StatusCode(StatusCodes.Status400BadRequest, ModelState);
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al registrar usuario en agencia", new Dictionary<string, string> {
                { "Email", model.Staff?.Email ?? "null" },
                { "AgencyName", model.Agency?.Name ?? "null" }
            });
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
                _loggingService.LogInformation("Agregando usuario a la base de datos", new Dictionary<string, string> { { "User", JsonSerializer.Serialize(entity) } });

                if (entity == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "El campo 'entity' es requerido." });
                }

                var usePrimarySecondary = !string.IsNullOrWhiteSpace(entity.PrimaryRoleName);
                if (!usePrimarySecondary && (entity.Roles == null || !entity.Roles.Any()))
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Debe asignar al menos un rol: use 'Roles' (lista) o 'PrimaryRoleName' (y opcionalmente 'SecondaryRoles')." });
                }

                var result = await _unitOfWork.UserRepository.RegisterUser(entity, entity.Roles ?? new List<string>(), queryParameters.AgencyId);
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
    /// Actualiza modelo de usuario usando Stored Procedure
    /// </summary>
    /// <param name="entity">Modelo a actualizar</param>
    /// <response code="200">Modelo no actualizado</response>
    /// <response code="202">Modelo actualizado correctamente</response>
    /// <response code="400">Incapaz actualizar el modelo</response>
    // [AuthorizePermission("UserEdit")] // Ejemplo: proteger por permiso granular
    [HttpPut("update-user-from-db-with-sp")]
    public async Task<IActionResult> PutWithSP([FromBody] DTOUser entity, [FromQuery] QueryParameters queryParameters)
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

            _loggingService.LogInformation("Iniciando actualización de usuario con SP", new Dictionary<string, string>
            {
                { "User", JsonSerializer.Serialize(entity) },
                { "UserId", entity.Id },
                { "Email", entity.Email },
                { "AgencyId", entity.AgencyId?.ToString() ?? "N/A" }
            });

            // Obtener el ID del usuario logueado
            var currentUserId = queryParameters.CurrentUserId;

            if (string.IsNullOrEmpty(currentUserId))
            {
                await _loggingService.LogError(new Exception("No se pudo obtener el ID del usuario logueado"), "Error al obtener usuario logueado");
                return StatusCode(StatusCodes.Status400BadRequest, new { Valid = false, Message = "No se pudo identificar al usuario" });
            }

            bool result = await _unitOfWork.UserRepository.UpdateWithSP(entity, currentUserId);

            if (result)
            {
                _loggingService.LogInformation("Usuario actualizado exitosamente con SP", new Dictionary<string, string> { { "User", JsonSerializer.Serialize(entity) } });
                return StatusCode(StatusCodes.Status200OK, new { Valid = true, Message = "Usuario actualizado exitosamente" });
            }
            else
            {
                await _loggingService.LogError(new Exception("UpdateWithSP retornó false"), "Fallo en actualización de usuario con SP", new Dictionary<string, string>
                {
                    { "User", JsonSerializer.Serialize(entity) },
                    { "UserId", entity.Id }
                });
                return StatusCode(StatusCodes.Status400BadRequest, new { Valid = false, Message = "No se pudo actualizar el usuario" });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { Valid = false, Message = "Error interno al actualizar el usuario", Detail = ex.Message });
        }
    }

    /// <summary>
    /// Solicitar extensión de vigencia de un rol secundario temporal.
    /// </summary>
    [HttpPost("role-extension-request")]
    [SwaggerOperation(Summary = "Solicitar extensión de rol temporal", Description = "Crea una solicitud de extensión y notifica por email a administradores.")]
    public async Task<IActionResult> RequestRoleExtension([FromBody] RoleExtensionRequestRequest request, [FromQuery] QueryParameters queryParameters)
    {
        try
        {
            var userId = queryParameters.CurrentUserId ?? queryParameters.UserId;
            if (string.IsNullOrEmpty(userId))
                return StatusCode(StatusCodes.Status401Unauthorized, new { Message = "Usuario no identificado." });

            if (request.RequestedValidTo == default)
                return StatusCode(StatusCodes.Status400BadRequest, new { Message = "RequestedValidTo es requerido." });

            var roleId = request.RoleId;
            if (string.IsNullOrEmpty(roleId) && !string.IsNullOrWhiteSpace(request.RoleName))
                roleId = await _unitOfWork.UserRepository.GetRoleIdByNameAsync(request.RoleName!.Trim());
            if (string.IsNullOrEmpty(roleId))
                return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Debe indicar RoleId o RoleName válido." });

            var id = await _extensionRequestRepository.InsertAsync(userId, roleId, request.RequestedValidTo, request.Reason);
            if (id <= 0)
                return StatusCode(StatusCodes.Status400BadRequest, new { Message = "No se pudo crear la solicitud." });

            var user = await _unitOfWork.UserRepository.GetUserByIdWithSP(userId);
            var userName = user != null ? $"{user.FirstName} {user.FatherLastName}".Trim() : "Usuario";
            var userEmail = user?.Email ?? "";
            var roleName = request.RoleName ?? (user?.SecondaryRoles?.FirstOrDefault(r => r.RoleId == roleId)?.RoleName) ?? "Rol temporal";
            var adminEmails = await _unitOfWork.UserRepository.GetUserEmailsByRoleNameAsync("Administrator");
            if (adminEmails != null && adminEmails.Any())
                await _emailService.SendRoleExtensionRequestToAdmins(adminEmails, userName, userEmail, roleName, request.RequestedValidTo, request.Reason);

            return StatusCode(StatusCodes.Status200OK, new { Id = id, Message = "Solicitud enviada. Un administrador revisará su petición." });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al solicitar extensión de rol");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Listar solicitudes de extensión de rol (admin). Filtro por estado: Pending, Approved, Rejected o null para todas.
    /// </summary>
    [HttpGet("role-extension-requests")]
    [SwaggerOperation(Summary = "Listar solicitudes de extensión", Description = "Solo administradores. Por defecto devuelve pendientes.")]
    public async Task<IActionResult> GetRoleExtensionRequests([FromQuery] string? status = "Pending", [FromQuery] int take = 50, [FromQuery] int skip = 0)
    {
        try
        {
            var (rows, total) = await _extensionRequestRepository.GetAsync(status, take, skip);
            return StatusCode(StatusCodes.Status200OK, new { data = rows, total });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al listar solicitudes de extensión");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Aprobar una solicitud de extensión de rol (admin).
    /// </summary>
    [HttpPost("role-extension-requests/{id:int}/approve")]
    [SwaggerOperation(Summary = "Aprobar solicitud de extensión", Description = "Opcionalmente indicar newValidTo para una fecha distinta a la solicitada.")]
    public async Task<IActionResult> ApproveRoleExtensionRequest([FromRoute] int id, [FromBody] ApproveExtensionRequest? body, [FromQuery] QueryParameters queryParameters)
    {
        try
        {
            var processedBy = queryParameters.CurrentUserId ?? queryParameters.UserId;
            var newValidTo = body?.NewValidTo;
            var ok = await _extensionRequestRepository.ApproveAsync(id, newValidTo, processedBy);
            if (!ok)
                return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Solicitud no encontrada o ya procesada." });

            var request = await _extensionRequestRepository.GetByIdAsync(id);
            if (request != null && !string.IsNullOrEmpty(request.UserEmail))
                await _emailService.SendRoleExtensionApprovedEmail(request.UserEmail, request.UserName ?? "Usuario", request.RoleName, newValidTo ?? request.RequestedValidTo);

            return StatusCode(StatusCodes.Status200OK, new { Message = "Solicitud aprobada." });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al aprobar solicitud de extensión");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Rechazar una solicitud de extensión de rol (admin).
    /// </summary>
    [HttpPost("role-extension-requests/{id:int}/reject")]
    [SwaggerOperation(Summary = "Rechazar solicitud de extensión")]
    public async Task<IActionResult> RejectRoleExtensionRequest([FromRoute] int id, [FromQuery] QueryParameters queryParameters)
    {
        try
        {
            var processedBy = queryParameters.CurrentUserId ?? queryParameters.UserId;
            var ok = await _extensionRequestRepository.RejectAsync(id, processedBy);
            if (!ok)
                return StatusCode(StatusCodes.Status400BadRequest, new { Message = "Solicitud no encontrada o ya procesada." });

            var request = await _extensionRequestRepository.GetByIdAsync(id);
            if (request != null && !string.IsNullOrEmpty(request.UserEmail))
                await _emailService.SendRoleExtensionRejectedEmail(request.UserEmail, request.UserName ?? "Usuario", request.RoleName);

            return StatusCode(StatusCodes.Status200OK, new { Message = "Solicitud rechazada." });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al rechazar solicitud de extensión");
            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Actualiza modelo de usuario (método original mantenido para compatibilidad)
    /// </summary>
    /// <param name="entity">Modelo a actualizar</param>
    /// <response code="200">Modelo no actualizado</response>
    /// <response code="202">Modelo actualizado correctamente</response>
    /// <response code="400">Incapaz actualizar el modelo</response>
    // [AuthorizePermission("UserEdit")] // Ejemplo: proteger por permiso granular
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
                _loggingService.LogInformation("Usuario actualizado exitosamente", new Dictionary<string, string> { { "User", JsonSerializer.Serialize(entity) } });
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
    // [AuthorizePermission("UserDelete")] // Ejemplo: proteger por permiso granular
    [HttpDelete("delete-user-from-db")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (queryParameters != null)
            {
                bool _result = await _unitOfWork.UserRepository.Delete(queryParameters.UserId);

                return _result
                    ? StatusCode(StatusCodes.Status202Accepted, new { Valid = _result, Message = "Eliminado correctamente" })
                    : StatusCode(StatusCodes.Status200OK, new { Valid = _result, Message = "No se pudo eliminar el usuario" });
            }

            return StatusCode(StatusCodes.Status400BadRequest, new { Valid = false, Message = "El UserId es requerido" });
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
                    ? StatusCode(StatusCodes.Status202Accepted, new { Valid = true, Message = "Actualizado correctamente" })
                    : StatusCode(StatusCodes.Status200OK, new { Valid = false, Message = "No se pudo actualizar" });
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
                    ? StatusCode(StatusCodes.Status202Accepted, new { Valid = true, Message = "Actualizado correctamente" })
                    : StatusCode(StatusCodes.Status200OK, new { Valid = false, Message = "No se pudo actualizar" });
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
                    ? StatusCode(StatusCodes.Status202Accepted, new { Valid = true, Message = "Actualizado correctamente" })
                    : StatusCode(StatusCodes.Status200OK, new { Valid = false, Message = "No se pudo actualizar" });
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
                    return StatusCode(StatusCodes.Status200OK, new { Valid = true, Message = "Contraseña actualizada exitosamente" });
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { Valid = false, Message = "No se pudo actualizar la contraseña" });
                }
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al forzar contraseña", new Dictionary<string, string> {
                { "ErrorType", ex.GetType().Name },
                { "ErrorMessage", ex.Message }
            });
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

    /// <summary>
    /// Inicia el proceso de restablecimiento de contraseña
    /// </summary>
    /// <param name="queryParameters">Parámetros con el email del usuario</param>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Inicia el proceso de restablecimiento de contraseña", Description = "Envía un correo electrónico con un enlace para restablecer la contraseña.")]
    public async Task<IActionResult> ForgotPassword([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (string.IsNullOrEmpty(queryParameters.Email))
            {
                return BadRequest(new { Message = "El correo electrónico es requerido." });
            }

            // Por seguridad, siempre devolver éxito aunque el email no exista
            await _unitOfWork.UserRepository.GeneratePasswordResetTokenAndSendEmail(queryParameters.Email);

            return Ok(new { Message = "Si el correo existe en nuestro sistema, recibirás instrucciones para restablecer tu contraseña." });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error en el proceso de contraseña olvidada", new Dictionary<string, string> { { "Email", queryParameters.Email } });
            return StatusCode(500, new { Message = "Error al procesar la solicitud." });
        }
    }

    /// <summary>
    /// Valida un token de restablecimiento de contraseña
    /// </summary>
    /// <param name="queryParameters">Parámetros con el email y token</param>
    [HttpPost("validate-reset-token")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Valida un token de restablecimiento de contraseña", Description = "Verifica si el token de restablecimiento es válido.")]
    public async Task<IActionResult> ValidateResetToken([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (string.IsNullOrEmpty(queryParameters.Email) || string.IsNullOrEmpty(queryParameters.Token))
            {
                return BadRequest(new { Message = "El correo electrónico y el token son requeridos." });
            }

            var isValid = await _unitOfWork.UserRepository.ValidatePasswordResetToken(queryParameters.Email, queryParameters.Token);

            if (!isValid)
            {
                return BadRequest(new { Message = "El token no es válido o ha expirado." });
            }

            return Ok(new { Valid = true });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al validar token de restablecimiento", new Dictionary<string, string> { { "Email", queryParameters.Email } });
            return StatusCode(500, new { Message = "Error al validar el token." });
        }
    }

    /// <summary>
    /// Restablece la contraseña usando un token válido
    /// </summary>
    /// <param name="queryParameters">Parámetros con el email, token y nueva contraseña</param>
    [HttpPost("reset-password-with-token")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Restablece la contraseña usando un token", Description = "Cambia la contraseña del usuario usando un token válido.")]
    public async Task<IActionResult> ResetPasswordWithToken([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (string.IsNullOrEmpty(queryParameters.Email) ||
                string.IsNullOrEmpty(queryParameters.Token) ||
                string.IsNullOrEmpty(queryParameters.NewPassword))
            {
                return BadRequest(new { Message = "Todos los campos son requeridos." });
            }

            var result = await _unitOfWork.UserRepository.ResetPasswordWithToken(
                queryParameters.Email,
                queryParameters.Token,
                queryParameters.NewPassword
            );

            if (!result)
            {
                return BadRequest(new { Message = "No se pudo restablecer la contraseña. El token puede ser inválido o haber expirado." });
            }

            return Ok(new { Message = "Contraseña restablecida exitosamente." });
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al restablecer contraseña con token", new Dictionary<string, string> { { "Email", queryParameters.Email } });
            return StatusCode(500, new { Message = "Error al restablecer la contraseña." });
        }
    }

    /// <summary>
    /// Verifica si un correo electrónico ya existe en el sistema
    /// </summary>
    /// <param name="queryParameters">Parámetros con el email a verificar</param>
    /// <returns>True si el correo existe, False si no existe</returns>
    [HttpGet("check-email-exists")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Verifica si un correo electrónico existe", Description = "Verifica si un correo electrónico ya está registrado en el sistema (AspNetUsers o Staff).")]
    public async Task<IActionResult> CheckEmailExists([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (string.IsNullOrEmpty(queryParameters.Email))
            {
                return BadRequest(new { Message = "El correo electrónico es requerido." });
            }

            var exists = await _unitOfWork.UserRepository.EmailExists(queryParameters.Email);

            return Ok(exists);
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al verificar si el correo existe", new Dictionary<string, string> { { "Email", queryParameters.Email } });
            return StatusCode(500, new { Message = "Error al verificar el correo electrónico." });
        }
    }

    /// <summary>
    /// Verifica si un IUE (Identificador Único de Entidad) ya existe en el sistema
    /// </summary>
    /// <param name="queryParameters">Parámetros con el IUE a verificar</param>
    /// <returns>True si el IUE existe, False si no existe</returns>
    [HttpGet("check-uie-exists")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Verifica si un IUE existe", Description = "Verifica si un Identificador Único de Entidad (IUE) ya está registrado en el sistema.")]
    public async Task<IActionResult> CheckUieExists([FromQuery(Name = "uieNumber")] long? uieNumber)
    {
        try
        {
            if (!uieNumber.HasValue)
            {
                return BadRequest(new { Message = "El número IUE es requerido." });
            }

            var uieNumberValue = uieNumber.Value;
            
            // Log para debugging
            _loggingService.LogInformation($"Verificando IUE: {uieNumberValue}", new Dictionary<string, string> 
            { 
                { "UieNumber", uieNumberValue.ToString() },
                { "UieNumberType", uieNumberValue.GetType().Name }
            });

            var exists = await _unitOfWork.AgencyRepository.UieNumberExists(uieNumberValue);

            // Log del resultado
            _loggingService.LogInformation($"Resultado verificación IUE: {exists}", new Dictionary<string, string> 
            { 
                { "UieNumber", uieNumberValue.ToString() },
                { "Exists", exists.ToString() }
            });

            return Ok(exists);
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al verificar si el IUE existe", new Dictionary<string, string> { { "UieNumber", uieNumber?.ToString() ?? "null" } });
            return StatusCode(500, new { Message = "Error al verificar el IUE." });
        }
    }

    /// <summary>
    /// Verifica si un SDR (Número de Registro del Departamento de Estado) ya existe en el sistema
    /// </summary>
    /// <param name="queryParameters">Parámetros con el SDR a verificar</param>
    /// <returns>True si el SDR existe, False si no existe</returns>
    [HttpGet("check-sdr-exists")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Verifica si un SDR existe", Description = "Verifica si un Número de Registro del Departamento de Estado (SDR) ya está registrado en el sistema.")]
    public async Task<IActionResult> CheckSdrExists([FromQuery(Name = "sdrNumber")] long? sdrNumber)
    {
        try
        {
            if (!sdrNumber.HasValue)
            {
                return BadRequest(new { Message = "El número SDR es requerido." });
            }

            var exists = await _unitOfWork.AgencyRepository.SdrNumberExists(sdrNumber.Value);

            return Ok(exists);
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al verificar si el SDR existe", new Dictionary<string, string> { { "SdrNumber", sdrNumber?.ToString() ?? "null" } });
            return StatusCode(500, new { Message = "Error al verificar el SDR." });
        }
    }

    /// <summary>
    /// Verifica si un EIN (Número de Seguro Social Patronal) ya existe en el sistema
    /// </summary>
    /// <param name="queryParameters">Parámetros con el EIN a verificar</param>
    /// <returns>True si el EIN existe, False si no existe</returns>
    [HttpGet("check-ein-exists")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Verifica si un EIN existe", Description = "Verifica si un Número de Seguro Social Patronal (EIN) ya está registrado en el sistema.")]
    public async Task<IActionResult> CheckEinExists([FromQuery(Name = "einNumber")] int? einNumber)
    {
        try
        {
            if (!einNumber.HasValue)
            {
                return BadRequest(new { Message = "El número EIN es requerido." });
            }

            var exists = await _unitOfWork.AgencyRepository.EinNumberExists(einNumber.Value);

            return Ok(exists);
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al verificar si el EIN existe", new Dictionary<string, string> { { "EinNumber", einNumber?.ToString() ?? "null" } });
            return StatusCode(500, new { Message = "Error al verificar el EIN." });
        }
    }

    [HttpPut("update-user-avatar")]
    [SwaggerOperation(Summary = "Actualiza el avatar del usuario", Description = "Actualiza la imagen de perfil del usuario.")]
    public async Task<IActionResult> UpdateUserAvatar([FromBody] UserAvatarRequest request)
    {
        try
        {
            // Validar parámetros requeridos
            if (string.IsNullOrEmpty(request.UserId) || string.IsNullOrEmpty(request.ImageUrl))
            {
                _loggingService.LogWarning("ID de usuario o URL de imagen no proporcionados");
                return BadRequest(new { message = "ID de usuario y URL de imagen son requeridos" });
            }

            // Limpiar la URL de la imagen (solucionar problemas con barras invertidas)
            var cleanImageUrl = request.ImageUrl.Replace("\\\\", "/").Replace("\\", "/");
            _loggingService.LogInformation("Actualizando avatar del usuario", new Dictionary<string, string> {
                { "UserId", request.UserId },
                { "OriginalUrl", request.ImageUrl },
                { "CleanUrl", cleanImageUrl }
            });

            // Llamar al método del repositorio para actualizar el avatar
            var result = await _unitOfWork.UserRepository.UpdateUserAvatar(request.UserId, cleanImageUrl);

            // Devolver el resultado
            return Ok(result);
        }
        catch (Exception ex)
        {
            await _loggingService.LogError(ex, "Error al actualizar el avatar del usuario", new Dictionary<string, string> { { "UserId", request.UserId } });
            return StatusCode(500, new { message = "Error al actualizar el avatar del usuario", error = ex.Message });
        }
    }

}
