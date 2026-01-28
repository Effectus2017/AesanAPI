namespace Api.Models.Request;

public class SiteChildGroupRequest
{
    public string GroupName { get; set; }
    public int NumberOfChildren { get; set; }
    public List<SiteServiceRequest>? Services { get; set; }
}
