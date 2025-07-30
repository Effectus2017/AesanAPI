namespace Api.Models.Request;

public class StaffTypeRequest
{
    public int? Id { get; set; }
    public string Name { get; set; } = "";
    public string NameEn { get; set; } = "";
    public string OptionKey { get; set; } = "";
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}