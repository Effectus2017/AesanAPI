using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja las asignaciones de usuarios a agencias.
/// </summary>
[ApiController]
[Route("agency-user-assignment")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
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
            if (string.IsNullOrEmpty(queryParameters.UserId))
            {
                return BadRequest("El ID del usuario es requerido");
            }

            var agencies = await _unitOfWork.AgencyUsersRepository.GetUserAssignedAgencies(
                queryParameters.UserId,
                queryParameters.Take,
                queryParameters.Skip,
                queryParameters.Alls,
                queryParameters.IsList
            );

            return Ok(agencies);
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
            if (string.IsNullOrEmpty(queryParameters.UserId))
            {
                return BadRequest("El ID del usuario es requerido");
            }

            if (queryParameters.AgencyId == null || queryParameters.AgencyId == 0)
            {
                return BadRequest("El ID de la agencia es requerido");
            }

            // Calcular AgencyAssignmentType según el rol del usuario
            string agencyAssignmentType = await _unitOfWork.AgencyUsersRepository.CalculateAgencyAssignmentTypeFromRole(queryParameters.UserId);

            var result = await _unitOfWork.AgencyUsersRepository.AssignAgencyToUser(
                queryParameters.UserId,
                queryParameters.AgencyId,
                queryParameters.AssignedBy ?? queryParameters.UserId, // Si no se proporciona, usar el mismo usuario
                agencyAssignmentType
            );

            return Ok(result);
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
            if (string.IsNullOrEmpty(queryParameters.UserId))
            {
                return BadRequest("El ID del usuario es requerido");
            }

            if (queryParameters.AgencyId == 0)
            {
                return BadRequest("El ID de la agencia es requerido");
            }

            var result = await _unitOfWork.AgencyUsersRepository.UnassignAgencyFromUser(
                queryParameters.UserId,
                queryParameters.AgencyId
            );

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desasignar la agencia del usuario");
            return StatusCode(500, "Error al desasignar la agencia del usuario");
        }
    }
}