using Exchange_Art.Data;
using Microsoft.AspNetCore.Identity;
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

        public DbSet<Art> Art { get; set; }
        public DbSet<ArtLease> ArtLease { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var admin = new ApplicationUser
            {
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@novi.nl",
                NormalizedEmail = "ADMIN@NOVI.NL"
            };

            var teacher = new ApplicationUser
            {
                UserName = "Teacher",
                NormalizedUserName = "TEACHER",
                Email = "teacher@novi.nl",
                NormalizedEmail = "TEACHER@NOVI.NL"
            };

            var student = new ApplicationUser
            {
                UserName = "Student",
                NormalizedUserName = "STUDENT",
                Email = "student@novi.nl",
                NormalizedEmail = "STUDENT@NOVI.NL"
            };

            var adminRole = new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            };

            //set user passwords
            PasswordHasher<ApplicationUser> ph = new PasswordHasher<ApplicationUser>();
            admin.PasswordHash = ph.HashPassword(admin, "Pas.@l1ve");
            teacher.PasswordHash = ph.HashPassword(teacher, "Tea.@l1ve");
            student.PasswordHash = ph.HashPassword(student, "Stu.@l1ve");

            //seed users/roles
            modelBuilder.Entity<ApplicationUser>().HasData(admin);
            modelBuilder.Entity<ApplicationUser>().HasData(teacher);
            modelBuilder.Entity<ApplicationUser>().HasData(student);
            modelBuilder.Entity<IdentityRole>().HasData(adminRole);
        }

    }
}