using System;
using System.Collections.Generic;

namespace Api.Models.Response;

public class SiteChildGroupResponse
{
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string GroupName { get; set; }
    public int NumberOfChildren { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<SiteServiceResponse>? Services { get; set; }
}
