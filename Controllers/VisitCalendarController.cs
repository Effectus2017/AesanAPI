using Api.Filters;
using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Calendario de visitas por sitio (sponsor-evaluation AESAN).
/// </summary>
[ApiController]
[Route("visit-calendar")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class VisitCalendarController(ISiteVisitCalendarRepository siteVisitCalendarRepository) : Controller
{
    private readonly ISiteVisitCalendarRepository _siteVisitCalendarRepository =
        siteVisitCalendarRepository ?? throw new ArgumentNullException(nameof(siteVisitCalendarRepository));

    [HttpGet("get-visits")]
    [SwaggerOperation(Summary = "Obtiene visitas de un sitio por mes/año", Description = "Devuelve visitas de un sitio de la agencia; filtra por mes y año.")]
    public async Task<IActionResult> GetVisits([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.AgencyId <= 0)
            return BadRequest("El ID de la agencia debe ser mayor a 0");
        if (!queryParameters.SiteId.HasValue || queryParameters.SiteId.Value <= 0)
            return BadRequest("El ID del sitio es obligatorio");

        var result = await _siteVisitCalendarRepository.GetVisits(
            queryParameters.AgencyId,
            queryParameters.SiteId.Value,
            queryParameters.Month,
            queryParameters.Year);
        return Ok(result);
    }

    [HttpGet("get-visit-types")]
    [SwaggerOperation(Summary = "Lista tipos de visita", Description = "Catálogo para combos del calendario. Alls=false (defecto): solo activos. ForDropdown se acepta por convención con otros catálogos.")]
    public async Task<IActionResult> GetVisitTypes([FromQuery] QueryParameters queryParameters)
    {
        var result = await _siteVisitCalendarRepository.GetVisitTypes(queryParameters.Alls);
        return Ok(result);
    }

    [HttpPost("create-visit")]
    [SwaggerOperation(Summary = "Crea una visita de sitio", Description = "Registra una visita en el calendario del sitio.")]
    public async Task<IActionResult> CreateVisit([FromBody] SiteVisitRequest request)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var newId = await _siteVisitCalendarRepository.CreateVisit(request, userId);

        if (newId.HasValue)
            return Ok(new { id = newId.Value, success = true });

        return BadRequest("No se pudo procesar la solicitud");
    }

    [HttpPut("update-visit")]
    [SwaggerOperation(Summary = "Actualiza una visita de sitio", Description = "Actualiza fecha, hora, tipo o comentarios.")]
    public async Task<IActionResult> UpdateVisit([FromBody] SiteVisitRequest request)
    {
        if (!request.Id.HasValue || request.Id.Value <= 0)
            return BadRequest("El ID de la visita es requerido para actualizar");

        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var result = await _siteVisitCalendarRepository.UpdateVisit(request.Id.Value, request, userId);

        if (result)
            return Ok(true);

        return BadRequest("No se pudo procesar la solicitud");
    }

    [HttpDelete("delete-visit/{id}")]
    [SwaggerOperation(Summary = "Elimina una visita (lógico)", Description = "Elimina de forma lógica una visita del sitio.")]
    public async Task<IActionResult> DeleteVisit(int id, [FromQuery] int agencyId)
    {
        if (id <= 0)
            return BadRequest("El ID debe ser mayor a 0");
        if (agencyId <= 0)
            return BadRequest("El ID de la agencia es obligatorio");

        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        var result = await _siteVisitCalendarRepository.DeleteVisit(id, agencyId, userId);

        if (result)
            return Ok(true);

        return BadRequest("No se pudo eliminar la visita");
    }
}
