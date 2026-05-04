using UsersProducts.Api.Domain.Enums;

namespace UsersProducts.Api.Contracts.Users;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string Email,
    UserRole Role,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);