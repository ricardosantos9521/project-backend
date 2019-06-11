using backendProject.Database.AccountTables;
using backendProject.Database.AdminTables;
using Microsoft.EntityFrameworkCore;

namespace backendProject.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Identity> Identity { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Identity>().HasKey(u => new
            {
                u.SubjectId,
                u.Issuer
            });

            builder.Entity<Identity>()
                .HasOne(x => x.Profile)
                .WithOne(x => x.Identity)
                .HasPrincipalKey<Identity>(x => x.UniqueId);

            builder.Entity<Identity>()
                .HasOne(x => x.Admin)
                .WithOne(x => x.Identity)
                .HasPrincipalKey<Identity>(x => x.UniqueId);
        }
    }
}