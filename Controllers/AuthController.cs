using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Api.Interfaces; // Asegúrate de que este espacio de nombres coincida con el de tu interfaz
using Api.Models;
using Api.Data;

namespace Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IUnitOfWork _unitOfWork; // Agregar UnitOfWork

    public AuthController(IAuthRepository authRepository, IUnitOfWork unitOfWork)
    {
        _authRepository = authRepository;
        _unitOfWork = unitOfWork; // Asignar UnitOfWork
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        if (!ModelState.IsValid) // Validar ModelState
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = await _authRepository.Register(model);
            if (result.Succeeded)
            {
                //await _unitOfWork.Save(); // Guardar cambios
                return Ok();
            }
            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            // Manejo de excepciones
            return StatusCode(500, "Error interno del servidor");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var success = await _authRepository.Login(model);
        if (success)
        {
            return Ok();
        }
        return Unauthorized();
    }

    [HttpGet("users")]
    public  IActionResult GetUsers([FromQuery] QueryParams queryParams)
    {
        var users = _unitOfWork.AuthRepository.GetUsers();
        return Ok(users);
    }
}
