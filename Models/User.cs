using Microsoft.AspNetCore.Identity;

namespace Api.Models;

public class User : IdentityUser
{
    public string? FirstName { get; set; } = "";
    public string? MiddleName { get; set; } = "";
    public string? FatherLastName { get; set; } = "";
    public string? MotherLastName { get; set; } = "";
    public string? AdministrationTitle { get; set; } = "";
    public string? ImageURL { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public bool IsTemporalPasswordActived { get; set; } = true;
    public DateTime? UpdatedAt { get; set; } = null;
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}

