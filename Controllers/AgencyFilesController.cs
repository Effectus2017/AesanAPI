using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("agency-files")]
[Authorize]
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los archivos de agencias.
/// Proporciona endpoints para subir, obtener, actualizar y eliminar archivos asociados a agencias.
/// </summary>
public class AgencyFilesController : Controller
{
    private readonly ILogger<AgencyFilesController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AgencyFilesController(ILogger<AgencyFilesController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Obtiene los archivos asociados a una agencia específica
    /// </summary>
    /// <param name="agencyId">Id de la agencia</param>
    /// <param name="take">Número de registros a tomar</param>
    /// <param name="skip">Número de registros a saltar</param>
    /// <param name="documentType">Tipo de documento (opcional)</param>
    /// <returns>Lista paginada de archivos de la agencia</returns>
    [HttpGet("get-agency-files")]
    [SwaggerOperation(Summary = "Obtiene los archivos de una agencia", Description = "Devuelve una lista paginada de archivos asociados a una agencia específica.")]
    public async Task<IActionResult> GetAgencyFiles([FromQuery] int agencyId, [FromQuery] int take = 10, [FromQuery] int skip = 0, [FromQuery] string documentType = null)
    {
        try
        {
            _logger.LogInformation("Obteniendo archivos para la agencia {AgencyId}", agencyId);
            var result = await _unitOfWork.AgencyFilesRepository.GetAgencyFiles(agencyId, take, skip, documentType);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener archivos de la agencia {AgencyId}", agencyId);
            return StatusCode(500, new { message = "Error interno del servidor al obtener los archivos" });
        }
    }

    /// <summary>
    /// Obtiene un archivo específico por su ID
    /// </summary>
    /// <param name="id">ID del archivo</param>
    /// <returns>Información del archivo</returns>
    [HttpGet("get-agency-file-by-id")]
    [SwaggerOperation(Summary = "Obtiene un archivo por su ID", Description = "Devuelve la información de un archivo específico.")]
    public async Task<IActionResult> GetAgencyFileById([FromQuery] int id)
    {
        try
        {
            _logger.LogInformation("Obteniendo archivo con ID {FileId}", id);
            var result = await _unitOfWork.AgencyFilesRepository.GetAgencyFileById(id);

            if (result == null)
            {
                return NotFound(new { message = "Archivo no encontrado" });
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener archivo con ID {FileId}", id);
            return StatusCode(500, new { message = "Error interno del servidor al obtener el archivo" });
        }
    }

    /// <summary>
    /// Agrega un nuevo archivo a una agencia
    /// </summary>
    /// <param name="request">Datos del archivo a agregar</param>
    /// <returns>ID del nuevo archivo</returns>
    [HttpPost("add-agency-file")]
    [SwaggerOperation(Summary = "Agrega un archivo a una agencia", Description = "Registra un nuevo archivo asociado a una agencia.")]
    public async Task<IActionResult> AddAgencyFile([FromBody] AgencyFileRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest(new { message = "Datos de archivo inválidos" });
            }

            // Obtener el ID del usuario actual desde los claims
            var userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;

            _logger.LogInformation("Agregando archivo a la agencia {AgencyId} por el usuario {UserId}", request.AgencyId, userId);
            var newFileId = await _unitOfWork.AgencyFilesRepository.AddAgencyFile(request, userId);

            return Ok(new { id = newFileId, message = "Archivo agregado correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar archivo a la agencia {AgencyId}", request?.AgencyId);
            return StatusCode(500, new { message = "Error interno del servidor al agregar el archivo" });
        }
    }

    /// <summary>
    /// Actualiza la información de un archivo
    /// </summary>
    /// <param name="id">ID del archivo</param>
    /// <param name="description">Nueva descripción (opcional)</param>
    /// <param name="documentType">Nuevo tipo de documento (opcional)</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPut("update-agency-file")]
    [SwaggerOperation(Summary = "Actualiza un archivo", Description = "Actualiza la información de un archivo existente.")]
    public async Task<IActionResult> UpdateAgencyFile([FromQuery] int id, [FromQuery] string description = null, [FromQuery] string documentType = null)
    {
        try
        {
            _logger.LogInformation("Actualizando archivo con ID {FileId}", id);
            var result = await _unitOfWork.AgencyFilesRepository.UpdateAgencyFile(id, description, documentType);

            if (!result)
            {
                return NotFound(new { message = "Archivo no encontrado o no se pudo actualizar" });
            }

            return Ok(new { message = "Archivo actualizado correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar archivo con ID {FileId}", id);
            return StatusCode(500, new { message = "Error interno del servidor al actualizar el archivo" });
        }
    }

    /// <summary>
    /// Elimina lógicamente un archivo (lo marca como inactivo)
    /// </summary>
    /// <param name="id">ID del archivo a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("delete-agency-file")]
    [SwaggerOperation(Summary = "Elimina un archivo", Description = "Elimina lógicamente un archivo (lo marca como inactivo).")]
    public async Task<IActionResult> DeleteAgencyFile([FromQuery] int id)
    {
        try
        {
            _logger.LogInformation("Eliminando archivo con ID {FileId}", id);
            var result = await _unitOfWork.AgencyFilesRepository.DeleteAgencyFile(id);

            if (!result)
            {
                return NotFound(new { message = "Archivo no encontrado o no se pudo eliminar" });
            }

            return Ok(new { message = "Archivo eliminado correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar archivo con ID {FileId}", id);
            return StatusCode(500, new { message = "Error interno del servidor al eliminar el archivo" });
        }
    }
}