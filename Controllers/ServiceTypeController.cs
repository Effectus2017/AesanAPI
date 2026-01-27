using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador para tipos de servicio por programa (AESAN-257).
/// Los servicios se obtienen desde ServiceType/ServiceTypeProgram, no desde OptionSelection.
/// </summary>
[Route("service-type")]
[ApiController]
public class ServiceTypeController(IServiceTypeRepository serviceTypeRepository, ILogger<ServiceTypeController> logger)
    : ControllerBase
{
    private readonly IServiceTypeRepository _serviceTypeRepository = serviceTypeRepository;
    private readonly ILogger<ServiceTypeController> _logger = logger;

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
        try
        {
            if (!queryParameters.ProgramId.HasValue || queryParameters.ProgramId.Value == 0)
            {
                return BadRequest("El ID del programa es requerido");
            }

            _logger.LogInformation(
                "Obteniendo tipos de servicio para el programa: {ProgramId}",
                queryParameters.ProgramId.Value);

            var result = await _serviceTypeRepository.GetServiceTypesByProgram(queryParameters.ProgramId.Value);
            return Ok(result.ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al obtener tipos de servicio para el programa {ProgramId}",
                queryParameters.ProgramId);
            return StatusCode(500, "Error interno del servidor al obtener los tipos de servicio");
        }
    }
}
