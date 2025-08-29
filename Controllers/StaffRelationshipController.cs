using Api.Interfaces;
using Api.Models;
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
public class StaffRelationshipController(IStaffRelationshipRepository staffRelationshipRepository, ILogger<StaffRelationshipController> logger) : ControllerBase
{
    private readonly IStaffRelationshipRepository _staffRelationshipRepository = staffRelationshipRepository;
    private readonly ILogger<StaffRelationshipController> _logger = logger;

    /// <summary>
    /// Obtiene todas las relaciones de un empleado específico
    /// </summary>
    /// <param name="staffId">ID del empleado</param>
    /// <returns>Lista de relaciones del empleado</returns>
    [HttpGet("get-relationships-by-staff-id")]
    public async Task<IActionResult> GetRelationshipsByStaffId([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID del empleado es requerido");
                }

                _logger.LogInformation("Obteniendo relaciones del empleado con ID: {Id}", queryParameters.Id);

                var relationships = await _staffRelationshipRepository.GetRelationshipsByStaffId(queryParameters.Id, queryParameters.IsActive);
                return Ok(relationships);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las relaciones del empleado {StaffId}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener las relaciones del empleado");
        }
    }

    /// <summary>
    /// Obtiene una relación específica por su ID
    /// </summary>
    /// <param name="id">ID de la relación</param>
    /// <returns>Relación encontrada</returns>
    [HttpGet("get-relationship-by-id")]
    public async Task<IActionResult> GetRelationshipById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID de la relación es requerido");
                }

                _logger.LogInformation("Obteniendo relación con ID: {Id}", queryParameters.Id);

                var relationship = await _staffRelationshipRepository.GetRelationshipById(queryParameters.Id);
                if (relationship == null)
                {
                    return NotFound($"No se encontró la relación con ID {queryParameters.Id}");
                }

                return Ok(relationship);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la relación {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener la relación");
        }
    }

    /// <summary>
    /// Obtiene todas las relaciones activas de la agencia
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta para paginación y filtros</param>
    /// <returns>Lista de todas las relaciones activas</returns>
    [HttpGet("get-all-active-relationships-from-db")]
    public async Task<IActionResult> GetAllActiveRelationshipsFromDb([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo todas las relaciones activas");

                var relationships = await _staffRelationshipRepository.GetAllActiveRelationshipsFromDb(
                    queryParameters.Take,
                    queryParameters.Skip,
                    queryParameters.Alls,
                    queryParameters.IsList
                );
                return Ok(relationships);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las relaciones activas");
            return StatusCode(500, "Error interno del servidor al obtener todas las relaciones activas");
        }
    }

    /// <summary>
    /// Obtiene las relaciones por tipo específico
    /// </summary>
    /// <param name="relationshipTypeId">ID del tipo de parentesco</param>
    /// <returns>Lista de relaciones del tipo especificado</returns>
    [HttpGet("get-relationships-by-type")]
    public async Task<IActionResult> GetRelationshipsByType([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.OptionSelectionId == 0)
                {
                    return BadRequest("El ID del tipo de parentesco es requerido");
                }

                _logger.LogInformation("Obteniendo relaciones por tipo: {RelationshipTypeId}", queryParameters.OptionSelectionId);

                var relationships = await _staffRelationshipRepository.GetRelationshipsByType(queryParameters.OptionSelectionId);
                return Ok(relationships);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las relaciones por tipo {RelationshipTypeId}", queryParameters.OptionSelectionId);
            return StatusCode(500, "Error interno del servidor al obtener las relaciones por tipo");
        }
    }

    /// <summary>
    /// Crea una nueva relación entre empleados
    /// </summary>
    /// <param name="request">Datos de la relación a crear</param>
    /// <returns>ID de la relación creada</returns>
    [HttpPost("insert-staff-relationship")]
    public async Task<IActionResult> CreateRelationship([FromBody] StaffRelationshipRequest request)
    {
        try
        {
            if (ModelState.IsValid)
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

                _logger.LogInformation("Creando relación entre empleados {StaffId} y {RelatedStaffId}",
                    request.StaffId, request.RelatedStaffId);

                // Verificar si ya existe una relación entre estos empleados
                var relationshipExists = await _staffRelationshipRepository.RelationshipExists(request.StaffId, request.RelatedStaffId);

                if (relationshipExists)
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

                _logger.LogInformation("Relación creada exitosamente con ID {RelationshipId}", relationshipId);

                return CreatedAtAction(nameof(GetRelationshipById), new { id = relationshipId }, new { id = relationshipId });
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear la relación entre empleados {StaffId} y {RelatedStaffId}",
                request.StaffId, request.RelatedStaffId);
            return StatusCode(500, "Error interno del servidor al crear la relación");
        }
    }

    /// <summary>
    /// Actualiza una relación existente
    /// </summary>
    /// <param name="request">Datos de la relación a actualizar</param>
    /// <returns>Resultado de la actualización</returns>
    [HttpPut("update-staff-relationship")]
    public async Task<IActionResult> UpdateRelationship([FromBody] UpdateStaffRelationshipRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("Los datos de la relación son requeridos");
                }

                _logger.LogInformation("Actualizando relación con ID: {Id}", request.Id);

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

                _logger.LogInformation("Relación {RelationshipId} actualizada exitosamente", request.Id);

                return Ok(new { message = "Relación actualizada exitosamente" });
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la relación {Id}", request.Id);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Desactiva una relación (soft delete)
    /// </summary>
    /// <param name="id">ID de la relación a desactivar</param>
    /// <returns>Resultado de la desactivación</returns>
    [HttpDelete("delete-staff-relationship")]
    public async Task<IActionResult> DeactivateRelationship([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.Id == 0)
                {
                    return BadRequest("El ID de la relación es requerido");
                }

                _logger.LogInformation("Desactivando relación con ID: {Id}", queryParameters.Id);

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

                _logger.LogInformation("Relación {RelationshipId} desactivada exitosamente", queryParameters.Id);

                return Ok(new { message = "Relación desactivada exitosamente" });
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al desactivar la relación {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al desactivar la relación");
        }
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
        try
        {
            if (ModelState.IsValid)
            {
                if (queryParameters.StaffId == 0 || queryParameters.Id == 0)
                {
                    return BadRequest("Los IDs de empleados son requeridos");
                }

                _logger.LogInformation("Verificando si existe relación entre empleados {StaffId} y {RelatedStaffId}",
                    queryParameters.StaffId, queryParameters.Id);

                var exists = await _staffRelationshipRepository.RelationshipExists(queryParameters.StaffId, queryParameters.Id);
                return Ok(new { exists });
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si existe relación entre empleados {StaffId} y {RelatedStaffId}",
                queryParameters.StaffId, queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al verificar la existencia de la relación");
        }
    }
}
