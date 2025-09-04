using Microsoft.AspNetCore.Identity;

namespace Api.Models;

public class User : IdentityUser
{
    // Solo campos esenciales para Identity y autenticación
    public bool IsActive { get; set; } = true;
    public bool IsTemporalPasswordActived { get; set; } = true;
    public DateTime? UpdatedAt { get; set; } = null;

    // Relación con Staff (opcional, para consultas)
    public virtual Staff? Staff { get; set; }

    // Relación con roles
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];

    // Campos removidos (ahora van en Staff):
    // - FirstName
    // - MiddleName  
    // - FatherLastName
    // - MotherLastName
    // - AdministrationTitle
    // - ImageURL
}

