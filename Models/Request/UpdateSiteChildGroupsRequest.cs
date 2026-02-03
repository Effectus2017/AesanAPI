namespace Api.Models.Request;

public class UpdateSiteChildGroupsRequest
{
    public int SiteId { get; set; }
    public List<SiteChildGroupRequest> ChildGroups { get; set; } = [];
}
