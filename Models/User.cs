using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Api.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public string ImageURL { get; set; }
        public bool Enabled { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
