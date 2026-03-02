namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para opciones de dropdown de staff (Id, Name y opcionalmente pocos campos más).
/// </summary>
public class StaffDropdownItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
