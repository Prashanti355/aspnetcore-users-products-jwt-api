using Microsoft.EntityFrameworkCore;
using UsersProducts.Api.Domain.Entities;

namespace UsersProducts.Api.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

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

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");

            entity.HasKey(product => product.Id);

            entity.Property(product => product.Id)
                .HasColumnName("id");

            entity.Property(product => product.Name)
                .HasColumnName("name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(product => product.Description)
                .HasColumnName("description")
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(product => product.Price)
                .HasColumnName("price")
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(product => product.Stock)
                .HasColumnName("stock")
                .IsRequired();

            entity.Property(product => product.CategoryId)
                .HasColumnName("category_id");

            entity.HasOne(product => product.Category)
                .WithMany()
                .HasForeignKey(product => product.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(product => product.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.Property(product => product.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            entity.Property(product => product.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");

            entity.HasIndex(product => product.Name);

            entity.HasIndex(product => product.CategoryId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");

            entity.HasKey(category => category.Id);

            entity.Property(category => category.Id)
                .HasColumnName("id");

            entity.Property(category => category.Name)
                .HasColumnName("name")
                .HasMaxLength(150)
                .IsRequired();

            entity.Property(category => category.Slug)
                .HasColumnName("slug")
                .HasMaxLength(180)
                .IsRequired();

            entity.HasIndex(category => category.Slug)
                .IsUnique();

            entity.Property(category => category.Description)
                .HasColumnName("description")
                .HasMaxLength(1000)
                .IsRequired();

            entity.Property(category => category.IsActive)
                .HasColumnName("is_active")
                .IsRequired();

            entity.Property(category => category.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .IsRequired();

            entity.Property(category => category.UpdatedAtUtc)
                .HasColumnName("updated_at_utc");
        });
    }

    
}