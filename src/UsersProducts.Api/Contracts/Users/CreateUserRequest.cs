using System.ComponentModel.DataAnnotations;
using UsersProducts.Api.Domain.Enums;

namespace UsersProducts.Api.Contracts.Users;

public sealed class CreateUserRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [MinLength(8)]
    [MaxLength(100)]
    public string Password { get; init; } = string.Empty;

    public UserRole Role { get; init; } = UserRole.User;
}