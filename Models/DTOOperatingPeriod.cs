namespace Api.Models;

public class DTOOperatingPeriod
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required string NameEN { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}
