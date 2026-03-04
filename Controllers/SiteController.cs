using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Exceptions;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Services;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los sitios.
/// Proporciona endpoints para la gestión completa de sitios, incluyendo creación,
/// lectura, actualización y eliminación de registros de sitios.
/// </summary>
[ApiController]
[Route("site")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SiteController(ILogger<SiteController> logger, IUnitOfWork unitOfWork, MessageTemplateService messageTemplateService) : Controller
{
    private readonly ILogger<SiteController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    private readonly MessageTemplateService _messageTemplateService = messageTemplateService ?? throw new ArgumentNullException(nameof(messageTemplateService));

    /// <summary>
    /// Obtiene un sitio por su ID
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta para la obtención del sitio</param>
    /// <returns>El sitio encontrado</returns>
    [HttpGet("get-site-by-id")]
    [SwaggerOperation(Summary = "Obtiene un sitio por su ID", Description = "Devuelve un sitio basado en el ID proporcionado.")]
    public async Task<IActionResult> GetSiteById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            var result = await _unitOfWork.SiteRepository.GetSiteById(queryParameters.Id);

            if (result == null)
            {
                return NotFound($"Sitio con ID {queryParameters.Id} no encontrado");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el sitio: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todos los sitios
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta para la paginación y filtrado</param>
    /// <returns>Una lista paginada de sitios</returns>
    [HttpGet("get-all-sites-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los sitios", Description = "Devuelve una lista paginada de sitios.")]
    public async Task<IActionResult> GetAllSitesFromDB([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.SiteRepository.GetAllSitesFromDB(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.CityId, queryParameters.RegionId, queryParameters.AgencyId, queryParameters.Alls, queryParameters.IsList, queryParameters.IsDayCareHomeId);

                if (result == null)
                {
                    return NotFound("No se encontraron sitios");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los sitios: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Inserta un nuevo sitio
    /// </summary>
    /// <param name="request">El sitio a insertar</param>
    /// <returns>El sitio insertado</returns>
    [HttpPost("insert-site")]
    [SwaggerOperation(Summary = "Inserta un nuevo sitio", Description = "Crea un nuevo sitio en la base de datos.")]
    public async Task<IActionResult> InsertSite([FromBody] SiteRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var academicRangeError = ValidateAcademicTimesWithinOperatingHours(request);
                if (academicRangeError != null)
                {
                    return BadRequest(academicRangeError);
                }

                var result = await _unitOfWork.SiteRepository.InsertSite(request);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("Error al insertar el sitio");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (SiteValidationException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el sitio: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza un sitio existente
    /// </summary>
    /// <param name="request">El sitio a actualizar</param>
    /// <returns>El sitio actualizado</returns>
    [HttpPut("update-site")]
    [SwaggerOperation(Summary = "Actualiza un sitio existente", Description = "Actualiza los datos de un sitio existente.")]
    public async Task<IActionResult> UpdateSite([FromBody] SiteRequest request)
    {
        try
        {
            // Validación condicional: requerir justificación solo cuando IsActive es false
            if (request.IsActive == false && string.IsNullOrWhiteSpace(request.InactiveJustification))
            {
                ModelState.AddModelError("InactiveJustification", "Se requiere justificación para inactiva el sitio");
            }

            if (ModelState.IsValid)
            {
                var academicRangeError = ValidateAcademicTimesWithinOperatingHours(request);
                if (academicRangeError != null)
                {
                    return BadRequest(academicRangeError);
                }

                var result = await _unitOfWork.SiteRepository.UpdateSite(request);

                if (result)
                {
                    return Ok(result);
                }

                return NotFound($"Sitio con ID {request.Id} no encontrado");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (SiteValidationException ex)
        {
            return BadRequest(new { code = ex.Code, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el sitio: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza solo los grupos de niños y sus servicios de un sitio (persistencia inmediata desde el modal).
    /// </summary>
    /// <param name="request">SiteId y lista de ChildGroups</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-site-child-groups")]
    [SwaggerOperation(Summary = "Actualiza grupos de niños del sitio", Description = "Persiste en el momento los grupos y servicios editados desde el modal.")]
    public async Task<IActionResult> UpdateSiteChildGroups([FromBody] UpdateSiteChildGroupsRequest request)
    {
        try
        {
            if (request == null || request.SiteId <= 0)
            {
                return BadRequest("SiteId es requerido y debe ser mayor que 0");
            }

            var result = await _unitOfWork.SiteRepository.UpdateSiteChildGroupsOnly(request.SiteId, request.ChildGroups ?? []);

            if (result)
            {
                return Ok(true);
            }

            return NotFound($"Sitio con ID {request.SiteId} no encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar grupos del sitio: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Elimina un sitio
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta para la eliminación</param>
    /// <returns>El sitio eliminado</returns>
    [HttpDelete("delete-site")]
    [SwaggerOperation(Summary = "Elimina un sitio", Description = "Elimina un sitio de la base de datos.")]
    public async Task<IActionResult> DeleteSite([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            var result = await _unitOfWork.SiteRepository.DeleteSite(queryParameters.Id);

            if (result)
            {
                return Ok(result);
            }

            return NotFound($"Sitio con ID {queryParameters.Id} no encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el sitio: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza el estado activo/inactivo de un sitio
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <param name="isActive">Estado activo (true) o inactivo (false)</param>
    /// <param name="inactiveJustification">Justificación cuando se inactiva (requerida si isActive es false)</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-active-status")]
    [SwaggerOperation(Summary = "Actualiza el estado activo/inactivo de un sitio", Description = "Permite activar o inactiva un sitio. Requiere justificación al inactiva.")]
    public async Task<IActionResult> UpdateSiteActiveStatus([FromBody] QueryParameters queryParameters)
    {
        _logger.LogInformation("UpdateSiteActiveStatus - INICIO - SiteId: {SiteId}, IsActive: {IsActive}",
            queryParameters.SiteId, queryParameters.IsActive);

        try
        {
            if (queryParameters.IsActive == false && string.IsNullOrWhiteSpace(queryParameters.InactiveJustification))
            {
                return BadRequest("Se requiere justificación para inactiva el sitio");
            }

            if (!queryParameters.SiteId.HasValue)
            {
                return BadRequest("El ID del sitio es requerido");
            }

            var result = await _unitOfWork.SiteRepository.UpdateSiteActiveStatus(
                queryParameters.SiteId.Value,
                queryParameters.IsActive ?? true,
                queryParameters.InactiveJustification,
                queryParameters.InactiveDate,
                queryParameters.ProvidedRationsService);
            _logger.LogInformation("UpdateSiteActiveStatus - Estado actualizado para SiteId: {SiteId}, Result: {Result}", queryParameters.SiteId.Value, result);

            // Si se inactivó el sitio, enviar notificación al evaluador asignado
            if (result && queryParameters.IsActive == false)
            {
                // Obtener información del sitio
                var site = await _unitOfWork.SiteRepository.GetSiteById(queryParameters.SiteId.Value);

                if (site != null)
                {
                    _logger.LogInformation("UpdateSiteActiveStatus - Sitio {SiteId} obtenido correctamente", queryParameters.SiteId.Value);

                    // Obtener información de la agencia del sitio
                    var agencyId = ((dynamic)site).AgencyId;
                    if (agencyId != null)
                    {
                        var agency = await _unitOfWork.AgencyRepository.GetAgencyById((int)agencyId);

                        if (agency != null)
                        {

                            var agencyIdInt = (int)agencyId;
                            var evaluatorUserId = await _unitOfWork.AgencyRepository.GetEvaluatorUserIdByAgencyId(agencyIdInt);
                            _logger.LogInformation("UpdateSiteActiveStatus - EvaluatorUserId final: {EvaluatorUserId}", evaluatorUserId ?? "NULL");

                            if (!string.IsNullOrEmpty(evaluatorUserId))
                            {
                                // Preparar variables para los templates
                                var siteName = ((dynamic)site).Name?.ToString() ?? "";
                                var siteCode = ((dynamic)site).SiteCode?.ToString() ?? ((dynamic)site).SiteNumber?.ToString() ?? "";
                                var agencyName = ((dynamic)agency).Name?.ToString() ?? "";
                                var inactiveJustification = queryParameters.InactiveJustification ?? "";
                                var inactiveDate = DateTime.Now.ToString("dd/MM/yyyy");

                                _logger.LogInformation("UpdateSiteActiveStatus - Variables preparadas: SiteName={SiteName}, SiteCode={SiteCode}, AgencyName={AgencyName}, InactiveDate={InactiveDate}",
                                    (string)siteName, (string)siteCode, (string)agencyName, (string)inactiveDate);

                                var variables = new Dictionary<string, string>
                                {
                                    { "SiteName", siteName },
                                    { "SiteCode", siteCode },
                                    { "AgencyName", agencyName },
                                    { "InactiveJustification", inactiveJustification },
                                    { "InactiveDate", inactiveDate }
                                };

                                // Enviar mensaje interno Y email usando templates separados
                                try
                                {
                                    _logger.LogInformation("UpdateSiteActiveStatus - Llamando a SendMessageAndEmailFromTemplate para UserId: {EvaluatorUserId}", evaluatorUserId);
                                    await _messageTemplateService.SendMessageAndEmailFromTemplate(
                                        messageTemplateKey: "SiteInactivated",
                                        emailTemplateKey: "SiteInactivated",
                                        recipientUserId: evaluatorUserId,
                                        variables: variables,
                                        language: "es"
                                    );
                                    _logger.LogInformation("UpdateSiteActiveStatus - SendMessageAndEmailFromTemplate completado exitosamente");
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error al enviar mensaje y email al evaluador {EvaluatorUserId}: {Message}", evaluatorUserId, ex.Message);
                                    // No fallar la operación principal si falla el envío de mensaje/email
                                }
                            }
                            else
                            {
                                var agencyIdForLog = (int)agencyId;
                                _logger.LogWarning("UpdateSiteActiveStatus - No se encontró evaluador asignado para la agencia {AgencyId}", agencyIdForLog);
                            }
                        }
                        else
                        {
                            var agencyIdForLog = (int)agencyId;
                            _logger.LogWarning("UpdateSiteActiveStatus - Agencia con ID {AgencyId} no encontrada", agencyIdForLog);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("UpdateSiteActiveStatus - Site {SiteId} no tiene AgencyId", queryParameters.SiteId.Value);
                    }
                }
                else
                {
                    _logger.LogWarning("UpdateSiteActiveStatus - Sitio con ID {SiteId} no encontrado después de actualizar", queryParameters.SiteId.Value);
                }
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado activo del sitio {SiteId}: {Message}", queryParameters.SiteId, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Valida que las horas académicas (FirstAcademicClassStartTime, LastAcademicClassEndTime) estén dentro del rango de funcionamiento (OperatingStartTime, OperatingEndTime).
    /// </summary>
    /// <returns>Objeto con mensaje de error para BadRequest, o null si la validación es correcta.</returns>
    private static object? ValidateAcademicTimesWithinOperatingHours(SiteRequest request)
    {
        var hasAcademicStart = request.FirstAcademicClassStartTime.HasValue;
        var hasAcademicEnd = request.LastAcademicClassEndTime.HasValue;
        if (!hasAcademicStart && !hasAcademicEnd)
        {
            return null;
        }

        if (!request.OperatingStartTime.HasValue || !request.OperatingEndTime.HasValue)
        {
            return new { message = "Las horas de clase académica deben estar dentro del horario de funcionamiento del sitio." };
        }

        var operatingStart = request.OperatingStartTime.Value;
        var operatingEnd = request.OperatingEndTime.Value;

        if (hasAcademicStart)
        {
            var start = request.FirstAcademicClassStartTime!.Value;
            if (start < operatingStart || start > operatingEnd)
            {
                return new { message = "Las horas de clase académica deben estar dentro del horario de funcionamiento del sitio." };
            }
        }

        if (hasAcademicEnd)
        {
            var end = request.LastAcademicClassEndTime!.Value;
            if (end < operatingStart || end > operatingEnd)
            {
                return new { message = "Las horas de clase académica deben estar dentro del horario de funcionamiento del sitio." };
            }
        }

        return null;
    }
}
