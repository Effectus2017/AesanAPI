using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las asignaciones School-Site.
/// Proporciona endpoints para la gestión de asignaciones entre escuelas y sitios.
/// </summary>
[ApiController]
[Route("school-site")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class SchoolSiteController(ILogger<SchoolSiteController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<SchoolSiteController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene todos los Sites asignados a una School específica con paginación
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el schoolId, take y skip</param>
    /// <returns>Lista paginada de sites asignados a la escuela</returns>
    [HttpGet("get-sites-by-school")]
    [SwaggerOperation(Summary = "Obtiene sites por escuela", Description = "Devuelve todos los sites asignados a una escuela específica con paginación.")]
    public async Task<IActionResult> GetSitesBySchoolId([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (!queryParameters.SchoolId.HasValue)
            {
                return BadRequest("El SchoolId es requerido");
            }

            var result = await _unitOfWork.SchoolSiteRepository.GetSchoolSitesBySchoolId(
                queryParameters.SchoolId.Value,
                queryParameters.Take,
                queryParameters.Skip,
                queryParameters.Name);

            if (result == null)
            {
                return NotFound("No se encontraron sites para esta escuela");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener sites por escuela: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Obtiene la School asignada a un Site específico
    /// </summary>
    /// <param name="siteId">ID del sitio</param>
    /// <returns>La escuela asignada al sitio</returns>
    [HttpGet("get-school-by-site/{siteId}")]
    [SwaggerOperation(Summary = "Obtiene escuela por sitio", Description = "Devuelve la escuela asignada a un sitio específico.")]
    public async Task<IActionResult> GetSchoolBySiteId(int siteId)
    {
        try
        {
            var result = await _unitOfWork.SchoolSiteRepository.GetSchoolSiteBySiteId(siteId);

            if (result == null)
            {
                return NotFound($"No se encontró escuela asignada para el sitio {siteId}");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener escuela por sitio: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Asigna un Site a una School
    /// </summary>
    /// <param name="request">La asignación School-Site a crear</param>
    /// <returns>Resultado de la asignación</returns>
    [HttpPost("assign-site-to-school")]
    [SwaggerOperation(Summary = "Asigna sitio a escuela", Description = "Crea una nueva asignación entre un sitio y una escuela.")]
    public async Task<IActionResult> AssignSiteToSchool([FromBody] SchoolSiteRequest request)
    {
        try
        {
            var result = await _unitOfWork.SchoolSiteRepository.InsertSchoolSite(request);

            if (result)
            {
                return Ok(result);
            }

            return BadRequest("Error al asignar el sitio a la escuela");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al asignar sitio a escuela: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza una asignación School-Site existente
    /// </summary>
    /// <param name="request">La asignación School-Site a actualizar</param>
    /// <returns>Resultado de la actualización</returns>
    [HttpPut("update-school-site-assignment")]
    [SwaggerOperation(Summary = "Actualiza asignación School-Site", Description = "Actualiza los datos de una asignación School-Site existente.")]
    public async Task<IActionResult> UpdateSchoolSiteAssignment([FromBody] SchoolSiteRequest request)
    {
        try
        {
            var result = await _unitOfWork.SchoolSiteRepository.UpdateSchoolSite(request);

            if (result)
            {
                return Ok(result);
            }

            return BadRequest("Error al actualizar la asignación School-Site");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar asignación School-Site: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Elimina una asignación School-Site
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta que incluyen el ID de la asignación</param>
    /// <returns>Resultado de la eliminación</returns>
    [HttpDelete("remove-school-site-assignment")]
    [SwaggerOperation(Summary = "Elimina asignación School-Site", Description = "Elimina una asignación School-Site (soft delete).")]
    public async Task<IActionResult> RemoveSchoolSiteAssignment([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            var result = await _unitOfWork.SchoolSiteRepository.DeleteSchoolSite(queryParameters.Id);

            if (result)
            {
                return Ok(result);
            }

            return BadRequest("Error al eliminar la asignación School-Site");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar asignación School-Site: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
