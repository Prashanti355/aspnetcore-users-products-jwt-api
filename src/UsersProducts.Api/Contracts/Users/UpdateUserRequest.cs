using System.ComponentModel.DataAnnotations;
using UsersProducts.Api.Domain.Enums;

namespace UsersProducts.Api.Contracts.Users;

public sealed class UpdateUserRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    public UserRole Role { get; init; }

    public bool IsActive { get; init; }
}