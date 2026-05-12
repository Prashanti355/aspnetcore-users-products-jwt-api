using UsersProducts.Api.Contracts.Users;

namespace UsersProducts.Api.Contracts.Auth;

public sealed record AuthResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    UserResponse User
);