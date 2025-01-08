using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("agency-status")]
#if !DEBUG
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
#endif
public class AgencyStatusController(IAgencyStatusRepository agencyStatusRepository, ILogger<AgencyStatusController> logger) : ControllerBase
{
    private readonly IAgencyStatusRepository _agencyStatusRepository = agencyStatusRepository;
    private readonly ILogger<AgencyStatusController> _logger = logger;

    [HttpGet("get-all-agency-statuses-from-db")]
#if !DEBUG
    [Authorize]
#endif
    [SwaggerOperation(Summary = "Obtiene todos los estados de agencia", Description = "Devuelve una lista de estados de agencia.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var statuses = await _agencyStatusRepository.GetAllAgencyStatuses(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(statuses);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los estados de agencia");
            return StatusCode(500, "Error interno del servidor al obtener los estados de agencia");
        }
    }

    [HttpGet("get-agency-status-by-id")]
    [SwaggerOperation(Summary = "Obtiene un estado de agencia por su ID", Description = "Devuelve un estado de agencia basado en el ID proporcionado.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var status = await _agencyStatusRepository.GetAgencyStatusById(id);
            if (status == null)
                return NotFound($"Estado de agencia con ID {id} no encontrado");

            return Ok(status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el estado de agencia con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener el estado de agencia");
        }
    }

    [HttpPost("insert-agency-status")]
    [SwaggerOperation(Summary = "Crea un nuevo estado de agencia", Description = "Crea un nuevo estado de agencia.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> Insert([FromBody] DTOAgencyStatus status)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _agencyStatusRepository.InsertAgencyStatus(status);
                if (result)
                    return CreatedAtAction(nameof(GetById), new { id = status.Id }, status);

                return BadRequest("No se pudo crear el estado de agencia");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el estado de agencia");
            return StatusCode(500, "Error interno del servidor al crear el estado de agencia");
        }
    }

    [HttpPut("update-agency-status")]
    [SwaggerOperation(Summary = "Actualiza un estado de agencia existente", Description = "Actualiza los datos de un estado de agencia existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Update([FromBody] DTOAgencyStatus status)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _agencyStatusRepository.UpdateAgencyStatus(status);
                if (!result)
                    return NotFound($"Estado de agencia con ID {status.Id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado de agencia con ID {Id}", status.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el estado de agencia");
        }
    }

    [HttpDelete("delete-agency-status")]
    [SwaggerOperation(Summary = "Elimina un estado de agencia existente", Description = "Elimina un estado de agencia existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _agencyStatusRepository.DeleteAgencyStatus(id);
                if (!result)
                    return NotFound($"Estado de agencia con ID {id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el estado de agencia con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar el estado de agencia");
        }
    }
}