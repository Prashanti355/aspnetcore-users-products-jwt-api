using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersProducts.Api.Common.Exceptions;
using UsersProducts.Api.Contracts.Auth;
using UsersProducts.Api.Contracts.Users;
using UsersProducts.Api.Domain.Entities;
using UsersProducts.Api.Domain.Enums;
using UsersProducts.Api.Infrastructure.Persistence;

namespace UsersProducts.Api.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IConfiguration _configuration;

    public AuthService(
        AppDbContext dbContext,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _configuration = configuration;
    }

    public async Task<AuthResponse> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(request.Email);

        var emailAlreadyExists = await _dbContext.Users
            .AnyAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (emailAlreadyExists)
        {
            throw new ConflictException("Ya existe un usuario registrado con ese correo electrónico.");
        }

        var user = new User(
            name: request.Name,
            email: normalizedEmail,
            role: UserRole.User
        );

        var passwordHash = _passwordHasher.HashPassword(user, request.Password);
        user.SetPasswordHash(passwordHash);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(request.Email);

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (user is null || !user.IsActive)
        {
            throw new UnauthorizedException("Correo o contraseña incorrectos.");
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password
        );

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException("Correo o contraseña incorrectos.");
        }

        return CreateAuthResponse(user);
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var expirationMinutes = _configuration.GetValue<int>("Jwt:ExpirationMinutes");

        if (expirationMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt:ExpirationMinutes debe ser mayor que cero.");
        }

        var expiresAtUtc = DateTime.UtcNow.AddMinutes(expirationMinutes);
        var accessToken = _jwtTokenGenerator.GenerateToken(user, expiresAtUtc);

        var userResponse = new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            user.IsActive,
            user.CreatedAtUtc,
            user.UpdatedAtUtc
        );

        return new AuthResponse(
            AccessToken: accessToken,
            ExpiresAtUtc: expiresAtUtc,
            User: userResponse
        );
    }
}