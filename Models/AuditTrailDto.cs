namespace Api.Models;

/// <summary>
/// DTO para representar un registro de auditoría
/// </summary>
public class AuditTrailDto
{
    public long Id { get; set; }
    public Guid OperationId { get; set; }
    public string Action { get; set; } = "";
    public string ChangedBy { get; set; } = "";
    public DateTime ChangedAt { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? ChangedFields { get; set; }
    public string? Reason { get; set; }
    public string? BusinessContext { get; set; }
    public string? Tags { get; set; }
    public string? OperationDescription { get; set; }
    public string? OperationStatus { get; set; }
    public string? UserFullName { get; set; }
    public string? UserEmail { get; set; }
}
