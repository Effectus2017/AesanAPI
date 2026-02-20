namespace Api.Models.Request;

/// <summary>
/// Un contrato por clasificación (Administrativo u Operacional) en create/update de staff.
/// </summary>
public class StaffClassificationContractItemRequest
{
    public int StaffClassificationId { get; set; }
    public int PositionId { get; set; }
    public DateTime? ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }
    /// <summary>Horario desde, formato "HH:mm".</summary>
    public string? ScheduleFrom { get; set; }
    /// <summary>Horario hasta, formato "HH:mm".</summary>
    public string? ScheduleTo { get; set; }
}
