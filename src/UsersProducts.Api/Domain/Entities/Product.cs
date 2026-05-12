namespace UsersProducts.Api.Domain.Entities;

public sealed class Product
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public Guid? CategoryId { get; private set; }

    public Category? Category { get; private set; }

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; private set; }

    private Product()
    {
    }

    public Product(string name, string description, decimal price, int stock, Guid categoryId)
    {
        Name = NormalizeName(name);
        Description = NormalizeDescription(description);
        SetPrice(price);
        SetStock(stock);
        SetCategory(categoryId);
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Update(string name, string description, decimal price, int stock, Guid categoryId, bool isActive)
    {
        Name = NormalizeName(name);
        Description = NormalizeDescription(description);
        SetPrice(price);
        SetStock(stock);
        SetCategory(categoryId);
        IsActive = isActive;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    private void SetCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty)
        {
            throw new ArgumentException("La categoría del producto es obligatoria.");
        }

        CategoryId = categoryId;
    }

    private void SetPrice(decimal price)
    {
        if (price < 0)
        {
            throw new ArgumentException("El precio no puede ser negativo.");
        }

        Price = price;
    }

    private void SetStock(int stock)
    {
        if (stock < 0)
        {
            throw new ArgumentException("El stock no puede ser negativo.");
        }

        Stock = stock;
    }

    private static string NormalizeName(string name)
    {
        return name.Trim();
    }

    private static string NormalizeDescription(string description)
    {
        return description.Trim();
    }
}