using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace UsersProducts.Api.Domain.Entities;

public sealed class Category
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool IsActive { get; private set; } = true;

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public DateTime? UpdatedAtUtc { get; private set; }

    private Category()
    {
    }

    public Category(string name, string description)
    {
        Name = NormalizeName(name);
        Slug = GenerateSlug(name);
        Description = NormalizeDescription(description);
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Update(string name, string description, bool isActive)
    {
        Name = NormalizeName(name);
        Slug = GenerateSlug(name);
        Description = NormalizeDescription(description);
        IsActive = isActive;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public static string GenerateSlug(string value)
    {
        var normalizedValue = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);

        var builder = new StringBuilder();

        foreach (var character in normalizedValue)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);

            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        var withoutDiacritics = builder.ToString().Normalize(NormalizationForm.FormC);
        var slug = Regex.Replace(withoutDiacritics, "[^a-z0-9]+", "-").Trim('-');

        if (string.IsNullOrWhiteSpace(slug))
        {
            throw new ArgumentException("El nombre de la categoría no permite generar un slug válido.");
        }

        return slug;
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