using Microsoft.AspNetCore.Mvc;
using Api.Models;


namespace Api.Controllers;

[ApiController]
[Route("auth")]
/// <summary>
/// Controlador que maneja la autenticación y gestión de contraseñas de usuarios.
/// Proporciona endpoints para inicio de sesión y restablecimiento de contraseñas.
/// </summary>
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

}
