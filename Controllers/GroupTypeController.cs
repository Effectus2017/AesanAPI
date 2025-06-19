using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de grupo.
/// Proporciona endpoints para la gestión completa de tipos de grupo, incluyendo creación,
/// lectura, actualización y eliminación de tipos de grupo.
/// </summary>
[Route("group-type")]
[ApiController]
public class GroupTypeController(IGroupTypeRepository groupTypeRepository, ILogger<GroupTypeController> logger) : ControllerBase
{
    private readonly IGroupTypeRepository _groupTypeRepository = groupTypeRepository;
    private readonly ILogger<GroupTypeController> _logger = logger;

    /// <summary>
    /// Obtiene un tipo de grupo por su ID
    /// </summary>
    /// <param name="id">El ID del tipo de grupo a obtener</param>
    /// <returns>El tipo de grupo si se encuentra, NotFound si no existe, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-group-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de grupo por su ID", Description = "Devuelve un tipo de grupo basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo tipo de grupo por ID: {Id}", queryParameters.Id);

                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID del tipo de grupo es requerido");
                }

                var result = await _groupTypeRepository.GetGroupTypeById(queryParameters.Id);

                if (result == null)
                {
                    return NotFound($"Tipo de grupo con ID {queryParameters.Id} no encontrado");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de grupo con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el tipo de grupo");
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de grupo con opciones de filtrado y paginación
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de tipos de grupo, BadRequest si los parámetros son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-group-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de grupo", Description = "Devuelve una lista de tipos de grupo.")]
    public async Task<ActionResult> GetAllGroupTypes([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _groupTypeRepository.GetAllGroupTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.IsList);

                if (result == null)
                {
                    return NotFound("No se encontraron tipos de grupo");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de grupo");
            return StatusCode(500, "Error interno del servidor al obtener los tipos de grupo");
        }
    }

    /// <summary>
    /// Crea un nuevo tipo de grupo
    /// </summary>
    /// <param name="type">El tipo de grupo a crear</param>
    /// <returns>El tipo de grupo creado si la operación es exitosa, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpPost("insert-group-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de grupo", Description = "Crea un nuevo tipo de grupo.")]
    public async Task<ActionResult> Insert([FromBody] DTOGroupType request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _groupTypeRepository.InsertGroupType(request);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("No se pudo crear el tipo de grupo");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el tipo de grupo");
            return StatusCode(500, "Error interno del servidor al crear el tipo de grupo");
        }
    }

    /// <summary>
    /// Actualiza un tipo de grupo existente
    /// </summary>
    /// <param name="type">El tipo de grupo a actualizar</param>
    /// <returns>El tipo de grupo actualizado si la operación es exitosa, NotFound si no existe, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpPut("update-group-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de grupo existente", Description = "Actualiza los datos de un tipo de grupo existente.")]
    public async Task<IActionResult> Update([FromBody] DTOGroupType request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _groupTypeRepository.UpdateGroupType(request);

                if (!result)
                {
                    return NotFound($"Tipo de grupo con ID {request.Id} no encontrado");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de grupo con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el tipo de grupo");
        }
    }

    /// <summary>
    /// Actualiza el orden de visualización de un tipo de grupo
    /// </summary>
    /// <param name="groupTypeId">ID del tipo de grupo a actualizar</param>
    /// <param name="displayOrder">Nuevo orden de visualización</param>
    /// <returns>NoContent si la actualización es exitosa, NotFound si no existe, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpPut("update-group-type-display-order")]
    [SwaggerOperation(Summary = "Actualiza el orden de visualización de un tipo de grupo", Description = "Actualiza el orden de visualización de un tipo de grupo existente.")]
    public async Task<IActionResult> UpdateDisplayOrder([FromQuery] int groupTypeId, [FromQuery] int displayOrder)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _groupTypeRepository.UpdateGroupTypeDisplayOrder(groupTypeId, displayOrder);

                if (!result)
                {
                    return NotFound($"Tipo de grupo con ID {groupTypeId} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el orden de visualización del tipo de grupo con ID {Id}", groupTypeId);
            return StatusCode(500, "Error interno del servidor al actualizar el orden de visualización del tipo de grupo");
        }
    }

    /// <summary>
    /// Elimina un tipo de grupo existente
    /// </summary>
    /// <param name="id">ID del tipo de grupo a eliminar</param>
    /// <returns>NoContent si se elimina exitosamente, NotFound si no existe, BadRequest si los datos son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpDelete("delete-group-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de grupo existente", Description = "Elimina un tipo de grupo existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _groupTypeRepository.DeleteGroupType(queryParameters.Id);

                if (!result)
                {
                    return NotFound($"Tipo de grupo con ID {queryParameters.Id} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de grupo con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar el tipo de grupo");
        }
    }
}