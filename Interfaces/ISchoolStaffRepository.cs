using Api.Models.Request;
using Api.Models.Response;

namespace Api.Interfaces;

/// <summary>
/// Repositorio de asignaciones Staff–School (SchoolStaff).
/// </summary>
public interface ISchoolStaffRepository
{
    Task<IEnumerable<SchoolStaffResponse>> GetStaffBySchool(int schoolId);
    Task<IEnumerable<SchoolStaffResponse>> GetSchoolsByStaff(int staffId);
    Task<int> AssignStaffToSchool(SchoolStaffRequest request);
    Task<bool> UnassignStaffFromSchool(int schoolId, int staffId);
    Task<bool> UpdateSchoolStaff(int id, UpdateSchoolStaffRequest request);
    Task<SchoolStaffResponse?> GetSchoolStaffById(int id);
}
