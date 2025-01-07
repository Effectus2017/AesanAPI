using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Api.Controllers;

[Route("school")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SchoolController(ILogger<SchoolController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<SchoolController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    [HttpGet("get-all-schools-from-db")]
    [SwaggerOperation(Summary = "Obtiene todas las escuelas", Description = "Devuelve una lista paginada de escuelas.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> GetAllSchools([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var schools = await _unitOfWork.SchoolRepository.GetAllSchools(
                    queryParameters.Take,
                    queryParameters.Skip,
                    queryParameters.Name,
                    queryParameters.Alls
                );
                return Ok(schools);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener las escuelas");
            return StatusCode(500, "Error al obtener las escuelas");
        }
    }

    [HttpGet("get-school-by-id")]
    [SwaggerOperation(Summary = "Obtiene una escuela por su ID", Description = "Devuelve una escuela basada en el ID proporcionado.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> GetSchoolById([FromQuery] int id)
    {
        try
        {
            var school = await _unitOfWork.SchoolRepository.GetSchoolById(id);
            if (school == null)
            {
                return NotFound($"Escuela con ID {id} no encontrada");
            }
            return Ok(school);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la escuela");
            return StatusCode(500, "Error al obtener la escuela");
        }
    }

    [HttpPost("insert-school")]
    [SwaggerOperation(Summary = "Inserta una nueva escuela", Description = "Crea una nueva escuela en la base de datos.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> InsertSchool([FromBody] SchoolRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var schoolId = await _unitOfWork.SchoolRepository.InsertSchool(request);
                return Ok(new { id = schoolId });
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar la escuela");
            return StatusCode(500, "Error al insertar la escuela");
        }
    }

    [HttpPut("update-school")]
    [SwaggerOperation(Summary = "Actualiza una escuela existente", Description = "Actualiza los datos de una escuela existente.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> UpdateSchool([FromBody] SchoolRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var result = await _unitOfWork.SchoolRepository.UpdateSchool(request);
                if (result)
                {
                    return Ok(new { message = "Escuela actualizada correctamente" });
                }
                return NotFound($"Escuela con ID {request.Id} no encontrada");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la escuela");
            return StatusCode(500, "Error al actualizar la escuela");
        }
    }

    [HttpDelete("delete-school")]
    [SwaggerOperation(Summary = "Elimina una escuela", Description = "Elimina una escuela de la base de datos.")]
#if !DEBUG
    [Authorize]
#endif
    public async Task<IActionResult> DeleteSchool(int id)
    {
        try
        {
            var result = await _unitOfWork.SchoolRepository.DeleteSchool(id);
            if (result)
            {
                return Ok(new { message = "Escuela eliminada correctamente" });
            }
            return NotFound($"Escuela con ID {id} no encontrada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar la escuela");
            return StatusCode(500, "Error al eliminar la escuela");
        }
    }

}