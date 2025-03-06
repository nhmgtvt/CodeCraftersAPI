using CodeCrafters.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;

namespace CodeCrafters.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<ExternalLogin> ExternalLogins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique(); // Ensure email uniqueness

            modelBuilder.Entity<ExternalLogin>()
            .HasIndex(e => new { e.Provider, e.ProviderUserId })
            .IsUnique(); // Ensure unique provider-user mapping

            modelBuilder.Entity<ExternalLogin>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
