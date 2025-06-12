namespace Api.Models;

public class QueryParameters
{
    public int Take { get; set; } = 10;
    public int Skip { get; set; } = 0;
    public string? Name { get; set; } // Para endpoints existentes
    public string? Names { get; set; } // Para el filtrado de programas
    public bool Alls { get; set; } = false;
    public int Id { get; set; }
    public int AgencyId { get; set; }
    public string? AssignedBy { get; set; }
    public int? ProgramId { get; set; }
    public int RegionId { get; set; }  // ID de la región
    public int CityId { get; set; } // ID de la ciudad
    public int? StatusId { get; set; } // ID del estado
    public int? MemberId { get; set; } // ID del miembro
    public int OptionSelectionId { get; set; } // ID de la opción de selección
    public int? PermissionId { get; set; } // ID del permiso
    public string? RoleId { get; set; } // ID del rol
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
}
