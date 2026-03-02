namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para opciones de dropdown de sitio (Id, Name).
/// Convención: usar sufijo DropdownItemResponse para dropdowns. Misma forma que SiteListItemResponse.
/// </summary>
public class SiteDropdownItemResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
