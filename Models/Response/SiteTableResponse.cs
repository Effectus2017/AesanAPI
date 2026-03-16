namespace Api.Models.Response;

public class SiteTableResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string CityName { get; set; }
    public string RegionName { get; set; }
    public bool IsActive { get; set; }
    public int? GeneralEnrollment { get; set; }
    public string? SiteCode { get; set; }
}
