namespace Api.Models.Response;

/// <summary>
/// Modelo de respuesta para tipos de entrega
/// </summary>
public class DeliveryTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameEN { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public bool? RequiresPermission { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
