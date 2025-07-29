using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Api.Interfaces;
using Api.Models.Request;

namespace Api.Controllers;

/// <summary>
/// Controlador para gestionar empleados
/// Proporciona endpoints para la gestión completa de empleados, incluyendo creación,
/// lectura, actualización y eliminación de empleados.
/// </summary>
[Route("employee")]
[ApiController]
// [Authorize]
public class EmployeeController(ILogger<EmployeeController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<EmployeeController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <summary>
    /// Obtiene un empleado por su ID
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El empleado si se encuentra, NotFound si no existe, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-employee-by-id")]
    [SwaggerOperation(Summary = "Obtiene un empleado por su ID", Description = "Devuelve un empleado basado en el ID proporcionado.")]
    public async Task<IActionResult> GetEmployeeById([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo empleado por ID: {Id}", queryParameters.Id);

                var employee = await _unitOfWork.EmployeeRepository.GetEmployeeById(queryParameters.Id);

                if (employee == null)
                {
                    return NotFound($"Empleado con ID {queryParameters.Id} no encontrado");
                }

                return Ok(employee);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener el empleado con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error interno del servidor al obtener el empleado");
        }
    }

    /// <summary>
    /// Obtiene todos los empleados de la base de datos
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta</param>
    /// <returns>La lista de empleados si se encuentran, NotFound si no se encuentran, o Error interno del servidor en caso de error</returns>
    [HttpGet("get-all-employees-from-db")]
    [SwaggerOperation(Summary = "Obtiene todos los empleados de la base de datos", Description = "Devuelve una lista de todos los empleados. Se pueden filtrar por múltiples nombres separados por coma.")]
    public async Task<IActionResult> GetAllEmployees([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Obteniendo todos los empleados");

                var employees = await _unitOfWork.EmployeeRepository.GetAllEmployeesFromDb(
                    queryParameters.Take,
                    queryParameters.Skip,
                    queryParameters.Names,
                    queryParameters.Alls,
                    queryParameters.IsList
                );
                return Ok(employees);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los empleados");
            return StatusCode(500, "Error al obtener los empleados");
        }
    }

    /// <summary>
    /// Inserta un nuevo empleado
    /// </summary>
    /// <param name="request">El empleado a insertar</param>
    /// <returns>El resultado de la inserción</returns>
    [HttpPost("insert-employee")]
    [SwaggerOperation(Summary = "Inserta un nuevo empleado", Description = "Crea un nuevo empleado en la base de datos.")]
    public async Task<IActionResult> InsertEmployee([FromBody] EmployeeRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("El empleado es requerido");
                }

                var result = await _unitOfWork.EmployeeRepository.InsertEmployee(request);

                if (result)
                {
                    _logger.LogInformation("Empleado insertado con ID: {Id}", request.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo insertar el empleado");
                return BadRequest("No se pudo insertar el empleado");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al insertar el empleado");
            return StatusCode(500, "Error al insertar el empleado");
        }
    }

    /// <summary>
    /// Actualiza un empleado existente
    /// </summary>
    /// <param name="request">El empleado a actualizar</param>
    /// <returns>El resultado de la actualización</returns>
    [HttpPut("update-employee")]
    [SwaggerOperation(Summary = "Actualiza un empleado existente", Description = "Actualiza un empleado existente en la base de datos.")]
    public async Task<IActionResult> UpdateEmployee([FromBody] EmployeeRequest request)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (request == null)
                {
                    return BadRequest("El empleado es requerido");
                }

                var result = await _unitOfWork.EmployeeRepository.UpdateEmployee(request);

                if (result)
                {
                    _logger.LogInformation("Empleado actualizado con ID: {Id}", request.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo actualizar el empleado");
                return BadRequest("No se pudo actualizar el empleado");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar el empleado");
            return StatusCode(500, "Error al actualizar el empleado");
        }
    }

    /// <summary>
    /// Elimina un empleado (baja lógica)
    /// </summary>
    /// <param name="queryParameters">Parámetros de consulta que incluyen el ID</param>
    /// <returns>El resultado de la eliminación</returns>
    [HttpDelete("delete-employee")]
    [SwaggerOperation(Summary = "Elimina un empleado", Description = "Realiza una baja lógica del empleado en la base de datos.")]
    public async Task<IActionResult> DeleteEmployee([FromQuery] QueryParameters queryParameters)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Eliminando empleado con ID: {Id}", queryParameters.Id);

                var result = await _unitOfWork.EmployeeRepository.DeleteEmployee(queryParameters.Id);

                if (result)
                {
                    _logger.LogInformation("Empleado eliminado con ID: {Id}", queryParameters.Id);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo eliminar el empleado con ID: {Id}", queryParameters.Id);
                return BadRequest("No se pudo eliminar el empleado");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar el empleado con ID {Id}", queryParameters.Id);
            return StatusCode(500, "Error al eliminar el empleado");
        }
    }

    /// <summary>
    /// Convierte un empleado en usuario del sistema
    /// </summary>
    /// <param name="employeeId">ID del empleado</param>
    /// <param name="userId">ID del usuario</param>
    /// <returns>El resultado de la conversión</returns>
    [HttpPost("convert-employee-to-user")]
    [SwaggerOperation(Summary = "Convierte un empleado en usuario", Description = "Convierte un empleado existente en usuario del sistema.")]
    public async Task<IActionResult> ConvertEmployeeToUser([FromQuery] int employeeId, [FromQuery] string userId)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Convirtiendo empleado a usuario. EmployeeId: {EmployeeId}, UserId: {UserId}", employeeId, userId);

                var result = await _unitOfWork.EmployeeRepository.ConvertEmployeeToUser(employeeId, userId);

                if (result)
                {
                    _logger.LogInformation("Empleado convertido a usuario exitosamente. EmployeeId: {EmployeeId}", employeeId);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo convertir el empleado a usuario. EmployeeId: {EmployeeId}", employeeId);
                return BadRequest("No se pudo convertir el empleado a usuario");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al convertir empleado a usuario. EmployeeId: {EmployeeId}", employeeId);
            return StatusCode(500, "Error al convertir empleado a usuario");
        }
    }

    /// <summary>
    /// Verifica si existe un empleado principal
    /// </summary>
    /// <returns>El resultado de la verificación</returns>
    [HttpGet("has-main-employee")]
    [SwaggerOperation(Summary = "Verifica si existe un empleado principal", Description = "Verifica si existe un empleado marcado como principal en el sistema.")]
    public async Task<IActionResult> HasMainEmployee()
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Verificando si existe empleado principal");

                var result = await _unitOfWork.EmployeeRepository.HasMainEmployee();
                return Ok(result);
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar empleado principal");
            return StatusCode(500, "Error al verificar empleado principal");
        }
    }

    /// <summary>
    /// Actualiza el estado activo de un empleado
    /// </summary>
    /// <param name="employeeId">ID del empleado</param>
    /// <param name="isActive">Nuevo estado activo</param>
    /// <returns>El resultado de la actualización</returns>
    [HttpPut("update-employee-active-status")]
    [SwaggerOperation(Summary = "Actualiza el estado activo de un empleado", Description = "Actualiza el estado activo de un empleado en la base de datos.")]
    public async Task<IActionResult> UpdateEmployeeActiveStatus([FromQuery] int employeeId, [FromQuery] bool isActive)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Actualizando estado activo del empleado. EmployeeId: {EmployeeId}, IsActive: {IsActive}", employeeId, isActive);

                var result = await _unitOfWork.EmployeeRepository.UpdateEmployeeActiveStatus(employeeId, isActive);

                if (result)
                {
                    _logger.LogInformation("Estado activo del empleado actualizado exitosamente. EmployeeId: {EmployeeId}", employeeId);
                    return Ok(result);
                }

                _logger.LogWarning("No se pudo actualizar el estado activo del empleado. EmployeeId: {EmployeeId}", employeeId);
                return BadRequest("No se pudo actualizar el estado activo del empleado");
            }

            return BadRequest(Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar estado activo del empleado. EmployeeId: {EmployeeId}", employeeId);
            return StatusCode(500, "Error al actualizar estado activo del empleado");
        }
    }
}