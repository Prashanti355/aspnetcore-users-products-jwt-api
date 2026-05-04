using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UsersProducts.Api.Common.Exceptions;
using UsersProducts.Api.Contracts.Users;
using UsersProducts.Api.Domain.Entities;
using UsersProducts.Api.Infrastructure.Persistence;

namespace UsersProducts.Api.Services.Users;

public sealed class UserService : IUserService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(AppDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.CreatedAtUtc)
            .Select(user => ToResponse(user))
            .ToListAsync(cancellationToken);
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

        return user is null ? null : ToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(request.Email);

        var emailAlreadyExists = await _dbContext.Users
            .AnyAsync(user => user.Email == normalizedEmail, cancellationToken);

        if (emailAlreadyExists)
        {
            throw new ConflictException("Ya existe un usuario registrado con ese correo electrónico.");
        }

        var user = new User(request.Name, normalizedEmail, request.Role);
        var passwordHash = _passwordHasher.HashPassword(user, request.Password);

        user.SetPasswordHash(passwordHash);

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(user);
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

        if (user is null)
        {
            return null;
        }

        var normalizedEmail = User.NormalizeEmail(request.Email);

        var emailAlreadyExists = await _dbContext.Users
            .AnyAsync(existingUser =>
                existingUser.Email == normalizedEmail &&
                existingUser.Id != id,
                cancellationToken);

        if (emailAlreadyExists)
        {
            throw new ConflictException("Ya existe otro usuario registrado con ese correo electrónico.");
        }

        user.UpdateProfile(request.Name, normalizedEmail, request.Role, request.IsActive);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(user);
    }

    public async Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

        if (user is null)
        {
            return false;
        }

        user.Deactivate();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static UserResponse ToResponse(User user)
    {
        return new UserResponse(
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            user.IsActive,
            user.CreatedAtUtc,
            user.UpdatedAtUtc
        );
    }
}