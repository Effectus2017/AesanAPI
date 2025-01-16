using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[Route("federal-funding-certification")]
/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las certificaciones de fondos federales.
/// Proporciona endpoints para la gestión completa de certificaciones, incluyendo creación,
/// lectura, actualización y eliminación de registros de certificaciones federales.
/// </summary>
public class FederalFundingCertificationController(IFederalFundingCertificationRepository federalFundingCertificationRepository, ILogger<FederalFundingCertificationController> logger) : ControllerBase
{
    private readonly IFederalFundingCertificationRepository _federalFundingCertificationRepository = federalFundingCertificationRepository;
    private readonly ILogger<FederalFundingCertificationController> _logger = logger;

    [HttpGet("get-all-federal-funding-certifications-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las certificaciones de fondos federales", Description = "Devuelve una lista de certificaciones de fondos federales.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var certifications = await _federalFundingCertificationRepository.GetAllFederalFundingCertifications(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
                return Ok(certifications);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las certificaciones de fondos federales");
            return StatusCode(500, "Error interno del servidor al obtener las certificaciones de fondos federales");
        }
    }

    [HttpGet("get-federal-funding-certification-by-id")]
    [SwaggerOperation(Summary = "Obtiene una certificación de fondos federales por su ID", Description = "Devuelve una certificación de fondos federales basada en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var certification = await _federalFundingCertificationRepository.GetFederalFundingCertificationById(id);
            if (certification == null)
                return NotFound($"Certificación de fondos federales con ID {id} no encontrada");

            return Ok(certification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la certificación de fondos federales con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al obtener la certificación de fondos federales");
        }
    }

    [HttpPost("insert-federal-funding-certification")]
    [SwaggerOperation(Summary = "Crea una nueva certificación de fondos federales", Description = "Crea una nueva certificación de fondos federales.")]
    public async Task<ActionResult> Insert([FromBody] DTOFederalFundingCertification certification)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _federalFundingCertificationRepository.InsertFederalFundingCertification(certification);
                if (result)
                    return CreatedAtAction(nameof(GetById), new { id = certification.Id }, certification);

                return BadRequest("No se pudo crear la certificación de fondos federales");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la certificación de fondos federales");
            return StatusCode(500, "Error interno del servidor al crear la certificación de fondos federales");
        }
    }

    [HttpPut("update-federal-funding-certification")]
    [SwaggerOperation(Summary = "Actualiza una certificación de fondos federales existente", Description = "Actualiza los datos de una certificación de fondos federales existente.")]
    public async Task<IActionResult> Update([FromBody] DTOFederalFundingCertification certification)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _federalFundingCertificationRepository.UpdateFederalFundingCertification(certification);
                if (!result)
                    return NotFound($"Certificación de fondos federales con ID {certification.Id} no encontrada");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la certificación de fondos federales con ID {Id}", certification.Id);
            return StatusCode(500, "Error interno del servidor al actualizar la certificación de fondos federales");
        }
    }

    [HttpDelete("delete-federal-funding-certification")]
    [SwaggerOperation(Summary = "Elimina una certificación de fondos federales existente", Description = "Elimina una certificación de fondos federales existente.")]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _federalFundingCertificationRepository.DeleteFederalFundingCertification(id);
                if (!result)
                    return NotFound($"Certificación de fondos federales con ID {id} no encontrada");

                return NoContent();
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la certificación de fondos federales con ID {Id}", id);
            return StatusCode(500, "Error interno del servidor al eliminar la certificación de fondos federales");
        }
    }
}