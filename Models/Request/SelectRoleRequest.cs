using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

/// <summary>
/// Modelo de solicitud para seleccionar rol (usuarios multi-rol AESAN).
/// </summary>
public class SelectRoleRequest
{
    [Required]
    public required string Role { get; set; }
}
