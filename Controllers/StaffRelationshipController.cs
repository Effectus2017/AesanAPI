using Api.Interfaces;
using Api.Models;
using Api.Filters;
using Api.Models.Request;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// Controlador para manejar las relaciones de parentesco entre empleados
/// </summary>
[ApiController]
[Route("staff-relationship")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ValidateModelState]
public class StaffRelationshipController(IStaffRelationshipRepository staffRelationshipRepository) : ControllerBase
{
    private readonly IStaffRelationshipRepository _staffRelationshipRepository = staffRelationshipRepository;

    /// <summary>
    /// Obtiene todas las relaciones de un empleado específico
    /// </summary>
    /// <param name="staffId">ID del empleado</param>
    /// <returns>Lista de relaciones del empleado</returns>
    [HttpGet("get-relationships-by-staff-id")]
    public async Task<IActionResult> GetRelationshipsByStaffId([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID del empleado es requerido");
        }

        var result = await _staffRelationshipRepository.GetRelationshipsByStaffId(queryParameters.Id, queryParameters.IsActive);

        if (result == null)
        {
            return NotFound("No se encontraron relaciones del empleado");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene una relación específica por su ID
    /// </summary>
    /// <param name="id">ID de la relación</param>
    /// <returns>Relación encontrada</returns>
    [HttpGet("get-relationship-by-id")]
    public async Task<IActionResult> GetRelationshipById([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID de la relación es requerido");
        }

        var result = await _staffRelationshipRepository.GetRelationshipById(queryParameters.Id);

        if (result == null)
        {
            return NotFound($"No se encontró la relación con ID {queryParameters.Id}");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene todas las relaciones activas de la agencia
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para paginación y filtros</param>
    /// <returns>Lista de todas las relaciones activas</returns>
    [HttpGet("get-all-active-relationships-from-db")]
    public async Task<IActionResult> GetAllActiveRelationshipsFromDb([FromQuery] QueryParameters queryParameters)
    {
        var result = await _staffRelationshipRepository.GetAllActiveRelationshipsFromDb(
            queryParameters.Take,
            queryParameters.Skip,
            queryParameters.Alls,
            queryParameters.ForDropdown
        );

        if (result == null)
        {
            return NotFound("No se encontraron relaciones activas");
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtiene las relaciones por tipo específico
    /// </summary>
    /// <param name="relationshipTypeId">ID del tipo de parentesco</param>
    /// <returns>Lista de relaciones del tipo especificado</returns>
    [HttpGet("get-relationships-by-type")]
    public async Task<IActionResult> GetRelationshipsByType([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.OptionSelectionId == 0)
        {
            return BadRequest("El ID del tipo de parentesco es requerido");
        }

        var result = await _staffRelationshipRepository.GetRelationshipsByType(queryParameters.OptionSelectionId);

        if (result == null)
        {
            return NotFound("No se encontraron relaciones del tipo especificado");
        }

        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva relación entre empleados
    /// </summary>
    /// <param name="request">Datos de la relación a crear</param>
    /// <returns>ID de la relación creada</returns>
    [HttpPost("insert-staff-relationship")]
    public async Task<IActionResult> CreateRelationship([FromBody] StaffRelationshipRequest request)
    {
        if (request == null)
        {
            return BadRequest("Los datos de la relación son requeridos");
        }

        // Validar que no sea la misma persona
        if (request.StaffId == request.RelatedStaffId)
        {
            return BadRequest("Un empleado no puede tener parentesco consigo mismo");
        }

        // Verificar si ya existe una relación entre estos empleados
        var result = await _staffRelationshipRepository.RelationshipExists(request.StaffId, request.RelatedStaffId);

        if (result)
        {
            return BadRequest("Ya existe una relación activa entre estos empleados");
        }

        // Verificar si el empleado puede tener este tipo de relación
        var canHaveRelationship = await _staffRelationshipRepository.CanHaveRelationshipType(request.StaffId, request.RelationshipTypeId);

        if (!canHaveRelationship)
        {
            return BadRequest("El empleado no puede tener este tipo de relación");
        }

        var relationshipId = await _staffRelationshipRepository.CreateRelationship(request);

        return CreatedAtAction(nameof(GetRelationshipById), new { id = relationshipId }, new { id = relationshipId });
    }

    /// <summary>
    /// Actualiza una relación existente
    /// </summary>
    /// <param name="request">Datos de la relación a actualizar</param>
    /// <returns>Resultado de la actualización</returns>
    [HttpPut("update-staff-relationship")]
    public async Task<IActionResult> UpdateRelationship([FromBody] UpdateStaffRelationshipRequest request)
    {
        if (request == null)
        {
            return BadRequest("Los datos de la relación son requeridos");
        }

        // Verificar si la relación existe
        var existingRelationship = await _staffRelationshipRepository.GetRelationshipById(request.Id);

        if (existingRelationship == null)
        {
            return NotFound($"No se encontró la relación con ID {request.Id}");
        }

        // Verificar si el empleado puede tener este tipo de relación
        // Excluimos la relación actual para permitir ediciones
        var canHaveRelationship = await _staffRelationshipRepository.CanHaveRelationshipType(existingRelationship.Staff!.Id, request.RelationshipTypeId, request.Id);

        if (!canHaveRelationship)
        {
            return BadRequest("El empleado no puede tener este tipo de relación");
        }

        var success = await _staffRelationshipRepository.UpdateRelationship(request);

        if (!success)
        {
            return BadRequest("No se pudo actualizar la relación");
        }

        return Ok(new { message = "Relación actualizada exitosamente" });
    }

    /// <summary>
    /// Desactiva una relación (soft delete)
    /// </summary>
    /// <param name="id">ID de la relación a desactivar</param>
    /// <returns>Resultado de la desactivación</returns>
    [HttpDelete("delete-staff-relationship")]
    public async Task<IActionResult> DeactivateRelationship([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.Id == 0)
        {
            return BadRequest("El ID de la relación es requerido");
        }

        // Verificar si la relación existe
        var existingRelationship = await _staffRelationshipRepository.GetRelationshipById(queryParameters.Id);

        if (existingRelationship == null)
        {
            return NotFound($"No se encontró la relación con ID {queryParameters.Id}");
        }

        var success = await _staffRelationshipRepository.DeleteRelationship(queryParameters.Id);

        if (!success)
        {
            return BadRequest("No se pudo desactivar la relación");
        }

        return Ok(new { message = "Relación desactivada exitosamente" });
    }

    /// <summary>
    /// Verifica si existe una relación activa entre dos empleados
    /// </summary>
    /// <param name="staffId">ID del primer empleado</param>
    /// <param name="relatedStaffId">ID del segundo empleado</param>
    /// <returns>True si existe una relación activa</returns>
    [HttpGet("check-relationship-exists")]
    public async Task<IActionResult> CheckRelationshipExists([FromQuery] QueryParameters queryParameters)
    {
        if (queryParameters.StaffId == 0 || queryParameters.Id == 0)
        {
            return BadRequest("Los IDs de empleados son requeridos");
        }

        var exists = await _staffRelationshipRepository.RelationshipExists(queryParameters.StaffId, queryParameters.Id);

        return Ok(new { exists });
    }
}
