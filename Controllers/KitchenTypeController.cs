using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("kitchen-type")]
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de cocina.
/// Proporciona endpoints para la gestión completa de tipos de cocina, incluyendo creación,
/// lectura, actualización y eliminación de tipos de cocina.
/// </summary>
public class KitchenTypeController(IKitchenTypeRepository kitchenTypeRepository, ILogger<KitchenTypeController> logger) : ControllerBase
{
    private readonly IKitchenTypeRepository _kitchenTypeRepository = kitchenTypeRepository;
    private readonly ILogger<KitchenTypeController> _logger = logger;

    [HttpGet("get-kitchen-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de cocina por su ID", Description = "Devuelve un tipo de cocina basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var type = await _kitchenTypeRepository.GetKitchenTypeById(id);
            if (type == null)
                return NotFound($"Tipo de cocina con ID {id} no encontrado");

            return Ok(type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de cocina con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener el tipo de cocina");
        }
    }

    [HttpGet("get-all-kitchen-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de cocina", Description = "Devuelve una lista de tipos de cocina.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var types = await _kitchenTypeRepository.GetAllKitchenTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(types);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de cocina");
            return StatusCode(500, "Error interno del servidor al obtener los tipos de cocina");
        }
    }

    [HttpPost("insert-kitchen-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de cocina", Description = "Crea un nuevo tipo de cocina.")]
    public async Task<ActionResult> Insert([FromBody] DTOKitchenType type)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _kitchenTypeRepository.InsertKitchenType(type);
                if (result)
                {
                    return Ok(type);
                }

                return BadRequest("No se pudo crear el tipo de cocina");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el tipo de cocina");
            return StatusCode(500, "Error interno del servidor al crear el tipo de cocina");
        }
    }

    [HttpPut("update-kitchen-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de cocina existente", Description = "Actualiza los datos de un tipo de cocina existente.")]
    public async Task<IActionResult> Update([FromBody] DTOKitchenType type)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _kitchenTypeRepository.UpdateKitchenType(type);

                if (!result)
                {
                    return NotFound($"Tipo de cocina con ID {type.Id} no encontrado");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de cocina con ID {Id}", type.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el tipo de cocina");
        }
    }

    [HttpPut("update-kitchen-type-display-order")]
    [SwaggerOperation(Summary = "Actualiza el orden de visualización de un tipo de cocina", Description = "Actualiza el orden de visualización de un tipo de cocina existente.")]
    public async Task<IActionResult> UpdateDisplayOrder([FromQuery] int kitchenTypeId, [FromQuery] int displayOrder)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _kitchenTypeRepository.UpdateKitchenTypeDisplayOrder(kitchenTypeId, displayOrder);
                if (!result)
                    return NotFound($"Tipo de cocina con ID {kitchenTypeId} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el orden de visualización del tipo de cocina con ID {Id}", kitchenTypeId);
            return StatusCode(500, "Error interno del servidor al actualizar el orden de visualización del tipo de cocina");
        }
    }

    [HttpDelete("delete-kitchen-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de cocina existente", Description = "Elimina un tipo de cocina existente.")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _kitchenTypeRepository.DeleteKitchenType(id);
                if (!result)
                    return NotFound($"Tipo de cocina con ID {id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de cocina con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar el tipo de cocina");
        }
    }
}