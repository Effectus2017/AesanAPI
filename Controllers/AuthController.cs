using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Api.Interfaces; // Asegúrate de que este espacio de nombres coincida con el de tu interfaz
using Api.Models;
using Api.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IUnitOfWork unitOfWork, ILogger<AuthController> logger) : ControllerBase
{

    private readonly IUnitOfWork _unitOfWork = unitOfWork; // Agregar UnitOfWork
    private readonly ILogger<AuthController> _logger = logger;
    /// <summary>
    /// Login
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<object>> Login([FromBody] LoginRequest model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                return await _unitOfWork.UserRepository.Login(model);
            }

            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Resetea la contraseña de un usuario usando la contraseña temporal
    /// </summary>
    /// <param name="model">Modelo con email, contraseña temporal y nueva contraseña</param>
    /// <returns>El resultado de la operación</returns>
    [HttpPost("reset-password")]
    [SwaggerOperation(Summary = "Resetea la contraseña de un usuario", Description = "Permite a un usuario cambiar su contraseña usando una contraseña temporal.")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Reseteando contraseña para el usuario con email: {Email}", model.Email);
                var result = await _unitOfWork.UserRepository.ResetPassword(model);
                return result;
            }

            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetErrorListFromModelState(ModelState));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al resetear la contraseña");
            return StatusCode(StatusCodes.Status400BadRequest, Utilities.GetResponseFromException(ex));
        }
    }

}
