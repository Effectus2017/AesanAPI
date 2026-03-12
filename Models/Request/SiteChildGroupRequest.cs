namespace Api.Models.Request;

public class SiteChildGroupRequest
{
    public int? Id { get; set; }
    public string GroupName { get; set; }
    public int NumberOfChildren { get; set; }
    public List<SiteChildGroupServiceSlotRequest>? ServiceSlots { get; set; }
}
