using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

[Route("agency")]
#if !DEBUG
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
#endif
public class AgencyController(ILogger<AgencyController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<AgencyController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene todas las agencias de la base de datos
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>Las agencias</returns>
    [HttpGet("get-all-agencies-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las agencias de la base de datos", Description = "Devuelve una lista de todas las agencias.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> GetAllAgencies([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var agencies = await _unitOfWork.AgencyRepository.GetAllAgenciesFromDb(
                    queryParameters.Take,
                    queryParameters.Skip,
                    queryParameters.Name,
                    queryParameters.RegionId,
                    queryParameters.CityId,
                    queryParameters.ProgramId,
                    queryParameters.StatusId,
                    queryParameters.Alls
                );
                return Ok(agencies);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las agencias");
            return StatusCode(500, "Error al obtener las agencias");
        }
    }

    /// <summary>
    /// Obtiene una agencia por su ID
    /// </summary>
    /// <param name="id">El ID de la agencia</param>
    /// <returns>La agencia</returns>
    [HttpGet("get-agency-by-id")]
    [SwaggerOperation(Summary = "Obtiene una agencia por su ID", Description = "Devuelve una agencia basada en el ID proporcionado.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> GetAgencyById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo agencia por ID: {Id}", queryParameters.AgencyId);

                if (queryParameters.AgencyId == 0)
                {
                    return BadRequest("El ID de la agencia es requerido");
                }

                var agency = await _unitOfWork.AgencyRepository.GetAgencyById(queryParameters.AgencyId ?? 0);

                if (agency == null)
                {
                    return NotFound($"Agencia con ID {queryParameters.AgencyId} no encontrada");
                }

                return Ok(agency);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la agencia");
            return StatusCode(500, "Error al obtener la agencia");
        }
    }

    /// <summary>
    /// Obtiene todos los estados de la agencia
    /// </summary>
    /// <returns>Los estados de la agencia</returns>
    [HttpGet("get-all-agency-status-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los estados de la agencia", Description = "Devuelve todos los estados de la agencia.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> GetAllAgencyStatus([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            _logger.LogInformation("Obteniendo todos los estados de la agencia");

            var agencyStatus = await _unitOfWork.AgencyRepository.GetAllAgencyStatus(
                queryParameters.Take,
                queryParameters.Skip,
                queryParameters.Name,
                queryParameters.Alls
            );

            if (agencyStatus == null)
            {
                return NotFound("No se encontraron estados de la agencia");
            }

            return Ok(agencyStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los estados de la agencia");
            return StatusCode(500, "Error al obtener los estados de la agencia");
        }
    }


    /// <summary>
    /// Actualiza una agencia
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <param name="agencyRequest">El modelo de la agencia a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-agency")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> UpdateAgency([FromQuery] QueryParameters queryParameters, [FromBody] AgencyRequest agencyRequest)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.AgencyRepository.UpdateAgency(queryParameters.AgencyId ?? 0, agencyRequest);
                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la agencia");
            return StatusCode(500, "Error al actualizar la agencia");
        }
    }

    /// <summary>
    /// Actualiza el logo de una agencia
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-agency-logo")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> UpdateAgencyLogo([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.AgencyRepository.UpdateAgencyLogo(queryParameters.AgencyId ?? 0, queryParameters.ImageUrl ?? "");
                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el logo de la agencia");
            return StatusCode(500, "Error al actualizar el logo de la agencia");
        }
    }

    /// <summary>
    /// Actualiza el estado de una agencia
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-agency-status")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> UpdateAgencyStatus([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                return Ok(await _unitOfWork.AgencyRepository.UpdateAgencyStatus(queryParameters.AgencyId ?? 0, queryParameters.StatusId ?? 0, queryParameters.RejectionJustification ?? ""));
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado de la agencia");
            return StatusCode(500, "Error al actualizar el estado de la agencia");
        }
    }
}
