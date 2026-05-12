using UsersProducts.Api.Domain.Entities;

namespace UsersProducts.Api.Services.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user, DateTime expiresAtUtc);
}