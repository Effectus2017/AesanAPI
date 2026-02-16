namespace Api.Models;

public class DTOUser
{
    public string Id { get; set; } = "";
    public int StaffId { get; set; } = 0; // StaffId para comparación con DTOStaff
    public string? UserName { get; set; }
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
    /// <summary>Rol principal (nombre). Para create/update.</summary>
    public string? PrimaryRoleName { get; set; }
    /// <summary>Roles secundarios con vigencia. Para create/update y respuesta.</summary>
    public List<DTOUserSecondaryRole>? SecondaryRoles { get; set; }
    /// <summary>Texto para mostrar en listados: roles unidos por coma, o el primer rol si solo hay uno.</summary>
    public string RolesDisplay => Roles != null && Roles.Count > 0 ? string.Join(", ", Roles) : (Role?.Name ?? "");
    public DTOAgency? Agency { get; set; } = null;
    public string PhoneNumber { get; set; } = "";
    public string Password { get; set; } = "";
    public string? ImageURL { get; set; }
    public bool IsActive { get; set; }
    public string Phone { get; set; } = "";
    public int? AgencyId { get; set; }
    public string? AgencyName { get; set; }

    /// <summary>Programa asignado al usuario (para filtrado de información). Solo en Admin Portal se asigna.</summary>
    public int? ProgramId { get; set; }
    public string? ProgramName { get; set; }
}
