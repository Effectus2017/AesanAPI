using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
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
[ValidateModelState]
public class SiteController(IUnitOfWork unitOfWork, MessageTemplateService messageTemplateService) : Controller
{
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
        var result = await _unitOfWork.SiteRepository.GetSiteById(queryParameters.Id);

        if (result == null)
        {
            return NotFound($"Sitio con ID {queryParameters.Id} no encontrado");
        }

        return Ok(result);
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
        var result = await _unitOfWork.SiteRepository.GetAllSitesFromDB(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.CityId, queryParameters.RegionId, queryParameters.AgencyId, queryParameters.Alls, queryParameters.ForDropdown, queryParameters.IsDayCareHomeId);

        if (result == null)
        {
            return NotFound("No se encontraron sitios");
        }

        return Ok(result);
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
        var result = await _unitOfWork.SiteRepository.InsertSite(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("Error al insertar el sitio");
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
        // Validación condicional: requerir justificación solo cuando IsActive es false
        if (request.IsActive == false && string.IsNullOrWhiteSpace(request.InactiveJustification))
        {
            ModelState.AddModelError("InactiveJustification", "Se requiere justificación para inactiva el sitio");
        }

        var result = await _unitOfWork.SiteRepository.UpdateSite(request);

        if (result)
        {
            return Ok(result);
        }

        return NotFound($"Sitio con ID {request.Id} no encontrado");
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

    /// <summary>
    /// Elimina un sitio
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta para la eliminación</param>
    /// <returns>El sitio eliminado</returns>
    [HttpDelete("delete-site")]
    [SwaggerOperation(Summary = "Elimina un sitio", Description = "Elimina un sitio de la base de datos.")]
    public async Task<IActionResult> DeleteSite([FromQuery] QueryParameters queryParameters)
    {
        var result = await _unitOfWork.SiteRepository.DeleteSite(queryParameters.Id);

        if (result)
        {
            return Ok(result);
        }

        return NotFound($"Sitio con ID {queryParameters.Id} no encontrado");
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
            queryParameters.IsActive,
            queryParameters.InactiveJustification,
            queryParameters.InactiveDate,
            queryParameters.ProvidedRationsService);

        // Si se inactivó el sitio, enviar notificación al evaluador asignado
        if (result && queryParameters.IsActive == false)
        {
            // Obtener información del sitio
            var site = await _unitOfWork.SiteRepository.GetSiteById(queryParameters.SiteId.Value);

            if (site != null)
            {
                // Obtener información de la agencia del sitio
                var agencyId = ((dynamic)site).AgencyId;
                if (agencyId != null)
                {
                    var agency = await _unitOfWork.AgencyRepository.GetAgencyById((int)agencyId);

                    if (agency != null)
                    {

                        var agencyIdInt = (int)agencyId;
                        var evaluatorUserId = await _unitOfWork.AgencyRepository.GetEvaluatorUserIdByAgencyId(agencyIdInt);

                        if (!string.IsNullOrEmpty(evaluatorUserId))
                        {
                            // Preparar variables para los templates
                            var siteName = ((dynamic)site).Name?.ToString() ?? "";
                            var siteCode = ((dynamic)site).SiteCode?.ToString() ?? ((dynamic)site).SiteNumber?.ToString() ?? "";
                            var agencyName = ((dynamic)agency).Name?.ToString() ?? "";
                            var inactiveJustification = queryParameters.InactiveJustification ?? "";
                            var inactiveDate = DateTime.Now.ToString("dd/MM/yyyy");

                            var variables = new Dictionary<string, string>
                            {
                                { "SiteName", siteName },
                                { "SiteCode", siteCode },
                                { "AgencyName", agencyName },
                                { "InactiveJustification", inactiveJustification },
                                { "InactiveDate", inactiveDate }
                            };

                            await _messageTemplateService.SendMessageAndEmailFromTemplate(
                                messageTemplateKey: "SiteInactivated",
                                emailTemplateKey: "SiteInactivated",
                                recipientUserId: evaluatorUserId,
                                variables: variables,
                                language: "es"
                            );
                        }
                    }
                }
            }
        }

        return Ok(result);
    }
}
