namespace UsersProducts.Api.Contracts.Products;

public sealed record ProductResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);