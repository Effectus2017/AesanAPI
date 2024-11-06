namespace Api.Models;

/// ------------------------------------------------------------------------------------------------
/// Modelo de programa
/// ------------------------------------------------------------------------------------------------

public class DTOProgram
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public int? AgencyId { get; set; } = 0;
}
