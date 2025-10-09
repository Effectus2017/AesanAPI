namespace Api.Models.Response;

public class SiteTableResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string CityName { get; set; }
    public string RegionName { get; set; }
    public bool IsMainSite { get; set; }
    public string? MainSiteName { get; set; }
    public int? GeneralEnrollment { get; set; }
    public int SiteNumber { get; set; }
    public string? AgencyCode { get; set; }
    public string? SiteCode { get; set; }
}
