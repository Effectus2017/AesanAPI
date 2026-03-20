namespace Api.Models.Response;

/// <summary>
/// Opción de escuela para dropdowns (Id, nombre y agencia).
/// </summary>
public class SchoolDropdownItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int AgencyId { get; set; }
}
