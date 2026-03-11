using Api.Interfaces;
using Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las certificaciones de fondos federales.
/// Proporciona endpoints para la gestión completa de certificaciones, incluyendo creación,
/// lectura, actualización y eliminación de registros de certificaciones federales.
/// </summary>
[ApiController]
[Route("federal-funding-certification")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class FederalFundingCertificationController(IFederalFundingCertificationRepository federalFundingCertificationRepository) : ControllerBase
{
    private readonly IFederalFundingCertificationRepository _federalFundingCertificationRepository = federalFundingCertificationRepository;

    /// <summary>
    /// Obtiene una certificación de fondos federales por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Certificación de fondos federales</returns>
    [HttpGet("get-federal-funding-certification-by-id")]
    [SwaggerOperation(Summary = "Obtiene una certificación de fondos federales por su ID", Description = "Devuelve una certificación de fondos federales basada en el ID proporcionado.")]
    public async Task<ActionResult> GetById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID de la certificación de fondos federales es requerido");
        }

        var result = await _federalFundingCertificationRepository.GetFederalFundingCertificationById(queryParameters.Id);

        if (result == null)
        {
            return NotFound($"Certificación de fondos federales con ID {queryParameters.Id} no encontrada");
        }

        return Ok(result);
    }


    /// <summary>
    /// Obtiene todas las certificaciones de fondos federales
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Certificaciones de fondos federales</returns>
    [HttpGet("get-all-federal-funding-certifications-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las certificaciones de fondos federales", Description = "Devuelve una lista de certificaciones de fondos federales.")]
    public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
    {
        var result = await _federalFundingCertificationRepository.GetAllFederalFundingCertifications(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);

        if (result == null)
        {
            return NotFound("No se encontraron certificaciones de fondos federales");
        }

        return Ok(result);
    }


    /// <summary>
    /// Crea una nueva certificación de fondos federales
    /// </summary>
    /// <param name="certification">Certificación de fondos federales</param>
    /// <returns>Certificación de fondos federales creada</returns>
    [HttpPost("insert-federal-funding-certification")]
    [SwaggerOperation(Summary = "Crea una nueva certificación de fondos federales", Description = "Crea una nueva certificación de fondos federales.")]
    public async Task<ActionResult> Insert([FromBody] DTOFederalFundingCertification request)
    {
        var result = await _federalFundingCertificationRepository.InsertFederalFundingCertification(request);

        if (result)
        {
            return Ok(result);
        }

        return BadRequest("No se pudo crear la certificación de fondos federales");
    }

    /// <summary>
    /// Actualiza una certificación de fondos federales existente
    /// </summary>
    /// <param name="certification">Certificación de fondos federales</param>
    /// <returns>Certificación de fondos federales actualizada</returns>
    [HttpPut("update-federal-funding-certification")]
    [SwaggerOperation(Summary = "Actualiza una certificación de fondos federales existente", Description = "Actualiza los datos de una certificación de fondos federales existente.")]
    public async Task<IActionResult> Update([FromBody] DTOFederalFundingCertification request)
    {
        var result = await _federalFundingCertificationRepository.UpdateFederalFundingCertification(request);
        if (!result)
        {
            return NotFound($"Certificación de fondos federales con ID {request.Id} no encontrada");
        }

        return NoContent();
    }

    /// <summary>
    /// Elimina una certificación de fondos federales existente
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>Certificación de fondos federales eliminada</returns>
    [HttpDelete("delete-federal-funding-certification")]
    [SwaggerOperation(Summary = "Elimina una certificación de fondos federales existente", Description = "Elimina una certificación de fondos federales existente.")]
    public async Task<IActionResult> Delete([FromQuery] QueryParameters queryParameters)
    {
        var result = await _federalFundingCertificationRepository.DeleteFederalFundingCertification(queryParameters.Id);
        if (!result)
        {
            return NotFound($"Certificación de fondos federales con ID {queryParameters.Id} no encontrada");
        }

        return NoContent();
    }
}
