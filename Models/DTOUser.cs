namespace Api.Models;

public class DTOUser
{
    public string Id { get; set; } = "";
    public int StaffId { get; set; } = 0; // StaffId para comparación con DTOStaff
    public string Email { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string? MiddleName { get; set; } = "";
    public string FatherLastName { get; set; } = "";
    public string? MotherLastName { get; set; } = "";
    public string AdministrationTitle { get; set; } = "";
    public bool EmailConfirmed { get; set; } = false;
    public bool IsTemporalPasswordActived { get; set; } = false;
    public List<string> Roles { get; set; } = [];
    public DTOUserRole? Role { get; set; } = null; // Rol completo (un solo rol por usuario)
    public DTOAgency? Agency { get; set; } = null;
    public string PhoneNumber { get; set; } = "";
    public string Password { get; set; } = "";
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; }
    public string Phone { get; set; } = "";
    public int? AgencyId { get; set; }
    public string? AgencyName { get; set; }
}
