using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("operating-period")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class OperatingPeriodController(IOperatingPeriodRepository operatingPeriodRepository, ILogger<OperatingPeriodController> logger) : ControllerBase
{
    private readonly IOperatingPeriodRepository _operatingPeriodRepository = operatingPeriodRepository;
    private readonly ILogger<OperatingPeriodController> _logger = logger;

    [HttpGet("get-all-operating-periods-from-db")]
#if !DEBUG
    [Authorize]
#endif
    [SwaggerOperation(Summary = "Obtiene todos los períodos operativos", Description = "Devuelve una lista de períodos operativos.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var operatingPeriods = await _operatingPeriodRepository.GetAllOperatingPeriods(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(operatingPeriods);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los períodos operativos");
            return StatusCode(500, "Error interno del servidor al obtener los períodos operativos");
        }
    }

    [HttpGet("get-operating-period-by-id")]
    [SwaggerOperation(Summary = "Obtiene un período operativo por su ID", Description = "Devuelve un período operativo basado en el ID proporcionado.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var operatingPeriod = await _operatingPeriodRepository.GetOperatingPeriodById(id);
            if (operatingPeriod == null)
                return NotFound($"Período operativo con ID {id} no encontrado");

            return Ok(operatingPeriod);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el período operativo con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener el período operativo");
        }
    }

    [HttpPost("insert-operating-period")]
    [SwaggerOperation(Summary = "Crea un nuevo período operativo", Description = "Crea un nuevo período operativo.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> Insert([FromBody] DTOOperatingPeriod operatingPeriod)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var id = await _operatingPeriodRepository.InsertOperatingPeriod(operatingPeriod);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el período operativo");
            return StatusCode(500, "Error interno del servidor al crear el período operativo");
        }
    }

    [HttpPut("update-operating-period")]
    [SwaggerOperation(Summary = "Actualiza un período operativo existente", Description = "Actualiza los datos de un período operativo existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Update([FromBody] DTOOperatingPeriod operatingPeriod)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPeriodRepository.UpdateOperatingPeriod(operatingPeriod);
                if (!result)
                    return NotFound($"Período operativo con ID {operatingPeriod.Id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el período operativo con ID {Id}", operatingPeriod.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el período operativo");
        }
    }

    [HttpDelete("delete-operating-period")]
    [SwaggerOperation(Summary = "Elimina un período operativo existente", Description = "Elimina un período operativo existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPeriodRepository.DeleteOperatingPeriod(id);
                if (!result)
                    return NotFound($"Período operativo con ID {id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el período operativo con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar el período operativo");
        }
    }
}