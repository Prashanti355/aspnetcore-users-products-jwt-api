namespace UsersProducts.Api.Contracts.Categories;

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);