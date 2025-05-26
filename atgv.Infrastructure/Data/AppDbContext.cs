using atgv.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace atgv.Infrastructure
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):
            base(options) {}
        public DbSet<User> Users { get; set; }
        public DbSet<AccessToken> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.CreatedAt);
                entity.Property(e => e.ModifiedAt);
                entity.Property(e => e.Verified);
            });
            modelBuilder.Entity<AccessToken>(entity =>
            {
                entity.Property(e => e.Token).IsRequired().HasMaxLength(6);
                entity.HasIndex(e => e.Token).IsUnique();
                entity.Property(e => e.ExpiryDate).IsRequired();
                entity.Property(e => e.UserEmail).IsRequired();
            });
        }
    }
}
