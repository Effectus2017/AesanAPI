using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Models.Tables;
using Api.Repositories;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;

namespace Api.Controllers
{
    [ApiController]
    [Route("household-member-income")]
    /// <summary>
    /// Controlador que maneja todas las operaciones relacionadas con los ingresos de miembros del hogar.
    /// </summary>
    public class HouseholdMemberIncomeController(IHouseholdMemberIncomeRepository repository, ILogger<HouseholdMemberIncomeController> logger) : ControllerBase
    {
        private readonly IHouseholdMemberIncomeRepository _repository = repository;
        private readonly ILogger<HouseholdMemberIncomeController> _logger = logger;

        /// <summary>
        /// Obtiene todos los ingresos de un miembro del hogar
        /// </summary>
        [HttpGet("get-all-household-member-incomes-from-db")]
        [SwaggerOperation(Summary = "Obtiene todos los ingresos de un miembro del hogar", Description = "Devuelve una lista de ingresos de un miembro del hogar.")]
        public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _repository.GetAllAsync(queryParameters.Take, queryParameters.Skip);
                    return Ok(result);
                }
                return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los ingresos del miembro con ID {Id}", queryParameters.MemberId);
                return StatusCode(500, "Error interno del servidor al obtener los ingresos");
            }
        }

        /// <summary>
        /// Obtiene un ingreso de miembro del hogar por su Id
        /// </summary>
        [HttpGet("get-household-member-income-by-id")]
        [SwaggerOperation(Summary = "Obtiene un ingreso de miembro del hogar por su Id", Description = "Devuelve un ingreso de miembro del hogar basado en el ID proporcionado.")]
        public async Task<ActionResult> GetById([FromQuery] int id)
        {
            try
            {
                var result = await _repository.GetByIdAsync(id);
                if (result == null) return NotFound($"Ingreso con ID {id} no encontrado");
                return Ok(result);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el ingreso con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor al obtener el ingreso");
            }
        }

        /// <summary>
        /// Inserta un nuevo ingreso de miembro del hogar
        /// </summary>
        [HttpPost("insert-household-member-income")]
        [SwaggerOperation(Summary = "Crea un nuevo ingreso de miembro del hogar", Description = "Crea un nuevo ingreso de miembro del hogar.")]
        public async Task<ActionResult> Insert([FromBody] HouseholdMemberIncome entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var id = await _repository.InsertAsync(entity);
                    return CreatedAtAction(nameof(GetById), new { id }, entity);
                }
                return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al crear el ingreso de miembro del hogar");
                return StatusCode(500, "Error interno del servidor al crear el ingreso");
            }
        }

        /// <summary>
        /// Actualiza un ingreso de miembro del hogar existente
        /// </summary>
        [HttpPut("update-household-member-income")]
        [SwaggerOperation(Summary = "Actualiza un ingreso de miembro del hogar existente", Description = "Actualiza los datos de un ingreso de miembro del hogar existente.")]
        public async Task<ActionResult> Update([FromBody] HouseholdMemberIncome entity)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var updated = await _repository.UpdateAsync(entity);
                    if (!updated) return NotFound($"Ingreso con ID {entity.Id} no encontrado");
                    return NoContent();
                }
                return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el ingreso con ID {Id}", entity.Id);
                return StatusCode(500, "Error interno del servidor al actualizar el ingreso");
            }
        }

        /// <summary>
        /// Elimina un ingreso de miembro del hogar existente
        /// </summary>
        [HttpDelete("delete-household-member-income")]
        [SwaggerOperation(Summary = "Elimina un ingreso de miembro del hogar existente", Description = "Elimina un ingreso de miembro del hogar existente.")]
        public async Task<ActionResult> Delete([FromQuery] int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);
                if (!deleted) return NotFound($"Ingreso con ID {id} no encontrado");
                return NoContent();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el ingreso con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor al eliminar el ingreso");
            }
        }
    }
}