using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("alternative-communication")]
#if !DEBUG
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
#endif
public class AlternativeCommunicationController(IAlternativeCommunicationRepository alternativeCommunicationRepository, ILogger<AlternativeCommunicationController> logger) : ControllerBase
{
    private readonly IAlternativeCommunicationRepository _alternativeCommunicationRepository = alternativeCommunicationRepository;
    private readonly ILogger<AlternativeCommunicationController> _logger = logger;

    [HttpGet("get-all-alternative-communications-from-db")]
#if !DEBUG
    [Authorize]
#endif
    [SwaggerOperation(Summary = "Obtiene todas las comunicaciones alternativas", Description = "Devuelve una lista de comunicaciones alternativas.")]
    public async Task<ActionResult> GetAllAlternativeCommunicationsFromDb([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var alternativeCommunications = await _alternativeCommunicationRepository.GetAllAlternativeCommunications(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(alternativeCommunications);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las comunicaciones alternativas");
            return StatusCode(500, "Error interno del servidor al obtener las comunicaciones alternativas");
        }
    }

    [HttpGet("get-alternative-communication-by-id")]
    [SwaggerOperation(Summary = "Obtiene una comunicación alternativa por su ID", Description = "Devuelve una comunicación alternativa basada en el ID proporcionado.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var alternativeCommunication = await _alternativeCommunicationRepository.GetAlternativeCommunicationById(id);
            if (alternativeCommunication == null)
                return NotFound($"Comunicación alternativa con ID {id} no encontrada");

            return Ok(alternativeCommunication);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la comunicación alternativa con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener la comunicación alternativa");
        }
    }

    [HttpPost("insert-alternative-communication")]
    [SwaggerOperation(Summary = "Crea una nueva comunicación alternativa", Description = "Crea una nueva comunicación alternativa.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> Insert([FromBody] DTOAlternativeCommunication alternativeCommunication)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _alternativeCommunicationRepository.InsertAlternativeCommunication(alternativeCommunication);
                if (result)
                    return Ok(result);

                return BadRequest("No se pudo insertar la comunicación alternativa");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la comunicación alternativa");
            return StatusCode(500, "Error interno del servidor al crear la comunicación alternativa");
        }
    }

    [HttpPut("update-alternative-communication")]
    [SwaggerOperation(Summary = "Actualiza una comunicación alternativa existente", Description = "Actualiza los datos de una comunicación alternativa existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Update([FromBody] DTOAlternativeCommunication alternativeCommunication)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _alternativeCommunicationRepository.UpdateAlternativeCommunication(alternativeCommunication);
                if (!result)
                    return NotFound($"Comunicación alternativa con ID {alternativeCommunication.Id} no encontrada");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la comunicación alternativa con ID {Id}", alternativeCommunication.Id);
            return StatusCode(500, "Error interno del servidor al actualizar la comunicación alternativa");
        }
    }

    [HttpDelete("delete-alternative-communication")]
    [SwaggerOperation(Summary = "Elimina una comunicación alternativa existente", Description = "Elimina una comunicación alternativa existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _alternativeCommunicationRepository.DeleteAlternativeCommunication(id);
                if (!result)
                    return NotFound($"Comunicación alternativa con ID {id} no encontrada");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la comunicación alternativa con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar la comunicación alternativa");
        }
    }
}