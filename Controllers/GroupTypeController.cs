using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("group-type")]
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de grupo.
/// Proporciona endpoints para la gestión completa de tipos de grupo, incluyendo creación,
/// lectura, actualización y eliminación de tipos de grupo.
/// </summary>
public class GroupTypeController(IGroupTypeRepository groupTypeRepository, ILogger<GroupTypeController> logger) : ControllerBase
{
    private readonly IGroupTypeRepository _groupTypeRepository = groupTypeRepository;
    private readonly ILogger<GroupTypeController> _logger = logger;

    [HttpGet("get-group-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de grupo por su ID", Description = "Devuelve un tipo de grupo basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var type = await _groupTypeRepository.GetGroupTypeById(id);
            if (type == null)
                return NotFound($"Tipo de grupo con ID {id} no encontrado");

            return Ok(type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de grupo con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener el tipo de grupo");
        }
    }

    [HttpGet("get-all-group-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de grupo", Description = "Devuelve una lista de tipos de grupo.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var types = await _groupTypeRepository.GetAllGroupTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(types);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de grupo");
            return StatusCode(500, "Error interno del servidor al obtener los tipos de grupo");
        }
    }

    [HttpPost("insert-group-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de grupo", Description = "Crea un nuevo tipo de grupo.")]
    public async Task<ActionResult> Insert([FromBody] DTOGroupType type)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _groupTypeRepository.InsertGroupType(type);
                if (result)
                {
                    return Ok(type);
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

    [HttpPut("update-group-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de grupo existente", Description = "Actualiza los datos de un tipo de grupo existente.")]
    public async Task<IActionResult> Update([FromBody] DTOGroupType type)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _groupTypeRepository.UpdateGroupType(type);

                if (!result)
                {
                    return NotFound($"Tipo de grupo con ID {type.Id} no encontrado");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de grupo con ID {Id}", type.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el tipo de grupo");
        }
    }

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
                    return NotFound($"Tipo de grupo con ID {groupTypeId} no encontrado");

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

    [HttpDelete("delete-group-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de grupo existente", Description = "Elimina un tipo de grupo existente.")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _groupTypeRepository.DeleteGroupType(id);
                if (!result)
                    return NotFound($"Tipo de grupo con ID {id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de grupo con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar el tipo de grupo");
        }
    }
}