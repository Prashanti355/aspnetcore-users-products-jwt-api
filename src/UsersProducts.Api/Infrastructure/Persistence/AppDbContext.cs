using Microsoft.EntityFrameworkCore;
using UsersProducts.Api.Domain.Entities;

namespace UsersProducts.Api.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(user => user.Id);

            entity.Property(user => user.Id)
                .HasColumnName("id");

            entity.Property(user => user.Name)
                .HasColumnName("name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(user => user.Email)
                .HasColumnName("email")
                .HasMaxLength(254)
                .IsRequired();

            entity.HasIndex(user => user.Email)
                .IsUnique();

            entity.Property(user => user.PasswordHash)
                .HasColumnName("password_hash")
                .IsRequired();

            entity.Property(user => user.Role)
                .HasColumnName("role")
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(user => user.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.Property(user => user.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            entity.Property(user => user.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");
        });
    }
}