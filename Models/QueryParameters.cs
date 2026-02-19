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
    public bool? ProvidedRationsService { get; set; } // ¿Brindó servicio de raciones durante su periodo de funcionamiento?
    public int? SchoolId { get; set; } // ID de la escuela (deprecated)
    public int? SiteId { get; set; } // ID del sitio
    public int StaffId { get; set; } // ID del miembro del staff
    public int? StaffTypeId { get; set; } // ID del tipo de staff
    public int GroupTypeId { get; set; } // ID del tipo de grupo

    public string? ImageUrl { get; set; } // URL de la imagen
    public string? RejectionJustification { get; set; } // Justification for rejection
    public string? UserId { get; set; } // ID del usuario
    public List<string>? Roles { get; set; } // Lista de roles
    public bool ExcludeAdministrators { get; set; } = false; // Excluir usuarios con rol Administrator o Super-Administrator
    public string? Password { get; set; } // Contraseña actual
    public string? NewPassword { get; set; } // Nueva contraseña
    public string? TemporaryPassword { get; set; } // Contraseña temporal
    public string? Email { get; set; } // Email
    public string? Token { get; set; } // Token para restablecimiento de contraseña
    public string? DocumentType { get; set; } // Tipo de documento
    public string? Description { get; set; } // Descripción del archivo
    public string? OptionType { get; set; } // Tipo de opción
    public string? OptionKey { get; set; } // Clave de la opción
    public string? TemplateKey { get; set; } // Clave del template de email
    public string? CurrentUserId { get; set; } // ID del usuario actual logueado
    public bool? IsPropietary { get; set; } // Si es agencia propietaria (NUTRE)
    public DateTime? CompletedRegistrationDate { get; set; } // Fecha de registro completado
    public int? Month { get; set; } // Mes para filtrar días de funcionamiento (1-12)
    public int? Year { get; set; } // Año para filtrar días de funcionamiento
    public int? OperatingDayId { get; set; } // ID del día de funcionamiento
    public int? IsDayCareHomeId { get; set; } // ID de la opción IsDayCareHome para filtrar sitios (Sí, No, Ambos)
    public long? UieNumber { get; set; } // Identificador Único de Entidad (IUE)
    public long? SdrNumber { get; set; } // Número de Registro del Departamento de Estado (SDR)
    public int? EinNumber { get; set; } // Número de Seguro Social Patronal (EIN)

    // Centro de logs unificado (GET /api/logs)
    public string? LogCategory { get; set; } // Audit, Email, Job, Application
    public DateTime? LogFrom { get; set; }
    public DateTime? LogTo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
