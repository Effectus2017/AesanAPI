using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

[Route("agency")]
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las agencias.
/// Proporciona endpoints para crear, leer, actualizar y gestionar agencias,
/// incluyendo sus programas, estados y logos.
/// </summary>
public class AgencyController(ILogger<AgencyController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<AgencyController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene una agencia por su ID
    /// </summary>
    /// <param name="id">El ID de la agencia</param>
    /// <returns>La agencia</returns>
    [HttpGet("get-agency-by-id")]
    [SwaggerOperation(Summary = "Obtiene una agencia por su ID", Description = "Devuelve una agencia basada en el ID proporcionado.")]
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
    /// Obtiene una agencia por su ID para visita preoperacional
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta que incluyen el ID de la agencia y el ID del usuario</param>
    /// <returns>La agencia para visita preoperacional</returns>
    [HttpGet("get-agency-by-id-and-user-id")]
    [SwaggerOperation(Summary = "Obtiene una agencia por su ID y el ID del usuario", Description = "Devuelve una agencia basada en el ID proporcionado y el ID del usuario.")]
    public async Task<IActionResult> GetAgencyByIdAndUserId([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo agencia por ID y usuario: {AgencyId}, {UserId}", queryParameters.AgencyId, queryParameters.UserId);

                if (queryParameters.AgencyId == 0 || string.IsNullOrEmpty(queryParameters.UserId))
                {
                    return BadRequest("El ID de la agencia y el ID del usuario son requeridos");
                }

                var agency = await _unitOfWork.AgencyRepository.GetAgencyByIdAndUserId(queryParameters.AgencyId ?? 0, queryParameters.UserId);

                if (agency == null)
                {
                    return NotFound($"Agencia con ID {queryParameters.AgencyId} no encontrada para visita preoperacional");
                }

                return Ok(agency);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la agencia para visita preoperacional");
            return StatusCode(500, "Error al obtener la agencia para visita preoperacional");
        }
    }

    /// <summary>
    /// Obtiene todas las agencias de la base de datos
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>Las agencias</returns>
    [HttpGet("get-all-agencies-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las agencias de la base de datos", Description = "Devuelve una lista de todas las agencias.")]
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
                    queryParameters.UserId,
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
    /// Obtiene los programas de una agencia por el ID del usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>Los programas de la agencia</returns>
    [HttpGet("get-agency-programs-by-user-id")]
    [SwaggerOperation(Summary = "Obtiene los programas de una agencia por el ID del usuario", Description = "Devuelve los programas asociados a la agencia del usuario.")]
    public async Task<IActionResult> GetAgencyProgramsByUserId([FromQuery] string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("El ID del usuario no puede estar vacío.");
            }

            var programs = await _unitOfWork.AgencyRepository.GetAgencyProgramsByUserId(userId);
            if (programs == null || !programs.Any())
            {
                return NotFound("No se encontraron programas para el usuario especificado.");
            }

            return Ok(programs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los programas de la agencia por el ID del usuario");
            return StatusCode(500, "Error al obtener los programas de la agencia");
        }
    }

    /// <summary>
    /// Actualiza una agencia
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <param name="agencyRequest">El modelo de la agencia a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-agency")]
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

    /// <summary>
    /// Actualiza el programa de una agencia
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <param name="updateAgencyProgramRequest">El modelo de la agencia a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-agency-program")]
    [SwaggerOperation(Summary = "Actualiza el programa de una agencia", Description = "Actualiza el programa de una agencia con los datos proporcionados.")]
    public async Task<IActionResult> UpdateAgencyProgram([FromQuery] QueryParameters queryParameters, [FromBody] UpdateAgencyProgramRequest updateAgencyProgramRequest)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.AgencyRepository.UpdateAgencyProgram(
                    updateAgencyProgramRequest.AgencyId,
                    updateAgencyProgramRequest.ProgramId,
                    updateAgencyProgramRequest.StatusId,
                    updateAgencyProgramRequest.UserId,
                    updateAgencyProgramRequest.Comment,
                    updateAgencyProgramRequest.AppointmentCoordinated,
                    updateAgencyProgramRequest.AppointmentDate
                );

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el programa de la agencia");
            return StatusCode(500, "Error al actualizar el programa de la agencia");
        }
    }
}
