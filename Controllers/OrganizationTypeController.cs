using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("organization-type")]
#if !DEBUG
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
#endif
public class OrganizationTypeController(IOrganizationTypeRepository organizationTypeRepository, ILogger<OrganizationTypeController> logger) : ControllerBase
{
    private readonly IOrganizationTypeRepository _organizationTypeRepository = organizationTypeRepository;
    private readonly ILogger<OrganizationTypeController> _logger = logger;

    [HttpGet("get-all-organization-types-from-db")]
#if !DEBUG
    [Authorize]
#endif
    [SwaggerOperation(Summary = "Obtiene todos los tipos de organización", Description = "Devuelve una lista de tipos de organización.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var organizationTypes = await _organizationTypeRepository.GetAllOrganizationTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(organizationTypes);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de organización");
            return StatusCode(500, "Error interno del servidor al obtener los tipos de organización");
        }
    }

    [HttpGet("get-organization-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de organización por su ID", Description = "Devuelve un tipo de organización basado en el ID proporcionado.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var organizationType = await _organizationTypeRepository.GetOrganizationTypeById(id);
            if (organizationType == null)
                return NotFound($"Tipo de organización con ID {id} no encontrado");

            return Ok(organizationType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el tipo de organización con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener el tipo de organización");
        }
    }

    [HttpPost("insert-organization-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de organización", Description = "Crea un nuevo tipo de organización.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<ActionResult> Insert([FromBody] DTOOrganizationType organizationType)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _organizationTypeRepository.InsertOrganizationType(organizationType);
                if (result)
                    return CreatedAtAction(nameof(GetById), new { id = organizationType.Id }, organizationType);

                return BadRequest("No se pudo crear el tipo de organización");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el tipo de organización");
            return StatusCode(500, "Error interno del servidor al crear el tipo de organización");
        }
    }

    [HttpPut("update-organization-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de organización existente", Description = "Actualiza los datos de un tipo de organización existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Update([FromBody] DTOOrganizationType organizationType)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _organizationTypeRepository.UpdateOrganizationType(organizationType);
                if (!result)
                    return NotFound($"Tipo de organización con ID {organizationType.Id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el tipo de organización con ID {Id}", organizationType.Id);
            return StatusCode(500, "Error interno del servidor al actualizar el tipo de organización");
        }
    }

    [HttpDelete("delete-organization-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de organización existente", Description = "Elimina un tipo de organización existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _organizationTypeRepository.DeleteOrganizationType(id);
                if (!result)
                    return NotFound($"Tipo de organización con ID {id} no encontrado");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el tipo de organización con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar el tipo de organización");
        }
    }
}