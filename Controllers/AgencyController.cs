using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Data;
using Api.Filters;
using Dapper;
using System.Data;
// using ElmahCore;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las agencias.
/// Proporciona endpoints para crear, leer, actualizar y gestionar agencias,
/// incluyendo sus programas, estados y logos.
/// </summary>
[ApiController]
[Route("agency")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class AgencyController(IUnitOfWork unitOfWork, MessageTemplateService messageTemplateService, DapperContext dapperContext) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly MessageTemplateService _messageTemplateService = messageTemplateService ?? throw new ArgumentNullException(nameof(messageTemplateService));
    private readonly DapperContext _dapperContext = dapperContext ?? throw new ArgumentNullException(nameof(dapperContext));

    /// <summary>
    /// Obtiene una agencia por su ID
    /// </summary>
    /// <param name="id">El ID de la agencia</param>
    /// <returns>La agencia</returns>
    [HttpGet("get-agency-by-id")]
    [SwaggerOperation(Summary = "Obtiene una agencia por su ID", Description = "Devuelve una agencia basada en el ID proporcionado.")]
    public async Task<IActionResult> GetAgencyById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.AgencyId == 0)
        {
            return BadRequest("El ID de la agencia es requerido");
        }

        var agency = await _unitOfWork.AgencyRepository.GetAgencyById(queryParameters.AgencyId);

        if (agency == null)
        {
            return NotFound($"Agencia con ID {queryParameters.AgencyId} no encontrada");
        }

        return Ok(agency);
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
        if (queryParameters.AgencyId == 0 || string.IsNullOrEmpty(queryParameters.UserId))
        {
            return BadRequest("El ID de la agencia y el ID del usuario son requeridos");
        }

        var agency = await _unitOfWork.AgencyRepository.GetAgencyByIdAndUserId(queryParameters.AgencyId, queryParameters.UserId);

        if (agency == null)
        {
            return NotFound($"Agencia con ID {queryParameters.AgencyId} no encontrada para visita preoperacional");
        }

        return Ok(agency);
    }

    /// <summary>
    /// Obtiene la lista de usuarios AESAN asignados a una agencia (excluye agency_administrator).
    /// </summary>
    [HttpGet("get-assigned-users")]
    [SwaggerOperation(Summary = "Obtiene usuarios asignados a una agencia", Description = "Devuelve la lista de usuarios asignados a la agencia, excluyendo agency_administrator.")]
    public async Task<IActionResult> GetAssignedUsers([FromQuery] int agencyId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();
        if (agencyId == 0)
        {
            return BadRequest("El ID de la agencia es requerido");
        }

        var list = await _unitOfWork.AgencyRepository.GetAgencyAssignedUsers(agencyId, userId);
        return Ok(list);
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
        var agencies = await _unitOfWork.AgencyRepository.GetAllAgenciesFromDb(
            queryParameters.Take,
            queryParameters.Skip,
            queryParameters.Name,
            queryParameters.RegionId,
            queryParameters.CityId,
            queryParameters.ProgramId,
            queryParameters.StatusId,
            queryParameters.UserId,
            queryParameters.Alls,
            queryParameters.ForDropdown,
            queryParameters.IsPropietary,
            queryParameters.UserFirstName,
            queryParameters.StatusName,
            queryParameters.CreatedAtFrom,
            queryParameters.CreatedAtTo,
            queryParameters.UieNumber,
            queryParameters.EinNumber,
            queryParameters.SdrNumber
        );

        return Ok(agencies);
    }

    /// <summary>
    /// Obtiene los programas de una agencia por el ID del usuario
    /// </summary>
    /// <param name="userId">El ID del usuario</param>
    /// <returns>Los programas de la agencia</returns>
    [HttpGet("get-agency-programs-by-user-id")]
    [SwaggerOperation(Summary = "Obtiene los programas de una agencia por el ID del usuario", Description = "Devuelve los programas asociados a la agencia del usuario.")]
    public async Task<IActionResult> GetAgencyProgramsByUserId([FromQuery] QueryParameters queryParameters)
    {
        if (string.IsNullOrEmpty(queryParameters.UserId))
        {
            return BadRequest("El ID del usuario no puede estar vacío.");
        }

        var programs = await _unitOfWork.AgencyRepository.GetAgencyProgramsByUserId(queryParameters.UserId);

        if (programs == null || programs.Count == 0)
        {
            return NotFound("No se encontraron programas para el usuario especificado.");
        }

        return Ok(programs);
    }

    /// <summary>
    /// Actualiza una agencia
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <param name="agencyRequest">El modelo de la agencia a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-agency")]
    [SwaggerOperation(Summary = "Actualiza una agencia", Description = "Actualiza una agencia con los datos proporcionados.")]
    public async Task<IActionResult> UpdateAgency([FromQuery] QueryParameters queryParameters, [FromBody] UserAgencyRequest agencyRequest)
    {
        var result = await _unitOfWork.AgencyRepository.UpdateAgency(queryParameters.AgencyId, agencyRequest.Agency);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("Error al actualizar la agencia");
    }

    /// <summary>
    /// Actualiza el logo de una agencia
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-agency-logo")]
    public async Task<IActionResult> UpdateAgencyLogo([FromQuery] QueryParameters queryParameters)
    {
        var result = await _unitOfWork.AgencyRepository.UpdateAgencyLogo(queryParameters.AgencyId, queryParameters.ImageUrl ?? "");
        return Ok(result);
    }

    /// <summary>
    /// Actualiza el estado de una agencia
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-agency-status")]
    public async Task<IActionResult> UpdateAgencyStatus([FromQuery] QueryParameters queryParameters)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized("Usuario no identificado.");
        }

        var result = await _unitOfWork.AgencyRepository.UpdateAgencyStatus(queryParameters.AgencyId, queryParameters.StatusId ?? 0, queryParameters.RejectionJustification ?? "", userId);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("Error al actualizar el estado de la agencia");
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
        var result = await _unitOfWork.AgencyRepository.UpdateAgencyProgram(
            updateAgencyProgramRequest.AgencyId,
            updateAgencyProgramRequest.ProgramId,
            updateAgencyProgramRequest.UserId
        );

        return Ok(result);
    }

    /// <summary>
    /// Actualiza la inscripción de una agencia desde formulario pre-operacional
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <param name="updateAgencyInscriptionRequest">El modelo de la agencia a actualizar</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-agency-inscription")]
    [SwaggerOperation(Summary = "Actualiza la inscripción de una agencia", Description = "Actualiza la inscripción de una agencia con los datos proporcionados desde el formulario pre-operacional.")]
    public async Task<IActionResult> UpdateAgencyInscription([FromQuery] QueryParameters queryParameters, [FromBody] UpdateAgencyInscriptionRequest updateAgencyInscriptionRequest)
    {
        var result = await _unitOfWork.AgencyRepository.UpdateAgencyInscription(
            updateAgencyInscriptionRequest.AgencyId,
            updateAgencyInscriptionRequest.StatusId,
            updateAgencyInscriptionRequest.Comments,
            updateAgencyInscriptionRequest.AppointmentCoordinated,
            updateAgencyInscriptionRequest.AppointmentDate,
            updateAgencyInscriptionRequest.RejectionJustification
        );

        return Ok(result);
    }

    /// <summary>
    /// Actualiza la fecha de registro completado de una agencia
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-completed-registration-date")]
    [SwaggerOperation(Summary = "Actualiza la fecha de registro completado de una agencia", Description = "Actualiza la fecha de registro completado de una agencia específica.")]
    public async Task<IActionResult> UpdateCompletedRegistrationDate([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.AgencyId == 0)
        {
            return BadRequest("El ID de la agencia es requerido");
        }

        if (queryParameters.CompletedRegistrationDate == null)
        {
            return BadRequest("La fecha de registro completado es requerida");
        }

        var result = await _unitOfWork.AgencyRepository.UpdateCompletedRegistrationDate(queryParameters.AgencyId, queryParameters.CompletedRegistrationDate.Value);

        var agency = await _unitOfWork.AgencyRepository.GetAgencyById(queryParameters.AgencyId);

        if (agency != null)
        {
            var evaluatorUserIds = await _unitOfWork.AgencyRepository.GetAllEvaluatorUserIdsByAgencyId(queryParameters.AgencyId);

            if (evaluatorUserIds != null && evaluatorUserIds.Count != 0)
            {
                var agencyName = agency.Name?.ToString() ?? "";
                var agencyCode = agency.AgencyCode?.ToString() ?? "";
                var completionDate = queryParameters.CompletedRegistrationDate.Value.ToString("dd/MM/yyyy");

                var variables = new Dictionary<string, string>
                {
                    { "SponsorName", agencyName },
                    { "SponsorCode", agencyCode },
                    { "CompletionDate", completionDate }
                };

                int successCount = 0;

                foreach (var evaluatorUserId in evaluatorUserIds)
                {
                    if (string.IsNullOrEmpty(evaluatorUserId))
                    {
                        continue;
                    }

                    await _messageTemplateService.SendMessageAndEmailFromTemplate(
                        messageTemplateKey: "SponsorRegistrationCompleted",
                        emailTemplateKey: "SponsorRegistrationCompleted",
                        recipientUserId: evaluatorUserId,
                        variables: variables,
                        language: "es"
                    );
                    successCount++;
                }
            }
        }

        return Ok(result);
    }
}
