using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

public class StaffTypeRequest
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "El nombre en inglés es requerido")]
    [StringLength(100, ErrorMessage = "El nombre en inglés no puede exceder 100 caracteres")]
    public string NameEn { get; set; } = "";

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}