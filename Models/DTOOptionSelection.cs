public class DTOOptionSelection
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string NameEN { get; set; }
    public required string OptionKey { get; set; }
    public bool BooleanValue { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}