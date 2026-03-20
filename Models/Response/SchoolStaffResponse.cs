namespace Api.Models.Response;

/// <summary>
/// Respuesta para la relación Staff–School (SchoolStaff).
/// </summary>
public class SchoolStaffResponse
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

    // GetStaffBySchool
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? FatherLastName { get; set; }
    public string? MotherLastName { get; set; }
    public string? Email { get; set; }
    public string? AssignmentTypeName { get; set; }
    /// <summary>Nombre en inglés del tipo de asignación (OptionSelection.NameEN). Alias SQL: AssignmentTypeNameEN.</summary>
    public string? AssignmentTypeNameEN { get; set; }

    // GetSchoolsByStaff / GetSchoolStaffById
    public string? SchoolName { get; set; }
    public string? SchoolCode { get; set; }
    public int? AgencyId { get; set; }
    public int? SchoolNumber { get; set; }
    public bool? SchoolIsActive { get; set; }
    public string? AgencyName { get; set; }
    public bool? AgencyIsActive { get; set; }
}
