using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

/// <summary>
/// Controlador que maneja todas las operaciones relacionadas con las escuelas.
/// Proporciona endpoints para la gestión completa de escuelas, incluyendo creación,
/// lectura, actualización y eliminación de registros escolares.
/// </summary>
[Route("school")]
[ApiController]
public class SchoolController(ILogger<SchoolController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<SchoolController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene una escuela por su ID
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta para la obtención de la escuela</param>
    /// <returns>La escuela encontrada</returns>
    [HttpGet("get-school-by-id")]
    [SwaggerOperation(Summary = "Obtiene una escuela por su ID", Description = "Devuelve una escuela basada en el ID proporcionado.")]
    public async Task<IActionResult> GetSchoolById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            var result = await _unitOfWork.SchoolRepository.GetSchoolById(queryParameters.Id);

            if (result == null)
            {
                return NotFound($"Escuela con ID {queryParameters.Id} no encontrada");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la escuela: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todas las escuelas
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta para la paginación y filtrado</param>
    /// <returns>Una lista paginada de escuelas</returns>
    [HttpGet("get-all-schools-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las escuelas", Description = "Devuelve una lista paginada de escuelas.")]
    public async Task<IActionResult> GetAllSchoolsFromDB([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.SchoolRepository.GetAllSchoolsFromDB(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.CityId, queryParameters.RegionId, queryParameters.AgencyId, queryParameters.Alls, queryParameters.IsList);

                if (result == null)
                {
                    return NotFound("No se encontraron escuelas");
                }

                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las escuelas: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Inserta una nueva escuela
    /// </summary>
    /// <param name="request">La escuela a insertar</param>
    /// <returns>La escuela insertada</returns>
    [HttpPost("insert-school")]
    [SwaggerOperation(Summary = "Inserta una nueva escuela", Description = "Crea una nueva escuela en la base de datos.")]
    public async Task<IActionResult> InsertSchool([FromBody] SchoolRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.SchoolRepository.InsertSchool(request);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("Error al insertar la escuela");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la escuela: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza una escuela existente
    /// </summary>
    /// <param name="request">La escuela a actualizar</param>
    /// <returns>La escuela actualizada</returns>
    [HttpPut("update-school")]
    [SwaggerOperation(Summary = "Actualiza una escuela existente", Description = "Actualiza los datos de una escuela existente.")]
    public async Task<IActionResult> UpdateSchool([FromBody] SchoolRequest request)
    {
        try
        {
            // Validación condicional: requerir justificación solo cuando IsActive es false
            if (request.IsActive == false && string.IsNullOrWhiteSpace(request.InactiveJustification))
            {
                ModelState.AddModelError("InactiveJustification", "Se requiere justificación para inactivar la escuela");
            }

            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.SchoolRepository.UpdateSchool(request);

                if (result)
                {
                    return Ok(result);
                }

                return NotFound($"Escuela con ID {request.Id} no encontrada");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la escuela: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Elimina una escuela
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta para la eliminación</param>
    /// <returns>La escuela eliminada</returns>
    [HttpDelete("delete-school")]
    [SwaggerOperation(Summary = "Elimina una escuela", Description = "Elimina una escuela de la base de datos.")]
    public async Task<IActionResult> DeleteSchool([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            var result = await _unitOfWork.SchoolRepository.DeleteSchool(queryParameters.Id);

            if (result)
            {
                return Ok(result);
            }

            return NotFound($"Escuela con ID {queryParameters.Id} no encontrada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la escuela: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Verifica si existe una escuela principal en la base de datos
    /// </summary>
    /// <returns>True si existe una escuela principal, false en caso contrario</returns>
    [HttpGet("has-main-school")]
    [SwaggerOperation(Summary = "Verifica si existe una escuela principal", Description = "Devuelve true si existe una escuela principal, false en caso contrario.")]
    public async Task<ActionResult<bool>> HasMainSchool()
    {
        try
        {
            var result = await _unitOfWork.SchoolRepository.HasMainSchool();

            if (result == null)
            {
                return NotFound("No se encontró ninguna escuela principal");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar si existe una escuela principal: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Actualiza el estado activo/inactivo de una escuela
    /// </summary>
    /// <param name="schoolId">ID de la escuela</param>
    /// <param name="isActive">Estado activo (true) o inactivo (false)</param>
    /// <param name="inactiveJustification">Justificación cuando se inactiva (requerida si isActive es false)</param>
    /// <returns>True si se actualizó correctamente</returns>
    [HttpPut("update-active-status")]
    [SwaggerOperation(Summary = "Actualiza el estado activo/inactivo de una escuela", Description = "Permite activar o inactivar una escuela. Requiere justificación al inactivar.")]
    public async Task<IActionResult> UpdateSchoolActiveStatus([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (queryParameters.IsActive == false && string.IsNullOrWhiteSpace(queryParameters.InactiveJustification))
            {
                return BadRequest("Se requiere justificación para inactivar la escuela");
            }

            var result = await _unitOfWork.SchoolRepository.UpdateSchoolActiveStatus(queryParameters.SchoolId.Value, queryParameters.IsActive, queryParameters.InactiveJustification);

            if (result)
            {
                return Ok(result);
            }

            return NotFound($"Escuela con ID {queryParameters.SchoolId} no encontrada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el estado activo de la escuela {SchoolId}: {Message}", queryParameters.SchoolId, ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}