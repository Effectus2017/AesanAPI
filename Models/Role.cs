using Microsoft.AspNetCore.Identity;

namespace Api.Models;

public class Role : IdentityRole
{
    public bool IsAesanRole { get; set; }
    /// <summary>Nombre del rol en inglés (desde DB).</summary>
    public string? NameEN { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}