using Api.Interfaces;
using Api.Models;
using Api.Models.DTO;
using Api.Models.Response;
using Api.Models.Errors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los tipos de organización.
/// </summary>
[ApiController]
[Route("organization-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class OrganizationTypeController(IOrganizationTypeRepository organizationTypeRepository) : ControllerBase
{
    private readonly IOrganizationTypeRepository _organizationTypeRepository = organizationTypeRepository;

    /// <summary>
    /// Obtiene un tipo de organización por su ID
    /// </summary>
    [HttpGet("get-organization-type-by-id")]
    [SwaggerOperation(Summary = "Obtiene un tipo de organización por su ID")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            throw new ApiException(ErrorCode.VALIDATION_ERROR, "El ID del tipo de organización es requerido");
        }

        var result = await _organizationTypeRepository.GetOrganizationTypeById(queryParameters.Id);

        if (result == null)
        {
            throw new ApiException(ErrorCode.ENTITY_NOT_FOUND, $"Tipo de organización con ID {queryParameters.Id} no encontrado", 404);
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene todos los tipos de organización
    /// </summary>
    [HttpGet("get-all-organization-types-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los tipos de organización")]
    public async Task<ActionResult> GetAllFromDb([FromQuery] QueryParameters queryParameters)
    {
        var result = await _organizationTypeRepository.GetAllOrganizationTypes(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls, queryParameters.IsList);

        if (result == null)
        {
            throw new ApiException(ErrorCode.ENTITY_NOT_FOUND, "No se encontraron tipos de organización", 404);
        }

        return Ok(result);
    }

    /// <summary>
    /// Crea un nuevo tipo de organización
    /// </summary>
    [HttpPost("insert-organization-type")]
    [SwaggerOperation(Summary = "Crea un nuevo tipo de organización")]
    public async Task<ActionResult> Insert([FromBody] OrganizationTypeRequest request)
    {
        var result = await _organizationTypeRepository.InsertOrganizationType(request);

        if (!result)
        {
            throw new ApiException(ErrorCode.VALIDATION_ERROR, "No se pudo crear el tipo de organización");
        }

        return Ok(result);
    }

    /// <summary>
    /// Actualiza un tipo de organización existente
    /// </summary>
    [HttpPut("update-organization-type")]
    [SwaggerOperation(Summary = "Actualiza un tipo de organización existente")]
    public async Task<IActionResult> Update([FromBody] OrganizationTypeResponse request)
    {
        var result = await _organizationTypeRepository.UpdateOrganizationType(request);

        if (!result)
        {
            throw new ApiException(ErrorCode.VALIDATION_ERROR, "No se pudo actualizar el tipo de organización");
        }

        return Ok(result);
    }

    /// <summary>
    /// Elimina un tipo de organización existente
    /// </summary>
    [HttpDelete("delete-organization-type")]
    [SwaggerOperation(Summary = "Elimina un tipo de organización existente")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        var result = await _organizationTypeRepository.DeleteOrganizationType(queryParameters.Id);

        if (!result)
        {
            throw new ApiException(ErrorCode.ENTITY_NOT_FOUND, $"Tipo de organización con ID {queryParameters.Id} no encontrado", 404);
        }

        return NoContent();
    }

    /// <summary>
    /// Obtiene los tipos de organización válidos para un programa específico
    /// </summary>
    [HttpGet("get-organization-types-by-program")]
    [SwaggerOperation(Summary = "Obtiene tipos de organización por programa")]
    public async Task<ActionResult> GetOrganizationTypesByProgram([FromQuery] QueryParameters queryParameters)
    {
        if (!queryParameters.ProgramId.HasValue || queryParameters.ProgramId == 0)
        {
            throw new ApiException(ErrorCode.VALIDATION_ERROR, "El ID del programa es requerido");
        }

        var result = await _organizationTypeRepository.GetOrganizationTypesByProgram(queryParameters.ProgramId.Value);

        return Ok(result);
    }
}