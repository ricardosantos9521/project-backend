using backendProject.Data.Tables;
using Microsoft.EntityFrameworkCore;

namespace backendProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Identity> Identity { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Identity>().HasKey(u => new
            {
                u.SubjectId,
                u.Issuer
            });


            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}