using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Exchange_Art.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Represents the tables of the Models (Art & ArtLease) in the Database
        public DbSet<Art> Art { get; set; }
        public DbSet<ArtLease> ArtLease { get; set; }

    }
}