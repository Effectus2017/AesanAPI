using Api.Interfaces;
using Api.Models;
using Api.Models.DTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("organization-type")]
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de organización.
/// Proporciona endpoints para la gestión completa de tipos de organización, incluyendo creación,
/// lectura, actualización y eliminación de registros.
/// </summary>
[ApiController]
public class OrganizationTypeController(IOrganizationTypeRepository organizationTypeRepository, ILogger<OrganizationTypeController> logger) : ControllerBase
{
    private readonly IOrganizationTypeRepository _organizationTypeRepository = organizationTypeRepository;
    private readonly ILogger<OrganizationTypeController> _logger = logger;

    [HttpGet("get-organization-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de organización por su ID", Description = "Devuelve un tipo de organización basado en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        var type = await _organizationTypeRepository.GetOrganizationTypeById(id);
        if (type == null)
        {
            return NotFound($"Tipo de organización con ID {id} no encontrado");
        }
        else
        {
            return Ok(type);
        }
    }

    /// <summary>
    /// Obtiene todos los tipos de organización
    /// </summary>
    /// <param name="take">El número de tipos de organización a obtener</param>
    /// <param name="skip">El número de tipos de organización a saltar</param>
    /// <param name="name">El nombre del tipo de organización</param>
    /// <param name="alls">Indica si se deben obtener todos los tipos de organización</param>
    [HttpGet("get-all-organization-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de organización", Description = "Devuelve una lista de tipos de organización.")]
    public async Task<ActionResult> GetAllFromDb([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            var result = await _organizationTypeRepository.GetAllOrganizationTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los tipos de organización");
            return StatusCode(500, "Error al obtener todos los tipos de organización");
        }
    }

    [HttpPost("insert-organization-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de organización", Description = "Crea un nuevo tipo de organización.")]
    public async Task<ActionResult> Insert([FromBody] DTOOrganizationType organizationType)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _organizationTypeRepository.InsertOrganizationType(organizationType);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("No se pudo crear el tipo de organización");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear el tipo de organización");
            return StatusCode(500, "Error al crear el tipo de organización");
        }
    }

    [HttpPut("update-organization-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de organización existente", Description = "Actualiza los datos de un tipo de organización existente.")]
    public async Task<IActionResult> Update([FromBody] DTOOrganizationType organizationType)
    {
        var result = await _organizationTypeRepository.UpdateOrganizationType(organizationType);
        if (!result)
            return NotFound($"Tipo de organización con ID {organizationType.Id} no encontrado");
        return Ok(result);
    }

    [HttpDelete("delete-organization-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de organización existente", Description = "Elimina un tipo de organización existente.")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        var result = await _organizationTypeRepository.DeleteOrganizationType(id);
        if (!result)
            return NotFound($"Tipo de organización con ID {id} no encontrado");
        return NoContent();
    }
}