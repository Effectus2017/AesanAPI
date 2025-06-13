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
    [Route("household")]
    [ApiController]

    public class HouseholdController(ILogger<HouseholdController> logger, IUnitOfWork unitOfWork) : ControllerBase
    {

        private readonly ILogger<HouseholdController> _logger = logger;
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

        /// <summary>
        /// Obtiene un hogar por su Id
        /// </summary>
        /// <param name="id">El ID del hogar</param>
        /// <returns>El hogar</returns>
        [HttpGet("get-household-by-id")]
        [SwaggerOperation(Summary = "Obtiene un hogar por su Id", Description = "Devuelve un hogar basado en el ID proporcionado.")]
        public async Task<IActionResult> GetHouseholdById([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var result = await _unitOfWork.HouseholdRepository.GetHouseholdById(queryParameters.Id);

                if (result == null)
                {
                    return NotFound($"Hogar con ID {queryParameters.Id} no encontrado");
                }

                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el hogar con ID {Id}", queryParameters.Id);
                return StatusCode(500, "Error interno del servidor al obtener el hogar");
            }
        }

        /// <summary>
        /// Obtiene una lista paginada de hogares
        /// </summary>
        /// <param name="queryParameters">Los parámetros de consulta</param>
        /// <returns>Una lista de hogares</returns>
        [HttpGet("get-all-households-from-db")]
        [SwaggerOperation(Summary = "Obtiene todos los hogares", Description = "Devuelve una lista de hogares.")]
        public async Task<IActionResult> GetHouseholds([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _unitOfWork.HouseholdRepository.GetAllHouseholds(queryParameters.Take, queryParameters.Skip, queryParameters.Alls);
                    return Ok(result);
                }
                return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los hogares");
                return StatusCode(500, "Error interno del servidor al obtener los hogares");
            }
        }

        /// <summary>
        /// Inserta un nuevo hogar
        /// </summary>
        /// <param name="request">Los datos del hogar</param>
        /// <returns>El hogar creado</returns>
        [HttpPost("insert-household")]
        [SwaggerOperation(Summary = "Crea un nuevo hogar", Description = "Crea un nuevo hogar.")]
        public async Task<IActionResult> InsertHousehold([FromBody] HouseholdRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _unitOfWork.HouseholdRepository.InsertHousehold(request);

                    if (result)
                    {
                        return Ok(result);
                    }

                    return BadRequest("Error al crear el hogar");
                }
                return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al crear el hogar");
                return StatusCode(500, "Error interno del servidor al crear el hogar");
            }
        }

        /// <summary>
        /// Actualiza un hogar existente
        /// </summary>
        /// <param name="request">Los datos del hogar</param>
        /// <returns>El hogar actualizado</returns>
        [HttpPut("update-household")]
        [SwaggerOperation(Summary = "Actualiza un hogar existente", Description = "Actualiza los datos de un hogar existente.")]
        public async Task<IActionResult> UpdateHousehold([FromBody] HouseholdRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _unitOfWork.HouseholdRepository.UpdateHousehold(request);

                    if (result)
                    {
                        return Ok(result);
                    }

                    return BadRequest("Error al actualizar el hogar");
                }
                return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el hogar con ID {Id}", request.Id);
                return StatusCode(500, "Error interno del servidor al actualizar el hogar");
            }
        }

        /// <summary>
        /// Elimina un hogar existente
        /// </summary>
        /// <param name="id">El ID del hogar</param>
        /// <returns>El hogar eliminado</returns>
        [HttpDelete("delete-household")]
        [SwaggerOperation(Summary = "Elimina un hogar existente", Description = "Elimina un hogar existente.")]
        public async Task<IActionResult> DeleteHousehold([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var result = await _unitOfWork.HouseholdRepository.DeleteHousehold(queryParameters.Id);

                if (result)
                {
                    return Ok(result);
                }

                return BadRequest("Error al eliminar el hogar");
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el hogar con ID {Id}", queryParameters.Id);
                return StatusCode(500, "Error interno del servidor al eliminar el hogar");
            }
        }

        // HouseholdMember
        /// <summary>
        /// Obtiene un miembro del hogar por su Id
        /// </summary>
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
        [HttpPost("insert-household-member")]
        [SwaggerOperation(Summary = "Crea un nuevo miembro del hogar", Description = "Crea un nuevo miembro del hogar.")]
        public async Task<IActionResult> InsertHouseholdMember([FromBody] HouseholdMemberRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var id = await _unitOfWork.HouseholdMemberRepository.InsertHouseholdMember(request);
                    return CreatedAtAction(nameof(GetHouseholdMemberById), new { id }, request);
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
        [HttpPut("update-household-member")]
        [SwaggerOperation(Summary = "Actualiza un miembro del hogar existente", Description = "Actualiza los datos de un miembro del hogar existente.")]
        public async Task<IActionResult> UpdateHouseholdMember([FromBody] HouseholdMemberRequest request)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ok = await _unitOfWork.HouseholdMemberRepository.UpdateHouseholdMember(request);
                    if (!ok) return NotFound($"Miembro con ID {request.Id} no encontrado");
                    return NoContent();
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
        [HttpDelete("delete-household-member")]
        [SwaggerOperation(Summary = "Elimina un miembro del hogar existente", Description = "Elimina un miembro del hogar existente.")]
        public async Task<IActionResult> DeleteHouseholdMember([FromQuery] int id)
        {
            try
            {
                var ok = await _unitOfWork.HouseholdMemberRepository.DeleteHouseholdMember(id);
                if (!ok) return NotFound($"Miembro con ID {id} no encontrado");
                return NoContent();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el miembro con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor al eliminar el miembro");
            }
        }
    }
}