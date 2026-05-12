using Microsoft.EntityFrameworkCore;
using UsersProducts.Api.Common.Exceptions;
using UsersProducts.Api.Contracts.Categories;
using UsersProducts.Api.Domain.Entities;
using UsersProducts.Api.Infrastructure.Persistence;

namespace UsersProducts.Api.Services.Categories;

public sealed class CategoryService : ICategoryService
{
    private readonly AppDbContext _dbContext;

    public CategoryService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetActiveAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .OrderBy(category => category.Name)
            .Select(category => ToResponse(category))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => ToResponse(category))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryResponse?> GetActiveByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                category => category.Id == id && category.IsActive,
                cancellationToken
            );

        return category is null ? null : ToResponse(category);
    }

    public async Task<CategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

        return category is null ? null : ToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var slug = Category.GenerateSlug(request.Name);

        var slugAlreadyExists = await _dbContext.Categories
            .AnyAsync(category => category.Slug == slug, cancellationToken);

        if (slugAlreadyExists)
        {
            throw new ConflictException("Ya existe una categoría con ese nombre o slug.");
        }

        var category = new Category(request.Name, request.Description);

        _dbContext.Categories.Add(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(category);
    }

    public async Task<CategoryResponse?> UpdateAsync(
        Guid id,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

        if (category is null)
        {
            return null;
        }

        var newSlug = Category.GenerateSlug(request.Name);

        var slugAlreadyExists = await _dbContext.Categories
            .AnyAsync(
                existingCategory =>
                    existingCategory.Slug == newSlug &&
                    existingCategory.Id != id,
                cancellationToken
            );

        if (slugAlreadyExists)
        {
            throw new ConflictException("Ya existe otra categoría con ese nombre o slug.");
        }

        category.Update(request.Name, request.Description, request.IsActive);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(category);
    }

    public async Task<bool> DeactivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

        if (category is null)
        {
            return false;
        }

        category.Deactivate();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static CategoryResponse ToResponse(Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.IsActive,
            category.CreatedAtUtc,
            category.UpdatedAtUtc
        );
    }
}