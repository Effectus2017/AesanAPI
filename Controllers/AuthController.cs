using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Api.Interfaces; // Asegúrate de que este espacio de nombres coincida con el de tu interfaz
using Api.Models;
using Api.Data;

namespace Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController(IUnitOfWork unitOfWork) : ControllerBase
{

    private readonly IUnitOfWork _unitOfWork = unitOfWork; // Agregar UnitOfWork

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
