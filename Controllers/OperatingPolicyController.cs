using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("operating-policy")]
#if !DEBUG
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
#endif
public class OperatingPolicyController(IOperatingPolicyRepository operatingPolicyRepository, ILogger<OperatingPolicyController> logger) : ControllerBase
{
    private readonly IOperatingPolicyRepository _operatingPolicyRepository = operatingPolicyRepository;
    private readonly ILogger<OperatingPolicyController> _logger = logger;

    [HttpGet("get-all-operating-policies-from-db")]
#if !DEBUG
    [Authorize]
#endif
    [SwaggerOperation(Summary = "Obtiene todas las políticas operativas", Description = "Devuelve una lista de políticas operativas.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var operatingPolicies = await _operatingPolicyRepository.GetAllOperatingPolicies(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(operatingPolicies);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las políticas operativas");
            return StatusCode(500, "Error interno del servidor al obtener las políticas operativas");
        }
    }

    [HttpGet("get-operating-policy-by-id")]
    [SwaggerOperation(Summary = "Obtiene una política operativa por su ID", Description = "Devuelve una política operativa basada en el ID proporcionado.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var operatingPolicy = await _operatingPolicyRepository.GetOperatingPolicyById(id);
            if (operatingPolicy == null)
                return NotFound($"Política operativa con ID {id} no encontrada");

            return Ok(operatingPolicy);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la política operativa con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener la política operativa");
        }
    }

    [HttpPost("insert-operating-policy")]
    [SwaggerOperation(Summary = "Crea una nueva política operativa", Description = "Crea una nueva política operativa.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> Insert([FromBody] DTOOperatingPolicy operatingPolicy)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var id = await _operatingPolicyRepository.InsertOperatingPolicy(operatingPolicy);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la política operativa");
            return StatusCode(500, "Error interno del servidor al crear la política operativa");
        }
    }

    [HttpPut("update-operating-policy")]
    [SwaggerOperation(Summary = "Actualiza una política operativa existente", Description = "Actualiza los datos de una política operativa existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Update([FromBody] DTOOperatingPolicy operatingPolicy)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPolicyRepository.UpdateOperatingPolicy(operatingPolicy);
                if (!result)
                    return NotFound($"Política operativa con ID {operatingPolicy.Id} no encontrada");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la política operativa con ID {Id}", operatingPolicy.Id);
            return StatusCode(500, "Error interno del servidor al actualizar la política operativa");
        }
    }

    [HttpDelete("delete-operating-policy")]
    [SwaggerOperation(Summary = "Elimina una política operativa existente", Description = "Elimina una política operativa existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPolicyRepository.DeleteOperatingPolicy(id);
                if (!result)
                    return NotFound($"Política operativa con ID {id} no encontrada");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la política operativa con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar la política operativa");
        }
    }
}