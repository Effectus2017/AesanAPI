using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las políticas operativas.
/// Proporciona endpoints para la gestión completa de políticas operativas, incluyendo creación,
/// lectura, actualización y eliminación de políticas operativas.
/// </summary>
[Route("operating-policy")]
[ApiController]
public class OperatingPolicyController(IOperatingPolicyRepository operatingPolicyRepository, ILogger<OperatingPolicyController> logger) : ControllerBase
{
    private readonly IOperatingPolicyRepository _operatingPolicyRepository = operatingPolicyRepository;
    private readonly ILogger<OperatingPolicyController> _logger = logger;

    /// <summary>
    /// Obtiene una política operativa por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Política operativa</returns>
    [HttpGet("get-operating-policy-by-id")]
    [SwaggerOperation(Summary = "Obtiene una política operativa por su ID", Description = "Devuelve una política operativa basada en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo política operativa por ID: {Id}", queryParameters.Id);

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

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la política operativa con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener la política operativa");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPolicyRepository.GetAllOperatingPolicies(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);

                if (result == null)
                {
                    return NotFound("No se encontraron políticas operativas");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las políticas operativas");
            return StatusCode(500, "Error interno del servidor al obtener las políticas operativas");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPolicyRepository.InsertOperatingPolicy(request);

                if (result)
                {
                    _logger.LogInformation("Política operativa creada con ID: {Id}", request.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo crear la política operativa");
                return BadRequest("No se pudo crear la política operativa");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la política operativa");
            return StatusCode(500, "Error interno del servidor al crear la política operativa");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPolicyRepository.UpdateOperatingPolicy(request);

                if (!result)
                {
                    _logger.LogWarning("Política operativa con ID {Id} no encontrada", request.Id);
                    return NotFound($"Política operativa con ID {request.Id} no encontrada");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la política operativa con ID {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar la política operativa");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _operatingPolicyRepository.DeleteOperatingPolicy(queryParameters.Id);

                if (!result)
                {
                    _logger.LogWarning("Política operativa con ID {Id} no encontrada", queryParameters.Id);
                    return NotFound($"Política operativa con ID {queryParameters.Id} no encontrada");
                }

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la política operativa con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al eliminar la política operativa");
        }
    }
}