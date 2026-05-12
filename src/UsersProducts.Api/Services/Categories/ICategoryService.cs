using UsersProducts.Api.Contracts.Categories;

namespace UsersProducts.Api.Services.Categories;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryResponse>> GetActiveAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<CategoryResponse?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);

    Task<CategoryResponse?> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken);

    Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken);
}