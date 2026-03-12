using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar relaciones sitio-programa
/// </summary>
[ApiController]
[Route("site-program")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class SiteProgramController(ISiteProgramService service) : Controller
{
    private readonly ISiteProgramService _service = service ?? throw new ArgumentNullException(nameof(service));

    /// <summary>
    /// Obtiene todos los programas de un sitio
    /// </summary>
    [HttpGet("get-by-site-id")]
    [SwaggerOperation(Summary = "Obtiene todos los programas de un sitio")]
    public async Task<IActionResult> GetSiteProgramsBySiteId([FromQuery] int siteId)
    {
        if (siteId <= 0)
        {
            return BadRequest("El ID del sitio debe ser mayor que 0");
        }

        var programs = await _service.GetSiteProgramsBySiteId(siteId);
        return Ok(programs);
    }

    /// <summary>
    /// Inserta una nueva relación sitio-programa
    /// </summary>
    [HttpPost("insert")]
    [SwaggerOperation(Summary = "Inserta una nueva relación sitio-programa")]
    public async Task<IActionResult> InsertSiteProgram([FromBody] SiteProgramRequest request)
    {
        var result = await _service.InsertSiteProgram(request);
        return Ok(result);
    }

    /// <summary>
    /// Actualiza una relación sitio-programa existente
    /// </summary>
    [HttpPut("update")]
    [SwaggerOperation(Summary = "Actualiza una relación sitio-programa existente")]
    public async Task<IActionResult> UpdateSiteProgram([FromBody] SiteProgramRequest request)
    {
        if (!request.Id.HasValue)
        {
            return BadRequest("El ID de la relación es requerido para actualizar");
        }

        var result = await _service.UpdateSiteProgram(request);

        if (!result)
        {
            return NotFound($"No se encontró la relación sitio-programa con ID {request.Id}");
        }

        return Ok(new { success = true });
    }
}
