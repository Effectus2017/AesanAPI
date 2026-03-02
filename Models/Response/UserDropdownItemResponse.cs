namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para opciones de dropdown de usuario (Id, Name y opcional).
/// </summary>
public class UserDropdownItemResponse
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
