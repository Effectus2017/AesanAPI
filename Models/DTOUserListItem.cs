namespace Api.Models;

/// <summary>
/// Item de usuario devuelto por el SP 109_GetAllUsersFromDb. Tipo fuerte para que Dapper mapee correctamente
/// DisplayName, DisplayNameEN y el resto de columnas (evita problemas con dynamic y mayúsculas).
/// </summary>
public class DTOUserListItem
{
    public string Id { get; set; } = "";
    public int StaffId { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? FatherLastName { get; set; }
    public string? MotherLastName { get; set; }
    public string? Position { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; }
    public bool IsTemporalPasswordActived { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
    public string? RoleNormalizedName { get; set; }
    public string? DisplayName { get; set; }
    public string? DisplayNameEN { get; set; }
    public DateTime? ContractStartDate { get; set; }
    public DateTime? ContractEndDate { get; set; }
    public int? AgencyId { get; set; }
    public string? ProgramName { get; set; }
}
