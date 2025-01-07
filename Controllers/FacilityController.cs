using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("facility")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class FacilityController(IFacilityRepository facilityRepository, ILogger<FacilityController> logger) : ControllerBase
{
    private readonly IFacilityRepository _facilityRepository = facilityRepository;
    private readonly ILogger<FacilityController> _logger = logger;

    [HttpGet("get-all-facilities-from-db")]
#if !DEBUG
    [Authorize]
#endif
    [SwaggerOperation(Summary = "Obtiene todas las instalaciones", Description = "Devuelve una lista de instalaciones.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var facilities = await _facilityRepository.GetAllFacilities(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(facilities);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las instalaciones");
            return StatusCode(500, "Error interno del servidor al obtener las instalaciones");
        }
    }

    [HttpGet("get-facility-by-id")]
    [SwaggerOperation(Summary = "Obtiene una instalación por su ID", Description = "Devuelve una instalación basada en el ID proporcionado.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var facility = await _facilityRepository.GetFacilityById(id);
            if (facility == null)
                return NotFound($"Instalación con ID {id} no encontrada");

            return Ok(facility);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la instalación con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener la instalación");
        }
    }

    [HttpPost("insert-facility")]
    [SwaggerOperation(Summary = "Crea una nueva instalación", Description = "Crea una nueva instalación.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> Insert([FromBody] DTOFacility facility)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var id = await _facilityRepository.InsertFacility(facility);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la instalación");
            return StatusCode(500, "Error interno del servidor al crear la instalación");
        }
    }

    [HttpPut("update-facility")]
    [SwaggerOperation(Summary = "Actualiza una instalación existente", Description = "Actualiza los datos de una instalación existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Update([FromBody] DTOFacility facility)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _facilityRepository.UpdateFacility(facility);
                if (!result)
                    return NotFound($"Instalación con ID {facility.Id} no encontrada");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la instalación con ID {Id}", facility.Id);
            return StatusCode(500, "Error interno del servidor al actualizar la instalación");
        }
    }

    [HttpDelete("delete-facility")]
    [SwaggerOperation(Summary = "Elimina una instalación existente", Description = "Elimina una instalación existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _facilityRepository.DeleteFacility(id);
                if (!result)
                    return NotFound($"Instalación con ID {id} no encontrada");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la instalación con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar la instalación");
        }
    }
}