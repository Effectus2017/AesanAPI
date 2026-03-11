using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Filters;
using Api.Models;
using Api.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    /// <summary>
    /// Controlador que maneja todas las operaciones relacionadas con los ingresos de miembros del hogar.
    /// </summary>
    [ApiController]
    [Route("household-member-income")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ValidateModelState]
    public class HouseholdMemberIncomeController(IHouseholdMemberIncomeRepository repository) : ControllerBase
    {
        private readonly IHouseholdMemberIncomeRepository _repository = repository;

        /// <summary>
        /// Obtiene todos los ingresos de un miembro del hogar
        /// </summary>
        [HttpGet("get-all-household-member-incomes-from-db")]
        [SwaggerOperation(Summary = "Obtiene todos los ingresos de un miembro del hogar", Description = "Devuelve una lista de ingresos de un miembro del hogar.")]
        public async Task<ActionResult> GetAll([FromQuery] QueryParameters queryParameters)
        {
            var result = await _repository.GetAllAsync(queryParameters.Take, queryParameters.Skip, queryParameters.Name, queryParameters.Alls);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene un ingreso de miembro del hogar por su Id
        /// </summary>
        [HttpGet("get-household-member-income-by-id")]
        [SwaggerOperation(Summary = "Obtiene un ingreso de miembro del hogar por su Id", Description = "Devuelve un ingreso de miembro del hogar basado en el ID proporcionado.")]
        public async Task<ActionResult> GetById([FromQuery] int id)
        {
            var result = await _repository.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound($"Ingreso con ID {id} no encontrado");
            }
            return Ok(result);
        }

        /// <summary>
        /// Inserta un nuevo ingreso de miembro del hogar
        /// </summary>
        [HttpPost("insert-household-member-income")]
        [SwaggerOperation(Summary = "Crea un nuevo ingreso de miembro del hogar", Description = "Crea un nuevo ingreso de miembro del hogar.")]
        public async Task<ActionResult> Insert([FromBody] DTOHouseholdMemberIncome entity)
        {
            var id = await _repository.InsertAsync(entity);
            return CreatedAtAction(nameof(GetById), new { id }, entity);
        }

        /// <summary>
        /// Actualiza un ingreso de miembro del hogar existente
        /// </summary>
        [HttpPut("update-household-member-income")]
        [SwaggerOperation(Summary = "Actualiza un ingreso de miembro del hogar existente", Description = "Actualiza los datos de un ingreso de miembro del hogar existente.")]
        public async Task<ActionResult> Update([FromBody] DTOHouseholdMemberIncome entity)
        {
            var updated = await _repository.UpdateAsync(entity);
            if (!updated)
            {
                return NotFound($"Ingreso con ID {entity.Id} no encontrado");
            }
            return NoContent();
        }

        /// <summary>
        /// Elimina un ingreso de miembro del hogar existente
        /// </summary>
        [HttpDelete("delete-household-member-income")]
        [SwaggerOperation(Summary = "Elimina un ingreso de miembro del hogar existente", Description = "Elimina un ingreso de miembro del hogar existente.")]
        public async Task<ActionResult> Delete([FromQuery] int id)
        {
            var deleted = await _repository.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound($"Ingreso con ID {id} no encontrado");
            }
            return NoContent();
        }
    }
}
