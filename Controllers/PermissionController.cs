using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar permisos y sus asignaciones a usuarios/roles
/// Proporciona endpoints para la gestión completa de permisos, incluyendo creación,
/// lectura, actualización y eliminación de permisos, así como la asignación y eliminación de permisos a usuarios y roles.
/// </summary>
[Route("permission")]
[ApiController]
public class PermissionController(IPermissionRepository permissionRepository, ILogger<PermissionController> logger) : ControllerBase
{
    private readonly IPermissionRepository _permissionRepository = permissionRepository;
    private readonly ILogger<PermissionController> _logger = logger;

    /// <summary>
    /// Retrieves a specific permission by its ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El permiso encontrado o NotFound si no se encuentra</returns>
    [HttpGet("get-permission-by-id")]
    [SwaggerOperation(Summary = "Obtiene un permiso por su ID", Description = "Devuelve un permiso basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo permiso por ID: {Id}", queryParameters.Id);

                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID del permiso es requerido");
                }

                var result = await _permissionRepository.GetPermissionById(queryParameters.Id);

                if (result == null)
                {
                    return NotFound($"Permiso con ID {queryParameters.Id} no encontrado");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el permiso con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el permiso");
        }
    }

    /// <summary>
    /// Retrieves all permissions with pagination and filtering options
    /// </summary>
    /// <param name="queryParameters">Pagination and filtering parameters</param>
    /// <returns>Paginated list of permissions</returns>
    [HttpGet("get-all-permissions-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los permisos", Description = "Devuelve una lista de permisos.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _permissionRepository.GetAllPermissions(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);

                if (result == null)
                {
                    return NotFound("No se encontraron permisos");
                }

                return Ok(result);
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los permisos");
            return StatusCode(500, "Error interno del servidor al obtener los permisos");
        }
    }

    /// <summary>
    /// Obtiene los permisos de un usuario
    /// </summary>
    /// <param name="queryParameters">Contains UserId</param>
    /// <returns>List of permissions</returns>
    [HttpGet("get-user-permissions")]
    [SwaggerOperation(Summary = "Obtiene los permisos de un usuario", Description = "Devuelve los permisos asignados a un usuario.")]
    public async Task<ActionResult> GetUserPermissions([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(queryParameters.UserId) || queryParameters.UserId == "0" || queryParameters.UserId == null)
                {
                    return Ok(new List<DTOPermission>());
                }

                _logger.LogInformation("Obteniendo permisos del usuario: {UserId}", queryParameters.UserId);

                var result = await _permissionRepository.GetUserPermissions(queryParameters.UserId);

                if (result == null)
                {
                    return NotFound("No se encontraron permisos");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener permisos del usuario {UserId}", queryParameters.UserId);
            return StatusCode(500, "Error interno del servidor al obtener permisos del usuario");
        }
    }


    /// <summary>
    /// Obtiene los permisos de un rol
    /// </summary>
    /// <param name="queryParameters">Contains RoleId</param>
    /// <returns>List of permissions</returns>
    [HttpGet("get-role-permissions")]
    [SwaggerOperation(Summary = "Obtiene los permisos de un rol", Description = "Devuelve los permisos asignados a un rol.")]
    public async Task<ActionResult> GetRolePermissions([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(queryParameters.RoleId) || queryParameters.RoleId == "0" || queryParameters.RoleId == null)
                {
                    return Ok(new List<DTOPermission>());
                }

                _logger.LogInformation("Obteniendo permisos del rol: {RoleId}", queryParameters.RoleId);

                var result = await _permissionRepository.GetRolePermissions(queryParameters.RoleId);

                if (result == null)
                {
                    return NotFound("No se encontraron permisos");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener permisos del rol {RoleId}", queryParameters.RoleId);
            return StatusCode(500, "Error interno del servidor al obtener permisos del rol");
        }
    }

    /// <summary>
    /// Creates a new permission
    /// </summary>
    /// <param name="permission">The permission data to create</param>
    /// <returns>The created permission if successful</returns>
    [HttpPost("insert-permission")]
    [SwaggerOperation(Summary = "Crea un nuevo permiso", Description = "Crea un nuevo permiso.")]
    public async Task<ActionResult> Insert([FromBody] DTOPermission request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _permissionRepository.InsertPermission(request);

                if (result)
                {
                    _logger.LogInformation("Permiso creado con ID: {Id}", request.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo crear el permiso");
                return BadRequest("No se pudo crear el permiso");
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el permiso");
            return StatusCode(500, "Error interno del servidor al crear el permiso");
        }
    }

    /// <summary>
    /// Updates an existing permission
    /// </summary>
    /// <param name="permission">The permission data to update</param>
    /// <returns>True if update was successful</returns>
    [HttpPut("update-permission")]
    [SwaggerOperation(Summary = "Actualiza un permiso existente", Description = "Actualiza los datos de un permiso existente.")]
    public async Task<IActionResult> Update([FromBody] DTOPermission request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _permissionRepository.UpdatePermission(request);
                if (!result)
                {
                    _logger.LogWarning("Permiso con ID {Id} no encontrado", request.Id);
                    return NotFound($"Permiso con ID {request.Id} no encontrado");
                }

                _logger.LogInformation("Permiso actualizado con ID: {Id}", request.Id);
                return Ok(result);
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el permiso con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el permiso");
        }
    }

    /// <summary>
    /// Deletes a permission by ID
    /// </summary>
    /// <param name="id">ID of the permission to delete</param>
    /// <returns>True if deletion was successful</returns>
    [HttpDelete("delete-permission")]
    [SwaggerOperation(Summary = "Elimina un permiso existente", Description = "Elimina un permiso existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _permissionRepository.DeletePermission(queryParameters.Id);
                if (!result)
                {
                    _logger.LogWarning("Permiso con ID {Id} no encontrado", queryParameters.Id);
                    return NotFound($"Permiso con ID {queryParameters.Id} no encontrado");
                }

                _logger.LogInformation("Permiso eliminado con ID: {Id}", queryParameters.Id);
                return Ok(result);
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el permiso con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar el permiso");
        }
    }

    /// <summary>
    /// Assigns a permission to a user
    /// </summary>
    /// <param name="queryParameters">Contains UserId and PermissionId</param>
    /// <returns>True if assignment was successful</returns>
    [HttpPost("assign-permission-to-user")]
    [SwaggerOperation(Summary = "Asigna un permiso a un usuario", Description = "Asigna un permiso a un usuario.")]
    public async Task<IActionResult> AssignPermissionToUser([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.PermissionId == null)
                {
                    return BadRequest("PermissionId is required");
                }

                if (string.IsNullOrEmpty(queryParameters.UserId))
                {
                    return BadRequest("UserId is required");
                }

                var result = await _permissionRepository.AssignPermissionToUser(queryParameters.UserId, queryParameters.PermissionId.Value);

                if (result)
                {
                    _logger.LogInformation("Permiso asignado al usuario con ID: {UserId}", queryParameters.UserId);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo asignar el permiso al usuario");
                return BadRequest("No se pudo asignar el permiso al usuario");
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar permiso al usuario {UserId}", queryParameters.UserId);
            return StatusCode(500, "Error interno del servidor al asignar permiso");
        }
    }

    /// <summary>
    /// Removes a permission from a user
    /// </summary>
    /// <param name="queryParameters">Contains UserId and PermissionId</param>
    /// <returns>True if removal was successful</returns>
    [HttpDelete("remove-permission-from-user")]
    [SwaggerOperation(Summary = "Remueve un permiso de un usuario", Description = "Remueve un permiso de un usuario.")]
    public async Task<IActionResult> RemovePermissionFromUser([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.PermissionId == null)
                {
                    return BadRequest("PermissionId is required");
                }
                if (string.IsNullOrEmpty(queryParameters.UserId))
                {
                    return BadRequest("UserId is required");
                }

                var result = await _permissionRepository.RemovePermissionFromUser(queryParameters.UserId, queryParameters.PermissionId.Value);

                if (result)
                {
                    _logger.LogInformation("Permiso removido del usuario con ID: {UserId}", queryParameters.UserId);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo remover el permiso del usuario");
                return BadRequest("No se pudo remover el permiso del usuario");
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al remover permiso del usuario {UserId}", queryParameters.UserId);
            return StatusCode(500, "Error interno del servidor al remover permiso");
        }
    }

    /// <summary>
    /// Assigns a permission to a role
    /// </summary>
    /// <param name="queryParameters">Contains RoleId and PermissionId</param>
    /// <returns>True if assignment was successful</returns>
    [HttpPost("assign-permission-to-role")]
    [SwaggerOperation(Summary = "Asigna un permiso a un rol", Description = "Asigna un permiso a un rol.")]
    public async Task<IActionResult> AssignPermissionToRole([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.PermissionId == null)
                {
                    return BadRequest("PermissionId is required");
                }
                if (string.IsNullOrEmpty(queryParameters.RoleId))
                {
                    return BadRequest("RoleId is required");
                }

                var result = await _permissionRepository.AssignPermissionToRole(queryParameters.RoleId, queryParameters.PermissionId.Value);

                if (result)
                {
                    _logger.LogInformation("Permiso asignado al rol con ID: {RoleId}", queryParameters.RoleId);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo asignar el permiso al rol");
                return BadRequest("No se pudo asignar el permiso al rol");
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar permiso al rol {RoleId}", queryParameters.RoleId);
            return StatusCode(500, "Error interno del servidor al asignar permiso");
        }
    }

    /// <summary>
    /// Removes a permission from a role
    /// </summary>
    /// <param name="queryParameters">Contains RoleId and PermissionId</param>
    /// <returns>True if removal was successful</returns>
    [HttpDelete("remove-permission-from-role")]
    [SwaggerOperation(Summary = "Remueve un permiso de un rol", Description = "Remueve un permiso de un rol.")]
    public async Task<IActionResult> RemovePermissionFromRole([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.PermissionId == null)
                {
                    return BadRequest("PermissionId is required");
                }
                if (string.IsNullOrEmpty(queryParameters.RoleId))
                {
                    return BadRequest("RoleId is required");
                }
                var result = await _permissionRepository.RemovePermissionFromRole(queryParameters.RoleId, queryParameters.PermissionId.Value);

                if (result)
                {
                    _logger.LogInformation("Permiso removido del rol con ID: {RoleId}", queryParameters.RoleId);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo remover el permiso del rol");
                return BadRequest("No se pudo remover el permiso del rol");
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al remover permiso del rol {RoleId}", queryParameters.RoleId);
            return StatusCode(500, "Error interno del servidor al remover permiso");
        }
    }

}