using Api.Interfaces;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con los archivos de agencias.
/// Proporciona endpoints para subir, obtener, actualizar y eliminar archivos asociados a agencias.
/// </summary>
public class AgencyFilesController : ControllerBase
{
    private readonly ILogger<AgencyFilesController> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AgencyFilesController(
        ILogger<AgencyFilesController> logger,
        IUnitOfWork unitOfWork)
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
    [HttpGet("agency/{agencyId}")]
    [SwaggerOperation(Summary = "Obtiene los archivos de una agencia", Description = "Devuelve una lista paginada de archivos asociados a una agencia específica.")]
    public async Task<IActionResult> GetAgencyFiles(int agencyId, [FromQuery] int take = 10, [FromQuery] int skip = 0, [FromQuery] string documentType = null)
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
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obtiene un archivo por su ID", Description = "Devuelve la información de un archivo específico.")]
    public async Task<IActionResult> GetFile(int id)
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
    /// Elimina lógicamente un archivo
    /// </summary>
    /// <param name="id">ID del archivo a eliminar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Elimina un archivo", Description = "Elimina lógicamente un archivo (lo marca como eliminado).")]
    public async Task<IActionResult> DeleteFile(int id)
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

    /// <summary>
    /// Verifica un archivo
    /// </summary>
    /// <param name="id">ID del archivo a verificar</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("{id}/verify")]
    [SwaggerOperation(Summary = "Verifica un archivo", Description = "Marca un archivo como verificado.")]
    public async Task<IActionResult> VerifyFile(int id)
    {
        try
        {
            _logger.LogInformation("Verificando archivo con ID {FileId}", id);
            var result = await _unitOfWork.AgencyFilesRepository.VerifyAgencyFile(id);

            if (!result)
            {
                return NotFound(new { message = "Archivo no encontrado o no se pudo verificar" });
            }

            return Ok(new { message = "Archivo verificado correctamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar archivo con ID {FileId}", id);
            return StatusCode(500, new { message = "Error interno del servidor al verificar el archivo" });
        }
    }
}