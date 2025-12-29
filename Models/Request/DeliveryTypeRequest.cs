using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

/// <summary>
/// Modelo de request para tipos de entrega
/// </summary>
public class DeliveryTypeRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Name { get; set; }

    [Required(ErrorMessage = "El nombre en inglés es requerido")]
    [StringLength(255, ErrorMessage = "El nombre en inglés no puede exceder 255 caracteres")]
    public string NameEN { get; set; }

    public bool IsActive { get; set; } = true;

    [Required(ErrorMessage = "El orden de visualización es requerido")]
    [Range(0, int.MaxValue, ErrorMessage = "El orden de visualización debe ser un número positivo")]
    public int DisplayOrder { get; set; }
}
