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
/// lectura, actualización y eliminación de registros de escuelas.
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
    [HttpGet("get-all-schools")]
    [SwaggerOperation(Summary = "Obtiene todas las escuelas", Description = "Devuelve una lista paginada de escuelas.")]
    public async Task<IActionResult> GetAllSchools([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.SchoolRepository.GetAllSchools(
                    queryParameters.Take,
                    queryParameters.Skip,
                    queryParameters.Name,
                    queryParameters.AgencyId,
                    queryParameters.Alls);

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
                    return Ok(new { success = true, message = "Escuela creada exitosamente" });
                }

                return BadRequest("No se pudo crear la escuela");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al insertar escuela: {Message}", ex.Message);
            return BadRequest(ex.Message);
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
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.SchoolRepository.UpdateSchool(request);

                if (result)
                {
                    return Ok(new { success = true, message = "Escuela actualizada exitosamente" });
                }

                return BadRequest("No se pudo actualizar la escuela");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al actualizar escuela: {Message}", ex.Message);
            return BadRequest(ex.Message);
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
    /// <param name="queryParameters">Los parámetros de consulta que incluyen el ID de la escuela</param>
    /// <returns>Resultado de la eliminación</returns>
    [HttpDelete("delete-school")]
    [SwaggerOperation(Summary = "Elimina una escuela", Description = "Elimina una escuela de la base de datos (soft delete).")]
    public async Task<IActionResult> DeleteSchool([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (queryParameters.Id <= 0)
            {
                return BadRequest("ID de escuela inválido");
            }

            var result = await _unitOfWork.SchoolRepository.DeleteSchool(queryParameters.Id);

            if (result)
            {
                return Ok(new { success = true, message = "Escuela eliminada exitosamente" });
            }

            return BadRequest("No se pudo eliminar la escuela");
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Error de validación al eliminar escuela: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la escuela: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Obtiene todas las escuelas de una agencia específica con paginación y filtros
    /// </summary>
    /// <param name="queryParameters">Los parámetros de consulta que incluyen el agencyId, take, skip y name</param>
    /// <returns>Lista de escuelas de la agencia</returns>
    [HttpGet("get-schools-by-agency")]
    [SwaggerOperation(Summary = "Obtiene escuelas por agencia", Description = "Devuelve todas las escuelas de una agencia específica con paginación y búsqueda por nombre.")]
    public async Task<IActionResult> GetSchoolsByAgencyId([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            var result = await _unitOfWork.SchoolRepository.GetSchoolsByAgencyId(
                queryParameters.AgencyId,
                queryParameters.Take,
                queryParameters.Skip,
                queryParameters.Name);

            if (result == null)
            {
                return NotFound("No se encontraron escuelas para esta agencia");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener escuelas por agencia: {Message}", ex.Message);
            return StatusCode(500, ex.Message);
        }
    }
}
