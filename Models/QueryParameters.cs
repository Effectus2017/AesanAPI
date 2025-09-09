namespace Api.Models;

public class QueryParameters
{
    public int Take { get; set; } = 10;
    public int Skip { get; set; } = 0;
    public string? Name { get; set; } // Para endpoints existentes
    public string? Names { get; set; } // Para el filtrado de programas
    public bool Alls { get; set; } = false;
    public bool IsList { get; set; } = false;
    public bool ExcludeRelated { get; set; } = false;
    public int Id { get; set; }
    public string? StringId { get; set; } // Para IDs de tipo string (como Permission)
    public string? ValueKey { get; set; } // Para filtrar por ValueKey en permisos
    public int AgencyId { get; set; }
    public string? AssignedBy { get; set; }
    public int? ProgramId { get; set; }
    public int RegionId { get; set; }  // ID de la región
    public int CityId { get; set; } // ID de la ciudad
    public int? StatusId { get; set; } // ID del estado
    public int? MemberId { get; set; } // ID del miembro
    public int OptionSelectionId { get; set; } // ID de la opción de selección
    public string? PermissionId { get; set; } // ID del permiso (cambiado a string)
    public string? RoleId { get; set; } // ID del rol
    public bool IsActive { get; set; } = true; // Estado activo
    public string? InactiveJustification { get; set; } // Justificación para inactivación
    public DateTime? InactiveDate { get; set; } // Fecha de inactivación
    public int? SchoolId { get; set; } // ID de la escuela
    public int StaffId { get; set; } // ID del miembro del staff
    public int? StaffTypeId { get; set; } // ID del tipo de staff

    public string? ImageUrl { get; set; } // URL de la imagen
    public string? RejectionJustification { get; set; } // Justification for rejection
    public string? UserId { get; set; } // ID del usuario
    public List<string>? Roles { get; set; } // Lista de roles
    public string? Password { get; set; } // Contraseña actual
    public string? NewPassword { get; set; } // Nueva contraseña
    public string? TemporaryPassword { get; set; } // Contraseña temporal
    public string? Email { get; set; } // Email
    public string? Token { get; set; } // Token para restablecimiento de contraseña
    public string? DocumentType { get; set; } // Tipo de documento
    public string? Description { get; set; } // Descripción del archivo
    public string? OptionType { get; set; } // Tipo de opción
    public string? OptionKey { get; set; } // Clave de la opción
    public string? CurrentUserId { get; set; } // ID del usuario actual logueado
    public bool? IsPropietary { get; set; } // Si es agencia propietaria (NUTRE)
}
