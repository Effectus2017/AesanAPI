namespace Api.Models;

public class StaffClassification
{
    // Constantes para IDs de clasificaciones
    public const int ADMINISTRATIVE_ID = 1;
    public const int OPERATIONAL_ID = 2;
    public const int BOTH_ID = 3;

    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string NameEn { get; set; } = "";
    public int SortOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}