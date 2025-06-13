using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Api.Interfaces;
using Api.Models.Request;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;

namespace Api.Controllers
{
    /// <summary>
    /// Controlador que maneja todas las operaciones relacionadas con los hogares y sus miembros.
    /// Proporciona endpoints para la gestión completa de hogares, incluyendo creación,
    /// lectura, actualización y eliminación de registros de hogares.
    /// </summary>
    [Route("household-member")]
    [ApiController]

    public class HouseholdMemberController(ILogger<HouseholdMemberController> logger, IUnitOfWork unitOfWork) : ControllerBase
    {

        private readonly ILogger<HouseholdMemberController> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        /// <summary>
        /// Obtiene un miembro del hogar por su Id
        /// </summary>
        /// <param name="id">El ID del miembro del hogar</param>
        /// <returns>El miembro del hogar</returns>
        [HttpGet("get-household-member-by-id")]
        [SwaggerOperation(Summary = "Obtiene un miembro del hogar por su Id", Description = "Devuelve un miembro del hogar basado en el ID proporcionado.")]
        public async Task<IActionResult> GetHouseholdMemberById([FromQuery] int id)
        {
            try
            {
                var result = await _unitOfWork.HouseholdMemberRepository.GetHouseholdMemberById(id);
                if (result == null) return NotFound($"Miembro con ID {id} no encontrado");
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el miembro con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor al obtener el miembro");
            }
        }

        /// <summary>
        /// Obtiene una lista paginada de miembros del hogar
        /// </summary>
        /// <param name="queryParameters">Los parámetros de consulta</param>
        /// <returns>Una lista de miembros del hogar</returns>
        [HttpGet("get-household-members")]
        [SwaggerOperation(Summary = "Obtiene miembros del hogar", Description = "Devuelve una lista de miembros del hogar.")]
        public async Task<IActionResult> GetHouseholdMembers([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var result = await _unitOfWork.HouseholdMemberRepository.GetAllHouseholdMembers(queryParameters.Take, queryParameters.Skip, queryParameters.Alls);
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los miembros del hogar con HouseholdId {Id}", queryParameters.Id);
                return StatusCode(500, "Error interno del servidor al obtener los miembros");
            }
        }

        /// <summary>
        /// Inserta un nuevo miembro del hogar
        /// </summary>
        /// <param name="request">Los datos del miembro del hogar</param>
        /// <returns>El miembro del hogar creado</returns>
        [HttpPost("insert-household-member")]
        [SwaggerOperation(Summary = "Crea un nuevo miembro del hogar", Description = "Crea un nuevo miembro del hogar.")]
        public async Task<IActionResult> InsertHouseholdMember([FromBody] HouseholdMemberRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _unitOfWork.HouseholdMemberRepository.InsertHouseholdMember(request);

                    if (result)
                    {
                        return Ok(result);
                    }

                    return BadRequest("Error al crear el miembro del hogar");
                }
                return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al crear el miembro del hogar");
                return StatusCode(500, "Error interno del servidor al crear el miembro");
            }
        }

        /// <summary>
        /// Actualiza un miembro del hogar existente
        /// </summary>
        /// <param name="request">Los datos del miembro del hogar</param>
        /// <returns>El miembro del hogar actualizado</returns>
        [HttpPut("update-household-member")]
        [SwaggerOperation(Summary = "Actualiza un miembro del hogar existente", Description = "Actualiza los datos de un miembro del hogar existente.")]
        public async Task<IActionResult> UpdateHouseholdMember([FromBody] HouseholdMemberRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _unitOfWork.HouseholdMemberRepository.UpdateHouseholdMember(request);

                    if (result)
                    {
                        return Ok(result);
                    }

                    return BadRequest("Error al actualizar el miembro del hogar");
                }
                return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el miembro con ID {Id}", request.Id);
                return StatusCode(500, "Error interno del servidor al actualizar el miembro");
            }
        }

        /// <summary>
        /// Elimina un miembro del hogar existente
        /// </summary>
        /// <param name="id">El ID del miembro del hogar</param>
        /// <returns>El miembro del hogar eliminado</returns>
        [HttpDelete("delete-household-member")]
        [SwaggerOperation(Summary = "Elimina un miembro del hogar existente", Description = "Elimina un miembro del hogar existente.")]
        public async Task<IActionResult> DeleteHouseholdMember([FromQuery] int id)
        {
            try
            {
                var result = await _unitOfWork.HouseholdMemberRepository.DeleteHouseholdMember(id);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("Error al eliminar el miembro del hogar");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el miembro con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor al eliminar el miembro");
            }
        }
    }
}