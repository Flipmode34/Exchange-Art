using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Exchange_Art.Models
{
    // The RoleEdit class is used to represent the Role and the details of the Users in the Identity System.
    public class RoleEdit
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<ApplicationUser> Members { get; set; }
        public IEnumerable<ApplicationUser> NonMembers { get; set; }
    }
}