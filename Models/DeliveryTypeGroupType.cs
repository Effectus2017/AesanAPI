namespace Api.Models;

/// <summary>
/// DTO para la relación entre DeliveryType y GroupType
/// </summary>
public class DeliveryTypeGroupType
{
    public int Id { get; set; }
    public int DeliveryTypeId { get; set; }
    public int GroupTypeId { get; set; }
    public bool RequiresPermission { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

