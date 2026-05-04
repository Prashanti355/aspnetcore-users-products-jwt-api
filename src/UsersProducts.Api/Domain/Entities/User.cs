using UsersProducts.Api.Domain.Enums;

namespace UsersProducts.Api.Domain.Entities;

public sealed class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public UserRole Role { get; private set; } = UserRole.User;

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; private set; }

    private User()
    {
    }

    public User(string name, string email, UserRole role)
    {
        Name = NormalizeName(name);
        Email = NormalizeEmail(email);
        Role = role;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("El hash de contraseña no puede estar vacío.");
        }

        PasswordHash = passwordHash;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void UpdateProfile(string name, string email, UserRole role, bool isActive)
    {
        Name = NormalizeName(name);
        Email = NormalizeEmail(email);
        Role = role;
        IsActive = isActive;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private static string NormalizeName(string name)
    {
        return name.Trim();
    }
}