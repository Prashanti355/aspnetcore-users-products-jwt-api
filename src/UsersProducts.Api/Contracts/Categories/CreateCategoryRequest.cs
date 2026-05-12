using System.ComponentModel.DataAnnotations;

namespace UsersProducts.Api.Contracts.Categories;

public sealed class CreateCategoryRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; init; } = string.Empty;
}