using System.Security.Claims;
using UsersProducts.Api.Common.Exceptions;

namespace UsersProducts.Api.Common.Security;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserIdOrThrow(this ClaimsPrincipal user)
    {
        var userIdValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdValue) || !Guid.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedException("No se pudo identificar al usuario autenticado.");
        }

        return userId;
    }
}