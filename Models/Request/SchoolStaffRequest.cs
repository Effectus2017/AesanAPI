namespace Api.Models.Request;

/// <summary>
/// Request para asignar staff a una escuela.
/// </summary>
public class SchoolStaffRequest
{
    public int SchoolId { get; set; }
    public int StaffId { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Comments { get; set; }
}

/// <summary>
/// Request para actualizar una asignación SchoolStaff existente.
/// </summary>
public class UpdateSchoolStaffRequest
{
    public bool IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Comments { get; set; }
}
