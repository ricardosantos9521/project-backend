using Project.Backend.Database.AccountTables;
using Project.Backend.Database.AdminTables;
using Project.Backend.Database.FilesTables;
using Microsoft.EntityFrameworkCore;

namespace Project.Backend.Database
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Identity> Identity { get; set; }
        public DbSet<Profile> Profile { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Session> Session { get; set; }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<File> File { get; set; }
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

            //profile and files -> ownedby

            builder.Entity<Profile>()
                .HasMany(x => x.OwnedByMe)
                .WithOne(x => x.OwnedBy)
                .HasPrincipalKey(x => x.UniqueId);

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

            builder.Entity<Read>()
                .HasOne(pc => pc.SharedByIdentity)
                .WithMany(c => c.SharedByMeReadPermissions)
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
            
            builder.Entity<Write>()
                .HasOne(pc => pc.SharedByIdentity)
                .WithMany(c => c.SharedByMeWritePermissions)
                .HasPrincipalKey(pc => pc.UniqueId);
        }
    }
}