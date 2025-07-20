using Api.Interfaces;
using Api.Models.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("employee")]
[Authorize]
public class EmployeeController : Controller
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<EmployeeController> _logger;

    public EmployeeController(IEmployeeRepository employeeRepository, ILogger<EmployeeController> logger)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene todos los empleados con paginación y filtros
    /// </summary>
    /// <param name="take">Número de registros a tomar</param>
    /// <param name="skip">Número de registros a saltar</param>
    /// <param name="name">Nombre para filtrar</param>
    /// <param name="alls">Si se deben obtener todos</param>
    /// <param name="isList">Si es para lista simple</param>
    /// <returns>Lista de empleados</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int take = 15,
        [FromQuery] int skip = 0,
        [FromQuery] string name = "",
        [FromQuery] bool alls = false,
        [FromQuery] bool isList = false)
    {
        try
        {
            var result = await _employeeRepository.GetAllEmployeesFromDb(take, skip, name, alls, isList);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los empleados");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Obtiene un empleado por su ID
    /// </summary>
    /// <param name="id">ID del empleado</param>
    /// <returns>El empleado</returns>
    [HttpGet("by-id")]
    public async Task<IActionResult> GetById([FromQuery] int id)
    {
        try
        {
            var employee = await _employeeRepository.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound(new { error = "Empleado no encontrado" });
            }
            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empleado con ID {Id}", id);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Crea un nuevo empleado
    /// </summary>
    /// <param name="model">Datos del empleado a crear</param>
    /// <returns>El empleado creado</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EmployeeRequest model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _employeeRepository.InsertEmployee(model);
            if (success)
            {
                return Ok(new { message = "Empleado creado exitosamente" });
            }

            return BadRequest(new { error = "Error al crear el empleado" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear empleado");
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Actualiza un empleado existente
    /// </summary>
    /// <param name="model">Datos del empleado a actualizar</param>
    /// <returns>Resultado de la actualización</returns>
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] EmployeeRequest model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var success = await _employeeRepository.UpdateEmployee(model);
            if (!success)
            {
                return NotFound(new { error = "Empleado no encontrado" });
            }

            return Ok(new { message = "Empleado actualizado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar empleado con ID {Id}", model.Id);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Elimina un empleado (baja lógica)
    /// </summary>
    /// <param name="id">ID del empleado a eliminar</param>
    /// <returns>Resultado de la eliminación</returns>
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] int id)
    {
        try
        {
            var success = await _employeeRepository.DeleteEmployee(id);
            if (!success)
            {
                return NotFound(new { error = "Empleado no encontrado" });
            }

            return Ok(new { message = "Empleado eliminado exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar empleado con ID {Id}", id);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }

    /// <summary>
    /// Convierte un empleado en usuario del sistema
    /// </summary>
    /// <param name="employeeId">ID del empleado</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>Resultado de la conversión</returns>
    [HttpPost("convert-to-user")]
    public async Task<IActionResult> ConvertToUser([FromQuery] int employeeId, [FromQuery] string userId)
    {
        try
        {
            var success = await _employeeRepository.ConvertToUser(employeeId, userId);
            if (!success)
            {
                return NotFound(new { error = "Empleado no encontrado" });
            }

            return Ok(new { message = "Empleado convertido a usuario exitosamente" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al convertir empleado {EmployeeId} a usuario {UserId}", employeeId, userId);
            return StatusCode(500, new { error = "Error interno del servidor" });
        }
    }
}