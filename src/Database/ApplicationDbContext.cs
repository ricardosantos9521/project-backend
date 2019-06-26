using backendProject.Database.AccountTables;
using backendProject.Database.AdminTables;
using backendProject.Database.FilesTables;
using Microsoft.EntityFrameworkCore;

namespace backendProject.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Identity> Identity { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<FileBytes> File { get; set; }
        public DbSet<Read> ReadPermissions { get; set; }
        public DbSet<Write> WritePermissions { get; set; }
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

            builder.Entity<Identity>()
                .HasMany(x => x.Sessions)
                .WithOne(x => x.Identity)
                .HasPrincipalKey(x => x.UniqueId);

            builder.Entity<Session>()
                .HasOne(x => x.RefreshToken)
                .WithOne(x => x.Session)
                .HasPrincipalKey<Session>(x => x.SessionId);

            //read permissions foreign keys

            builder.Entity<Read>()
                .HasKey(x => new { x.FileId, x.UniqueId });

            builder.Entity<Read>()
                .HasOne(x => x.File)
                .WithMany(p => p.ReadPermissions)
                .HasPrincipalKey(pc => pc.FileId);

            builder.Entity<Read>()
                .HasOne(pc => pc.Identity)
                .WithMany(c => c.ReadPermissions)
                .HasPrincipalKey(pc => pc.UniqueId);

            //write permissions foreign keys

            builder.Entity<Write>()
                .HasKey(x => new { x.FileId, x.UniqueId });

            builder.Entity<Write>()
                .HasOne(x => x.File)
                .WithMany(p => p.WritePermissions)
                .HasPrincipalKey(pc => pc.FileId);

            builder.Entity<Write>()
                .HasOne(pc => pc.Identity)
                .WithMany(c => c.WritePermissions)
                .HasPrincipalKey(pc => pc.UniqueId);
        }
    }
}