using Microsoft.EntityFrameworkCore;
using UsersProducts.Api.Common.Exceptions;
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
            .Include(product => product.Category)
            .Where(product =>
                product.IsActive &&
                product.CategoryId != null &&
                product.Category != null &&
                product.Category.IsActive
            )
            .OrderBy(product => product.Name)
            .Select(product => ToResponse(product))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .OrderBy(product => product.Name)
            .Select(product => ToResponse(product))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProductResponse?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .FirstOrDefaultAsync(
                product =>
                    product.Id == id &&
                    product.IsActive &&
                    product.CategoryId != null &&
                    product.Category != null &&
                    product.Category.IsActive,
                cancellationToken
            );

        return product is null ? null : ToResponse(product);
    }

    public async Task<ProductResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .FirstOrDefaultAsync(product => product.Id == id, cancellationToken);

        return product is null ? null : ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        await EnsureActiveCategoryExistsAsync(request.CategoryId, cancellationToken);

        var product = new Product(
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.CategoryId
        );

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var createdProduct = await _dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .FirstAsync(savedProduct => savedProduct.Id == product.Id, cancellationToken);

        return ToResponse(createdProduct);
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

        await EnsureActiveCategoryExistsAsync(request.CategoryId, cancellationToken);

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.Stock,
            request.CategoryId,
            request.IsActive
        );

        await _dbContext.SaveChangesAsync(cancellationToken);

        var updatedProduct = await _dbContext.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .FirstAsync(product => product.Id == id, cancellationToken);

        return ToResponse(updatedProduct);
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

    private async Task EnsureActiveCategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        if (categoryId == Guid.Empty)
        {
            throw new NotFoundException("Categoría no encontrada.");
        }

        var categoryExists = await _dbContext.Categories
            .AsNoTracking()
            .AnyAsync(
                category => category.Id == categoryId && category.IsActive,
                cancellationToken
            );

        if (!categoryExists)
        {
            throw new NotFoundException("Categoría no encontrada.");
        }
    }

    private static ProductResponse ToResponse(Product product)
    {
        return new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            product.Category?.Name,
            product.Category?.Slug,
            product.IsActive,
            product.CreatedAtUtc,
            product.UpdatedAtUtc
        );
    }
}