using System.ComponentModel.DataAnnotations;

namespace Api.Models;

/// <summary>
/// Modelo de solicitud de inicio de sesión
/// </summary>
public class LoginRequest
{
    [Required]
    public required string UserName { get; set; }
    [Required]
    public required string Password { get; set; }
    public bool RememberMe { get; set; } = false;
}
