namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para opciones de dropdown de agencia (Id, Name y opcional).
/// </summary>
public class AgencyDropdownItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
