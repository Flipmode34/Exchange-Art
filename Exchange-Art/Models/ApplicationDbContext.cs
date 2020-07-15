using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Exchange_Art.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //public static bool hasMigrated;

        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //if (!hasMigrated)
            //{
            //    Database.Migrate();
            //    hasMigrated = true;
            //}
        }

        public DbSet<Art> Art { get; set; }

    }
}