using System;
using System.Collections.Generic;
using System.Text;
using Exchange_Art.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Exchange_Art.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public static bool hasMigrated;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            if (!hasMigrated)
            {
                Database.Migrate();
                hasMigrated = true;
            }
        }
    }
}
