using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
using Api.Interfaces;
using Api.Models.Response;
using Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con reportes.
/// Proporciona endpoints para generar y obtener diferentes tipos de reportes del sistema.
/// </summary>
[ApiController]
[Route("reports")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class ReportsController(ILogger<ReportsController> logger, IReportsRepository reportsRepository) : Controller
{
    private readonly ILogger<ReportsController> _logger = logger;
    private readonly IReportsRepository _reportsRepository = reportsRepository ?? throw new ArgumentNullException(nameof(reportsRepository));

    /// <summary>
    /// Obtiene la estructura jerárquica para el árbol de jerarquía de escuelas
    /// </summary>
    /// <param name="year">Año para filtrar la estructura</param>
    /// <param name="sponsorId">ID del auspiciador (opcional). Si no se proporciona, obtiene todos los auspiciadores</param>
    /// <returns>Estructura jerárquica con Auspiciador → Año → Escuelas → Sitios</returns>
    [HttpGet("school-hierarchy-tree")]
    [SwaggerOperation(
        Summary = "Obtiene la estructura jerárquica para el árbol de jerarquía de escuelas",
        Description = "Devuelve la estructura jerárquica completa mostrando Auspiciador Administrador → Año → Escuelas → Sitios para el año y auspiciador especificados."
    )]
    public async Task<IActionResult> GetSchoolHierarchyTree([FromQuery] int year, [FromQuery] int? sponsorId = null)
    {
        try
        {
            if (year <= 0)
            {
                return BadRequest("El año debe ser un valor válido mayor a 0");
            }

            _logger.LogInformation("Obteniendo estructura jerárquica para año: {Year}, SponsorId: {SponsorId}", year, sponsorId);

            var result = await _reportsRepository.GetHierarchyStructure(year, sponsorId);

            if (result == null)
            {
                return NotFound("No se encontró información para los parámetros especificados");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la estructura jerárquica: {Message}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error al obtener la estructura jerárquica", error = ex.Message });
        }
    }
}

