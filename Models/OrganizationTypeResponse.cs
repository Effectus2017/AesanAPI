namespace Api.Models;

public class OrganizationTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameEN { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public bool RequiresCenterType { get; set; }
}
