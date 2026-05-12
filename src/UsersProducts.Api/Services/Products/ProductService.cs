using Microsoft.EntityFrameworkCore;
using UsersProducts.Api.Contracts.Products;
using UsersProducts.Api.Domain.Entities;
using UsersProducts.Api.Infrastructure.Persistence;

namespace UsersProducts.Api.Services.Products;

public sealed class ProductService : IProductService
{
    private readonly AppDbContext _dbContext;

    public ProductService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ProductResponse>> GetActiveAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Where(product => product.IsActive)
            .OrderBy(product => product.Name)
            .Select(product => ToResponse(product))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .Select(product => ToResponse(product))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductResponse?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(
                product => product.Id == id && product.IsActive,
                cancellationToken
            );

        return product is null ? null : ToResponse(product);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        return product is null ? null : ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = new Product(
            request.Name,
            request.Description,
            request.Price,
            request.Stock
        );

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(product);
    }

    public async Task<ProductResponse?> UpdateAsync(
        Guid id,
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return null;
        }

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.IsActive
        );

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(product);
    }

    public async Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        if (product is null)
        {
            return false;
        }

        product.Deactivate();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.IsActive,
            product.CreatedAtUtc,
            product.UpdatedAtUtc
        );
    }
}