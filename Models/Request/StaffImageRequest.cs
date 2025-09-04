using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para actualizar la imagen del staff
/// </summary>
public class StaffImageRequest
{
    /// <summary>
    /// ID del staff
    /// </summary>
    [Required(ErrorMessage = "El ID del staff es requerido")]
    public int StaffId { get; set; }

    /// <summary>
    /// URL de la nueva imagen (puede ser null para remover la imagen)
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Comentario opcional sobre el cambio de imagen
    /// </summary>
    [MaxLength(500, ErrorMessage = "El comentario no puede exceder 500 caracteres")]
    public string? Comment { get; set; }
}
