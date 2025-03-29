namespace Api.Models;

/// <summary>
/// Modelo para la solicitud de actualización del avatar de un usuario
/// </summary>
public class UserAvatarRequest
{
    /// <summary>
    /// ID del usuario cuyo avatar se actualizará
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// URL de la nueva imagen de avatar
    /// </summary>
    public string ImageUrl { get; set; }
}