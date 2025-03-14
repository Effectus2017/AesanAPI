using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;

namespace Api.Controllers;

[Route("agency-user-assignment")]
public class AgencyUserAssignmentController(ILogger<AgencyUserAssignmentController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<AgencyUserAssignmentController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene las agencias asignadas a un usuario
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>Lista de agencias asignadas al usuario</returns>
    [HttpGet("get-user-assigned-agencies")]
    [SwaggerOperation(Summary = "Obtiene las agencias asignadas a un usuario", Description = "Devuelve una lista de agencias asignadas al usuario especificado.")]
    public async Task<IActionResult> GetUserAssignedAgencies([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(queryParameters.UserId))
                {
                    return BadRequest("El ID del usuario es requerido");
                }

                var agencies = await _unitOfWork.AgencyUserAssignmentRepository.GetUserAssignedAgencies(
                    queryParameters.UserId,
                    queryParameters.Take,
                    queryParameters.Skip
                );

                return Ok(agencies);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las agencias asignadas al usuario");
            return StatusCode(500, "Error al obtener las agencias asignadas al usuario");
        }
    }

    /// <summary>
    /// Asigna una agencia a un usuario
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>True si la asignación fue exitosa</returns>
    [HttpPost("assign-agency-to-user")]
    [SwaggerOperation(Summary = "Asigna una agencia a un usuario", Description = "Asigna una agencia específica a un usuario.")]
    public async Task<IActionResult> AssignAgencyToUser([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(queryParameters.UserId))
                {
                    return BadRequest("El ID del usuario es requerido");
                }

                if (queryParameters.AgencyId == null || queryParameters.AgencyId == 0)
                {
                    return BadRequest("El ID de la agencia es requerido");
                }

                var result = await _unitOfWork.AgencyUserAssignmentRepository.AssignAgencyToUser(
                    queryParameters.UserId,
                    queryParameters.AgencyId,
                    queryParameters.AssignedBy
                );

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar la agencia al usuario");
            return StatusCode(500, "Error al asignar la agencia al usuario");
        }
    }

    /// <summary>
    /// Desasigna una agencia de un usuario
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>True si la desasignación fue exitosa</returns>
    [HttpDelete("unassign-agency-from-user")]
    [SwaggerOperation(Summary = "Desasigna una agencia de un usuario", Description = "Elimina la asignación de una agencia específica a un usuario.")]
    public async Task<IActionResult> UnassignAgencyFromUser([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(queryParameters.UserId))
                {
                    return BadRequest("El ID del usuario es requerido");
                }

                if (queryParameters.AgencyId == 0)
                {
                    return BadRequest("El ID de la agencia es requerido");
                }

                var result = await _unitOfWork.AgencyUserAssignmentRepository.UnassignAgencyFromUser(
                    queryParameters.UserId,
                    queryParameters.AgencyId
                );

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desasignar la agencia del usuario");
            return StatusCode(500, "Error al desasignar la agencia del usuario");
        }
    }
}