namespace Api.Models;

public class DTOFederalFundingCertification
{
    public required int Id { get; set; }
    public required decimal FundingAmount { get; set; }
    public required string Description { get; set; }
    public DateTime? UpdatedAt { get; set; }
}