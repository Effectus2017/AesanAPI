namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para opciones de dropdown de staff.
/// </summary>
public class StaffDropdownItemResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string FatherLastName { get; set; } = string.Empty;
    public string? MotherLastName { get; set; }
    public string? StaffTypeName { get; set; }
    public string? StaffTypeNameEn { get; set; }
    public bool IsActive { get; set; } = true;
}
