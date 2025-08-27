namespace Api.Models.Request;

/// <summary>
/// Modelo de request para las operaciones de asignación de staff a sitios
/// </summary>
public class SchoolStaffRequest
{
    public int SchoolId { get; set; }
    public int StaffId { get; set; }
    public int AssignmentTypeId { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Comments { get; set; }
}

/// <summary>
/// Modelo de request para actualizar una asignación existente
/// </summary>
public class UpdateSchoolStaffRequest
{
    public int AssignmentTypeId { get; set; }
    public bool IsPrimary { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Comments { get; set; }
}
