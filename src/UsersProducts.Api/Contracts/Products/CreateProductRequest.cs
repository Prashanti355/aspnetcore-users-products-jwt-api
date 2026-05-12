using System.ComponentModel.DataAnnotations;

namespace UsersProducts.Api.Contracts.Products;

public sealed class CreateProductRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; init; } = string.Empty;

    [Range(0, 999999999)]
    public decimal Price { get; init; }

    [Range(0, int.MaxValue)]
    public int Stock { get; init; }
}