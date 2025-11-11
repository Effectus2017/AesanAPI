namespace Api.Models;

public class DTOSponsorType
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = "";
    public string NameEN { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    public bool SelectionNotification { get; set; } = false;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}