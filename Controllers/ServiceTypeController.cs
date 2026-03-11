using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador para tipos de servicio por programa (AESAN-257).
/// Los servicios se obtienen desde ServiceType/ServiceTypeProgram, no desde OptionSelection.
/// </summary>
[ApiController]
[Route("service-type")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class ServiceTypeController(IServiceTypeRepository serviceTypeRepository) : ControllerBase
{
    private readonly IServiceTypeRepository _serviceTypeRepository = serviceTypeRepository;

    /// <summary>
    /// Obtiene los tipos de servicio válidos para un programa, con IsStrongService y MinimumMinutesToNextService.
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID del programa.</param>
    /// <returns>Lista de tipos de servicio válidos para el programa.</returns>
    [HttpGet("get-service-types-by-program")]
    [SwaggerOperation(
        Summary = "Obtiene tipos de servicio por programa",
        Description = "Devuelve los tipos de servicio válidos para un programa, con IsStrongService y MinimumMinutesToNextService (AESAN-257).")]
    public async Task<ActionResult> GetServiceTypesByProgram([FromQuery] QueryParameters queryParameters)
    {
        if (!queryParameters.ProgramId.HasValue || queryParameters.ProgramId.Value == 0)
        {
            return BadRequest("El ID del programa es requerido");
        }

        var result = await _serviceTypeRepository.GetServiceTypesByProgram(queryParameters.ProgramId.Value);
        return Ok(result.ToList());
    }
}
