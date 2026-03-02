namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para opciones de dropdown de clasificación de staff (Id, Name y opcional).
/// </summary>
public class StaffClassificationDropdownItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
}
