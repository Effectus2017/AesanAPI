using System.ComponentModel.DataAnnotations;

namespace Api.Models;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña temporal es requerida")]
    public string TemporaryPassword { get; set; }

    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    public string NewPassword { get; set; }
}