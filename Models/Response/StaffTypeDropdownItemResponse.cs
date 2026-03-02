namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para opciones de dropdown de tipo de staff (Id, Name y opcional).
/// </summary>
public class StaffTypeDropdownItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
