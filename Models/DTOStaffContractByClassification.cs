namespace Api.Models;

/// <summary>
/// Contrato por clasificación (Administrativo/Operacional) de un staff.
/// </summary>
public class DTOStaffContractByClassification
{
    public int Id { get; set; }
    public int StaffId { get; set; }
    public int StaffClassificationId { get; set; }
    public string StaffClassificationName { get; set; } = "";
    public string StaffClassificationNameEn { get; set; } = "";
    public int PositionId { get; set; }
    public string PositionName { get; set; } = "";
    public string PositionNameEn { get; set; } = "";
    public DateTime? ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }
    /// <summary>Horario desde (HH:mm).</summary>
    public string? ScheduleFrom { get; set; }
    /// <summary>Horario hasta (HH:mm).</summary>
    public string? ScheduleTo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; }
}
