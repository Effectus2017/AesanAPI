using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class LoginRequest
{
    [Required]
    public required string UserName { get; set; }
    [Required]
    public required string Password { get; set; }
    public bool RememberMe { get; set; } = false;
}
