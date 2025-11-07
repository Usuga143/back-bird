using BackBird.Api.src.Bird.Modules.Users.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackBird.Api.src.Bird.Modules.Users.Infrastructure.Persistence
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(eb =>
            {
                eb.HasKey(u => u.Id);
                eb.HasIndex(u => u.Email).IsUnique();
                eb.Property(u => u.Email).IsRequired();
                eb.Property(u => u.PasswordHash).IsRequired();
                eb.Property(u => u.Name).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}