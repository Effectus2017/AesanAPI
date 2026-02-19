using Microsoft.AspNetCore.Identity;

namespace Api.Models;

public class Role : IdentityRole
{
    public bool IsAesanRole { get; set; }
    /// <summary>Nombre a mostrar en español (frontend).</summary>
    public string? DisplayName { get; set; }
    /// <summary>Nombre a mostrar en inglés (frontend).</summary>
    public string? DisplayNameEN { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}