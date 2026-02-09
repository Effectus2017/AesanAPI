using Microsoft.AspNetCore.Identity;

namespace Api.Models;

public class Role : IdentityRole
{
    public bool IsAesanRole { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}