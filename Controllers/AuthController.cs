using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Api.Models;
using Api.Models.Request;

namespace Api.Controllers;

[ApiController]
[Route("auth")]
/// <summary>
/// Controlador que maneja la autenticación y gestión de contraseñas de usuarios.
/// Proporciona endpoints para inicio de sesión y restablecimiento de contraseñas.
/// </summary>
public class AuthController(IUnitOfWork unitOfWork, ILogger<AuthController> logger, IConfiguration configuration)
    : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<AuthController> _logger = logger;
    private readonly IConfiguration _configuration = configuration;

    /// <summary>
    /// Obtiene los nombres de roles AESAN (participan en selección multi-rol).
    /// </summary>
    [AllowAnonymous]
    [HttpGet("aesan-roles")]
    public async Task<ActionResult<object>> GetAesanRoles()
    {
        try
        {
            var names = await _unitOfWork.UserRepository.GetAesanRoleNames();
            return Ok(new { names });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "GetAesanRoles error");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Login
    /// </summary>
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
    /// Selecciona el rol con el que el usuario desea entrar (usuarios multi-rol AESAN).
    /// Requiere token Bearer válido (validación manual para evitar conflictos con Identity).
    /// </summary>
    [AllowAnonymous]
    [HttpPost("select-role")]
    public async Task<ActionResult<object>> SelectRole([FromBody] SelectRoleRequest model)
    {
        try
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Role))
            {
                return BadRequest(new { Message = "El rol es requerido." });
            }

            var userId = ValidateBearerTokenAndGetUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { Message = "Token inválido o expirado." });
            }

            return await _unitOfWork.UserRepository.SelectRole(userId, model.Role);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "SelectRole error");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Valida el token Bearer del header Authorization y extrae el userId.
    /// Usa la misma configuración JWT que el middleware para consistencia.
    /// </summary>
    private string? ValidateBearerTokenAndGetUserId()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var token = authHeader.Substring(7).Trim();
        if (string.IsNullOrEmpty(token))
        {
            return null;
        }

        try
        {
            var key = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            var issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            var audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal.FindFirstValue("nameid")
                ?? principal.FindFirstValue("sub");

            return userId;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "JWT validation failed for select-role");
            return null;
        }
    }
}
