namespace Api.Models;

/// <summary>
/// DTO que representa la relación entre una agencia y un programa
/// Incluye información del programa y la relación con la agencia
/// </summary>
public class DTOAgencyProgram
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int AgencyId { get; set; }
}
