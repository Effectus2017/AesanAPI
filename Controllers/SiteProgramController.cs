using Api.Interfaces;
using Api.Models.Request;
using Api.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar relaciones sitio-programa
/// </summary>
[Route("site-program")]
[ApiController]
[Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
public class SiteProgramController(
    ISiteProgramService service,
    ILogger<SiteProgramController> logger) : Controller
{
    private readonly ISiteProgramService _service = service ?? throw new ArgumentNullException(nameof(service));
    private readonly ILogger<SiteProgramController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Obtiene todos los programas de un sitio
    /// </summary>
    [HttpGet("get-by-site-id")]
    [SwaggerOperation(Summary = "Obtiene todos los programas de un sitio")]
    public async Task<IActionResult> GetSiteProgramsBySiteId([FromQuery] int siteId)
    {
        try
        {
            if (siteId <= 0)
            {
                return BadRequest("El ID del sitio debe ser mayor que 0");
            }

            var programs = await _service.GetSiteProgramsBySiteId(siteId);
            return Ok(programs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener programas del sitio {SiteId}", siteId);
            return StatusCode(500, "Error interno del servidor al obtener los programas del sitio");
        }
    }

    /// <summary>
    /// Inserta una nueva relación sitio-programa
    /// </summary>
    [HttpPost("insert")]
    [SwaggerOperation(Summary = "Inserta una nueva relación sitio-programa")]
    public async Task<IActionResult> InsertSiteProgram([FromBody] SiteProgramRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _service.InsertSiteProgram(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar relación sitio-programa");
            return StatusCode(500, "Error interno del servidor al insertar la relación sitio-programa");
        }
    }

    /// <summary>
    /// Actualiza una relación sitio-programa existente
    /// </summary>
    [HttpPut("update")]
    [SwaggerOperation(Summary = "Actualiza una relación sitio-programa existente")]
    public async Task<IActionResult> UpdateSiteProgram([FromBody] SiteProgramRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar relación sitio-programa {Id}", request.Id);
            return StatusCode(500, "Error interno del servidor al actualizar la relación sitio-programa");
        }
    }
}

