namespace UsersProducts.Api.Contracts.Products;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid? CategoryId,
    string? CategoryName,
    string? CategorySlug,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);