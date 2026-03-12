using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar permisos y sus asignaciones a usuarios/roles
/// Proporciona endpoints para la gestión completa de permisos, incluyendo creación,
/// lectura, actualización y eliminación de permisos, así como la asignación y eliminación de permisos a usuarios y roles.
/// </summary>
[ApiController]
[Route("permission")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class PermissionController(IPermissionRepository permissionRepository) : ControllerBase
{
    private readonly IPermissionRepository _permissionRepository = permissionRepository;

    /// <summary>
    /// Retrieves a specific permission by its ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El permiso encontrado o NotFound si no se encuentra</returns>
    [HttpGet("get-permission-by-id")]
    [SwaggerOperation(Summary = "Obtiene un permiso por su ID", Description = "Devuelve un permiso basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        if (string.IsNullOrEmpty(queryParameters.PermissionId))
        {
            return BadRequest("El ID del permiso es requerido");
        }

        var result = await _permissionRepository.GetPermissionById(queryParameters.PermissionId);

        if (result == null)
        {
            return NotFound($"Permiso con ID {queryParameters.PermissionId} no encontrado");
        }

        return Ok(result);
    }

    /// <summary>
    /// Retrieves a specific permission by its ValueKey
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ValueKey</param>
    /// <returns>El permiso encontrado o NotFound si no se encuentra</returns>
    [HttpGet("get-permission-by-value-key")]
    [SwaggerOperation(Summary = "Obtiene un permiso por su ValueKey", Description = "Devuelve un permiso basado en el ValueKey proporcionado.")]
    public async Task<ActionResult> GetByValueKey([FromQuery] QueryParameters queryParameters)
    {
        if (string.IsNullOrEmpty(queryParameters.ValueKey))
        {
            return BadRequest("El ValueKey del permiso es requerido");
        }

        var result = await _permissionRepository.GetPermissionByValueKey(queryParameters.ValueKey);

        if (result == null)
        {
            return NotFound($"Permiso con ValueKey {queryParameters.ValueKey} no encontrado");
        }

        return Ok(result);
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
        var result = await _permissionRepository.GetAllPermissions(queryParameters.Take, queryParameters.Skip, queryParameters.ValueKey, queryParameters.Name, queryParameters.Alls);

        if (result == null)
        {
            return NotFound("No se encontraron permisos");
        }

        return Ok(result);
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
        if (string.IsNullOrEmpty(queryParameters.UserId) || queryParameters.UserId == "0" || queryParameters.UserId == null)
        {
            return Ok(new List<DTOPermission>());
        }

        var result = await _permissionRepository.GetUserPermissions(queryParameters.UserId);

        if (result == null)
        {
            return NotFound("No se encontraron permisos");
        }

        return Ok(result);
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
        var result = await _permissionRepository.InsertPermission(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo crear el permiso");
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
        var result = await _permissionRepository.UpdatePermission(request);
        if (!result)
        {
            return NotFound($"Permiso con ID {request.Id} no encontrado");
        }

        return Ok(result);
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
        var result = await _permissionRepository.DeletePermission(queryParameters.PermissionId);
        if (!result)
        {
            return NotFound($"Permiso con ID {queryParameters.PermissionId} no encontrado");
        }

        return Ok(result);
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
        if (string.IsNullOrEmpty(queryParameters.PermissionId))
        {
            return BadRequest("PermissionId is required");
        }

        if (string.IsNullOrEmpty(queryParameters.UserId))
        {
            return BadRequest("UserId is required");
        }

        var result = await _permissionRepository.AssignPermissionToUser(queryParameters.UserId, queryParameters.PermissionId);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo asignar el permiso al usuario");
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
        if (string.IsNullOrEmpty(queryParameters.PermissionId))
        {
            return BadRequest("PermissionId is required");
        }
        if (string.IsNullOrEmpty(queryParameters.UserId))
        {
            return BadRequest("UserId is required");
        }

        var result = await _permissionRepository.RemovePermissionFromUser(queryParameters.UserId, queryParameters.PermissionId);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo remover el permiso del usuario");
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
        if (string.IsNullOrEmpty(queryParameters.PermissionId))
        {
            return BadRequest("PermissionId is required");
        }
        if (string.IsNullOrEmpty(queryParameters.RoleId))
        {
            return BadRequest("RoleId is required");
        }

        var result = await _permissionRepository.AssignPermissionToRole(queryParameters.RoleId, queryParameters.PermissionId);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo asignar el permiso al rol");
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
        if (string.IsNullOrEmpty(queryParameters.PermissionId))
        {
            return BadRequest("PermissionId is required");
        }
        if (string.IsNullOrEmpty(queryParameters.RoleId))
        {
            return BadRequest("RoleId is required");
        }
        var result = await _permissionRepository.RemovePermissionFromRole(queryParameters.RoleId, queryParameters.PermissionId);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo remover el permiso del rol");
    }

}
