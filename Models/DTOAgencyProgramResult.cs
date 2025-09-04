namespace Api.Models;

/// <summary>
/// DTO que representa el resultado de programas con AgencyId del SP 117_GetAgencies
/// Incluye los campos del programa más la relación con la agencia
/// </summary>
public class DTOAgencyProgramResult
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string NameEN { get; set; } = "";
    public string Description { get; set; } = "";
    public string DescriptionEN { get; set; } = "";
    public int AgencyId { get; set; }
}
