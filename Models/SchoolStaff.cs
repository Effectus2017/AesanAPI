namespace Api.Models;

/// <summary>
/// Relación entre una escuela (School) y un miembro del staff.
/// </summary>
public class SchoolStaff
{
    public int Id { get; set; }
    public int SchoolId { get; set; }
    public int StaffId { get; set; }
    public DateTime AssignmentDate { get; set; }
    public int AssignmentTypeId { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Comments { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
