using IdP;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using STS.Models;

namespace STS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<VUserClaims> VUsersClaims { get; set; }
        public DbSet<DbUser> dbUsers { get; set; }
        public DbSet<STS.Models.DbClaim> dbClaims { get; set; }
        public DbSet<CustomPasswordOptions> CustomPasswordOptions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //    this.ChangeTracker.AutoDetectChangesEnabled = false;


        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CustomPasswordOptions>().ToTable("CustomPasswordOption");
            builder.Entity<CustomPasswordOptions>().HasKey(t => t.Id);
            builder.Entity<CustomPasswordOptions>().HasData(new CustomPasswordOptions()
            {
                Id = 1,
                RequireDigit = false,
                RequiredLength = 1,
                RequiredUniqueChars = 1,
                RequireLowercase = false,
                RequireNonAlphanumeric = false,
                RequireUppercase = false
            });

            builder.Entity<DbClaim>().HasKey(t => t.Id);

            builder.Entity<DbClaim>().HasKey(t => t.Id);
            builder.Entity<DbClaim>().HasOne(t => t.dbUser).WithMany(t => t.dbClaims).HasForeignKey(t => t.UserId);
            builder.Entity<DbClaim>().ToTable("AspNetUserClaims", "dbo", t => t.ExcludeFromMigrations());

            builder.Entity<DbUser>().HasKey(t => t.Id);
            builder.Entity<DbUser>().ToTable("AspNetUsers", "dbo", t => t.ExcludeFromMigrations());


            builder.Entity<VUserClaims>().HasNoKey().ToView("VUserClaims");


            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
