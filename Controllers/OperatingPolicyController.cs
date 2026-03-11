using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las políticas operativas.
/// Proporciona endpoints para la gestión completa de políticas operativas, incluyendo creación,
/// lectura, actualización y eliminación de políticas operativas.
/// </summary>
[ApiController]
[Route("operating-policy")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class OperatingPolicyController(IOperatingPolicyRepository operatingPolicyRepository) : ControllerBase
{
    private readonly IOperatingPolicyRepository _operatingPolicyRepository = operatingPolicyRepository;

    /// <summary>
    /// Obtiene una política operativa por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Política operativa</returns>
    [HttpGet("get-operating-policy-by-id")]
    [SwaggerOperation(Summary = "Obtiene una política operativa por su ID", Description = "Devuelve una política operativa basada en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID de la política operativa es requerido");
        }

        var result = await _operatingPolicyRepository.GetOperatingPolicyById(queryParameters.Id);

        if (result == null)
        {
            return NotFound($"Política operativa con ID {queryParameters.Id} no encontrada");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene todas las políticas operativas
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para filtrado y paginación</param>
    /// <returns>Lista de políticas operativas, BadRequest si los parámetros son inválidos, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-operating-policies-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las políticas operativas", Description = "Devuelve una lista de políticas operativas.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var result = await _operatingPolicyRepository.GetAllOperatingPolicies(
            queryParameters.Take,
            queryParameters.Skip,
            queryParameters.Name,
            queryParameters.Alls,
            queryParameters.IsList);

        if (result == null)
        {
            return NotFound("No se encontraron políticas operativas");
        }

        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva política operativa
    /// </summary>
    /// <param name="request">La política operativa a crear</param>
    /// <returns>La política operativa creada</returns>
    [HttpPost("insert-operating-policy")]
    [SwaggerOperation(Summary = "Crea una nueva política operativa", Description = "Crea una nueva política operativa.")]
    public async Task<ActionResult> Insert([FromBody] DTOOperatingPolicy request)
    {
        var result = await _operatingPolicyRepository.InsertOperatingPolicy(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo crear la política operativa");
    }

    /// <summary>
    /// Actualiza una política operativa existente
    /// </summary>
    /// <param name="request">La política operativa a actualizar</param>
    /// <returns>La política operativa actualizada</returns>
    [HttpPut("update-operating-policy")]
    [SwaggerOperation(Summary = "Actualiza una política operativa existente", Description = "Actualiza los datos de una política operativa existente.")]
    public async Task<IActionResult> Update([FromBody] DTOOperatingPolicy request)
    {
        var result = await _operatingPolicyRepository.UpdateOperatingPolicy(request);

        if (!result)
        {
            return NotFound($"Política operativa con ID {request.Id} no encontrada");
        }

        return Ok(result);
    }

    /// <summary>
    /// Elimina una política operativa existente
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>La política operativa eliminada</returns>
    [HttpDelete("delete-operating-policy")]
    [SwaggerOperation(Summary = "Elimina una política operativa existente", Description = "Elimina una política operativa existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        var result = await _operatingPolicyRepository.DeleteOperatingPolicy(queryParameters.Id);

        if (!result)
        {
            return NotFound($"Política operativa con ID {queryParameters.Id} no encontrada");
        }

        return NoContent();
    }
}
