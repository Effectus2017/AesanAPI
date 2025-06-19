using Api.Interfaces;
using Api.Models;
using Api.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de entrega.
/// Proporciona endpoints para la gestión completa de tipos de entrega, incluyendo creación,
/// lectura, actualización y eliminación de tipos de entrega.
/// </summary>
[Route("delivery-type")]
[ApiController]
public class DeliveryTypeController(IDeliveryTypeRepository deliveryTypeRepository, ILogger<DeliveryTypeController> logger) : ControllerBase
{
    private readonly IDeliveryTypeRepository _deliveryTypeRepository = deliveryTypeRepository;
    private readonly ILogger<DeliveryTypeController> _logger = logger;

    /// <summary>
    /// Obtiene un tipo de entrega por su ID.
    /// </summary>
    /// <param name="id">El ID del tipo de entrega a obtener.</param>
    /// <returns>El tipo de entrega encontrado o null si no se encuentra.</returns>
    [HttpGet("get-delivery-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de entrega por su ID", Description = "Devuelve un tipo de entrega basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _deliveryTypeRepository.GetDeliveryTypeById(queryParameters.Id);
                if (result == null)
                {
                    return NotFound($"Tipo de entrega con ID {queryParameters.Id} no encontrado");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de entrega con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el tipo de entrega");
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de entrega.
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta.</param>
    /// <returns>Una lista de tipos de entrega y el total.</returns>
    [HttpGet("get-all-delivery-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de entrega", Description = "Devuelve una lista de tipos de entrega.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _deliveryTypeRepository.GetAllDeliveryTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.IsList);

                if (result == null)
                {
                    return NotFound("No se encontraron tipos de entrega");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de entrega");
            return StatusCode(500, "Error interno del servidor al obtener los tipos de entrega");
        }
    }

    /// <summary>
    /// Crea un nuevo tipo de entrega.
    /// </summary>
    /// <param name="deliveryType">El tipo de entrega a crear.</param>
    /// <returns>El tipo de entrega creado.</returns>
    [HttpPost("insert-delivery-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de entrega", Description = "Crea un nuevo tipo de entrega.")]
    public async Task<ActionResult> Insert([FromBody] DTODeliveryType request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _deliveryTypeRepository.InsertDeliveryType(request);

                if (result)
                {
                    return Ok(request);
                }

                return BadRequest("No se pudo crear el tipo de entrega");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el tipo de entrega");
            return StatusCode(500, "Error interno del servidor al crear el tipo de entrega");
        }
    }

    /// <summary>
    /// Actualiza un tipo de entrega existente.
    /// </summary>
    /// <param name="deliveryType">El tipo de entrega a actualizar.</param>
    /// <returns>El tipo de entrega actualizado.</returns>
    [HttpPut("update-delivery-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de entrega existente", Description = "Actualiza los datos de un tipo de entrega existente.")]
    public async Task<IActionResult> Update([FromBody] DTODeliveryType request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _deliveryTypeRepository.UpdateDeliveryType(request);

                if (!result)
                {
                    return NotFound($"Tipo de entrega con ID {request.Id} no encontrado");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de entrega con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el tipo de entrega");
        }
    }

    /// <summary>
    /// Elimina un tipo de entrega existente.
    /// </summary>
    /// <param name="id">El ID del tipo de entrega a eliminar.</param>
    /// <returns>True si la eliminación es exitosa, false en caso contrario.</returns>
    [HttpDelete("delete-delivery-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de entrega existente", Description = "Elimina un tipo de entrega existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _deliveryTypeRepository.DeleteDeliveryType(queryParameters.Id);
                if (!result)
                {
                    return NotFound($"Tipo de entrega con ID {queryParameters.Id} no encontrado");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de entrega con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar el tipo de entrega");
        }
    }
}