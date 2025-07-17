using Api.Interfaces;
using Api.Models;
using Api.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("area-type")]
[ApiController]
public class AreaTypeController(IAreaTypeRepository areaTypeRepository, ILogger<AreaTypeController> logger) : ControllerBase
{
    private readonly IAreaTypeRepository _areaTypeRepository = areaTypeRepository;
    private readonly ILogger<AreaTypeController> _logger = logger;

    [HttpGet("get-area-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de área por su ID", Description = "Devuelve un tipo de área basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo tipo de área por ID: {Id}", queryParameters.Id);
                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID del tipo de área es requerido");
                }
                var result = await _areaTypeRepository.GetAreaTypeById(queryParameters.Id);
                if (result == null)
                {
                    return NotFound($"Tipo de área con ID {queryParameters.Id} no encontrado");
                }
                else
                {
                    return Ok(result);
                }
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de área con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el tipo de área");
        }
    }

    [HttpGet("get-all-area-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de área", Description = "Devuelve una lista de tipos de área.")]
    public async Task<ActionResult> GetAllFromDb([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _areaTypeRepository.GetAllAreaTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.IsList);
                if (result == null)
                {
                    return NotFound("No se encontraron tipos de área");
                }
                return Ok(result);
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de área");
            return StatusCode(500, "Error al obtener todos los tipos de área");
        }
    }

    [HttpPost("insert-area-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de área", Description = "Crea un nuevo tipo de área.")]
    public async Task<ActionResult> Insert([FromBody] AreaTypeRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _areaTypeRepository.InsertAreaType(request);
                if (result)
                {
                    return Ok(result);
                }
                return BadRequest("No se pudo crear el tipo de área");
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el tipo de área");
            return StatusCode(500, "Error al crear el tipo de área");
        }
    }

    [HttpPut("update-area-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de área existente", Description = "Actualiza los datos de un tipo de área existente.")]
    public async Task<IActionResult> Update([FromBody] DTOAreaType request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _areaTypeRepository.UpdateAreaType(request);
                if (result)
                {
                    return Ok(result);
                }
                return BadRequest("No se pudo actualizar el tipo de área");
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de área con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el tipo de área");
        }
    }

    [HttpDelete("delete-area-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de área existente", Description = "Elimina un tipo de área existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _areaTypeRepository.DeleteAreaType(queryParameters.Id);
                if (!result)
                {
                    return NotFound($"Tipo de área con ID {queryParameters.Id} no encontrado");
                }
                return NoContent();
            }
            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de área con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar el tipo de área");
        }
    }
}