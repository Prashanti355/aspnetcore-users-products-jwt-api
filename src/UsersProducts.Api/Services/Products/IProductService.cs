using UsersProducts.Api.Contracts.Products;

namespace UsersProducts.Api.Services.Products;

public interface IProductService
{
    Task<IReadOnlyList<ProductResponse>> GetActiveAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<ProductResponse?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<ProductResponse> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken);

    Task<ProductResponse?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken);

    Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken);
}