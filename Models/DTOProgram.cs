namespace Api.Models;

/// ------------------------------------------------------------------------------------------------
/// Modelo de programa
/// ------------------------------------------------------------------------------------------------

public class DTOProgram
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "";
    public string NameEN { get; set; } = "";
    public string Description { get; set; } = "";
    public string DescriptionEN { get; set; } = "";
    public bool IsActive { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.MinValue;
    public DateTime? UpdatedAt { get; set; } = null;
}
