namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para el estado de una agencia.
/// </summary>
public class AgencyStatusResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string NameEN { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}
