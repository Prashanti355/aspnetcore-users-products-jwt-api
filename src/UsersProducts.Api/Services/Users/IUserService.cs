using UsersProducts.Api.Contracts.Users;

namespace UsersProducts.Api.Services.Users;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<UserResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);

    Task<UserResponse?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken);

    Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken);
}