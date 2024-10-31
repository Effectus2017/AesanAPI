using Microsoft.AspNetCore.Identity;

namespace Api.Models;

public class User : IdentityUser
{
    public int? AgencyId { get; set; } = null;
    public string? FirstName { get; set; } = "";
    public string? MiddleName { get; set; } = "";
    public string? FatherLastName { get; set; } = "";
    public string? MotherLastName { get; set; } = "";
    public string? AdministrationTitle { get; set; } = "";
    //public string ImageURL { get; set; } = "";
    //public bool Enabled { get; set; } = true;
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];
}

