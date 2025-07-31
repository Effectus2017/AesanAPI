using System.ComponentModel.DataAnnotations;

namespace Api.Models.Request;

/// <summary>
/// Modelo de solicitud para empleados
/// </summary>
[Obsolete("Esta clase está deprecada. Use StaffRequest en su lugar. Será eliminada en una versión futura.")]
public class EmployeeRequest
{
    public int? Id { get; set; }
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; } = "";
    public string FatherLastName { get; set; } = "";
    public string MotherLastName { get; set; } = "";
    public int StatusId { get; set; } = 1;
    public int PositionId { get; set; } = 0;
    public DateTime BirthDate { get; set; }
    public string Email { get; set; } = "";
    public string PostalAddress { get; set; } = "";
    public int CityId { get; set; } = 0;
    public int RegionId { get; set; } = 0;
    public string AreaCode { get; set; } = "";
    public string? Comments { get; set; } = "";
    public string? UserId { get; set; } = null;
    public bool IsActive { get; set; } = true;
}