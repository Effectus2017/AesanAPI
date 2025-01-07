namespace Api.Models;

public class DTOFederalFundingSource
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required DateTime DateFrom { get; set; }
    public required DateTime DateTo { get; set; }
    public required decimal Amount { get; set; }
}