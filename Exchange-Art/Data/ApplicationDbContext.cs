using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Exchange_Art.Data.Entities;
using Exchange_Art.Models;

namespace Exchange_Art.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public static bool hasMigrated;

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            if (!hasMigrated)
            {
                Database.Migrate();
                hasMigrated = true;
            }
        }

        public DbSet<Art> Art { get; set; }

    }
}