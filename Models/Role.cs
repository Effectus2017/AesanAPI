
using Microsoft.AspNetCore.Identity;

namespace Api.Models;

public class Role : IdentityRole
{
    public ICollection<UserRole> UserRoles { get; set; }
}