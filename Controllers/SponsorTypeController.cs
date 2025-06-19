using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de auspiciador.
/// Proporciona endpoints para la gestión completa de tipos de auspiciador, incluyendo creación,
/// lectura, actualización y eliminación de tipos de auspiciador.
/// </summary>
[Route("sponsor-type")]
[ApiController]
public class SponsorTypeController(ISponsorTypeRepository sponsorTypeRepository, ILogger<SponsorTypeController> logger) : ControllerBase
{
    private readonly ISponsorTypeRepository _sponsorTypeRepository = sponsorTypeRepository;
    private readonly ILogger<SponsorTypeController> _logger = logger;

    /// <summary>
    /// Obtiene un tipo de auspiciador por su ID
    /// </summary>
    /// <param name="id">ID del tipo de auspiciador</param>
    /// <returns>Tipo de auspiciador</returns>
    [HttpGet("get-sponsor-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de auspiciador por su ID", Description = "Devuelve un tipo de auspiciador basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var result = await _sponsorTypeRepository.GetSponsorTypeById(id);

            if (result == null)
            {
                return NotFound($"Tipo de auspiciador con ID {id} no encontrado");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de auspiciador con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener el tipo de auspiciador");
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de auspiciador
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>Lista de tipos de auspiciador</returns>
    [HttpGet("get-all-sponsor-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de auspiciador", Description = "Devuelve una lista de tipos de auspiciador.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var types = await _sponsorTypeRepository.GetAllSponsorTypes(queryParameters.Take,
                    queryParameters.Skip,
                    queryParameters.Name,
                    queryParameters.Alls,
                    queryParameters.IsList);
                return Ok(types);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de auspiciador");
            return StatusCode(500, "Error interno del servidor al obtener los tipos de auspiciador");
        }
    }

    /// <summary>
    /// Crea un nuevo tipo de auspiciador
    /// </summary>
    /// <param name="type">Tipo de auspiciador a crear</param>
    /// <returns>Tipo de auspiciador creado</returns>
    [HttpPost("insert-sponsor-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de auspiciador", Description = "Crea un nuevo tipo de auspiciador.")]
    public async Task<ActionResult> Insert([FromBody] DTOSponsorType type)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _sponsorTypeRepository.InsertSponsorType(type);
                if (result)
                {
                    return Ok(type);
                }

                return BadRequest("No se pudo crear el tipo de auspiciador");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el tipo de auspiciador");
            return StatusCode(500, "Error interno del servidor al crear el tipo de auspiciador");
        }
    }

    /// <summary>
    /// Actualiza un tipo de auspiciador existente
    /// </summary>
    /// <param name="type">Tipo de auspiciador a actualizar</param>
    /// <returns>Tipo de auspiciador actualizado</returns>
    [HttpPut("update-sponsor-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de auspiciador existente", Description = "Actualiza los datos de un tipo de auspiciador existente.")]
    public async Task<IActionResult> Update([FromBody] DTOSponsorType type)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _sponsorTypeRepository.UpdateSponsorType(type);

                if (!result)
                {
                    return NotFound($"Tipo de auspiciador con ID {type.Id} no encontrado");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de auspiciador con ID {Id}", type.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el tipo de auspiciador");
        }
    }

    /// <summary>
    /// Elimina un tipo de auspiciador existente
    /// </summary>
    /// <param name="id">ID del tipo de auspiciador a eliminar</param>
    /// <returns>Tipo de auspiciador eliminado</returns>
    [HttpDelete("delete-sponsor-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de auspiciador existente", Description = "Elimina un tipo de auspiciador existente.")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _sponsorTypeRepository.DeleteSponsorType(id);
                if (!result)
                    return NotFound($"Tipo de auspiciador con ID {id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de auspiciador con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar el tipo de auspiciador");
        }
    }
}