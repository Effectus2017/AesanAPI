using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los sitios.
/// Proporciona endpoints para la gestión completa de sitios, incluyendo creación,
/// lectura, actualización y eliminación de registros de sitios.
/// </summary>
[Route("site")]
[ApiController]
public class SiteController(ILogger<SiteController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<SiteController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

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
                var result = await _unitOfWork.SiteRepository.GetAllSitesFromDB(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.CityId, queryParameters.RegionId, queryParameters.AgencyId, queryParameters.Alls, queryParameters.IsList);

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
                var result = await _unitOfWork.SiteRepository.InsertSite(request);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("Error al insertar el sitio");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
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
                var result = await _unitOfWork.SiteRepository.UpdateSite(request);

                if (result)
                {
                    return Ok(result);
                }

                return NotFound($"Sitio con ID {request.Id} no encontrado");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el sitio: {Message}", ex.Message);
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
    /// Verifica si existe un sitio principal para una agencia
    /// </summary>
    /// <param name="agencyId">ID de la agencia</param>
    /// <returns>True si existe un sitio principal, false en caso contrario</returns>
    [HttpGet("has-main-site")]
    [SwaggerOperation(Summary = "Verifica si existe un sitio principal", Description = "Devuelve true si existe un sitio principal para la agencia especificada, false en caso contrario.")]
    public async Task<ActionResult<bool>> HasMainSite([FromQuery] int agencyId)
    {
        try
        {
            var result = await _unitOfWork.SiteRepository.HasMainSite(agencyId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si existe un sitio principal para la agencia {AgencyId}: {Message}", agencyId, ex.Message);
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
    public async Task<IActionResult> UpdateSiteActiveStatus([FromQuery] QueryParameters queryParameters)
    {
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

            var result = await _unitOfWork.SiteRepository.UpdateSiteActiveStatus(queryParameters.SiteId.Value, queryParameters.IsActive, queryParameters.InactiveJustification);

            if (result)
            {
                return Ok(result);
            }

            return NotFound($"Sitio con ID {queryParameters.SiteId} no encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado activo del sitio {SiteId}: {Message}", queryParameters.SiteId, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
